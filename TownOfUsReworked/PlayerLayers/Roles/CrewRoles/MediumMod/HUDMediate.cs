using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using System.Linq;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MediumMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDMediate
    {
        public static Sprite Mediate => TownOfUsReworked.MediateSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Medium))
                return;

            var role = Role.GetRole<Medium>(PlayerControl.LocalPlayer);

            if (role.MediateButton == null)
            {
                role.MediateButton = UnityEngine.Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.MediateButton.graphic.enabled = true;
                role.MediateButton.graphic.sprite = Mediate;
                role.MediateButton.gameObject.SetActive(false);
            }

            role.MediateButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.PrimaryButton = role.MediateButton;

            if (!PlayerControl.LocalPlayer.Data.IsDead)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (role.MediatedPlayers.Keys.Contains(player.PlayerId))
                    {
                        role.MediatedPlayers.GetValueSafe(player.PlayerId).target = player.transform.position;
                        player.Visible = true;

                        if (!CustomGameOptions.ShowMediatePlayer)
                        {
                            player.SetOutfit(CustomPlayerOutfitType.Camouflage, new GameData.PlayerOutfit()
                            {
                                ColorId = player.GetDefaultOutfit().ColorId,
                                HatId = "",
                                SkinId = "",
                                VisorId = "",
                                PlayerName = " "
                            });

                            PlayerMaterial.SetColors(Color.grey, player.myRend());
                        }
                    }
                }
            }

            role.MediateButton.SetCoolDown(role.MediateTimer(), CustomGameOptions.MediateCooldown);
            var renderer = role.MediateButton.graphic;

            if (!role.MediateButton.isCoolingDown)
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