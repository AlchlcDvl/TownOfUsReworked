using UnityEngine;
using HarmonyLib;

namespace TownOfUsReworked.Classes
{
    [HarmonyPatch]
    public class VisualAppearance
    {
        public float SpeedFactor = 1f;
        public Vector3 SizeFactor = new(0.7f, 0.7f, 1f);
        public PlayerControl Player;
    }

    public interface IVisualAlteration
    {
        bool TryGetModifiedAppearance(out VisualAppearance appearance);
    }
}