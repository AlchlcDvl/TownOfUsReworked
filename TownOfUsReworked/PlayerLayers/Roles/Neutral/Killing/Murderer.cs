namespace TownOfUsReworked.PlayerLayers.Roles;

public class Murderer : Neutral
{
    public CustomButton MurderButton { get; set; }

    public override Color Color => ClientGameOptions.CustomNeutColors ? Colors.Murderer : Colors.Neutral;
    public override string Name => "Murderer";
    public override LayerEnum Type => LayerEnum.Murderer;
    public override Func<string> StartText => () => "I Got Murder On My Mind";
    public override Func<string> Description => () => "- You can kill";
    public override InspectorResults InspectorResults => InspectorResults.IsBasic;

    public Murderer(PlayerControl player) : base(player)
    {
        Objectives = () => "- Murder anyone who can oppose you";
        Alignment = Alignment.NeutralKill;
        MurderButton = new(this, "Murder", AbilityTypes.Target, "ActionSecondary", Murder, CustomGameOptions.MurderCd, Exception);
    }

    public void Murder()
    {
        var interact = Interact(Player, MurderButton.TargetPlayer, true);
        var cooldown = CooldownType.Reset;

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;
        else if (interact.Vested)
            cooldown = CooldownType.Survivor;

        MurderButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        MurderButton.Update2("MURDER");
    }
}