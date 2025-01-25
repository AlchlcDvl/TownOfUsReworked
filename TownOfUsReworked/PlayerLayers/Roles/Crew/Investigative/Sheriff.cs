namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Sheriff : Crew
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number InterrogateCd = 25;

    [ToggleOption]
    public static bool NeutEvilRed = false;

    [ToggleOption]
    public static bool NeutKillingRed = false;

    public CustomButton InterrogateButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Sheriff : FactionColor;
    public override LayerEnum Type => LayerEnum.Sheriff;
    public override Func<string> StartText => () => "Reveal The Alignment Of Other Players";
    public override Func<string> Description => () => "- You can reveal alignments of other players relative to the <#8CFFFFFF>Crew</color>";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Investigative;
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