using InnerNet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TownOfUsReworked.Classes;
using HarmonyLib;
using TownOfUsReworked.Custom;

namespace TownOfUsReworked.MultiClientInstancing
{
    [HarmonyPatch]
    public static class InstanceControl
    {
        public static readonly Dictionary<int, ClientData> Clients = new();
        public static readonly Dictionary<byte, int> PlayerIdClientId = new();

        public static void SwitchTo(byte playerId)
        {
            if (!TownOfUsReworked.MCIActive)
                return;

            PlayerControl.LocalPlayer.DisableButtons();
            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(PlayerControl.LocalPlayer.transform.position);
            PlayerControl.LocalPlayer.moveable = false;

            var light = PlayerControl.LocalPlayer.lightSource;

            //Setup new player
            var newPlayer = Utils.PlayerById(playerId);
            PlayerControl.LocalPlayer = newPlayer;
            PlayerControl.LocalPlayer.lightSource = light;
            PlayerControl.LocalPlayer.moveable = true;

            AmongUsClient.Instance.ClientId = newPlayer.OwnerId;
            AmongUsClient.Instance.HostId = newPlayer.OwnerId;

            HudManager.Instance.SetHudActive(true);

            light.transform.SetParent(PlayerControl.LocalPlayer.transform);
            light.transform.localPosition = PlayerControl.LocalPlayer.Collider.offset;

            Camera.main!.GetComponent<FollowerCamera>().SetTarget(newPlayer);
            PlayerControl.LocalPlayer.MyPhysics.ResetMoveState(true);
            KillAnimation.SetMovement(PlayerControl.LocalPlayer, true);

            PlayerControl.LocalPlayer.EnableButtons();
        }

        public static void SwitchTo(int clientId)
        {
            byte? id = PlayerIdClientId.Keys.FirstOrDefault(x => PlayerIdClientId[x] == clientId);

            if (id != null)
                SwitchTo((byte)id);
        }
    }
}