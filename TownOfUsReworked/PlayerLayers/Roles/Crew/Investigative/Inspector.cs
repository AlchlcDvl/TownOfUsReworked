namespace TownOfUsReworked.PlayerLayers.Roles;

public class Inspector : Crew
{
    public List<byte> Inspected { get; set; }
    public CustomButton InspectButton { get; set; }

    public override Color Color => ClientGameOptions.CustomCrewColors ? Colors.Inspector : Colors.Crew;
    public override string Name => "Inspector";
    public override LayerEnum Type => LayerEnum.Inspector;
    public override Func<string> StartText => () => "Inspect Players For Their Roles";
    public override Func<string> Description => () => "- You can check a player to get a role list of what they could be";
    public override InspectorResults InspectorResults => InspectorResults.GainsInfo;

    public Inspector(PlayerControl player) : base(player)
    {
        Alignment = Alignment.CrewInvest;
        Inspected = new();
        InspectButton = new(this, "Inspect", AbilityTypes.Target, "ActionSecondary", Inspect, CustomGameOptions.InspectCd, Exception);
    }

    public void Inspect()
    {
        var interact = Interact(Player, InspectButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
            Inspected.Add(InspectButton.TargetPlayer.PlayerId);

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        InspectButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => Inspected.Contains(player.PlayerId) || (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) || (player.Is(SubFaction)
        && SubFaction != SubFaction.None)) && CustomGameOptions.FactionSeeRoles) || (Player.IsOtherLover(player) && CustomGameOptions.LoversRoles) || (Player.IsOtherRival(player) &&
        CustomGameOptions.RivalsRoles) || (player.Is(LayerEnum.Mafia) && Player.Is(LayerEnum.Mafia) && CustomGameOptions.MafiaRoles) || (Player.IsOtherLink(player) &&
        CustomGameOptions.LinkedRoles);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        InspectButton.Update2("INSPECT");
    }
}