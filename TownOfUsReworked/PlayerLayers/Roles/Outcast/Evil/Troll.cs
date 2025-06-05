namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Troll)]
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

    private CustomButton InteractButton { get; set; }

    protected override UColor MainColor => CustomColorManager.Troll;
    public override LayerEnum Type => LayerEnum.Troll;
    public override Func<string> StartText { get; } = () => "Troll Everyone With Your Death";
    public override Func<string> Description => () => "- If you are killed, you will also kill your killer" + (CanInteract ? "\n- You can interact with players\n- Your interactions do nothing "
        + "except spread infection and possibly kill you via touch sensitive roles" : "");
    public override AttackEnum AttackVal => AttackEnum.Unstoppable;
    public override bool HasWon => Handler.DeathReason is not (DeathReasonEnum.Alive or DeathReasonEnum.Ejected or DeathReasonEnum.Guessed or DeathReasonEnum.Revived);
    public override bool CanVent => base.CanVent && TrollVent;
    public override bool CanSwitchVents => TrollSwitchVent;
    public override WinLose EndState => WinLose.TrollWins;

    protected override void Init()
    {
        base.Init();
        Objectives = () => HasWon ? "- You have successfully trolled someone" : "- Get killed";
        InteractButton ??= new(this, new SpriteName("Interact"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Interact, new Cooldown(InteractCd), "INTERACT",
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