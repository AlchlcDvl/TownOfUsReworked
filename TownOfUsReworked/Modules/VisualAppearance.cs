namespace TownOfUsReworked.Modules
{
    [HarmonyPatch]
    public class VisualAppearance
    {
        public float SpeedFactor => Player.Data.IsDead && !Player.Caught() ? CustomGameOptions.GhostSpeed : (CustomGameOptions.PlayerSpeed * Player.GetModifiedSpeed());
        public Vector3 SizeFactor => new Vector3(0.7f, 0.7f, 1f) * Player.GetModifiedSize();
        public PlayerControl Player;

        public VisualAppearance(PlayerControl player) => Player = player;
    }

    public interface IVisualAlteration
    {
        bool TryGetModifiedAppearance(out VisualAppearance appearance);
    }
}