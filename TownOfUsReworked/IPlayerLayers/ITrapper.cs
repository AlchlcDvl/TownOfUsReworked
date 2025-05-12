namespace TownOfUsReworked.IPlayerLayers;

public interface ITrapper : IPlayerLayer
{
    HashSet<byte> Trapped { get; }
    bool Building { get; }

    void TriggerTrap(PlayerControl trapped, PlayerControl trigger, bool isAttack);
}