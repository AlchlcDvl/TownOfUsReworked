namespace TownOfUsReworked.PlayerLayers.Roles;

public class Sheriff : Crew
{
    public CustomButton InterrogateButton { get; set; }

    public override Color Color => ClientGameOptions.CustomCrewColors ? Colors.Sheriff : Colors.Crew;
    public override string Name => "Sheriff";
    public override LayerEnum Type => LayerEnum.Sheriff;
    public override Func<string> StartText => () => "Reveal The Alignment Of Other Players";
    public override Func<string> Description => () => "- You can reveal alignments of other players relative to the <color=#8CFFFFFF>Crew</color>";
    public override InspectorResults InspectorResults => InspectorResults.GainsInfo;

    public Sheriff(PlayerControl player) : base(player)
    {
        Alignment = Alignment.CrewKill;
        InterrogateButton = new(this, "Interrogate", AbilityTypes.Target, "ActionSecondary", Interrogate, CustomGameOptions.InterrogateCd, Exception);
    }

    public void Interrogate()
    {
        var interact = Interact(Player, InterrogateButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
            Flash(InterrogateButton.TargetPlayer.SeemsEvil() ? UColor.red : UColor.green);

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        InterrogateButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) || (player.Is(SubFaction) && SubFaction != SubFaction.None)) &&
        CustomGameOptions.FactionSeeRoles) || (Player.IsOtherLover(player) && CustomGameOptions.LoversRoles) || (Player.IsOtherRival(player) && CustomGameOptions.RivalsRoles) ||
        (player.Is(LayerEnum.Mafia) && Player.Is(LayerEnum.Mafia) && CustomGameOptions.MafiaRoles) || (Player.IsOtherLink(player) && CustomGameOptions.LinkedRoles);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        InterrogateButton.Update2("INTERROGATE");
    }
}