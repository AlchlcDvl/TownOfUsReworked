using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.RebelMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDEverything
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
            var role = Role.GetRole<Rebel>(PlayerControl.LocalPlayer);

            if (role.HasDeclared || role.WasSidekick)
                return;

            if (role.DeclareButton == null)
            {
                role.DeclareButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.DeclareButton.graphic.enabled = true;
                role.DeclareButton.graphic.sprite = Promote;
                role.DeclareButton.gameObject.SetActive(false);
            }

            role.DeclareButton.gameObject.SetActive(!isDead && !MeetingHud.Instance && !LobbyBehaviour.Instance && !role.WasSidekick && !role.HasDeclared);
            Utils.SetTarget(ref role.ClosestPlayer, role.DeclareButton);
        }
    }
}