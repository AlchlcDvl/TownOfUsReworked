using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using UnityEngine;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GrenadierMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite FlashSprite => TownOfUsReworked.FlashSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Grenadier))
                return;

            var role = Role.GetRole<Grenadier>(PlayerControl.LocalPlayer);

            if (role.FlashButton == null)
            {
                role.FlashButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.FlashButton.graphic.enabled = true;
                role.FlashButton.graphic.sprite = FlashSprite;
                role.FlashButton.gameObject.SetActive(false);
            }

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.graphic.enabled = true;
                role.KillButton.gameObject.SetActive(false);
            }

            var notImp = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Intruder)).ToList();
            role.FlashButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.KillButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.IntKillCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, role.KillButton, notImp);

            if (role.Flashed)
                role.FlashButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.GrenadeDuration);
            else 
                role.FlashButton.SetCoolDown(role.FlashTimer(), CustomGameOptions.GrenadeCd);

            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var specials = system.specials.ToArray();
            var dummyActive = system.dummy.IsActive;
            var sabActive = specials.Any(s => s.IsActive);
            var renderer = role.FlashButton.graphic;
            var renderer2 = role.KillButton.graphic;

            if (sabActive && !dummyActive && !role.FlashButton.isCoolingDown)
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
            else
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }

            if (role.ClosestPlayer != null && !role.KillButton.isCoolingDown)
            {
                renderer2.color = Palette.EnabledColor;
                renderer2.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer2.color = Palette.DisabledClear;
                renderer2.material.SetFloat("_Desat", 1f);
            }

            if (CustomGameOptions.GrenadierIndicators)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player != PlayerControl.LocalPlayer && !player.Data.IsImpostor())
                    {
                        var tempColour = player.nameText().color;
                        var data = player?.Data;

                        if (data == null || data.Disconnected || data.IsDead || PlayerControl.LocalPlayer.Data.IsDead)
                            continue;

                        if (role.flashedPlayers.Contains(player))
                        {
                            player.myRend().material.SetColor("_VisorColor", Color.black);
                            player.nameText().color = Color.black;
                        }
                        else
                        {
                            player.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
                            player.nameText().color = tempColour;
                        }
                    }
                }
            }
        }
    }
}