namespace TownOfUsReworked.PlayerLayers.Roles;

public class Sheriff : Crew
{
    public CustomButton InterrogateButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomCrewColors ? CustomColorManager.Sheriff : CustomColorManager.Crew;
    public override string Name => "Sheriff";
    public override LayerEnum Type => LayerEnum.Sheriff;
    public override Func<string> StartText => () => "Reveal The Alignment Of Other Players";
    public override Func<string> Description => () => "- You can reveal alignments of other players relative to the <color=#8CFFFFFF>Crew</color>";

    public Sheriff() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Alignment = Alignment.CrewKill;
        InterrogateButton = new(this, "Interrogate", AbilityTypes.Alive, "ActionSecondary", Interrogate, CustomGameOptions.InterrogateCd, Exception);
        return this;
    }

    public void Interrogate()
    {
        var cooldown = Interact(Player, InterrogateButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
            Flash(InterrogateButton.TargetPlayer.SeemsEvil() ? UColor.red : UColor.green);

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