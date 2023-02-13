using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.Lobby.CustomOption;
using Reactor.Networking.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.UndertakerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAbility
    {
        public static Sprite Drag => TownOfUsReworked.DragSprite;

        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Undertaker))
                return false;

            var role = Role.GetRole<Undertaker>(PlayerControl.LocalPlayer);

            if (__instance == role.DragDropButton)
            {
                if (role.DragDropButton.graphic.sprite == Drag)
                {
                    if (!Utils.ButtonUsable(__instance))
                        return false;

                    if (Utils.IsTooFar(role.Player, role.CurrentTarget))
                        return false;

                    var playerId = role.CurrentTarget.ParentId;
                    var player = Utils.PlayerById(playerId);
                    Utils.Spread(role.Player, player);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                    writer.Write((byte)ActionsRPC.Drag);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(playerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.CurrentlyDragging = role.CurrentTarget;
                    KillButtonTarget.SetTarget(__instance, null, role);
                    role.DragDropButton.graphic.sprite = TownOfUsReworked.DropSprite;
                    return false;
                }
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
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
                    __instance.graphic.sprite = TownOfUsReworked.DragSprite;
                    role.LastDragged = DateTime.UtcNow;
                    body.transform.position = position;
                    return false;
                }
            }
            else if (__instance == role.KillButton)
            {
                if (role.KillTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence), true);

                if (interact[3] == true && interact[0] == true)
                    role.LastKilled = DateTime.UtcNow;
                else if (interact[0] == true)
                    role.LastKilled = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2] == true)
                    role.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);

                return false;
            }

            return false;
        }
    }
}
