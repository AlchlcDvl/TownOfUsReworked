namespace TownOfUsReworked.PlayerLayers.Roles;

public class Miner : Intruder
{
    public CustomButton MineButton { get; set; }
    public List<Vent> Vents { get; set; }

    public override UColor Color => ClientGameOptions.CustomIntColors ? CustomColorManager.Miner : CustomColorManager.Intruder;
    public override string Name => MapPatches.CurrentMap == 5 ? "Herbalist" : "Miner";
    public override LayerEnum Type => LayerEnum.Miner;
    public override Func<string> StartText => () => MapPatches.CurrentMap == 5 ? "<size=80%>Screw The <color=#8CFFFFFF>Crew</color>, Plants Are Your New Best Friends Now</size>" :
        "From The Top, Make It Drop, Boom, That's A Vent";
    public override Func<string> Description => () => $"- You can mine a vent, forming a vent system of your own\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.IntruderSupport;
        MineButton = CreateButton(this, new SpriteName(SpriteName), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)Mine, new Cooldown(CustomGameOptions.MineCd), (LabelFunc)Label,
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

    public bool Condition()
    {
        var hits = Physics2D.OverlapBoxAll(Player.transform.position, GetSize(), 0);
        hits = hits.Where(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer is not (8 or 5)).ToArray();
        return hits.Count == 0 && Player.moveable && !GetPlayerElevator(Player).IsInElevator && !Vents.Any(x => x.transform.position == Player.transform.position);
    }

    public static string Label() => MapPatches.CurrentMap == 5 ? "PLANT" : "MINE VENT";
}