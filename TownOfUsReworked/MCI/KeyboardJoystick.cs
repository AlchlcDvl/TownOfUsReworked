using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.MCI
{
    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    class Keyboard_Joystick
    {
        public static int controllingFigure = 0;

        public static void Postfix()
        {
            if (!GameStates.IsLocalGame)
                return; //You must ensure you are only playing on local

            if (Input.GetKeyDown(KeyCode.F5))
            {
                if (!GameStates.IsLobby)
                    return; //Don't try to add bots when you are playtesting

                controllingFigure = PlayerControl.LocalPlayer.PlayerId;

                if (PlayerControl.AllPlayerControls.Count == 15)
                    return; //Remove this if you're willing to suffer with the consequences. 

                MCIUtils.CleanUpLoad();
                MCIUtils.CreatePlayerInstance("Robot");

                if (!InstanceControl.MCIActive)
                    InstanceControl.MCIActive = true;
            }

            if (!InstanceControl.MCIActive)
                return;

            if (Input.GetKeyDown(KeyCode.F9))
            {
                controllingFigure++;
                controllingFigure = Mathf.Clamp(controllingFigure, 0, PlayerControl.AllPlayerControls.Count - 1);
                InstanceControl.SwitchTo((byte)controllingFigure);
            }

            if (Input.GetKeyDown(KeyCode.F10))
            {
                controllingFigure--;
                controllingFigure = Mathf.Clamp(controllingFigure, 0, PlayerControl.AllPlayerControls.Count - 1);
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
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.LifeSupp, 16);
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16);
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Laboratory, 16);
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16 | 0);
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16 | 1);
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16 | 0);
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16 | 1);
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 0);
                Utils.DefaultOutfitAll();
            }
            
            if (Input.GetKeyDown(KeyCode.F2))
            {
                foreach (var task in PlayerControl.LocalPlayer.myTasks)
                    PlayerControl.LocalPlayer.RpcCompleteTask(task.Id);
            }
        
            if (Input.GetKeyDown(KeyCode.F12))
            {
                if (GameStates.IsLobby)
                    return;

                HudManager.Instance.StartCoroutine(HudManager.Instance.CoFadeFullScreen(Color.clear, Color.black));
                HudManager.Instance.StartCoroutine(DestroyableSingleton<HudManager>.Instance.CoShowIntro());
            }
        }
    }
}
