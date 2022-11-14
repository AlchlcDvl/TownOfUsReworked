using System;
using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.BlackmailerMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object __instance)
        {
            if (ExileController.Instance == null || __instance != ExileController.Instance.gameObject)
                return;
                
            foreach (var role in Role.GetRoles(RoleEnum.Blackmailer))
            {
                var role2 = (Blackmailer)role;

                if (role2.Player.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    role2.Blackmailed?.myRend().material.SetFloat("_Outline", 0f);

                role2.Blackmailed = null;
                role2.LastBlackmailed = DateTime.UtcNow;
            }
        }
    }
}