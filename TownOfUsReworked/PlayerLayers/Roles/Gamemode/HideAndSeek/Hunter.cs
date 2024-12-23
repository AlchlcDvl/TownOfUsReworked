namespace TownOfUsReworked.PlayerLayers.Roles;

public class Hunter : HideAndSeek
{
    public override string Name => "Hunter";
    public override LayerEnum Type => LayerEnum.Hunter;
    public override Func<string> StartText => () => "Hunt Them All Down";
    public override UColor Color => CustomColorManager.Hunter;
    public override string FactionName => "Hide And Seek";

    public CustomButton HuntButton { get; set; }
    public float StartingTimer { get; set; }
    public bool Starting => StartingTimer > 0f;

    public override void Init()
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

    public bool Exception(PlayerControl player) => player.Is<Hunter>();

    public bool Usable() => !Starting;

    public void TurnHunter(PlayerControl player)
    {
        var newRole = new Hunter();
        newRole.RoleUpdate(player);
        newRole.KilledBy = " By " + PlayerName;
        newRole.DeathReason = DeathReasonEnum.Converted;
        newRole.HuntButton.StartCooldown();
        UObject.Instantiate(GameManagerCreator.Instance.HideAndSeekManagerPrefab.DeathPopupPrefab, HUD().transform.parent).Show(player, 0);
        GameData.Instance.RecomputeTaskCounts();
    }

    public override void OnIntroEnd() => StartingTimer = GameModeSettings.StartTime;

    public void Hunt(PlayerControl target)
    {
        if (GameModeSettings.HnSMode == HnSMode.Classic)
            RpcMurderPlayer(Player, target);
        else if (GameModeSettings.HnSMode == HnSMode.Infection)
        {
            TurnHunter(target);
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, target);
        }

        HuntButton.StartCooldown();
    }

    public override void ReadRPC(MessageReader reader) => TurnHunter(reader.ReadPlayer());
}