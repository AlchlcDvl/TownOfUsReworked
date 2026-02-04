namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Cultist)]
public sealed class Cultist : Harbinger<Void>
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number SacrificeCd = 25;

    [ToggleOption]
    private static bool CultVent = false;

    private CustomButton SacrificeButton;

    public override Layer Type => Layer.Cultist;
    public override string StartText => "Spread The Words Of Your Lord";
    public override string Description => "<#99007FFF>- You can kill players</color>" + CommonAbilities;
    public override Attack Attack => Attack.Basic;
    public override Defense Defense => GetLayers<Apocalypse>().Count() > 1 ? Defense.None : Defense.Basic;
    public override bool CanVent => base.CanVent && CultVent;

    public override void Init() => SacrificeButton ??= new(this, (SpriteFunc)GetSpriteName, ReworkedAbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Sacrifice, new Cooldown(SacrificeCd),
        (PlayerBodyExclusion)Exception, "SACRIFICE");

    private string GetSpriteName() => $"{Handler.CurrentFaction}Kill";

    private void Sacrifice(PlayerControl target) => SacrificeButton.StartCooldown(Interact(Player, target, true));

    private bool Exception(PlayerControl player) => Player.IsBuddyWith(player, Handler.CurrentFaction);

    protected override bool CanTransform() => KillCounts.TryGetValue(PlayerId, out var count) && count >= 3;
}