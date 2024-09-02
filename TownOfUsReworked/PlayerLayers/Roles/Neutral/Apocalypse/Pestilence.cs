namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Pestilence : Neutral
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float ObliterateCd { get; set; } = 25f;

    [NumberOption(MultiMenu.LayerSubOptions, 2, 10, 1)]
    public static int MaxStacks { get; set; } = 4;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool PestVent { get; set; } = true;

    private CustomButton ObliterateButton { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Pestilence : CustomColorManager.Neutral;
    public override string Name => "Pestilence";
    public override LayerEnum Type => LayerEnum.Pestilence;
    public override Func<string> StartText => () => "THE APOCALYPSE IS NIGH";
    public override Func<string> Description => () => "- You can spread a deadly disease to kill everyone";
    public override DefenseEnum DefenseVal => DefenseEnum.Invincible;

    public static readonly Dictionary<byte, int> Infected = [];

    public override void Init()
    {
        BaseStart();
        Objectives = () => "- Obliterate anyone who can oppose you";
        Alignment = Alignment.NeutralApoc;
        ObliterateButton = CreateButton(this, new SpriteName("Obliterate"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Obliterate, (PlayerBodyExclusion)Exception, "OBLITERATE",
            new Cooldown(ObliterateCd));
    }

    private void Obliterate()
    {
        Interact(Player, ObliterateButton.TargetPlayer);
        ObliterateButton.StartCooldown();
    }

    private bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);
}