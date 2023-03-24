using System;
using HarmonyLib;
using Hazel;
using Reactor.Networking.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.TeleporterMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformAbility
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Teleporter))
                return true;

            var role = Role.GetRole<Teleporter>(PlayerControl.LocalPlayer);

            if (__instance == role.MarkButton)
            {
                if (!role.CanMark)
                    return false;

                if (role.MarkTimer() != 0f)
                    return false;

                if (role.TeleportPoint == PlayerControl.LocalPlayer.transform.position)
                    return false;

                role.TeleportPoint = PlayerControl.LocalPlayer.transform.position;
                role.LastMarked = DateTime.UtcNow;

                if (CustomGameOptions.TeleCooldownsLinked)
                    role.LastTeleport = DateTime.UtcNow;

                return false;
            }
            else if (__instance == role.TeleportButton)
            {
                if (role.TeleportTimer() != 0f)
                    return false;

                if (role.TeleportPoint == PlayerControl.LocalPlayer.transform.position)
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Teleport);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(role.TeleportPoint);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.LastTeleport = DateTime.UtcNow;
                Teleporter.Teleport(role.Player);

                if (CustomGameOptions.TeleCooldownsLinked)
                    role.LastMarked = DateTime.UtcNow;

                return false;
            }

            return true;
        }
    }
}
