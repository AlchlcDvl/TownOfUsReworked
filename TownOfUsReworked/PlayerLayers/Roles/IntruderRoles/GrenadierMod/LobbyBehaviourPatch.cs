using HarmonyLib;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GrenadierMod
{
    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    static class LobbyBehaviourPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            //Fix Grenadier blind in lobby
            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = false;
        }
    }
}
