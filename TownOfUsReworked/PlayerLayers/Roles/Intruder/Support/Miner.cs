namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Miner : Intruder
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number MineCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool MinerSpawnOnMira { get; set; } = true;

    public CustomButton MineButton { get; set; }
    public List<Vent> Vents { get; set; }

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Miner : CustomColorManager.Intruder;
    public override string Name => MapPatches.CurrentMap == 5 ? "Herbalist" : "Miner";
    public override LayerEnum Type => LayerEnum.Miner;
    public override Func<string> StartText => () => MapPatches.CurrentMap == 5 ? "<size=80%>Screw The <color=#8CFFFFFF>Crew</color>, Plants Are Your New Best Friends Now</size>" :
        "From The Top, Make It Drop, Boom, That's A Vent";
    public override Func<string> Description => () => $"- You can mine a vent, forming a vent system of your own\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.IntruderSupport;
        MineButton ??= CreateButton(this, new SpriteName(SpriteName), AbilityType.Targetless, KeybindType.Secondary, (OnClick)Mine, new Cooldown(MineCd), (LabelFunc)Label,
            (ConditionFunc)Condition);
        Vents = [];
    }

    public static string SpriteName => MapPatches.CurrentMap switch
    {
        5 => "PlantPlant",
        _ => "Mine"
    };

    public void Mine()
    {
        RpcSpawnVent(this);
        MineButton.StartCooldown();
    }

    public bool Condition() => !Physics2D.OverlapBoxAll(Player.transform.position, GetSize(), 0).Any(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer is not (8 or 5)) &&
        Player.moveable && !GetPlayerElevator(Player).IsInElevator && !Vents.Any(x => x.transform.position == Player.transform.position);

    public static string Label() => MapPatches.CurrentMap == 5 ? "PLANT" : "MINE VENT";
}