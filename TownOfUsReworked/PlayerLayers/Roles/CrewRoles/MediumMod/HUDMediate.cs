using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;
using System.Linq;
using System;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MediumMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDMediate
    {
        private static Sprite Mediate => TownOfUsReworked.MediateSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Medium))
            {
                var role = Role.GetRole<Medium>(PlayerControl.LocalPlayer);

                if (role.MediateButton == null)
                {
                    role.MediateButton = UnityEngine.Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.MediateButton.graphic.enabled = true;
                    role.MediateButton.graphic.sprite = Mediate;
                    role.MediateButton.gameObject.SetActive(false);
                }

                role.MediateButton.gameObject.SetActive(Utils.SetActive(PlayerControl.LocalPlayer, __instance));

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
                                player.SetOutfit(CustomPlayerOutfitType.Camouflage, new GameData.PlayerOutfit() {ColorId = player.GetDefaultOutfit().ColorId, HatId = "", SkinId = "",
                                    VisorId = "", PlayerName = " "});
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
            else if (CustomGameOptions.ShowMediumToDead && Role.AllRoles.Any(x => x.RoleType == RoleEnum.Medium && ((Medium)x).MediatedPlayers.Keys.Contains(PlayerControl.
                LocalPlayer.PlayerId)))
            {
                var role = (Medium)Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Medium && ((Medium)x).MediatedPlayers.Keys.Contains(PlayerControl.LocalPlayer.PlayerId));
                role.MediatedPlayers.GetValueSafe(PlayerControl.LocalPlayer.PlayerId).target = role.Player.transform.position;
            }
        }
    }
}