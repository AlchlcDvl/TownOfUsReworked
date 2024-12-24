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
            RadarArrow.Update(Player.GetClosestMono([ Player.GetClosestPlayer(ignoreWalls: true), Player.GetClosestBody(ignoreWalls: true) ], ignoreWalls: true).transform.position);
    }

    public override void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer) => Deinit();
}