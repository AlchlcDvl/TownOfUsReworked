using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.MultiClientInstancing
{
    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    class Keyboard_Joystick
    {
        public static int controllingFigure = 0;

        public static void Postfix()
        {
            if (!GameStates.IsLocalGame)
                return; // You must ensure you are only playing on local

            if (Input.GetKeyDown(KeyCode.F5))
            {
                if (GameStates.IsInGame)
                    return; //Don't try to add bots when you are playtesting

                controllingFigure = PlayerControl.LocalPlayer.PlayerId;

                if (PlayerControl.AllPlayerControls.Count == 15)
                    return; //Remove this if your willing to suffer with the consequences. 

                Utils.CleanUpLoad();
                Utils.CreatePlayerInstance("Robot");
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                controllingFigure++;
                controllingFigure = Mathf.Clamp(controllingFigure, 0, PlayerControl.AllPlayerControls.Count -1);
                InstanceControl.SwitchTo((byte)controllingFigure);
            }

            if (Input.GetKeyDown(KeyCode.F10))
            {
                controllingFigure--;
                controllingFigure = Mathf.Clamp(controllingFigure, 0, PlayerControl.AllPlayerControls.Count -1);
                InstanceControl.SwitchTo((byte)controllingFigure);
            }
            
            if (Input.GetKeyDown(KeyCode.F8))
            {
                Role.NobodyWins = true;
                Utils.EndGame();
            }
        
            if (Input.GetKeyDown(KeyCode.F4))
            {
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Doors, 79);
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Doors, 80);
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Doors, 81);
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Doors, 82);
            }
            
            if (Input.GetKeyDown(KeyCode.F3))
                PlayerControl.LocalPlayer.Data.Object.SetKillTimer(0f);
            
            if (Input.GetKeyDown(KeyCode.F2))
            {
                foreach (var task in PlayerControl.LocalPlayer.myTasks)
                    PlayerControl.LocalPlayer.RpcCompleteTask(task.Id);
            }
        
            if (Input.GetKeyDown(KeyCode.F12))
            {
                HudManager.Instance.StartCoroutine(HudManager.Instance.CoFadeFullScreen(Color.clear, Color.black));
                HudManager.Instance.StartCoroutine(DestroyableSingleton<HudManager>.Instance.CoShowIntro());
            }
        }
    }
}
