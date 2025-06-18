namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Pestilence)]
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
    public override Layer Type => Layer.Pestilence;
    public override string Description => "- You can spread a deadly disease to other players" + CommonAbilities;
    public override bool CanVent => base.CanVent && PestVent;

    public static readonly Dictionary<byte, byte> Infected = [];

    public override void Init()
    {
        base.Init();
        ObliterateButton ??= new(this, new SpriteName("Obliterate"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Obliterate, (PlayerBodyExclusion)Exception, "OBLITERATE",
            new Cooldown(ObliterateCd));
        AllPlayers().Where(x => !x.Is<Apocalypse>()).Do(x => Infected[x.PlayerId] = 1);
    }

    private void Obliterate(PlayerControl target)
    {
        Interact(Player, target);
        ObliterateButton.StartCooldown();
    }

    private bool Exception(PlayerControl player) => Player.IsBuddyWith(player, Handler.CurrentFaction);
}