using HarmonyLib;
using System.Linq;
using TownOfUsReworked.Classes;
using UnityEngine;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public static class SizePatch
    {
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static void Postfix()
        {
            if (GameStates.IsLobby)
                return;

            foreach (var player in PlayerControl.AllPlayerControls.ToArray())
            {
                if (!(player.Data.IsDead || player.Data.Disconnected))
                    player.transform.localScale = player.GetAppearance().SizeFactor;
                else
                    player.transform.localScale = new Vector3(0.7f, 0.7f, 1.0f);
            }

            var playerBindings = PlayerControl.AllPlayerControls.ToArray().ToDictionary(player => player.PlayerId);

            foreach (var body in Object.FindObjectsOfType<DeadBody>() )
                body.transform.localScale = playerBindings[body.ParentId].GetAppearance().SizeFactor;
        }
    }
}