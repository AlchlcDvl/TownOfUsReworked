using TownOfUsReworked.Crowded.Net;
using HarmonyLib;
using Reactor.Networking.Rpc;

namespace TownOfUsReworked.Crowded.Patches
{
    [HarmonyPatch]
    public static class GenericPatches
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckColor))]
        public static class CmdCheckColorPatch
        {
            public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] byte colorId)
            {
                Rpc<SetColorRpc>.Instance.Send(__instance, colorId);
                return false;
            }
        }

        [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.Update))]
        public static class IsSelectedItemEquippedPatch
        {
            public static void Postfix(PlayerTab __instance) => __instance.currentColorIsEquipped = true;
        }

        [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.UpdateAvailableColors))]
        public static class UpdateAvailableColorsPatch
        {
            public static bool Prefix(PlayerTab __instance)
            {
                __instance.AvailableColors.Clear();

                for (var i = 0; i < Palette.PlayerColors.Count; i++)
                {
                    if(!PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.CurrentOutfit.ColorId != i)
                        __instance.AvailableColors.Add(i);
                }

                return false;
            }
        }
    }
}