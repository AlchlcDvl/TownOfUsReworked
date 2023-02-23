using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MysticMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDReveal
    {
        public static Sprite Reveal => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Mystic))
                return;

            var role = Role.GetRole<Mystic>(PlayerControl.LocalPlayer);

            if (role.RevealButton == null)
            {
                role.RevealButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.RevealButton.graphic.enabled = true;
                role.RevealButton.graphic.sprite = Reveal;
                role.RevealButton.gameObject.SetActive(false);
            }

            role.RevealButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.RevealButton.SetCoolDown(role.RevealTimer(), CustomGameOptions.RevealCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, role.RevealButton);
            role.PrimaryButton = role.RevealButton;
            var renderer = role.RevealButton.graphic;
            
            if (Utils.EnableAbilityButton(role.RevealButton, role.Player, role.ClosestPlayer))
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }

            if (role.ConvertedDead && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                role.TurnSeer();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnSeer);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}