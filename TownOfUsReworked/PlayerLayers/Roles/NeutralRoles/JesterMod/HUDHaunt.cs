using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JesterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDHaunt
    {
        public static Sprite Haunt => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Jester))
                return;

            var role = Role.GetRole<Jester>(PlayerControl.LocalPlayer);

            if (role.HauntButton == null)
            {
                role.HauntButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.HauntButton.graphic.enabled = true;
                role.HauntButton.graphic.sprite = Haunt;
                role.HauntButton.gameObject.SetActive(false);
            }

            role.HauntButton.gameObject.SetActive(!MeetingHud.Instance && role.VotedOut && !LobbyBehaviour.Instance && !role.HasHaunted && PlayerControl.LocalPlayer.Data.IsDead);
            role.HauntButton.SetCoolDown(role.HauntTimer(), CustomGameOptions.HauntCooldown);
            var ToBeHaunted = PlayerControl.AllPlayerControls.ToArray().Where(x => role.ToHaunt.Contains(x.PlayerId)).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.HauntButton, ToBeHaunted);
            var renderer = role.HauntButton.graphic;
            
            if (role.ClosestPlayer != null && !role.HauntButton.isCoolingDown && role.CanHaunt)
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