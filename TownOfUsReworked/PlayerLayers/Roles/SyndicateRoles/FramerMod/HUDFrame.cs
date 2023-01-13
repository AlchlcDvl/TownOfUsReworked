using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.FramerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDFramer
    {
        public static Sprite FrameSprite => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Framer))
                return;

            var role = Role.GetRole<Framer>(PlayerControl.LocalPlayer);

            if (role.FrameButton == null)
            {
                role.FrameButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.FrameButton.graphic.enabled = true;
                role.FrameButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUsReworked.BelowVentPosition;
                role.FrameButton.gameObject.SetActive(false);
            }

            role.FrameButton.GetComponent<AspectPosition>().Update();
            role.FrameButton.graphic.sprite = FrameSprite;
            role.FrameButton.gameObject.SetActive(!MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead && !LobbyBehaviour.Instance);
            var notFramed = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.Framed.Contains(x.PlayerId)).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.FrameButton, GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance], notFramed);
            role.FrameButton.SetCoolDown(role.FrameTimer(), CustomGameOptions.FrameCooldown);
        }
    }
}
