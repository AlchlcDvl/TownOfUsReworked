namespace TownOfUsReworked.IPlayerLayers;

public interface ITrapper : IPlayerLayer
{
    public List<byte> Trapped { get; set; }

    void TriggerTrap(PlayerControl trapped, PlayerControl trigger, bool isAttack);
}