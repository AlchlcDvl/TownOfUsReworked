using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace TownOfUsReworked.Cosmetics.CustomNameplates
{
    [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetUnlockedNamePlates))]
    public class UnlockNameplates
    {
        public static bool Prefix(HatManager __instance, ref Il2CppReferenceArray<NamePlateData> __result)
        {
            var array =
            (
                from n
                in __instance.allNamePlates.ToArray()
                where n.Free || AmongUs.Data.DataManager.Player.Purchases.GetPurchase(n.ProductId, n.BundleId)
                select n
                into o
                orderby o.displayOrder descending,
                o.name
                select o
            ).ToArray();

            __result = array;
            return false;
        }
    }
}