using HarmonyLib;
using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.PuppeteerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDPossess
    {
        private static readonly int Desat = Shader.PropertyToID("_Desat");

        public static void Postfix(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Puppeteer))
                return;

            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            var role = Role.GetRole<Puppeteer>(PlayerControl.LocalPlayer);
            var isDead = PlayerControl.LocalPlayer.Data.IsDead;

            if (role.PossessButton == null)
            {
                role.PossessButton = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                role.PossessButton.graphic.enabled = true;
                role.PossessButton.graphic.sprite = Puppeteer.PossessSprite;
            }

            if (role.PossessButton.graphic.sprite != Puppeteer.PossessSprite && role.PossessButton.graphic.sprite != Puppeteer.UnPossessSprite)
                role.PossessButton.graphic.sprite = Puppeteer.PossessSprite;

            var position = __instance.KillButton.transform.localPosition;

            role.PossessButton.transform.localPosition = TownOfUsReworked.BelowVentPosition;

            if (role.PossessButton.graphic.sprite == Puppeteer.PossessSprite)
            {
                if ((role.lastPossess - DateTime.UtcNow).TotalMilliseconds / 1000.0f + PlayerControl.GameOptions.KillCooldown > 0)
                {
                    role.PossessButton.SetCoolDown((float)(role.lastPossess - DateTime.UtcNow).TotalMilliseconds / 1000 +
                        PlayerControl.GameOptions.KillCooldown, PlayerControl.GameOptions.KillCooldown);       
                    role.Player.SetKillTimer((float)(role.lastPossess - DateTime.UtcNow).TotalMilliseconds / 1000 +
                        PlayerControl.GameOptions.KillCooldown);
                    return;
                }

                if (isDead)
                    role.PossessButton.gameObject.SetActive(false);
                else if (role.duration > 0)
                    role.PossessButton.SetCoolDown(role.PossessTime, CustomGameOptions.PossessCooldown);
                else
                {
                    if ((float)(CustomGameOptions.PossessDuration - (DateTime.UtcNow - role.PossStart).TotalMilliseconds / 1000.0f) > 0)
                        role.PossessButton.SetCoolDown((float)(CustomGameOptions.PossessCooldown - (DateTime.UtcNow - role.PossStart).
                        TotalMilliseconds / 1000.0f), CustomGameOptions.PossessCooldown);
                    else
                        role.PossessButton.SetCoolDown(role.PossessTimer(), PlayerControl.GameOptions.KillCooldown);
                }

                Utils.SetTarget(ref role.ClosestPlayer, role.PossessButton);

                if (role.ClosestPlayer)
                {
                    role.PossessButton.graphic.color = Palette.EnabledColor;
                    role.PossessButton.graphic.material.SetFloat(Desat, 0f);
                }
                else
                {
                    role.PossessButton.graphic.color = Palette.DisabledClear;
                    role.PossessButton.graphic.material.SetFloat(Desat, 1.0f);
                }
            }
            else
            {
                role.PossessButton.SetCoolDown((float)((DateTime.UtcNow - role.PossStart).TotalMilliseconds / 1000.0f),
                    CustomGameOptions.PossessCooldown);
                role.PossessButton.graphic.material.SetFloat(Desat, 0f);
                role.PossessButton.graphic.color = Palette.EnabledColor;
                role.PossessButton.enabled = true;
            }
        }
    }
}