namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Troll)]
public sealed class Troll : Evil
{
    [ToggleOption]
    private static bool CanInteract = true;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number InteractCd = 25;

    [ToggleOption]
    private static bool TrollVent = false;

    [ToggleOption]
    private static bool TrollSwitchVent = false;

    private CustomButton InteractButton;

    protected override UColor MainColor => CustomColorManager.Troll;
    public override Layer Type => Layer.Troll;
    public override string StartText => "Troll Everyone With Your Death";
    public override string Description => "- If you are killed, you will also kill your killer" + (CanInteract ? "\n- You can interact with players\n- Your interactions do nothing "
        + "except spread infection and possibly kill you via touch sensitive roles" : string.Empty);
    public override Attack Attack => Attack.Unstoppable;
    public override bool HasWon => Handler.DeathReason is not (DeathReasonEnum.Alive or DeathReasonEnum.Ejected or DeathReasonEnum.Guessed or DeathReasonEnum.Revived);
    public override bool CanVent => base.CanVent && TrollVent;
    public override bool CanSwitchVents => TrollSwitchVent;
    protected override WinLose EndState => WinLose.TrollWins;

    public override void Init()
    {
        Objectives = () => HasWon ? "- You have successfully trolled someone" : "- Get killed";
        InteractButton ??= new(this, new SpriteName("Interact"), ReworkedAbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Interact, new Cooldown(InteractCd), "INTERACT",
            (UsableFunc)Usable);
    }

    public override void OnDeath(DeathReasonEnum reason, PlayerControl killer)
    {
        if (!OutcastSettings.AvoidOutcastKingmakers)
            Player.MurderPlayer(killer, DeathReasonEnum.Trolled, false);
    }

    private void Interact(PlayerControl target) => InteractButton.StartCooldown(InteractionManager.Interact(Player, target));

    private static bool Usable() => CanInteract;
}