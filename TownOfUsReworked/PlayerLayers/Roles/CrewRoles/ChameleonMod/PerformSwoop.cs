using HarmonyLib;
using Hazel;
using System;
using Reactor.Utilities;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ChameleonMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformSwoop
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Chameleon, true))
                return false;

            var role = Role.GetRole<Chameleon>(PlayerControl.LocalPlayer);

            if (__instance != role.SwoopButton)
                return false;
            
            if (!Utils.ButtonUsable(__instance))
                return false;

            if (role.SwoopTimer() != 0 && __instance == role.SwoopButton)
                return false;

            if (__instance == role.SwoopButton)
            {
                role.TimeRemaining = CustomGameOptions.SwoopDuration;
                role.RegenTask();
                role.Invis();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.Swoop);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return false;
            }

            return true;
        }
    }
}