using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using UnityEngine;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JackalMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDRecruit
    {
        public static Sprite Recruit => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Jackal))
                return;

            var role = Role.GetRole<Jackal>(PlayerControl.LocalPlayer);

            if (role.HasRecruited || !role.RecruitsDead)
                return;

            if (role.RecruitButton == null)
            {
                role.RecruitButton = Object.Instantiate(__instance.KillButton, __instance.UseButton.transform.parent);
                role.RecruitButton.graphic.enabled = true;
                role.RecruitButton.graphic.sprite = Recruit;
                role.RecruitButton.gameObject.SetActive(false);
            }

            role.RecruitButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.RecruitsDead);
            var notRecruited = PlayerControl.AllPlayerControls.ToArray().Where(player => player != role.GoodRecruit && player != role.EvilRecruit && player != role.BackupRecruit).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.RecruitButton, notRecruited);
            role.RecruitButton.SetCoolDown(role.RecruitTimer(), CustomGameOptions.RecruitCooldown);
        }
    }
}