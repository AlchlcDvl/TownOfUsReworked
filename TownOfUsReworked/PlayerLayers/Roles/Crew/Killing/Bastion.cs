namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Bastion : Crew
{
    [NumberOption(MultiMenu2.LayerSubOptions, 1, 15, 1)]
    public static int MaxBombs { get; set; } = 5;

    [NumberOption(MultiMenu2.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float BastionCd { get; set; } = 25f;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool BombRemovedOnKill { get; set; } = true;

    public CustomButton BombButton { get; set; }
    public List<int> BombedIDs { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Bastion : CustomColorManager.Crew;
    public override string Name => "Bastion";
    public override LayerEnum Type => LayerEnum.Bastion;
    public override Func<string> StartText => () => "Place Traps To Deter Venters";
    public override Func<string> Description => () => "- You can place traps in vents, which trigger and kill whenever someone uses the vent the trap is in";
    public override AttackEnum AttackVal => AttackEnum.Powerful;

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.CrewKill;
        BombedIDs = [];
        BombButton = CreateButton(this, "PLACE BOMB", new SpriteName($"{SpriteName}VentBomb"), AbilityTypes.Vent, KeybindType.ActionSecondary, (OnClick)Bomb, new Cooldown(BastionCd),
            MaxBombs, (VentExclusion)Exception);
    }

    public static string SpriteName => MapPatches.CurrentMap switch
    {
        2 => "Polus",
        5 => "Plant",
        _ => "Metal"
    };

    public bool Exception(Vent vent) => BombedIDs.Contains(vent.Id);

    public void Bomb()
    {
        var cooldown = Interact(Player, BombButton.TargetVent);

        if (cooldown != CooldownType.Fail)
        {
            BombedIDs.Add(BombButton.TargetVent.Id);
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, BombButton.TargetVent);
        }

        BombButton.StartCooldown(cooldown);
    }

    public override void ReadRPC(MessageReader reader) => BombedIDs.Add(reader.ReadInt32());
}