namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Pestilence)]
public sealed class Pestilence : Deity
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number ObliterateCd = 25;

    [NumberOption(2, 10, 1)]
    public static Number MaxStacks = 4;

    [ToggleOption]
    private static bool PestVent = true;

    private CustomButton ObliterateButton { get; set; }

    protected override UColor MainColor => CustomColorManager.Pestilence;
    public override LayerEnum Type { get; } = LayerEnum.Pestilence;
    public override Func<string> Description => () => "- You can spread a deadly disease to other players" + CommonAbilities;
    public override bool CanVent => base.CanVent && PestVent;

    public static readonly Dictionary<byte, uint> Infected = [];

    protected override void Init()
    {
        base.Init();
        ObliterateButton ??= new(this, new SpriteName("Obliterate"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Obliterate, (PlayerBodyExclusion)Exception, "OBLITERATE",
            new Cooldown(ObliterateCd));

        foreach (var player in AllPlayers())
        {
            if (player.GetAlignment() is not (Alignment.Deity or Alignment.Harbinger))
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