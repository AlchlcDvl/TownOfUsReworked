using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using UnityEngine;
using TownOfUsReworked.Lobby.CustomOption;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.MafiosoMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDKillOrPromote
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Mafioso))
                return;
            
            var role = Role.GetRole<Mafioso>(PlayerControl.LocalPlayer);

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.graphic.enabled = true;
                role.KillButton.gameObject.SetActive(false);
            }

            role.KillButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.IntKillCooldown);
            var notSyndicate = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Intruder)).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.KillButton, notSyndicate);
            var renderer = role.KillButton.graphic;
            
            if (role.ClosestPlayer != null && !role.KillButton.isCoolingDown)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }

            if (role.CanPromote && !role.Player.Data.IsDead)
            {
                role.TurnGodfather();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable, -1);
                writer.Write((byte)TurnRPC.TurnGodfather);
                writer.Write(role.Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return;
            }
        }
    }
}