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

    public Corrupted() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        CorruptButton = new(this, "Corrupt", AbilityTypes.Alive, "Quarternary", Corrupt, CustomGameOptions.CorruptCd);
        Player.GetRole().Alignment = Player.GetRole().Alignment.GetNewAlignment(Faction.Neutral);
        return this;
    }

    private void Corrupt() => CorruptButton.StartCooldown(Interact(Player, CorruptButton.TargetPlayer, true));

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        CorruptButton.Update2("CORRUPT");
    }
}