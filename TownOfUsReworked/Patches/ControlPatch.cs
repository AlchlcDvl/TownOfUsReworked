using HarmonyLib;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles;

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

            if (!TownOfUsReworked.isTest)
                return;
                
            if (Input.GetKeyDown(KeyCode.X))
                PlayerControl.LocalPlayer.Data.Object.SetKillTimer(0f);
            
            if (Input.GetKeyDown(KeyCode.O))
            {
                foreach (var task in PlayerControl.LocalPlayer.myTasks)
                    PlayerControl.LocalPlayer.RpcCompleteTask(task.Id);
            }
            
            /*if (Input.GetKeyDown(KeyCode.G))
            {
                HudManager.Instance.StartCoroutine(HudManager.Instance.CoFadeFullScreen(Color.clear, Color.black));
                HudManager.Instance.StartCoroutine(DestroyableSingleton<HudManager>.Instance.CoShowIntro());
            }*/
            
            if (Input.GetKeyDown(KeyCode.P))
            {
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Doors, 79);
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Doors, 80);
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Doors, 81);
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Doors, 82);
            }
            
            if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
            {
                if (Input.GetKeyDown(KeyCode.F8))
                {
                    Role.NobodyWins = true;
                    Utils.EndGame();
                }
            }
        }
    }
}
