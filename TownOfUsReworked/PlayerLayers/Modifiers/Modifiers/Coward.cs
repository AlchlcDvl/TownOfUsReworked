namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Coward : Modifier
{
    public override UColor Color => ClientGameOptions.CustomModColors ? CustomColorManager.Coward : CustomColorManager.Modifier;
    public override string Name => "Coward";
    public override LayerEnum Type => LayerEnum.Coward;
    public override Func<string> Description => () => "- You cannot report bodies";

    public Coward() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        return this;
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        __instance.ReportButton.SetActive(false);
    }
}