namespace TownOfUsReworked.PlayerLayers.Roles;

public class Juggernaut : Neutral
{
    public int JuggKills { get; set; }
    public CustomButton AssaultButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomNeutColors ? CustomColorManager.Juggernaut : CustomColorManager.Neutral;
    public override string Name => "Juggernaut";
    public override LayerEnum Type => LayerEnum.Juggernaut;
    public override Func<string> StartText => () => "Your Power Grows With Every Kill";
    public override Func<string> Description => () => "- With each kill, your kill cooldown decreases" + (JuggKills >= 4 ? "\n- You can bypass all forms of protection" : "");
    public override AttackEnum AttackVal => (AttackEnum)Mathf.Clamp(JuggKills, 1, 3);

    public Juggernaut(PlayerControl player) : base(player)
    {
        Objectives = () => "- Assault anyone who can oppose you";
        Alignment = Alignment.NeutralKill;
        JuggKills = 0;
        AssaultButton = new(this, "Assault", AbilityTypes.Alive, "ActionSecondary", Assault, CustomGameOptions.AssaultCd, Exception);
    }

    public void Assault()
    {
        var cooldown = Interact(Player, AssaultButton.TargetPlayer, true, false, JuggKills >= 4);

        if (cooldown != CooldownType.Fail)
        {
            JuggKills++;
            Flash(Color);
        }

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