namespace TownOfUsReworked.PlayerLayers.Roles;

public class Juggernaut : Neutral
{
    public int JuggKills { get; set; }
    public CustomButton AssaultButton { get; set; }

    public override Color Color => ClientGameOptions.CustomNeutColors ? Colors.Juggernaut : Colors.Neutral;
    public override string Name => "Juggernaut";
    public override LayerEnum Type => LayerEnum.Juggernaut;
    public override Func<string> StartText => () => "Your Power Grows With Every Kill";
    public override Func<string> Description => () => "- With each kill, your kill cooldown decreases" + (JuggKills >= 4 ? "\n- You can bypass all forms of protection" : "");

    public Juggernaut(PlayerControl player) : base(player)
    {
        Objectives = () => "- Assault anyone who can oppose you";
        Alignment = Alignment.NeutralKill;
        JuggKills = 0;
        AssaultButton = new(this, "Assault", AbilityTypes.Target, "ActionSecondary", Assault, CustomGameOptions.AssaultCd, Exception);
    }

    public void Assault()
    {
        var interact = Interact(Player, AssaultButton.TargetPlayer, true, false, JuggKills >= 4);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
            JuggKills++;

        if (JuggKills == 4 && Local)
            Flash(Color);

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;
        else if (interact.Vested)
            cooldown = CooldownType.Survivor;

        AssaultButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        AssaultButton.Update2("ASSAULT", difference: -(CustomGameOptions.AssaultBonus * JuggKills));
    }
}