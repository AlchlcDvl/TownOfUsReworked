using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.MultiClientInstancing
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public sealed class MCIControls
    {
        public static void Postfix()
        {
            if (!ConstantVariables.IsLocalGame)
                return; //You must ensure you are only playing on local

            if (Input.GetKeyDown(KeyCode.F1))
            {
                TownOfUsReworked.Debugger.TestWindow.Enabled = !TownOfUsReworked.Debugger.TestWindow.Enabled;

                if (!TownOfUsReworked.Debugger.TestWindow.Enabled)
                {
                    MCIUtils.RemoveAllPlayers();
                    TownOfUsReworked.MCIActive = false;
                }
            }
        }
    }
}