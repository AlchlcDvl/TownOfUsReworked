using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GrenadierMod
{
    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    static class LobbyBehaviourPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            //Fix Grenadier and screwed blind in lobby
            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = false;
            Utils.DefaultOutfitAll();
        }
    }
}
