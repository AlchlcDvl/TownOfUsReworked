namespace TownOfUsReworked.PlayerLayers.Roles;

public class Inspector : Crew
{
    public DateTime LastInspected { get; set; }
    public List<byte> Inspected { get; set; }
    public CustomButton InspectButton { get; set; }
    public float Timer => ButtonUtils.Timer(Player, LastInspected, CustomGameOptions.InspectCooldown);

    public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Inspector : Colors.Crew;
    public override string Name => "Inspector";
    public override LayerEnum Type => LayerEnum.Inspector;
    public override Func<string> StartText => () => "Inspect Players For Their Roles";
    public override Func<string> Description => () => "- You can check a player to get a role list of what they could be";
    public override InspectorResults InspectorResults => InspectorResults.GainsInfo;

    public Inspector(PlayerControl player) : base(player)
    {
        RoleAlignment = RoleAlignment.CrewInvest;
        Inspected = new();
        InspectButton = new(this, "Inspect", AbilityTypes.Direct, "ActionSecondary", Inspect, Exception);
    }

    public void Inspect()
    {
        if (Timer != 0f || IsTooFar(Player, InspectButton.TargetPlayer) || Inspected.Contains(InspectButton.TargetPlayer.PlayerId))
            return;

        var interact = Interact(Player, InspectButton.TargetPlayer);

        if (interact[3])
            Inspected.Add(InspectButton.TargetPlayer.PlayerId);

        if (interact[0])
            LastInspected = DateTime.UtcNow;
        else if (interact[1])
            LastInspected.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public bool Exception(PlayerControl player) => Inspected.Contains(player.PlayerId) || (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) || (player.Is(SubFaction)
        && SubFaction != SubFaction.None)) && CustomGameOptions.FactionSeeRoles) || (Player.IsOtherLover(player) && CustomGameOptions.LoversRoles) || (Player.IsOtherRival(player) &&
        CustomGameOptions.RivalsRoles) || (player.Is(LayerEnum.Mafia) && Player.Is(LayerEnum.Mafia) && CustomGameOptions.MafiaRoles) || (Player.IsOtherLink(player) &&
        CustomGameOptions.LinkedRoles);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        InspectButton.Update("INSPECT", Timer, CustomGameOptions.InspectCooldown);
    }
}