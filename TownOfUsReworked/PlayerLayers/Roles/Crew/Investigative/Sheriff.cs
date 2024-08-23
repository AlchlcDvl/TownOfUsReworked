namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Sheriff : Crew
{
    [NumberOption(MultiMenu2.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float InterrogateCd { get; set; } = 25f;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool NeutEvilRed { get; set; } = false;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool NeutKillingRed { get; set; } = false;

    public CustomButton InterrogateButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Sheriff : CustomColorManager.Crew;
    public override string Name => "Sheriff";
    public override LayerEnum Type => LayerEnum.Sheriff;
    public override Func<string> StartText => () => "Reveal The Alignment Of Other Players";
    public override Func<string> Description => () => "- You can reveal alignments of other players relative to the <color=#8CFFFFFF>Crew</color>";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.CrewKill;
        InterrogateButton = CreateButton(this, "INTERROGATE", new SpriteName("Interrogate"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Interrogate,
            new Cooldown(InterrogateCd), (PlayerBodyExclusion)Exception);
    }

    public void Interrogate()
    {
        var cooldown = Interact(Player, InterrogateButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
            Flash(InterrogateButton.TargetPlayer.SeemsEvil() ? UColor.red : UColor.green);

        InterrogateButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) || (player.Is(SubFaction) && SubFaction != SubFaction.None)) &&
        GameModifiers.FactionSeeRoles) || (Player.IsOtherLover(player) && CustomGameOptions.LoversRoles) || (Player.IsOtherRival(player) && CustomGameOptions.RivalsRoles) ||
        (player.Is(LayerEnum.Mafia) && Player.Is(LayerEnum.Mafia) && CustomGameOptions.MafiaRoles) || (Player.IsOtherLink(player) && CustomGameOptions.LinkedRoles);
}