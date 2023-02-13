using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VampireHunterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDStake
    {
        public static Sprite Placeholder => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.VampireHunter))
                return;
                
            var role = Role.GetRole<VampireHunter>(PlayerControl.LocalPlayer);

            if (role.StakeButton == null)
            {
                role.StakeButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.StakeButton.graphic.enabled = true;
                role.StakeButton.graphic.sprite = Placeholder;
                role.StakeButton.gameObject.SetActive(false);
            }

            role.StakeButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && !role.VampsDead);
            role.StakeButton.SetCoolDown(role.StakeTimer(), CustomGameOptions.StakeCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, role.StakeButton);
            var renderer = role.StakeButton.graphic;
            
            if (role.ClosestPlayer != null && !role.StakeButton.isCoolingDown)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }

            if (role.VampsDead && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                role.TurnVigilante();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable, -1);
                writer.Write((byte)TurnRPC.TurnVigilante);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return;
            }
        }
    }
}
