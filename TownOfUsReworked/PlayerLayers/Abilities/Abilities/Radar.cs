namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Radar : Ability
{
    private CustomArrow RadarArrow { get; set; }

    public override UColor Color => ClientOptions.CustomAbColors ? CustomColorManager.Radar : CustomColorManager.Ability;
    public override LayerEnum Type => LayerEnum.Radar;
    public override Func<string> Description => () => "- You are aware of those close to you";

    protected override void Init() => RadarArrow = new(Player, Color, Target);

    public override void ClearArrows() => RadarArrow?.Destroy();

    public override void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer) => ClearArrows();

    private Vector3 Target()
    {
        if (Dead)
            return Player.transform.position;

        var pos = Player.GetTruePosition();
        var player = GetClosestPlayer(pos, predicate: x => x != Player, ignoreWalls: true, maxDistance: float.MaxValue);
        var body = GetClosestBody(pos, ignoreWalls: true, maxDistance: float.MaxValue);

        if (body)
        {
            if (player)
                pos = Vector2.Distance(pos, body.TruePosition) < Vector2.Distance(pos, player.GetTruePosition()) ? body.TruePosition : player.GetTruePosition();
            else
                pos = body.TruePosition;
        }
        else if (player)
            pos = player.GetTruePosition();

        return pos;
    }
}