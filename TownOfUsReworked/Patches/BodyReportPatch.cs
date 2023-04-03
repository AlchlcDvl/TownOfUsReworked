using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    public static class BodyReportPatch
    {
        public static void Postfix([HarmonyArgument(0)] GameData.PlayerInfo info)
        {
            if (ConstantVariables.Inactive || info == null || (CustomGameOptions.PlaguebearerOn == 0 && CustomGameOptions.ArsonistOn == 0 && CustomGameOptions.CryomaniacOn == 0))
                return;

            Utils.Spread(PlayerControl.LocalPlayer, info.Object);
        }
    }
}