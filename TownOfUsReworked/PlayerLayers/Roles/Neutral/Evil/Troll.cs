namespace TownOfUsReworked.PlayerLayers.Roles;

public class Troll : Neutral
{
    public bool Killed { get; set; }
    public CustomButton InteractButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomNeutColors ? CustomColorManager.Troll : CustomColorManager.Neutral;
    public override string Name => "Troll";
    public override LayerEnum Type => LayerEnum.Troll;
    public override Func<string> StartText => () => "Troll Everyone With Your Death";
    public override Func<string> Description => () => "- You can interact with players\n- Your interactions do nothing except spread infection and possibly kill you via touch sensitive" +
        " roles\n- If you are killed, you will also kill your killer";
    public override AttackEnum AttackVal => AttackEnum.Unstoppable;

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.NeutralEvil;
        Objectives = () => Killed ? "- You have successfully trolled someone" : "- Get killed";
        InteractButton = CreateButton(this, new SpriteName("Placeholder"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Interact, new Cooldown(CustomGameOptions.InteractCd),
            "INTERACT");
    }

    public void Interact() => InteractButton.StartCooldown(Interactions.Interact(Player, InteractButton.TargetPlayer));
}