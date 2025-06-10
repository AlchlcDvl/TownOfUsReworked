namespace TownOfUsReworked.PlayerLayers.Roles;

public sealed class Impostor : Intruder
{
    public override LayerEnum Type => LayerEnum.Impostor;
    public override Func<string> StartText { get; } = () => "Sabotage And Kill Everyone";
    public override Func<string> Description => () => CommonAbilities;

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Utility;
    }
}