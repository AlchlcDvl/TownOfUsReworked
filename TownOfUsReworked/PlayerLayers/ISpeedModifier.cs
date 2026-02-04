namespace TownOfUsReworked.PlayerLayers;

public interface ISpeedModifier
{
    public static readonly List<ISpeedModifier> AllModifiers = [];

    void ModifySpeed(PlayerControl player, ref float result);
}