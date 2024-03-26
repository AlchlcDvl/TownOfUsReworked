namespace TownOfUsReworked.PlayerLayers.Roles;

public class Veteran : Crew
{
    public CustomButton AlertButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomCrewColors ? CustomColorManager.Veteran : CustomColorManager.Crew;
    public override string Name => "Veteran";
    public override LayerEnum Type => LayerEnum.Veteran;
    public override Func<string> StartText => () => "Alert To Kill Anyone Who Dares To Touch You";
    public override Func<string> Description => () => "- You can go on alert\n- When on alert, you will kill whoever interacts with you";
    public override DefenseEnum DefenseVal => AlertButton.EffectActive ? DefenseEnum.Basic : DefenseEnum.None;
    public override AttackEnum AttackVal => AlertButton.EffectActive ? AttackEnum.Powerful : AttackEnum.None;

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.CrewKill;
        AlertButton = CreateButton(this, "ALERT", new SpriteName("Alert"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClick)Alert, new Cooldown(CustomGameOptions.AlertCd),
            new Duration(CustomGameOptions.AlertDur), CustomGameOptions.MaxAlerts, (EndFunc)EndEffect);
    }

    public void Alert()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, AlertButton);
        AlertButton.Begin();
    }

    public bool EndEffect() => Dead;
}