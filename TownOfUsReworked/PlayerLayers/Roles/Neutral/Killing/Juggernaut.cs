namespace TownOfUsReworked.PlayerLayers.Roles;

public class Juggernaut : Neutral
{
    public DateTime LastKilled { get; set; }
    public int JuggKills { get; set; }
    public CustomButton AssaultButton { get; set; }

    public override Color Color => ClientGameOptions.CustomNeutColors ? Colors.Juggernaut : Colors.Neutral;
    public override string Name => "Juggernaut";
    public override LayerEnum Type => LayerEnum.Juggernaut;
    public override Func<string> StartText => () => "Your Power Grows With Every Kill";
    public override Func<string> Description => () => "- With each kill, your kill cooldown decreases" + (JuggKills >= 4 ? "\n- You can bypass all forms of protection" : "");
    public override InspectorResults InspectorResults => InspectorResults.IsAggressive;
    public float Timer => ButtonUtils.Timer(Player, LastKilled, CustomGameOptions.AssaultCd, -(CustomGameOptions.AssaultBonus * JuggKills));

    public Juggernaut(PlayerControl player) : base(player)
    {
        Objectives = () => "- Assault anyone who can oppose you";
        Alignment = Alignment.NeutralKill;
        JuggKills = 0;
        AssaultButton = new(this, "Assault", AbilityTypes.Direct, "ActionSecondary", Assault, Exception);
    }

    public void Assault()
    {
        if (IsTooFar(Player, AssaultButton.TargetPlayer) || Timer != 0f)
            return;

        var interact = Interact(Player, AssaultButton.TargetPlayer, true, false, JuggKills >= 4);

        if (interact.AbilityUsed)
            JuggKills++;

        if (JuggKills == 4 && Local)
            Flash(Color);

        if (interact.Reset)
            LastKilled = DateTime.UtcNow;
        else if (interact.Protected)
            LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
        else if (interact.Vested)
            LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
    }

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate)
        || Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        AssaultButton.Update("ASSAULT", Timer, CustomGameOptions.AssaultCd, -(CustomGameOptions.AssaultBonus * JuggKills));
    }
}