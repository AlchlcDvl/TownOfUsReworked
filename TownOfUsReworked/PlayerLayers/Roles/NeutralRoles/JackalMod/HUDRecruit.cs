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
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jackal))
                return;

            var role = Role.GetRole<Jackal>(PlayerControl.LocalPlayer);

            if (role.HasRecruited)
                return;

            if (role.RecruitButton == null)
            {
                role.RecruitButton = Object.Instantiate(__instance.KillButton, __instance.UseButton.transform.parent);
                role.RecruitButton.graphic.enabled = true;
                role.RecruitButton.gameObject.SetActive(false);
            }

            role.RecruitButton.GetComponent<AspectPosition>().Update();
            role.RecruitButton.graphic.sprite = Recruit;
            role.RecruitButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance && !role.HasRecruited && !LobbyBehaviour.Instance);
            var notRecruited = PlayerControl.AllPlayerControls.ToArray().Where(player => player != role.GoodRecruit && player != role.EvilRecruit && player != role.BackupRecruit).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.RecruitButton, GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance], notRecruited);
            role.RecruitButton.SetCoolDown(role.RecruitTimer(), CustomGameOptions.RecruitCooldown);
        }
    }
}