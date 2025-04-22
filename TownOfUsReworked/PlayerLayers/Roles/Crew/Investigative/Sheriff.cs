namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Sheriff)]
public sealed class Sheriff : Crew
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number InterrogateCd = 25;

    [ToggleOption]
    public static bool NeutEvilRed = false;

    [ToggleOption]
    public static bool NeutKillingRed = false;

    [ToggleOption]
    public static bool NeutNeophyteRed = false;

    private CustomButton InterrogateButton { get; set; }

    protected override UColor MainColor => CustomColorManager.Sheriff;
    public override LayerEnum Type => LayerEnum.Sheriff;
    public override Func<string> StartText { get; } = () => "Reveal The Alignment Of Other Players";
    public override Func<string> Description => () => "- You can reveal alignments of other players relative to the <#8CFFFFFF>Crew</color>";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Investigative;
        InterrogateButton ??= new(this, "INTERROGATE", new SpriteName("Interrogate"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Interrogate, new Cooldown(InterrogateCd),
            (PlayerBodyExclusion)Exception);
    }

    private void Interrogate(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Flash(target.SeemsEvil() ? UColor.red : UColor.green);

        InterrogateButton.StartCooldown(cooldown);
    }

    private bool Exception(PlayerControl player) => (((Faction is not (Faction.Crew or Faction.Neutral) && player.Is(Faction)) || (player.Is(SubFaction) && SubFaction != SubFaction.None)) &&
        GameModifiers.FactionSeeRoles) || (Player.IsOtherLover(player) && Lovers.LoversRoles) || (Player.IsOtherRival(player) && Rivals.RivalsRoles) || (player.Is<Mafia>() && Player.Is<Mafia>()
        && Mafia.MafiaRoles) || (Player.IsOtherLink(player) && Linked.LinkedRoles);
}