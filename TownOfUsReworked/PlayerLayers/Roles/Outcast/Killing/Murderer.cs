namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Murderer)]
public sealed class Murderer : OKilling
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number MurderCd = 25;

    [ToggleOption]
    private static bool MurdVent = false;

    private CustomButton MurderButton;

    protected override UColor MainColor => CustomColorManager.Murderer;
    public override Layer Type => Layer.Murderer;
    public override string StartText => "I Got Murder On My Mind";
    public override string Description => "- You can kill";
    public override Attack Attack => Attack.Basic;
    public override Defense Defense => Defense.Basic;
    public override bool CanVent => base.CanVent && MurdVent;
    protected override Faction ActualFaction => Faction.Murderer;

    public override void Init()
    {
        base.Init();
        Objectives = () => "- Murder anyone who can oppose you";
        MurderButton ??= new(this, new SpriteName("Murder"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Murder, new Cooldown(MurderCd), "MURDER",
            (PlayerBodyExclusion)Exception);
    }

    private void Murder(PlayerControl target) => MurderButton.StartCooldown(Interact(Player, target, true));

    private bool Exception(PlayerControl player) => Player.IsBuddyWith(player, Faction);
}