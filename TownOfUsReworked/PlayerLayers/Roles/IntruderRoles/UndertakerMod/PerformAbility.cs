using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.Lobby.CustomOption;
using Reactor.Networking.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using AmongUs.GameOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.UndertakerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAbility
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Undertaker);

            if (!flag)
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var role = Role.GetRole<Undertaker>(PlayerControl.LocalPlayer);

            if (__instance == role.DragDropButton)
            {
                if (role.DragDropButton.graphic.sprite == TownOfUsReworked.DragSprite)
                {
                    if (__instance.isCoolingDown)
                        return false;

                    if (!__instance.enabled)
                        return false;

                    var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];

                    if (Vector2.Distance(role.CurrentTarget.TruePosition, PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance)
                        return false;

                    var playerId = role.CurrentTarget.ParentId;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                    writer.Write((byte)ActionsRPC.Drag);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(playerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.CurrentlyDragging = role.CurrentTarget;
                    KillButtonTarget.SetTarget(__instance, null, role);
                    __instance.graphic.sprite = TownOfUsReworked.DropSprite;
                    return false;
                }
                else
                {
                    if (!__instance.enabled)
                        return false;

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

            return true;
        }
    }
}
