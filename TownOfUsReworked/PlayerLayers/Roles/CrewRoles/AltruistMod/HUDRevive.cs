using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.AltruistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDRevive
    {
        private static Sprite Revive => TownOfUsReworked.ReviveSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.Is(RoleEnum.Altruist))
                return;

            var role = Role.GetRole<Altruist>(PlayerControl.LocalPlayer);
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];
            var flag = (CustomGameOptions.GhostTasksCountToWin || !data.IsDead) && (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) && PlayerControl.LocalPlayer.CanMove;
            var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance, LayerMask.GetMask(new[] {"Players", "Ghost"}));
            var killButton = role.ReviveButton;
            DeadBody closestBody = null;
            var closestDistance = float.MaxValue;

            if (killButton == null)
            {
                killButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform);
                killButton.graphic.enabled = true;
                killButton.gameObject.SetActive(false);
            }

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
            
            killButton.graphic.sprite = Revive;
            killButton.gameObject.SetActive(!MeetingHud.Instance && !isDead && !LobbyBehaviour.Instance && !role.ReviveUsed);
            KillButtonTarget.SetTarget(killButton, closestBody, role);
            killButton.SetCoolDown(0f, 1f);
        }
    }
}