namespace TownOfUsReworked.PlayerLayers.Roles;

public class GuardianAngel : Neutral
{
    public PlayerControl TargetPlayer { get; set; }
    public bool TargetAlive => !Disconnected && !TargetPlayer.HasDied();
    public CustomButton ProtectButton { get; set; }
    public CustomButton GraveProtectButton { get; set; }
    public int Rounds { get; set; }
    public CustomButton TargetButton { get; set; }
    public bool Failed => TargetPlayer == null && Rounds > 2;

    public override UColor Color => ClientGameOptions.CustomNeutColors ? CustomColorManager.GuardianAngel : CustomColorManager.Neutral;
    public override string Name => "Guardian Angel";
    public override LayerEnum Type => LayerEnum.GuardianAngel;
    public override Func<string> StartText => () => "Find Someone To Protect";
    public override Func<string> Description => () => TargetPlayer == null ? "- You can select a player to be your target" : ($"- You can protect {TargetPlayer?.name} from death for a " +
        $"short while\n- If {TargetPlayer?.name} dies, you will become a <color=#DDDD00FF>Survivor</color>");

    public GuardianAngel(PlayerControl player) : base(player)
    {
        Objectives = () => TargetPlayer == null ? "- Find a target to protect" : $"- Have {TargetPlayer?.name} live to the end of the game";
        Alignment = Alignment.NeutralBen;
        TargetPlayer = null;
        ProtectButton = new(this, "Protect", AbilityTypes.Targetless, "ActionSecondary", HitProtect, CustomGameOptions.ProtectCd, CustomGameOptions.ProtectDur, CustomGameOptions.MaxProtects);
        GraveProtectButton = new(this, "GraveProtect", AbilityTypes.Targetless, "ActionSecondary", HitProtect, CustomGameOptions.ProtectCd, CustomGameOptions.ProtectDur,
            CustomGameOptions.MaxProtects, true);
        TargetButton = new(this, "GATarget", AbilityTypes.Alive, "ActionSecondary", SelectTarget);
        Rounds = 0;
    }

    public void SelectTarget()
    {
        TargetPlayer = TargetButton.TargetPlayer;
        CallRpc(CustomRPC.Target, TargetRPC.SetGATarget, this, TargetPlayer);
    }

    public void TurnSurv() => new Survivor(Player).RoleUpdate(this);

    public void HitProtect()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, ProtectButton);
        ProtectButton.Begin();
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        ProtectButton.Update2("PROTECT", !Failed && TargetPlayer != null && TargetAlive);
        GraveProtectButton.Update2("PROTECT", !Failed && TargetPlayer != null && TargetAlive && CustomGameOptions.ProtectBeyondTheGrave);
        TargetButton.Update2("WATCH", TargetPlayer == null);

        if ((Failed || (TargetPlayer != null && !TargetAlive)) && !IsDead)
        {
            CallRpc(CustomRPC.Change, TurnRPC.TurnSurv, this);
            TurnSurv();
        }
    }
}