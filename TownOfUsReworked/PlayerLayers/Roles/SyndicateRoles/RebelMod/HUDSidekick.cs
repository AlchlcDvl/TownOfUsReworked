using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.RebelMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDSidekick
    {
        public static Sprite Promote => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Rebel))
                return;

            var isDead = PlayerControl.LocalPlayer.Data.IsDead;

            if (isDead)
                return;

            var role = Role.GetRole<Rebel>(PlayerControl.LocalPlayer);

            if (role.HasDeclared || role.WasSidekick)
                return;

            if (role.DeclareButton == null)
            {
                role.DeclareButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.DeclareButton.graphic.enabled = true;
                role.DeclareButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUsReworked.BelowVentPosition;
                role.DeclareButton.gameObject.SetActive(false);
            }

            role.DeclareButton.GetComponent<AspectPosition>().Update();
            role.DeclareButton.graphic.sprite = Promote;
            role.DeclareButton.gameObject.SetActive(!isDead && !MeetingHud.Instance);

            if (role.ClosestSyndicate.Is(Faction.Syndicate))
                Utils.SetTarget(ref role.ClosestSyndicate, role.DeclareButton);
        }
    }
}