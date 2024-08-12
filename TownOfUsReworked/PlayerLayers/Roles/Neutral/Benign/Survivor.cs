namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Survivor : Neutral
{
    public CustomButton VestButton { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Survivor : CustomColorManager.Neutral;
    public override string Name => "Survivor";
    public override LayerEnum Type => LayerEnum.Survivor;
    public override Func<string> StartText => () => "Do Whatever It Takes To Live";
    public override Func<string> Description => () => "- You can put on a vest, which makes you unkillable for a short duration of time";
    public override DefenseEnum DefenseVal => VestButton.EffectActive ? DefenseEnum.Basic : DefenseEnum.None;

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.NeutralBen;
        Objectives = () => "- Live to the end of the game";
        VestButton = CreateButton(this, new SpriteName("Vest"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClick)HitVest, new Cooldown(CustomGameOptions.VestCd), "VEST",
            new Duration(CustomGameOptions.VestDur), CustomGameOptions.MaxVests);
    }

    public void HitVest()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, VestButton);
        VestButton.Begin();
    }
}