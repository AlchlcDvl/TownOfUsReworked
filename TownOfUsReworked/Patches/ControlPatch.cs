using HarmonyLib;
using UnityEngine;

namespace TownOfUsReworked.Patches
{
    //Thanks to Town Of Host for this code
    [HarmonyPatch(typeof(ControllerManager), nameof(ControllerManager.Update))]
    class ControllerManagerUpdatePatch
    {
        public static void Postfix(ControllerManager __instance)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift) && GameStates.IsCountDown)
                    GameStartManager.Instance.countDownTimer = 0;

                if (Input.GetKeyDown(KeyCode.C) && GameStates.IsCountDown)
                    GameStartManager.Instance.ResetStartState();
            }
        }
    }
}
