using UnityEngine;
using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.Modules
{
    [HarmonyPatch]
    public class VisualAppearance
    {
        public float SpeedFactor;
        public Vector3 SizeFactor;
        public PlayerControl Player;

        public VisualAppearance(PlayerControl player)
        {
            Player = player;
            SizeFactor = new(0.7f, 0.7f, 1f);
            SpeedFactor = Player.Data.IsDead && !Player.UnCaught() ? CustomGameOptions.GhostSpeed : CustomGameOptions.PlayerSpeed;
        }
    }

    public interface IVisualAlteration
    {
        bool TryGetModifiedAppearance(out VisualAppearance appearance);
    }
}