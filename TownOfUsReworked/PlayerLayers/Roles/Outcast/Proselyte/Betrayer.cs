namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Betrayer)]
public sealed class Betrayer : Proselyte
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number BetrayCd = 25;

    [ToggleOption]
    private static bool BetrayerVent = true;

    private CustomButton KillButton;

    protected override UColor MainColor => CustomColorManager.Betrayer;
    public override LayerEnum Type => LayerEnum.Betrayer;
    public override string StartText => "Those Backs Are Ripe For Some Stabbing";
    public override string Description => "- You can kill";
    public override AttackEnum AttackVal => AttackEnum.Basic;
    public override bool CanVent => base.CanVent && BetrayerVent;

    public override void Init()
    {
        base.Init();
        Objectives = () => $"- Kill anyone who opposes the {FactionName}";
        KillButton ??= new(this, new SpriteName("BetrayerKill"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Kill, new Cooldown(BetrayCd), "BACKSTAB",
            (PlayerBodyExclusion)Exception);
    }

    private void Kill(PlayerControl target) => KillButton.StartCooldown(Interact(Player, target, true));

    private bool Exception(PlayerControl player) => Player.IsBuddyWith(player, Faction);
}