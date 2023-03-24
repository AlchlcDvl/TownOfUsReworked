using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace TownOfUsReworked.Cosmetics.CustomVisors
{
    [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetUnlockedVisors))]
    public static class UnlockVisors
    {
        public static bool Prefix(HatManager __instance, ref Il2CppReferenceArray<VisorData> __result)
        {
            __result =
            (
                from v
                in __instance.allVisors.ToArray()
                where v.Free || AmongUs.Data.DataManager.Player.Purchases.GetPurchase(v.ProductId, v.BundleId)
                select v
                into o
                orderby o.displayOrder descending,
                o.name
                select o
            ).ToArray();
            return false;
        }
    }
}