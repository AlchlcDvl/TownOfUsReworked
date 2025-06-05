namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Hunter)]
public sealed class Hunter : HideAndSeek
{
    [NumberOption(1, 13, 1)]
    public static Number HunterCount = 1;

    [NumberOption(5f, 60f, 5f, Format.Time)]
    public static Number HuntCd = 10;

    [NumberOption(5f, 60f, 5f, Format.Time)]
    public static Number StartTime = 10;

    [ToggleOption]
    public static bool HunterVent = true;

    [NumberOption(0.1f, 1f, 0.05f, Format.Multiplier)]
    public static Number HunterVision = 0.25f;

    [NumberOption(1f, 1.5f, 0.05f, Format.Multiplier)]
    public static Number HunterSpeedModifier = 1.25f;

    [ToggleOption]
    public static bool HunterFlashlight = false;

    public override LayerEnum Type => LayerEnum.Hunter;
    public override Func<string> StartText { get; } = () => "Hunt Them All Down";
    protected override UColor MainColor => CustomColorManager.Hunter;
    public override bool CanVent => HunterVent;

    private CustomButton HuntButton { get; set; }
    private float StartingTimer { get; set; }
    public bool Starting => StartingTimer > 0f;

    protected override void Init()
    {
        base.Init();
        Objectives = () => "- Hunt the others down before they finish their tasks";
        HuntButton ??= new(this, "HUNT", new SpriteName("HunterKill"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Hunt, new Cooldown(HuntCd), (UsableFunc)Usable,
            (PlayerBodyExclusion)Exception);
    }

    public override void UpdateHud(HudManager __instance)
    {
        if (!Starting)
            return;

        StartingTimer = Mathf.Clamp(StartingTimer - Time.deltaTime, 0f, StartTime);

        if (!Starting)
            HuntButton.StartCooldown();
    }

    private bool Exception(PlayerControl player) => player.Is<Hunter>();

    private bool Usable() => !Starting;

    private void TurnHunter(PlayerControl player)
    {
        var newRole = new Hunter();
        newRole.RoleUpdate(player.GetRole());
        newRole.Handler.KilledBy = " By " + PlayerName;
        newRole.Handler.DeathReason = DeathReasonEnum.Converted;
        newRole.HuntButton.StartCooldown();
        UObject.Instantiate(GameManagerCreator.Instance.HideAndSeekManagerPrefab.DeathPopupPrefab, HUD().transform.parent).Show(player, 0);
        GameData.Instance.RecomputeTaskCounts();
    }

    public override void OnIntroEnd() => StartingTimer = StartTime;

    private void Hunt(PlayerControl target)
    {
        switch (GameModeSettings.HnSMode)
        {
            case HnSMode.Classic:
            {
                Player.RpcMurderPlayer(target);
                break;
            }
            case HnSMode.Infection:
            {
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, target);
                TurnHunter(target);

                if (AmongUsClient.Instance.AmHost)
                    CheckEndGame.CheckPlayerWins();

                break;
            }
        }

        HuntButton.StartCooldown();
    }

    public override void ReadRPC(NetData reader)
    {
        TurnHunter(reader.ReadPlayer());

        if (AmongUsClient.Instance.AmHost)
            CheckEndGame.CheckPlayerWins();
    }

    protected override void CheckWin(HashSet<byte> winnerIds)
    {
        if (AllPlayers().Any(x => !x.HasDied() && x.Is<Hunted>()))
            return;

        WinState = WinLose.HunterWins;
        winnerIds.Add(PlayerId);
    }
}