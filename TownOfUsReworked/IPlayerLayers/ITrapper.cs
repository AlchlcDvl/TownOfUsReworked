namespace TownOfUsReworked.IPlayerLayers;

public interface ITrapper : IRole
{
    List<byte> Trapped { get; }

    void TriggerTrap(PlayerControl trapped, PlayerControl trigger, bool isAttack);
}