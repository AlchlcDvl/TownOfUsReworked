namespace TownOfUsReworked.PlayerLayers.Roles;

public class Crewmate : Crew
{
    public override string Name => "Crewmate";
    public override LayerEnum Type => LayerEnum.Crewmate;
    public override Func<string> StartText => () => "Do Your Tasks";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.CrewUtil;
        Data.Role.IntroSound = GetAudio("CrewmateIntro");
    }
}