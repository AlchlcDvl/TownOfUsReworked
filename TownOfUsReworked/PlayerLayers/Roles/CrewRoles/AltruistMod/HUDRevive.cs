using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Lobby.CustomOption;
using AmongUs.GameOptions;
using System.Linq;

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
            DeadBody closestBody = null;
            var closestDistance = float.MaxValue;
            var allBodies = Object.FindObjectsOfType<DeadBody>();

            if (role.ReviveButton == null)
            {
                role.ReviveButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ReviveButton.graphic.enabled = true;
                role.ReviveButton.graphic.sprite = Revive;
                role.ReviveButton.gameObject.SetActive(false);
            }

            foreach (var body in allBodies.Where(x => Vector2.Distance(x.TruePosition, truePosition) <= maxDistance))
            {
                var distance = Vector2.Distance(truePosition, body.TruePosition);

                if (!(distance < closestDistance))
                    continue;

                closestBody = body;
                closestDistance = distance;
            }
            
            role.ReviveButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && !role.ReviveUsed);
            KillButtonTarget.SetTarget(role.ReviveButton, closestBody, role);
            role.ReviveButton.SetCoolDown(0f, 1f);

            var renderer = role.ReviveButton.graphic;

            if (Utils.EnableDeadButton(role.ReviveButton, role.Player, role.CurrentTarget))
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