namespace TownOfUsReworked.PlayerLayers.Roles;

public class Trickster : Role
{
    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Trickster : FactionColor;
    public override LayerEnum Type => LayerEnum.Trickster;
    public override Func<string> StartText => () => "";
    public override Func<string> Description => () => "";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Protective;
    }
}