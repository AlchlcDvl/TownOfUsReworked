namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Timekeeper : Syndicate
{
    public CustomButton TimeButton { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Timekeeper : CustomColorManager.Syndicate;
    public override string Name => "Timekeeper";
    public override LayerEnum Type => LayerEnum.Timekeeper;
    public override Func<string> StartText => () => "Bend Time To Your Will";
    public override Func<string> Description => () => $"- You can {(HoldsDrive ? "rewind" : "freeze")} time, making people {(HoldsDrive ? "go backwards" : "unable to move")}\n" +
        CommonAbilities;

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.SyndicateDisrup;
        TimeButton = CreateButton(this, new SpriteName("Time"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)TimeControl, new Cooldown(CustomGameOptions.TimeCd), (LabelFunc)Label,
            new Duration(CustomGameOptions.TimeDur), (EffectVoid)Control, (EffectStartVoid)ControlStart, (EffectEndVoid)UnControl);
        Data.Role.IntroSound = GetAudio("TimekeeperIntro");
    }

    public void ControlStart() => Flash(Color, CustomGameOptions.TimeDur);

    public void Control()
    {
        if (HoldsDrive)
            CustomPlayer.AllPlayers.ForEach(x => x.GetRole().Rewinding = true);
    }

    public void UnControl() => CustomPlayer.AllPlayers.ForEach(x => x.GetRole().Rewinding = false);

    public void TimeControl()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, TimeButton);
        TimeButton.Begin();
    }

    public string Label() => HoldsDrive ? "REWIND" : "FREEZE";
}