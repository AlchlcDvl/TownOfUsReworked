using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using System.Linq;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.DraculaMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDConvert
    {
        public static Sprite ConvertSprite => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Dracula))
                return;

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;

            if (isDead)
                return;

            var role = Role.GetRole<Dracula>(PlayerControl.LocalPlayer);
            var vamps = role.Converted;
            var notVamp = PlayerControl.AllPlayerControls.ToArray().Where(player => vamps.Contains(player)).ToList();
            var button = role.BiteButton;

            if (button == null)
            {
                button = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                button.graphic.enabled = true;
                button.gameObject.SetActive(false);
            }

            button.GetComponent<AspectPosition>().Update();
            button.graphic.sprite = ConvertSprite;

            button.gameObject.SetActive(!MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead);
            
            if (!button.isCoolingDown)
            {
                button.graphic.color = Palette.EnabledColor;
                button.graphic.material.SetFloat("_Desat", 0f);
            }
            else
            {
                button.graphic.color = Palette.DisabledClear;
                button.graphic.material.SetFloat("_Desat", 1f);
            }

            button.SetCoolDown(role.ConvertTimer(), CustomGameOptions.BiteCd);
            Utils.SetTarget(ref role.ClosestPlayer, button, float.NaN, notVamp);
        }
    }
}
