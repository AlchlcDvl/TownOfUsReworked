using Hazel;
using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EscortMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Escort))
                return false;

            var role = Role.GetRole<Escort>(PlayerControl.LocalPlayer);

            if (Utils.CannotUseButton(role.Player, RoleEnum.Escort, role.ClosestPlayer, __instance))
                return false;

            if (role.RoleblockTimer() != 0f && __instance == role.BlockButton)
                return false;

            Utils.Spread(role.Player, role.ClosestPlayer);

            if (Utils.CheckInteractionSesitive(role.ClosestPlayer, Role.GetRoleValue(RoleEnum.SerialKiller)))
            {
                Utils.AlertKill(role.Player, role.ClosestPlayer);

                if (CustomGameOptions.ShieldBreaks)
                    role.LastBlock = DateTime.UtcNow;

                return false;
            }
            
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.EscRoleblock, SendOption.Reliable, -1);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            role.TimeRemaining = CustomGameOptions.EscRoleblockDuration;
            role.Block();
            return false;
        }
    }
}
