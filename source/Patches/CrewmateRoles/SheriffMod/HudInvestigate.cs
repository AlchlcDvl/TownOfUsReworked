﻿using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.SheriffMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class HudInvestigate
    {
        public static void Postfix(PlayerControl __instance)
        {
            UpdateInvButton(__instance);
        }

        public static void UpdateInvButton(PlayerControl __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff)) return;
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var investigateButton = DestroyableSingleton<HudManager>.Instance.KillButton;

            var role = Role.GetRole<Sheriff>(PlayerControl.LocalPlayer);


            if (isDead)
            {
                investigateButton.gameObject.SetActive(false);
             //   investigateButton.isActive = false;
            }
            else
            {
                investigateButton.gameObject.SetActive(!MeetingHud.Instance);
               // investigateButton.isActive = !MeetingHud.Instance;
                investigateButton.SetCoolDown(role.InterrogateTimer(), CustomGameOptions.InterrogateCd);

                var notInvestigated = PlayerControl.AllPlayerControls
                    .ToArray()
                    .Where(x => !role.Interrogated.Contains(x.PlayerId))
                    .ToList();

                Utils.SetTarget(ref role.ClosestPlayer, investigateButton, float.NaN, notInvestigated);
            }
        }
    }
}
