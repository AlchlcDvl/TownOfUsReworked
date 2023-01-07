using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using UnityEngine;

using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.JanitorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDClean
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Janitor))
                return;

            var role = Role.GetRole<Janitor>(PlayerControl.LocalPlayer);
            
            if (role.CleanButton == null)
            {
                role.CleanButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.CleanButton.graphic.enabled = true;
                role.CleanButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUsReworked.BelowVentPosition;
                role.CleanButton.gameObject.SetActive(false);
            }

            role.CleanButton.GetComponent<AspectPosition>().Update();
            role.CleanButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            role.CleanButton.graphic.sprite = TownOfUsReworked.JanitorClean;

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];
            var flag = (CustomGameOptions.GhostTasksCountToWin || !data.IsDead) && (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                PlayerControl.LocalPlayer.CanMove;
            var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance, LayerMask.GetMask(new[] {"Players", "Ghost"}));
            var killButton = role.CleanButton;
            DeadBody closestBody = null;
            var closestDistance = float.MaxValue;

            foreach (var collider2D in allocs)
            {
                if (!flag || isDead || collider2D.tag != "DeadBody")
                    continue;

                var component = collider2D.GetComponent<DeadBody>();

                if (!(Vector2.Distance(truePosition, component.TruePosition) <= maxDistance))
                    continue;

                var distance = Vector2.Distance(truePosition, component.TruePosition);

                if (!(distance < closestDistance))
                    continue;

                closestBody = component;
                closestDistance = distance;
            }

            KillButtonTarget.SetTarget(killButton, closestBody, role);
            role.CleanButton.SetCoolDown(role.CleanTimer(), CustomGameOptions.JanitorCleanCd);
        }
    }
}