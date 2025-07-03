namespace TownOfUsReworked.PlayerLayers.Abilities;

public sealed class Radar : Ability
{
    private CustomArrow RadarArrow;

    protected override UColor MainColor => CustomColorManager.Radar;
    public override Layer Type => Layer.Radar;
    public override string Description => "- You are aware of those close to you";

    public override void Init() => RadarArrow = new(Player, Color, Target);

    public override void ClearArrows() => RadarArrow?.Destroy();

    public override void OnDeath(DeathReasonEnum reason, PlayerControl killer) => ClearArrows();

    private Vector3 Target()
    {
        if (Dead)
            return Player.transform.position;

        var monos = new List<MonoBehaviour>(AllPlayers());
        monos.AddRange(AllBodies());
        var pos = Player.GetTruePosition();
        return monos.OrderBy(x => (pos - (Vector2)x.transform.position).sqrMagnitude).First().transform.position;
    }
}