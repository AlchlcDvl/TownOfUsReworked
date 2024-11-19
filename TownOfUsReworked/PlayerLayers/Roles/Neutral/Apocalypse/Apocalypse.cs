namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Apocalypse : Neutral
{
    public override Func<string> StartText => () => "THE APOCALYPSE IS NIGH";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.NeutralApoc;

        if (NeutralApocalypseSettings.PlayersAlerted)
            Flash(Color);
    }
}