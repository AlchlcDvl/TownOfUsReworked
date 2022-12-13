using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.HandleHud))]
    public class KeyboardJoystickPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (DestroyableSingleton<HudManager>.Instance != null && DestroyableSingleton<HudManager>.Instance.ImpostorVentButton != null &&
                DestroyableSingleton<HudManager>.Instance.ImpostorVentButton.isActiveAndEnabled && ConsoleJoystick.player.GetButtonDown(50))
                DestroyableSingleton<HudManager>.Instance.ImpostorVentButton.DoClick();
            
            if (!TownOfUsReworked.isDev)
                return;
            
            if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
            {
                if (Input.GetKeyDown(KeyCode.F8))
                    GameManager.Instance.RpcEndGame(GameOverReason.ImpostorDisconnect, false);
                
                if (Input.GetKeyDown(KeyCode.RightShift))
                    Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
            }
        }
    }
}
