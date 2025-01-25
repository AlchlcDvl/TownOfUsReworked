namespace TownOfUsReworked.IPlayerLayers;

public interface IIntimidator : IRole
{
    PlayerControl Target { get; set; }
    bool ShookAlready { get; set; }
}