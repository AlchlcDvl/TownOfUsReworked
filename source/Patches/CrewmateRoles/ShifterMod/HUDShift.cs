﻿using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.ShifterMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class HUDShift
    {
        public static void Postfix(PlayerControl __instance)
        {
            UpdateShiftButton(__instance);
        }

        public static void UpdateShiftButton(PlayerControl __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Shifter)) return;
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
            var shiftButton = DestroyableSingleton<HudManager>.Instance.KillButton;

            var role = Role.GetRole<Shifter>(PlayerControl.LocalPlayer);

            if (isDead)
            {
                shiftButton.gameObject.SetActive(false);
             //   shiftButton.isActive = false;
            }
            else
            {
                shiftButton.gameObject.SetActive(!MeetingHud.Instance);
               // shiftButton.isActive = !MeetingHud.Instance;
                shiftButton.SetCoolDown(role.ShifterShiftTimer(), CustomGameOptions.ShifterCd);

                Utils.SetTarget(ref role.ClosestPlayer, shiftButton);
            }
        }
    }
}
