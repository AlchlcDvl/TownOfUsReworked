using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using UnityEngine;
using Reactor.Networking.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.UndertakerMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformDrag
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Undertaker))
                return true;

            if (!Utils.ButtonUsable(__instance))
                return false;

            var role = Role.GetRole<Undertaker>(PlayerControl.LocalPlayer);

            if (role.IsBlocked)
                return false;

            if (__instance == role.DragButton)
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

                if (SubmergedCompatibility.isSubmerged())
                {
                    if (position.y > -7f)
                        position.z = 0.0208f;
                    else
                        position.z = -0.0273f;
                }
                
                writer.Write(position);
                writer.Write(position.z);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                var body = role.CurrentlyDragging;
                body.bodyRenderer.material.SetFloat("_Outline", 0f);
                role.CurrentlyDragging = null;
                role.LastDragged = DateTime.UtcNow;
                body.transform.position = position;
                return false;
            }

            return true;
        }
    }
}
