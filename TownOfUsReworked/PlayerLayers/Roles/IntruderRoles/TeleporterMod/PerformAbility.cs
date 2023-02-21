using System;
using HarmonyLib;
using Hazel;
using UnityEngine;
using Reactor.Networking.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.TeleporterMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAbility
    {
        public static Sprite MarkSprite => TownOfUsReworked.MarkSprite;
        public static Sprite TeleportSprite => TownOfUsReworked.TeleportSprite;

        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Teleporter))
                return false;

            var role = Role.GetRole<Teleporter>(PlayerControl.LocalPlayer);

            if (__instance == role.TeleportButton)
            {
                if (role.Player.inVent)
                    return false;

                if (!Utils.ButtonUsable(__instance))
                    return false;

                if (role.TeleportButton.graphic.sprite == MarkSprite)
                {
                    role.TeleportPoint = PlayerControl.LocalPlayer.transform.position;
                    role.TeleportButton.graphic.sprite = TeleportSprite;
                    DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);

                    if (role.TeleportTimer() < 5f)
                        role.LastTeleport = DateTime.UtcNow.AddSeconds(5 - CustomGameOptions.TeleportCd);
                }
                else
                {
                    if (role.TeleportTimer() != 0f)
                        return false;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                    writer.Write((byte)ActionsRPC.Teleport);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.TeleportPoint);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.LastTeleport = DateTime.UtcNow;
                    Teleporter.Teleport(role.Player);
                }

                return false;
            }
            else if (__instance == role.KillButton)
            {
                if (role.KillTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence), true);

                if (interact[3] == true || interact[0] == true)
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
