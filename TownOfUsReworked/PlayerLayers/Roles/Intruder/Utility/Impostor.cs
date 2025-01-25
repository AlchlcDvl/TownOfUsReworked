namespace TownOfUsReworked.PlayerLayers.Roles;

public class Impostor : Intruder
{
    public override LayerEnum Type => LayerEnum.Impostor;
    public override Func<string> StartText => () => "Sabotage And Kill Everyone";
    public override Func<string> Description => () => CommonAbilities;

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Utility;
    }
}