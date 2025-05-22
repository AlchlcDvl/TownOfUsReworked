namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Miner)]
public sealed class Miner : Intruder
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number MineCd = 25;

    [ToggleOption]
    public static bool MinerSpawnOnMira = true;

    private CustomButton MineButton { get; set; }
    public List<Vent> Vents { get; } = [];

    protected override UColor MainColor => CustomColorManager.Miner;
    public override LayerEnum Type => LayerEnum.Miner;
    public override Func<string> StartText { get; } = () => MapPatches.CurrentMap == 5 ? "<size=80%>Screw The <#8CFFFFFF>Crew</color>, Plants Are Your New Best Friends Now</size>" :
        "From The Top, Make It Drop, Boom, That's A Vent";
    public override Func<string> Description => () => $"- You can mine a vent, forming a vent system of your own\n{CommonAbilities}";

    protected override void Init()
    {
        base.Init();
        Name = TranslationManager.Translate($"Layer.{(MapPatches.CurrentMap == 5 ? "Herbalist" : "Miner")}");
        Alignment = Alignment.Support;
        MineButton ??= new(this, new SpriteName(SpriteName), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)Mine, new Cooldown(MineCd), (LabelFunc)Label,
            (ConditionFunc)Condition);
        Vents.Clear();
    }

    public static string SpriteName => MapPatches.CurrentMap switch
    {
        5 => "PlantPlant",
        _ => "Mine"
    };

    private void Mine()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, (Vector2)Player.transform.position);
        Vents.Add(SpawnVent(Vents, Player.transform.position, Player.transform.position.z));
        MineButton.StartCooldown();
    }

    private bool Condition() => !Physics2D.OverlapBoxAll(Player.transform.position, GetSize(), 0).Any(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer is not (8 or 5)) &&
        Player.moveable && !GetPlayerElevator(Player).IsInElevator && Vents.All(x => x.transform.position != Player.transform.position);

    public static string Label() => MapPatches.CurrentMap == 5 ? "PLANT" : "MINE VENT";

    public override void ReadRPC(NetData reader) => Vents.Add(SpawnVent(Vents, reader.ReadVector2(), Player.transform.position.z));

    private static Vent SpawnVent(List<Vent> vents, Vector2 position, float zAxis)
    {
        var ventPrefab = vents.FirstOrDefault() ?? UObject.FindObjectOfType<Vent>();
        var vent = UObject.Instantiate(ventPrefab, ventPrefab.transform.parent);

        vent.Id = GetAvailableVentId();
        vent.transform.position = new(position.x, position.y, zAxis + 0.001f);

        if (vents.Any())
        {
            var leftVent = vents[^1];
            vent.Left = leftVent;
            leftVent.Right = vent;
        }
        else
            vent.Left = null;

        vent.Right = null;
        vent.Center = null;
        vent.name = $"MinerVent{vents.Count}";
        vent.myAnim?.Stop();

        var allVents = AllMapVents().ToList();
        allVents.Add(vent);
        Ship().AllVents = allVents.ToArray();

        if (IsSubmerged())
        {
            vent.gameObject.layer = 12;
            vent.gameObject.AddSubmergedComponent("ElevatorMover"); // Just in case the elevator vent is not blocked

            if (vent.transform.position.y > -7)
                vent.transform.SetWorldZ(0.03f);
            else
            {
                vent.transform.SetWorldZ(0.0009f);
                vent.transform.SetLocalZ(-0.003f);
            }
        }

        return vent;
    }
}