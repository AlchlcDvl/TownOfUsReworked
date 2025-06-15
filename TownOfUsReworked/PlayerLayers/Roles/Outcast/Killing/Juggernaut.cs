namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Juggernaut)]
public sealed class Juggernaut : OKilling
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number AssaultCd = 25;

    [NumberOption(2.5f, 30f, 2.5f, Format.Time)]
    private static Number AssaultBonus = 5;

    [ToggleOption]
    private static bool JuggVent = false;

    private CustomButton AssaultButton;

    protected override UColor MainColor => CustomColorManager.Juggernaut;
    public override Layer Type => Layer.Juggernaut;
    public override string StartText => "Your Power Grows With Every Kill";
    public override string Description => "- With each kill, your kill cooldown decreases" + (KillCounts.GetValueOrDefault(PlayerId) >= 4 ? "\n- You can bypass all forms of protection" : "");
    public override Attack Attack => (Attack)Mathf.Clamp(KillCounts.GetValueOrDefault(PlayerId), 1, 3);
    public override Defense Defense => KillCounts.GetValueOrDefault(PlayerId) >= 3 ? Defense.Basic : Defense.None;
    public override bool CanVent => base.CanVent && JuggVent;
    protected override Faction ActualFaction => Faction.Juggernaut;

    public override void Init()
    {
        base.Init();
        Objectives = () => "- Assault anyone who can oppose you";
        AssaultButton ??= new(this, new SpriteName("Assault"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Assault, new Cooldown(AssaultCd), (DifferenceFunc)Difference,
            (PlayerBodyExclusion)Exception, "ASSAULT");
    }

    private void Assault(PlayerControl target)
    {
        var cooldown = Interact(Player, target, true, bypass: KillCounts.GetValueOrDefault(PlayerId) >= 4);

        if (KillCounts.GetValueOrDefault(PlayerId) == 4)
            Flash(Color);

        AssaultButton.StartCooldown(cooldown);
    }

    private bool Exception(PlayerControl player) => Player.IsBuddyWith(player, Faction);

    private float Difference() => -(AssaultBonus * KillCounts.GetValueOrDefault(PlayerId));
}