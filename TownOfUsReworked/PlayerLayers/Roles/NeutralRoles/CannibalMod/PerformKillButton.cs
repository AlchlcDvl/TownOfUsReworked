using System;
using HarmonyLib;
using Hazel;
using Reactor.Utilities;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CannibalMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Cannibal))
                return true;

            if (!PlayerControl.LocalPlayer.CanMove || PlayerControl.LocalPlayer.Data.IsDead || !__instance.isActiveAndEnabled || __instance.isCoolingDown)
                return false;

            var role = Role.GetRole<Cannibal>(PlayerControl.LocalPlayer);

            if (role.EatTimer() != 0)
                return false;

            var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];

            if (Vector2.Distance(role.CurrentTarget.TruePosition, PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance)
                return false;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
            writer.Write((byte)ActionsRPC.CannibalEat);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(role.CurrentTarget.ParentId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            
            //SoundManager.Instance.PlaySound(TownOfUsReworked.EatSound, false, 0.4f);

            role.LastEaten = DateTime.UtcNow;
            Coroutines.Start(Coroutine.EatCoroutine(role.CurrentTarget, role));
            return false;
        }
    }
}