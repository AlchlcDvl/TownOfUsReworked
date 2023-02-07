using System;
using HarmonyLib;
using Hazel;
using UnityEngine;
using Reactor.Networking.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
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
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Teleporter);

            if (!flag)
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var role = Role.GetRole<Teleporter>(PlayerControl.LocalPlayer);

            if (__instance == role.TeleportButton)
            {
                if (role.Player.inVent)
                    return false;

                if (!__instance.isActiveAndEnabled)
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
                    if (__instance.isCoolingDown)
                        return false;

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

            return true;
        }
    }
}
