namespace TownOfUsReworked.PlayerLayers.Roles;

public class Crewmate : Crew
{
    public override string Name => "Crewmate";
    public override LayerEnum Type => LayerEnum.Crewmate;
    public override Func<string> StartText => () => "Do Your Tasks";

    public Crewmate() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Alignment = Alignment.CrewUtil;
        Data.Role.IntroSound = GetAudio("CrewmateIntro");
        return this;
    }
}