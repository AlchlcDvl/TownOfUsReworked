namespace TownOfUsReworked.PlayerLayers.Abilities;

public sealed class Radar : Ability
{
    private CustomArrow RadarArrow { get; set; }

    protected override UColor MainColor => CustomColorManager.Radar;
    public override LayerEnum Type => LayerEnum.Radar;
    public override Func<string> Description => () => "- You are aware of those close to you";

    protected override void Init() => RadarArrow = new(Player, Color, Target);

    public override void ClearArrows() => RadarArrow?.Destroy();

    public override void OnDeath(DeathReasonEnum reason, PlayerControl killer) => ClearArrows();

    private Vector3 Target()
    {
        if (Dead)
            return Player.transform.position;

        var pos = Player.GetTruePosition();
        var player = GetClosestPlayer(pos, maxDistance: float.MaxValue, ignoreWalls: true, predicate: x => x != Player);
        var body = GetClosestBody(pos, maxDistance: float.MaxValue, ignoreWalls: true);

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