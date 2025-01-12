namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Pestilence : Apocalypse
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number ObliterateCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 2, 10, 1)]
    public static Number MaxStacks { get; set; } = new(4);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool PestVent { get; set; } = true;

    private CustomButton ObliterateButton { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Pestilence : FactionColor;
    public override LayerEnum Type => LayerEnum.Pestilence;
    public override Func<string> Description => () => "- You can spread a deadly disease to other players";

    public static readonly Dictionary<byte, int> Infected = [];

    public override void Init()
    {
        base.Init();
        Objectives = () => "- Obliterate anyone who can oppose you";
        ObliterateButton ??= new(this, new SpriteName("Obliterate"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Obliterate, (PlayerBodyExclusion)Exception, "OBLITERATE",
            new Cooldown(ObliterateCd));

        foreach (var player in AllPlayers())
        {
            if (player.GetAlignment() is not (Alignment.NeutralApoc or Alignment.NeutralHarb))
                Infected[player.PlayerId] = 1;
        }
    }

    private void Obliterate(PlayerControl target)
    {
        Interact(Player, target);
        ObliterateButton.StartCooldown();
    }

    private bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);
}