namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Drunk : Modifier
{
    private static float _time;
    public int Modify { get; set; }
    private bool Exposed { get; set; }

    public override UColor Color => ClientOptions.CustomModColors ? CustomColorManager.Drunk : CustomColorManager.Modifier;
    public override string Name => "Drunk";
    public override LayerEnum Type => LayerEnum.Drunk;
    public override Func<string> Description => () => CustomGameOptions.DrunkControlsSwap ? "- Your controls swap over time" : "- Your controls are inverted";
    public override bool Hidden => !CustomGameOptions.DrunkKnows && !Exposed && !Dead && CustomGameOptions.DrunkControlsSwap;

    public override void Init()
    {
        Modify = Hidden ? 1 : -1;
        Exposed = false;
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (CustomGameOptions.DrunkControlsSwap)
        {
            _time += Time.deltaTime;

            if (_time >= CustomGameOptions.DrunkInterval)
            {
                _time -= CustomGameOptions.DrunkInterval;
                Modify *= -1;
                Exposed = true;
            }
        }
    }
}