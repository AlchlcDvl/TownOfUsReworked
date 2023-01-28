using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Lobby.CustomOption;
using AmongUs.GameOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.AltruistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDRevive
    {
        public static Sprite Revive => TownOfUsReworked.ReviveSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Altruist))
                return;

            var role = Role.GetRole<Altruist>(PlayerControl.LocalPlayer);
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];
            var flag = (CustomGameOptions.GhostTasksCountToWin || !data.IsDead) && (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) && PlayerControl.LocalPlayer.CanMove;
            var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance, LayerMask.GetMask(new[] {"Players", "Ghost"}));
            DeadBody closestBody = null;
            var closestDistance = float.MaxValue;

            if (role.ReviveButton == null)
            {
                role.ReviveButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform);
                role.ReviveButton.graphic.enabled = true;
                role.ReviveButton.graphic.sprite = Revive;
                role.ReviveButton.gameObject.SetActive(false);
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
            
            role.ReviveButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && !role.ReviveUsed);
            KillButtonTarget.SetTarget(role.ReviveButton, closestBody, role);
            role.ReviveButton.SetCoolDown(0f, 1f);

            var renderer = role.ReviveButton.graphic;

            if (role.CurrentTarget != null)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }
}