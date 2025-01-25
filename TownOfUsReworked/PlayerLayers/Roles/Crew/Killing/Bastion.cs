namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Bastion : Crew, IVentBomber
{
    [NumberOption(0, 15, 1, zeroIsInf: true)]
    public static Number MaxBombs = 5;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number BastionCd = 25;

    [ToggleOption]
    public static bool BombRemovedOnKill = true;

    public CustomButton BombButton { get; set; }
    public List<int> BombedIDs { get; } = [];

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Bastion : FactionColor;
    public override LayerEnum Type => LayerEnum.Bastion;
    public override Func<string> StartText => () => "Place Traps To Deter Venters";
    public override Func<string> Description => () => "- You can place traps in vents, which trigger and kill whenever someone uses the vent the trap is in";
    public override AttackEnum AttackVal => AttackEnum.Powerful;

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Killing;
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

    public bool Exception(Vent vent) => BombedIDs.Contains(vent.Id);

    public void Bomb(Vent target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            BombedIDs.Add(target.Id);
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, target);
        }

        BombButton.StartCooldown(cooldown);
    }

    public override void ReadRPC(MessageReader reader) => BombedIDs.Add(reader.ReadInt32());
}