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

    public Hunter(PlayerControl player) : base(player)
    {
        Objectives = () => "- Hunt the others down before they finish their tasks";
        HuntButton = new(this, "IntruderKill", AbilityTypes.Alive, "ActionSecondary", Hunt, CustomGameOptions.HuntCd, Exception);
        Data.SetImpostor(true);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        HuntButton.Update2("HUNT");

        if (Starting)
        {
            StartingTimer = Mathf.Clamp(StartingTimer - Time.deltaTime, 0f, CustomGameOptions.StartTime);

            if (!Starting)
                HuntButton.StartCooldown();
        }
    }

    public bool Exception(PlayerControl player) => player.Is(LayerEnum.Hunter);

    public void TurnHunter(PlayerControl player)
    {
        var oldRole = GetRole<Hunted>(player);
        var newRole = new Hunter(player);
        newRole.RoleUpdate(oldRole);
        newRole.KilledBy = " By " + PlayerName;
        newRole.DeathReason = DeathReasonEnum.Converted;
        newRole.HuntButton.StartCooldown();
        UObject.Instantiate(GameManagerCreator.Instance.HideAndSeekManagerPrefab.DeathPopupPrefab, HUD.transform.parent).Show(player, 0);
        GameData.Instance.RecomputeTaskCounts();
    }

    public override void OnIntroEnd() => StartingTimer = CustomGameOptions.StartTime;

    public void Hunt()
    {
        if (CustomGameOptions.HnSMode == HnSMode.Classic)
            RpcMurderPlayer(Player, HuntButton.TargetPlayer);
        else if (CustomGameOptions.HnSMode == HnSMode.Infection)
        {
            TurnHunter(HuntButton.TargetPlayer);
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, HuntButton.TargetPlayer);
        }

        HuntButton.StartCooldown();
    }

    public override void ReadRPC(MessageReader reader) => TurnHunter(reader.ReadPlayer());
}