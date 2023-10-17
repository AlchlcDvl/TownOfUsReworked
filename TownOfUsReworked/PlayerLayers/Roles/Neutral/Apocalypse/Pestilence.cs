namespace TownOfUsReworked.PlayerLayers.Roles;

public class Pestilence : Neutral
{
    public CustomButton ObliterateButton { get; set; }

    public override Color Color => ClientGameOptions.CustomNeutColors ? Colors.Pestilence : Colors.Neutral;
    public override string Name => "Pestilence";
    public override LayerEnum Type => LayerEnum.Pestilence;
    public override Func<string> StartText => () => "THE APOCALYPSE IS NIGH";
    public override Func<string> Description => () => "- Anyone who interacts with you will be killed";

    public Pestilence(PlayerControl player) : base(player)
    {
        Objectives = () => "- Obliterate anyone who can oppose you";
        Alignment = Alignment.NeutralApoc;
        ObliterateButton = new(this, "Obliterate", AbilityTypes.Target, "ActionSecondary", Obliterate, CustomGameOptions.ObliterateCd, Exception);
    }

    public void Obliterate()
    {
        var interact = Interact(Player, ObliterateButton.TargetPlayer, true);
        var cooldown = CooldownType.Reset;

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;
        else if (interact.Vested)
            cooldown = CooldownType.Survivor;

        ObliterateButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        ObliterateButton.Update2("OBLITERATE");
    }
}