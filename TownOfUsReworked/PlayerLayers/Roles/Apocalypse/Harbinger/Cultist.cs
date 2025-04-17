namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Cultist)]
public sealed class Cultist : Harbinger<Void>
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number SacrificeCd = 25;

    [ToggleOption]
    private static bool CultVent = false;

    private CustomButton SacrificeButton { get; set; }

    public override LayerEnum Type { get; } = LayerEnum.Cultist;
    public override Func<string> StartText { get; } = () => "Spread The Words Of Your Lord";
    public override Func<string> Description => () => "<#99007FFF>- You can kill players</color>" + CommonAbilities;
    public override DefenseEnum DefenseVal => GetLayers<Apocalypse>().Count() > 1 ? DefenseEnum.None : DefenseEnum.Basic;
    public override bool CanVent => base.CanVent && CultVent;

    protected override void Init()
    {
        base.Init();
        SacrificeButton ??= new(this, (SpriteFunc)GetSpriteName, AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Sacrifice, (PlayerBodyExclusion)Exception, "SACRIFICE",
            new Cooldown(SacrificeCd));
    }

    private string GetSpriteName() => $"{Faction}Kill";

    public void Sacrifice(PlayerControl target) => SacrificeButton.StartCooldown(Interact(Player, target, true));

    private bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is not (Faction.Crew or Faction.Neutral)) ||
        Player.IsLinkedTo(player);

    protected override bool CanTransform() => KillCounts.TryGetValue(PlayerId, out var count) && count >= 3;
}