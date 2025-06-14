namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Cultist)]
public sealed class Cultist : Harbinger<Void>
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number SacrificeCd = 25;

    [ToggleOption]
    private static bool CultVent = false;

    private CustomButton SacrificeButton { get; set; }

    public override LayerEnum Type => LayerEnum.Cultist;
    public override string StartText => "Spread The Words Of Your Lord";
    public override string Description => "<#99007FFF>- You can kill players</color>" + CommonAbilities;
    public override AttackEnum AttackVal => AttackEnum.Basic;
    public override DefenseEnum DefenseVal => GetLayers<Apocalypse>().Count() > 1 ? DefenseEnum.None : DefenseEnum.Basic;
    public override bool CanVent => base.CanVent && CultVent;

    public override void Init()
    {
        base.Init();
        SacrificeButton ??= new(this, (SpriteFunc)GetSpriteName, AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Sacrifice, (PlayerBodyExclusion)Exception, "SACRIFICE",
            new Cooldown(SacrificeCd));
    }

    private string GetSpriteName() => $"{Faction}Kill";

    private void Sacrifice(PlayerControl target) => SacrificeButton.StartCooldown(Interact(Player, target, true));

    private bool Exception(PlayerControl player) => Player.IsBuddyWith(player, Faction);

    protected override bool CanTransform() => KillCounts.TryGetValue(PlayerId, out var count) && count >= 3;
}