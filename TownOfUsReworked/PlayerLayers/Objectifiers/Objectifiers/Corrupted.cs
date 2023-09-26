namespace TownOfUsReworked.PlayerLayers.Objectifiers;

public class Corrupted : Objectifier
{
    public CustomButton CorruptButton { get; set; }

    public override Color Color => ClientGameOptions.CustomObjColors ? Colors.Corrupted : Colors.Objectifier;
    public override string Name => "Corrupted";
    public override string Symbol => "δ";
    public override LayerEnum Type => LayerEnum.Corrupted;
    public override Func<string> Description => () => "- Corrupt everyone";

    public Corrupted(PlayerControl player) : base(player)
    {
        CorruptButton = new(this, "Corrupt", AbilityTypes.Target, "Quarternary", Corrupt, CustomGameOptions.CorruptCd);
        Role.GetRole(Player).Alignment = Role.GetRole(Player).Alignment.GetNewAlignment(Faction.Neutral);
    }

    public void Corrupt()
    {
        var interact = Interact(Player, CorruptButton.TargetPlayer, true);
        var cooldown = CooldownType.Reset;

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;
        else if (interact.Vested)
            cooldown = CooldownType.Survivor;

        CorruptButton.StartCooldown(cooldown);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        CorruptButton.Update2("CORRUPT");
    }
}