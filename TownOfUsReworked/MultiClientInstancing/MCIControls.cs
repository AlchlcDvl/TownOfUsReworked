using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.MultiClientInstancing
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public sealed class MCIControls
    {
        public static void Postfix()
        {
            if (!ConstantVariables.IsLocalGame)
            {
                TownOfUsReworked.Debugger.TestWindow.Enabled = false;
                return; //You must ensure you are only playing on local
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                TownOfUsReworked.Debugger.TestWindow.Enabled = !TownOfUsReworked.Debugger.TestWindow.Enabled;
                SettingsPatches.PresetButton.LoadPreset("LastUsed", true);

                if (!TownOfUsReworked.Debugger.TestWindow.Enabled)
                {
                    MCIUtils.RemoveAllPlayers();
                    TownOfUsReworked.MCIActive = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.F2))
                TownOfUsReworked.Debugger.TestWindow.Enabled = false;
        }
    }
}