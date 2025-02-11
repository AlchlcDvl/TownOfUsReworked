namespace TownOfUsReworked.IPlayerLayers;

public interface ITrapper : IRole
{
    List<byte> Trapped { get; }
    bool Building { get; }

    void TriggerTrap(PlayerControl trapped, PlayerControl trigger, bool isAttack);
}