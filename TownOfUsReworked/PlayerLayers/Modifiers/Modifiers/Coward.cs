namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Coward : Modifier
{
    public override Color32 Color => ClientGameOptions.CustomModColors ? Colors.Coward : Colors.Modifier;
    public override string Name => "Coward";
    public override LayerEnum Type => LayerEnum.Coward;
    public override Func<string> Description => () => "- You cannot report bodies";

    public Coward(PlayerControl player) : base(player) {}

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        __instance.ReportButton.SetActive(false);
    }
}