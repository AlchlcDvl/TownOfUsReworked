namespace TownOfUsReworked.IPlayerLayers;

public interface ITrapper : IPlayerLayer
{
    List<byte> Trapped { get; }
    bool Building { get; }

    void TriggerTrap(PlayerControl trapped, PlayerControl trigger, bool isAttack);
}