namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Radar : Ability
{
    private CustomArrow RadarArrow { get; set; }

    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Radar : CustomColorManager.Ability;
    public override string Name => "Radar";
    public override LayerEnum Type => LayerEnum.Radar;
    public override Func<string> Description => () => "- You are aware of those close to you";

    public override void Init() => RadarArrow = new(Player, Color);

    public override void Deinit() => RadarArrow?.Destroy();

    public override void UpdateHud(HudManager __instance)
    {
        if (!Dead)
        {
            var closest = Player.GetClosestPlayer(ignoreWalls: true);
            var body = Player.GetClosestBody(ignoreWalls: true);
            var transform = body && Vector2.Distance(closest.transform.position, Player.transform.position) > Vector2.Distance(body.transform.position, Player.transform.position) ?
                body.transform : closest.transform;
            RadarArrow.Update(transform.position);
        }
    }

    public override void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer) => Deinit();
}