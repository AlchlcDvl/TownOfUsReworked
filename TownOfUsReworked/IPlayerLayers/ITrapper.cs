namespace TownOfUsReworked.IPlayerLayers;

public interface ITrapper : IRole
{
    List<byte> Trapped { get; set; }

    void TriggerTrap(PlayerControl trapped, PlayerControl trigger, bool isAttack);
}