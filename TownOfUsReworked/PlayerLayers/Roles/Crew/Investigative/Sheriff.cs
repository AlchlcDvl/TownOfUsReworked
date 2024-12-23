namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Sheriff : Crew
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number InterrogateCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool NeutEvilRed { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool NeutKillingRed { get; set; } = false;

    public CustomButton InterrogateButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Sheriff: FactionColor;
    public override string Name => "Sheriff";
    public override LayerEnum Type => LayerEnum.Sheriff;
    public override Func<string> StartText => () => "Reveal The Alignment Of Other Players";
    public override Func<string> Description => () => "- You can reveal alignments of other players relative to the <#8CFFFFFF>Crew</color>";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.CrewKill;
        InterrogateButton ??= new(this, "INTERROGATE", new SpriteName("Interrogate"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Interrogate, new Cooldown(InterrogateCd),
            (PlayerBodyExclusion)Exception);
    }

    public void Interrogate(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Flash(target.SeemsEvil() ? UColor.red : UColor.green);

        InterrogateButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) || (player.Is(SubFaction) && SubFaction != SubFaction.None)) &&
        GameModifiers.FactionSeeRoles) || (Player.IsOtherLover(player) && Lovers.LoversRoles) || (Player.IsOtherRival(player) && Rivals.RivalsRoles) || (player.Is<Mafia>() && Player.Is<Mafia>()
        && Mafia.MafiaRoles) || (Player.IsOtherLink(player) && Linked.LinkedRoles);
}