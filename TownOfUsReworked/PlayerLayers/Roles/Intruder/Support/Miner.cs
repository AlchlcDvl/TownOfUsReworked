namespace TownOfUsReworked.PlayerLayers.Roles;

public class Miner : Intruder
{
    public CustomButton MineButton { get; set; }
    public List<Vent> Vents { get; set; }

    public override Color Color => ClientGameOptions.CustomIntColors ? Colors.Miner : Colors.Intruder;
    public override string Name => MapPatches.CurrentMap == 5 ? "Herbalist" : "Miner";
    public override LayerEnum Type => LayerEnum.Miner;
    public override Func<string> StartText => () => MapPatches.CurrentMap == 5 ? "<size=80%>Screw The <color=#8CFFFFFF>Crew</color>, Plants Are Your New Best Friends Now</size>" :
        "From The Top, Make It Drop, Boom, That's A Vent";
    public override Func<string> Description => () => $"- You can mine a vent, forming a vent system of your own\n{CommonAbilities}";

    public Miner(PlayerControl player) : base(player)
    {
        Alignment = Alignment.IntruderSupport;
        MineButton = new(this, SpriteName, AbilityTypes.Targetless, "Secondary", Mine, CustomGameOptions.MineCd);
        Vents = new();
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

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        MineButton.Update2(MapPatches.CurrentMap == 5 ? "PLANT" : "MINE VENT", condition: Condition());
    }
}