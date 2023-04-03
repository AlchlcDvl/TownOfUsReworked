using HarmonyLib;
using Hazel;
using Reactor.Utilities;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using System;
using TownOfUsReworked.Data;
using UnityEngine;
using Reactor.Networking.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.JanitorMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformClean
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Janitor))
                return true;

            var role = Role.GetRole<Janitor>(PlayerControl.LocalPlayer);

            if (__instance == role.CleanButton)
            {
                if (role.CleanTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.CurrentTarget))
                    return false;

                var playerId = role.CurrentTarget.ParentId;
                var player = Utils.PlayerById(playerId);
                Utils.Spread(role.Player, player);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.FadeBody);
                writer.Write(playerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Coroutines.Start(Utils.FadeBody(role.CurrentTarget));
                role.LastCleaned = DateTime.UtcNow;

                if (CustomGameOptions.JaniCooldownsLinked)
                    role.LastKilled = DateTime.UtcNow;

                return false;
            }
            else if (__instance == role.DragButton)
            {
                if (Utils.IsTooFar(role.Player, role.CurrentTarget))
                    return false;

                var playerId = role.CurrentTarget.ParentId;
                var player = Utils.PlayerById(playerId);
                Utils.Spread(role.Player, player);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Drag);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(playerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.CurrentlyDragging = role.CurrentTarget;
                return false;
            }
            else if (__instance == role.DropButton)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Drop);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                Vector3 position = PlayerControl.LocalPlayer.GetTruePosition();

                if (SubmergedCompatibility.IsSubmerged())
                {
                    if (position.y > -7f)
                        position.z = 0.0208f;
                    else
                        position.z = -0.0273f;
                }

                position.y -= 0.3636f;
                writer.Write(position);
                writer.Write(position.z);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                foreach (var component in role.CurrentlyDragging?.bodyRenderers)
                    component.material.SetFloat("_Outline", 0f);

                role.CurrentlyDragging.transform.position = position;
                role.CurrentlyDragging = null;
                role.LastDragged = DateTime.UtcNow;
                return false;
            }

            return true;
        }
    }
}