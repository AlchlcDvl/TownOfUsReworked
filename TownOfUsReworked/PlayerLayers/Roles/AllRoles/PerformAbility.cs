using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;
using Hazel;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.AllRoles
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformAbility
    {
        public static bool Prefix(AbilityButton __instance)
        {
            var role = Role.GetRole(PlayerControl.LocalPlayer);

            if (role == null)
                return true;

            if (!PlayerControl.LocalPlayer.Data.IsDead)
                return true;

            if (__instance == role.ZoomButton)
            {
                role.Zooming = !role.Zooming;
                var orthographicSize = !role.Zooming ? 3f : 12f;
                Camera.main.orthographicSize = orthographicSize;

                foreach (var cam in Camera.allCameras)
                {
                    if (cam?.gameObject.name == "UI Camera")
                        cam.orthographicSize = orthographicSize;
                }

                ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height);
                return false;
            }
            else if (__instance == role.BombKillButton)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestBoom))
                    return false;

                if (!role.Bombed)
                    return false;

                role.Player.GetEnforcer().BombSuccessful = Utils.Interact(role.Player, role.ClosestBoom, true)[3];
                role.Bombed = false;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.ForceKill);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return false;
            }

            return true;
        }
    }
}