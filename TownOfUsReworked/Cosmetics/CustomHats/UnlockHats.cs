using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace TownOfUsReworked.Cosmetics.CustomHats
{
    [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetUnlockedHats))]
    public static class UnlockHats
    {
        public static bool Prefix(HatManager __instance, ref Il2CppReferenceArray<HatData> __result)
        {
            __result =
            (
                from h
                in __instance.allHats.ToArray()
                where h.Free || AmongUs.Data.DataManager.Player.Purchases.GetPurchase(h.ProductId, h.BundleId)
                select h
                into o
                orderby o.displayOrder descending,
                o.name
                select o
            ).ToArray();
            return false;
        }
    }
}