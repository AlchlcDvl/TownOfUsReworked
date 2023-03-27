using HarmonyLib;
using System.Linq;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class SizePatch
    {
        public static void Postfix()
        {
            if (ConstantVariables.IsLobby)
            {
                foreach (var player in PlayerControl.AllPlayerControls.ToArray())
                    player.transform.localScale = new Vector3(0.7f, 0.7f, 1.0f);

                return;
            }

            foreach (var player in PlayerControl.AllPlayerControls.ToArray())
            {
                if (!(player.Data.IsDead || player.Data.Disconnected))
                    player.transform.localScale = player.GetAppearance().SizeFactor;
                else
                    player.transform.localScale = new Vector3(0.7f, 0.7f, 1.0f);
            }

            var playerBindings = PlayerControl.AllPlayerControls.ToArray().ToDictionary(player => player.PlayerId);

            foreach (var body in Object.FindObjectsOfType<DeadBody>())
                body.transform.localScale = playerBindings[body.ParentId].GetAppearance().SizeFactor;
        }
    }
}