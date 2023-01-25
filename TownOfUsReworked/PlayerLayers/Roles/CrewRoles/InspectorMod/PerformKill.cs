using Hazel;
using System;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.InspectorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Inspector))
                return false;

            var role = Role.GetRole<Inspector>(PlayerControl.LocalPlayer);

            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Inspector, role.ClosestPlayer, __instance) || __instance != role.InspectButton)
                return false;

            if (role.ExamineTimer() != 0f && __instance == role.InspectButton)
                return false;

            Utils.Spread(PlayerControl.LocalPlayer, role.ClosestPlayer);

            if (Utils.CheckInteractionSesitive(role.ClosestPlayer))
            {
                Utils.AlertKill(PlayerControl.LocalPlayer, role.ClosestPlayer);
                return false;
            }

            role.Inspected.Add(role.ClosestPlayer);
            role.LastInspected = DateTime.UtcNow;
            
            try
            {
                //SoundManager.Instance.PlaySound(TownOfUsReworked.PhantomWin, false, 1f);
            } catch {}
            
            return false;
        }
    }
}
