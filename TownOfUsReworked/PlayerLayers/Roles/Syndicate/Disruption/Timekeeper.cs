namespace TownOfUsReworked.PlayerLayers.Roles;

public class Timekeeper : Syndicate
{
    public CustomButton TimeButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomSynColors ? CustomColorManager.Timekeeper : CustomColorManager.Syndicate;
    public override string Name => "Timekeeper";
    public override LayerEnum Type => LayerEnum.Timekeeper;
    public override Func<string> StartText => () => "Bend Time To Your Will";
    public override Func<string> Description => () => $"- You can {(HoldsDrive ? "rewind" : "freeze")} time, making people {(HoldsDrive ? "go backwards" : "unable to move")}\n" +
        CommonAbilities;

    public Timekeeper(PlayerControl player) : base(player)
    {
        Alignment = Alignment.SyndicateDisrup;
        TimeButton = new(this, "Time", AbilityTypes.Targetless, "Secondary", TimeControl, CustomGameOptions.TimeCd, CustomGameOptions.TimeDur, Control, ControlStart, UnControl);
        player.Data.Role.IntroSound = GetAudio("TimekeeperIntro");
    }

    public void ControlStart() => Flash(Color, CustomGameOptions.TimeDur);

    public void Control()
    {
        if (HoldsDrive)
            CustomPlayer.AllPlayers.ForEach(x => GetRole(x).Rewinding = true);
    }

    public void UnControl() => CustomPlayer.AllPlayers.ForEach(x => GetRole(x).Rewinding = false);

    public void TimeControl()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, TimeButton);
        TimeButton.Begin();
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        TimeButton.Update2(HoldsDrive ? "REWIND" : "FREEZE");
    }
}