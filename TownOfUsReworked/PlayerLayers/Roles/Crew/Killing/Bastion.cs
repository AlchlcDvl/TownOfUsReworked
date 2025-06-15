namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Bastion)]
public sealed class Bastion : CKilling, IVentBomber
{
    [NumberOption(0, 15, 1, zeroIsInf: true)]
    public static Number MaxBombs = 5;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number BastionCd = 25;

    [ToggleOption]
    public static bool BombRemovedOnKill = true;

    private CustomButton BombButton;
    public List<int> BombedIDs { get; } = [];

    protected override UColor MainColor => CustomColorManager.Bastion;
    public override Layer Type => Layer.Bastion;
    public override string StartText => "Place Traps To Deter Venters";
    public override string Description => "- You can place traps in vents, which trigger and kill whenever someone uses the vent the trap is in";
    public override Attack Attack => Attack.Powerful;

    public override void Init()
    {
        base.Init();
        BombedIDs.Clear();
        BombButton ??= new(this, "PLACE BOMB", new SpriteName($"{SpriteName}VentBomb"), AbilityTypes.Vent, KeybindType.ActionSecondary, (OnClickVent)Bomb, new Cooldown(BastionCd), MaxBombs,
            (VentExclusion)Exception);
    }

    public static string SpriteName => MapPatches.CurrentMap switch
    {
        2 => "Polus",
        5 => "Plant",
        _ => "Metal"
    };

    private bool Exception(Vent vent) => BombedIDs.Contains(vent.Id);

    private void Bomb(Vent target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            BombedIDs.Add(target.Id);
            CallRpc(ActionsRpc.LayerAction, this, target);
        }

        BombButton.StartCooldown(cooldown);
    }

    public override void ReadRPC(RpcReader reader) => BombedIDs.Add(reader.ReadInt());
}