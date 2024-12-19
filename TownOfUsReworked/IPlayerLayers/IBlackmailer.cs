namespace TownOfUsReworked.IPlayerLayers;

public interface IBlackmailer : IIntimidator
{
    PlayerControl BlackmailedPlayer { get; set; }
}