using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using UnityEngine;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.PoisonerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDPoison
    {
        public static Sprite PoisonSprite => TownOfUsReworked.PoisonSprite;
        public static Sprite PoisonedSprite => TownOfUsReworked.PoisonedSprite;

        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner))
                return;

            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            var role = Role.GetRole<Poisoner>(PlayerControl.LocalPlayer);

            if (role.PoisonButton == null)
            {
                role.PoisonButton = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                role.PoisonButton.graphic.enabled = true;
                role.PoisonButton.graphic.sprite = PoisonSprite;
            }

            role.PoisonButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance && !LobbyBehaviour.Instance);
            __instance.KillButton.Hide();
            
            var position = __instance.KillButton.transform.localPosition;
            role.PoisonButton.transform.localPosition = position;
            var notImp = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsImpostor()).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.PoisonButton, notImp);

            if (role.ClosestPlayer != null)
                role.ClosestPlayer.myRend().material.SetColor("_OutlineColor", role.Color);

            role.Player.SetKillTimer(1f);

            if (role.Poisoned)
            {
                role.PoisonButton.graphic.sprite = PoisonedSprite;
                role.Poison();
                role.PoisonButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.PoisonDuration);
            }
            else
            {
                role.PoisonButton.graphic.sprite = PoisonSprite;

                if (role.PoisonedPlayer && role.PoisonedPlayer != PlayerControl.LocalPlayer)
                    role.PoisonKill();
                        
                if (role.ClosestPlayer != null)
                {
                    role.PoisonButton.graphic.color = Palette.EnabledColor;
                    role.PoisonButton.graphic.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    role.PoisonButton.graphic.color = Palette.DisabledClear;
                    role.PoisonButton.graphic.material.SetFloat("_Desat", 1f);
                }

                role.PoisonButton.SetCoolDown(role.PoisonTimer(), CustomGameOptions.PoisonCd);
                role.PoisonedPlayer = PlayerControl.LocalPlayer; //Only do this to stop repeatedly trying to re-kill poisoned player. null didn't work for some reason
            }
        }
    }
}
