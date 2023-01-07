using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class HUDInvestigate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                return;

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var role = Role.GetRole<Consigliere>(PlayerControl.LocalPlayer);

            if (role.InvestigateButton == null)
            {
                role.InvestigateButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.InvestigateButton.graphic.enabled = true;
                role.InvestigateButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUsReworked.BelowVentPosition;
                role.InvestigateButton.gameObject.SetActive(false);
            }

            role.InvestigateButton.graphic.sprite = TownOfUsReworked.Placeholder;
            role.InvestigateButton.gameObject.SetActive(!MeetingHud.Instance && !isDead);
            var notInvestigated = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.Investigated.Contains(x.PlayerId)).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.InvestigateButton, float.NaN, notInvestigated);
            role.InvestigateButton.SetCoolDown(role.ConsigliereTimer(), CustomGameOptions.ConsigCd);
        }
    }
}
