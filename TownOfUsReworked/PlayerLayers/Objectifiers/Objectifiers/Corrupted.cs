namespace TownOfUsReworked.PlayerLayers.Objectifiers;

public class Corrupted : Objectifier
{
    private CustomButton CorruptButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomObjColors ? CustomColorManager.Corrupted : CustomColorManager.Objectifier;
    public override string Name => "Corrupted";
    public override string Symbol => "δ";
    public override LayerEnum Type => LayerEnum.Corrupted;
    public override Func<string> Description => () => "- Corrupt everyone";
    public override AttackEnum AttackVal => AttackEnum.Basic;

    public override void Init()
    {
        CorruptButton = CreateButton(this, "CORRUPT", new SpriteName("Corrupt"), AbilityTypes.Alive, KeybindType.Quarternary, (OnClick)Corrupt, new Cooldown(CustomGameOptions.CorruptCd));
        Player.GetRole().Alignment = Player.GetRole().Alignment.GetNewAlignment(Faction.Neutral);
    }

    private void Corrupt() => CorruptButton.StartCooldown(Interact(Player, CorruptButton.TargetPlayer, true));
}