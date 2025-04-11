namespace TownOfUsReworked.PlayerLayers.Roles;

public sealed class Hunter : HideAndSeek
{
    public override LayerEnum Type { get; } = LayerEnum.Hunter;
    public override Func<string> StartText { get; } = () => "Hunt Them All Down";
    protected override UColor MainColor => CustomColorManager.Hunter;
    public override float VisionRange => Starting ? 0.001f : GameModeSettings.HunterVision;
    public override bool CanVent => GameModeSettings.HunterVent;

    private CustomButton HuntButton { get; set; }
    private float StartingTimer { get; set; }
    public bool Starting => StartingTimer > 0f;

    protected override void Init()
    {
        base.Init();
        Objectives = () => "- Hunt the others down before they finish their tasks";
        HuntButton ??= new(this, "HUNT", new SpriteName("HunterKill"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Hunt, new Cooldown(GameModeSettings.HuntCd),
            (UsableFunc)Usable, (PlayerBodyExclusion)Exception);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (!Starting)
            return;

        StartingTimer = Mathf.Clamp(StartingTimer - Time.deltaTime, 0f, GameModeSettings.StartTime);

        if (!Starting)
            HuntButton.StartCooldown();
    }

    private bool Exception(PlayerControl player) => player.Is<Hunter>();

    private bool Usable() => !Starting;

    private void TurnHunter(PlayerControl player)
    {
        var newRole = new Hunter();
        newRole.RoleUpdate(player.GetRole());
        newRole.KilledBy = " By " + PlayerName;
        newRole.DeathReason = DeathReasonEnum.Converted;
        newRole.HuntButton.StartCooldown();
        UObject.Instantiate(GameManagerCreator.Instance.HideAndSeekManagerPrefab.DeathPopupPrefab, HUD().transform.parent).Show(player, 0);
        GameData.Instance.RecomputeTaskCounts();
    }

    public override void OnIntroEnd() => StartingTimer = GameModeSettings.StartTime;

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

    protected override void CheckWin(List<byte> winnerIds)
    {
        if (AllPlayers().Any(x => !x.HasDied() && x.Is<Hunted>()))
            return;

        WinState = WinLose.HunterWins;
        winnerIds.Add(PlayerId);
    }
}