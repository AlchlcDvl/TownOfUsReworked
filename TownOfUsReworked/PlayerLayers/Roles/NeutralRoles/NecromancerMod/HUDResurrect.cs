using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using AmongUs.GameOptions;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.NecromancerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDRevive
    {
        public static Sprite Resurrect => TownOfUsReworked.RessurectSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Necromancer))
                return;

            var role = Role.GetRole<Necromancer>(PlayerControl.LocalPlayer);
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];
            var flag = (CustomGameOptions.GhostTasksCountToWin || !data.IsDead) && (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) && PlayerControl.LocalPlayer.CanMove;
            DeadBody closestBody = null;
            var closestDistance = float.MaxValue;
            var allBodies = Object.FindObjectsOfType<DeadBody>();

            if (role.ResurrectButton == null)
            {
                role.ResurrectButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ResurrectButton.graphic.enabled = true;
                role.ResurrectButton.graphic.sprite = Resurrect;
                role.ResurrectButton.gameObject.SetActive(false);
            }

            if (role.ResurrectUsesText == null && role.ResurrectUsesLeft > 0)
            {
                role.ResurrectUsesText = Object.Instantiate(__instance.KillButton.cooldownTimerText, __instance.KillButton.transform);
                role.ResurrectUsesText.gameObject.SetActive(false);
                role.ResurrectUsesText.transform.localPosition = new Vector3(role.ResurrectUsesText.transform.localPosition.x + 0.26f, role.ResurrectUsesText.transform.localPosition.y + 0.29f,
                    role.ResurrectUsesText.transform.localPosition.z);
                role.ResurrectUsesText.transform.localScale = role.ResurrectUsesText.transform.localScale * 0.65f;
                role.ResurrectUsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.ResurrectUsesText.fontStyle = TMPro.FontStyles.Bold;
            }

            if (role.ResurrectUsesText != null)
                role.ResurrectUsesText.text = $"{role.ResurrectUsesLeft}";

            foreach (var body in allBodies.Where(x => Vector2.Distance(x.TruePosition, truePosition) <= maxDistance))
            {
                var distance = Vector2.Distance(truePosition, body.TruePosition);

                if (!(distance < closestDistance))
                    continue;

                closestBody = body;
                closestDistance = distance;
            }
            
            role.ResurrectButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.ResurrectButtonUsable);
            role.ResurrectUsesText.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.ResurrectButtonUsable);
            role.ResurrectButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.ResurrectButtonUsable);
            KillButtonTarget.SetTarget(role.ResurrectButton, closestBody, role);

            if (role.IsResurrecting)
                role.ResurrectButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.NecroResurrectDuration);
            else
                role.ResurrectButton.SetCoolDown(role.ResurrectTimer(), CustomGameOptions.ResurrectCooldown + (CustomGameOptions.ResurrectCooldownIncrease * role.ResurrectedCount));

            var renderer = role.ResurrectButton.graphic;

            if (role.CurrentTarget != null && !role.ResurrectButton.isCoolingDown && role.ResurrectButtonUsable)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                role.ResurrectUsesText.color = Palette.EnabledColor;
                role.ResurrectUsesText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                role.ResurrectUsesText.color = Palette.DisabledClear;
                role.ResurrectUsesText.material.SetFloat("_Desat", 1f);
            }

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.graphic.enabled = true;
                role.KillButton.gameObject.SetActive(false);
            }

            if (role.KillUsesText == null && role.KillUsesLeft > 0)
            {
                role.KillUsesText = Object.Instantiate(__instance.KillButton.cooldownTimerText, __instance.KillButton.transform);
                role.KillUsesText.gameObject.SetActive(true);
                role.KillUsesText.transform.localPosition = new Vector3(role.KillUsesText.transform.localPosition.x + 0.26f, role.KillUsesText.transform.localPosition.y + 0.29f,
                    role.KillUsesText.transform.localPosition.z);
                role.KillUsesText.transform.localScale = role.KillUsesText.transform.localScale * 0.65f;
                role.KillUsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.KillUsesText.fontStyle = TMPro.FontStyles.Bold;
            }

            if (role.KillUsesText != null)
                role.KillUsesText.text = $"{role.KillUsesLeft}";

            role.KillButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.KillButtonUsable);
            role.KillUsesText.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.KillButtonUsable);
            role.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.NecroKillCooldown + (CustomGameOptions.NecroKillCooldownIncrease * role.KillCount));
            Utils.SetTarget(ref role.ClosestPlayer, role.KillButton);
            var renderer2 = role.KillButton.graphic;
            
            if (role.ClosestPlayer != null && !role.KillButton.isCoolingDown && role.KillButtonUsable)
            {
                renderer2.color = Palette.EnabledColor;
                renderer2.material.SetFloat("_Desat", 0f);
                role.KillUsesText.color = Palette.EnabledColor;
                role.KillUsesText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer2.color = Palette.DisabledClear;
                renderer2.material.SetFloat("_Desat", 1f);
                role.KillUsesText.color = Palette.DisabledClear;
                role.KillUsesText.material.SetFloat("_Desat", 1f);
            }
        }
    }
}