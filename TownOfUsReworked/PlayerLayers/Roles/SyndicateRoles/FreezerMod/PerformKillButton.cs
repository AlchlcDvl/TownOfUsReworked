using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.FreezerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton

    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Freezer))
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (__instance.isCoolingDown)
                return false;

            if (!__instance.enabled)
                return false;

            var role = Role.GetRole<Freezer>(PlayerControl.LocalPlayer);

            if (role.ClosestPlayer == null)
                return false;
            
            var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];

            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(), PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance)
                return false;
            
            if (role.freezeList.ContainsKey(role.ClosestPlayer.PlayerId))
                return false;

            unchecked
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Freeze,
                    SendOption.Reliable, -1);
                writer.Write(role.Player.PlayerId);
                writer.Write(role.ClosestPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            
            role.freezeList.Add(role.ClosestPlayer.PlayerId, 0);

            Utils.AirKill(role.Player, role.ClosestPlayer);
            SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.5f);
            __instance.SetCoolDown(role.Player.killTimer, CustomGameOptions.FreezerCooldown);
            role.Player.killTimer = CustomGameOptions.FreezerCooldown;
            return false;
        }
    }
}