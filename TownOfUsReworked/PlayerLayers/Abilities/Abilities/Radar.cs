namespace TownOfUsReworked.PlayerLayers.Abilities;

public sealed class Radar : Ability
{
    private CustomArrow RadarArrow;

    protected override UColor MainColor => CustomColorManager.Radar;
    public override LayerEnum Type => LayerEnum.Radar;
    public override string Description => "- You are aware of those close to you";

    public override void Init() => RadarArrow = new(Player, Color, Target);

    protected override void ClearArrows() => RadarArrow?.Destroy();

    public override void OnDeath(DeathReasonEnum reason, PlayerControl killer) => ClearArrows();

    private Vector3 Target() => (Dead ? Player : GetClosestMono(Player.GetTruePosition(), [ .. AllBodies(), .. AllPlayers() ], float.MaxValue, true, x => x != Player)).transform.position;
}