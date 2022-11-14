using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDManagerUpdate
    {
        public static Sprite InvestigateSprite => TownOfUsReworked.Placeholder;
        
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

            var role = Role.GetRole<Consigliere>(PlayerControl.LocalPlayer);

            if (role.InvestigateButton == null)
            {
                role.InvestigateButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.InvestigateButton.graphic.enabled = true;
                role.InvestigateButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUsReworked.BelowVentPosition;
                role.InvestigateButton.gameObject.SetActive(false);
            }

            role.InvestigateButton.GetComponent<AspectPosition>().Update();
            role.InvestigateButton.graphic.sprite = InvestigateSprite;
            role.InvestigateButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            __instance.KillButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            role.InvestigateButton.SetCoolDown(role.ConsigliereTimer(), CustomGameOptions.ConsigCd);

            if (role.ClosestPlayer != null)
                Utils.SetTarget(ref role.ClosestPlayer, role.InvestigateButton, GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance]);
        }
    }
}
