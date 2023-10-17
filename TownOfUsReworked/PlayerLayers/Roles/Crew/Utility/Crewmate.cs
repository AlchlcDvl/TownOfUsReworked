namespace TownOfUsReworked.PlayerLayers.Roles;

public class Crewmate : Crew
{
    public override string Name => "Crewmate";
    public override LayerEnum Type => LayerEnum.Crewmate;
    public override Func<string> StartText => () => "Do Your Tasks";

    public Crewmate(PlayerControl player) : base(player)
    {
        Alignment = Alignment.CrewUtil;
        player.Data.Role.IntroSound = GetAudio("CrewmateIntro");
    }
}