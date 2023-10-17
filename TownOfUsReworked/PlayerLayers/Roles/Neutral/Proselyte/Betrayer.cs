namespace TownOfUsReworked.PlayerLayers.Roles;

public class Betrayer : Neutral
{
    public CustomButton KillButton { get; set; }

    public override Color Color => ClientGameOptions.CustomNeutColors ? Colors.Betrayer : Colors.Neutral;
    public override string Name => "Betrayer";
    public override LayerEnum Type => LayerEnum.Betrayer;
    public override Func<string> StartText => () => "Those Backs Are Ripe For Some Stabbing";
    public override Func<string> Description => () => "- You can kill";

    public Betrayer(PlayerControl player) : base(player)
    {
        Objectives = () => $"- Kill anyone who opposes the {FactionName}";
        Alignment = Alignment.NeutralPros;
        KillButton = new(this, "BetKill", AbilityTypes.Target, "ActionSecondary", Kill, CustomGameOptions.BetrayCd, Exception);
    }

    public void Kill()
    {
        var interact = Interact(Player, KillButton.TargetPlayer, true);
        var cooldown = CooldownType.Reset;

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;
        else if (interact.Vested)
            cooldown = CooldownType.Survivor;

        KillButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        KillButton.Update2("BETRAY");
    }
}