namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Radar : Ability
{
    private CustomArrow RadarArrow { get; set; }

    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Radar : CustomColorManager.Ability;
    public override LayerEnum Type => LayerEnum.Radar;
    public override Func<string> Description => () => "- You are aware of those close to you";

    public override void Init() => RadarArrow = new(Player, Color, Target);

    public override void ClearArrows() => RadarArrow?.Destroy();

    public override void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer) => ClearArrows();

    public Vector3 Target()
    {
        if (Dead)
            return Player.transform.position;

        var pos = Player.GetTruePosition();
        var player = GetClosestPlayer(pos, predicate: x => x != Player, ignoreWalls: true, maxDistance: float.MaxValue);
        var body = GetClosestBody(pos, ignoreWalls: true, maxDistance: float.MaxValue);

        if (body)
        {
            if (player)
            {
                var distance1 = Vector2.Distance(pos, body.TruePosition);
                var distance2 = Vector2.Distance(pos, player.GetTruePosition());
                pos = distance1 < distance2 ? body.TruePosition : player.GetTruePosition();
            }
            else
                pos = body.TruePosition;
        }
        else if (player)
            pos = player.GetTruePosition();

        return pos;
    }
}