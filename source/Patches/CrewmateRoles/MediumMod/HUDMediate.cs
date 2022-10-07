using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using System.Linq;
using System;
using TownOfUs.Extensions;

namespace TownOfUs.CrewmateRoles.MediumMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class HUDMediate
    {
        public static void Postfix(PlayerControl __instance)
        {
            UpdateButton(__instance);
        }
        public static void UpdateButton(PlayerControl __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            var data = PlayerControl.LocalPlayer.Data;

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Medium))
            {
                var mediateButton = DestroyableSingleton<HudManager>.Instance.KillButton;

                var role = Role.GetRole<Medium>(PlayerControl.LocalPlayer);
                mediateButton.gameObject.SetActive(!MeetingHud.Instance && !data.IsDead);
                if (data.IsDead) return;

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
                            //player.nameText().text = "";
                            //PlayerControl.SetPlayerMaterialColors(Color.grey, player.MyRend);
                            PlayerMaterial.SetColors(Color.grey, player.myRend());
                        }
                    }
                }
                mediateButton.SetCoolDown(role.MediateTimer(), CustomGameOptions.MediateCooldown);

                var renderer = mediateButton.graphic;
                if (!mediateButton.isCoolingDown)
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                    return;
                }

                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
            else if (CustomGameOptions.ShowMediumToDead && Role.AllRoles.Any(x => x.RoleType == RoleEnum.Medium && ((Medium) x).MediatedPlayers.Keys.Contains(PlayerControl.LocalPlayer.PlayerId)))
            {
                var role = (Medium) Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Medium && ((Medium) x).MediatedPlayers.Keys.Contains(PlayerControl.LocalPlayer.PlayerId));
                role.MediatedPlayers.GetValueSafe(PlayerControl.LocalPlayer.PlayerId).target = role.Player.transform.position;
            }
        }
    }
}