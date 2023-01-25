using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using System.Linq;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JesterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDHaunt
    {
        public static Sprite ConvertSprite => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.Is(RoleEnum.Jester))
                return;

            var role = Role.GetRole<Jester>(PlayerControl.LocalPlayer);

            if (!role.VotedOut)
                return;

            if (role.HauntButton == null)
            {
                role.HauntButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.HauntButton.graphic.enabled = true;
                role.HauntButton.gameObject.SetActive(false);
            }

            role.HauntButton.gameObject.SetActive(!MeetingHud.Instance && role.VotedOut && !LobbyBehaviour.Instance && !role.HasHaunted && PlayerControl.LocalPlayer.Data.IsDead);
            role.HauntButton.SetCoolDown(role.HauntTimer(), CustomGameOptions.BiteCd);
            var ToBeHaunted = PlayerControl.AllPlayerControls.ToArray().Where(x => role.ToHaunt.Contains(x.PlayerId)).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.HauntButton, ToBeHaunted);
            var renderer = role.HauntButton.graphic;
            
            if (role.ClosestPlayer != null)
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
