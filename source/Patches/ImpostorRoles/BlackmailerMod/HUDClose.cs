using System;
using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using Object = UnityEngine.Object;

namespace TownOfUs.ImpostorRoles.BlackmailerMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            foreach (var role in Role.GetRoles(RoleEnum.Blackmailer))
            {
                var blackmailer = (Blackmailer)role;
                if (blackmailer.Player.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                {
                    blackmailer.Blackmailed?.myRend().material.SetFloat("_Outline", 0f);
                }
                blackmailer.Blackmailed = null;
                blackmailer.LastBlackmailed = DateTime.UtcNow;
            }
        }
    }
}