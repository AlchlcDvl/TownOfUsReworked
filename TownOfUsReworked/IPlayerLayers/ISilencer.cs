namespace TownOfUsReworked.IPlayerLayers;

public interface ISilencer : IIntimidator, ISyndicate
{
    PlayerControl SilencedPlayer { get; }
}