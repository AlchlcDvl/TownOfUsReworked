using InnerNet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.MCI
{
    public static class InstanceControl
    {
        public static readonly Dictionary<int, ClientData> Clients = new();
        public static readonly Dictionary<byte, int> PlayerIdClientId = new();

        public static void SwitchTo(byte playerId)
        {
            if (!TownOfUsReworked.MCIActive)
                return;

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

            //Hacky "fix" for twix, AD and Det
            HudManager.Instance.KillButton.transform.parent.GetComponentsInChildren<Transform>().ToList().ForEach(x =>
            {
                if (x.gameObject.name == "KillButton(Clone)")
                    Object.Destroy(x.gameObject);
            });

            HudManager.Instance.AbilityButton.transform.parent.GetComponentsInChildren<Transform>().ToList().ForEach(x =>
            {
                if (x.gameObject.name == "AbilityButton(Clone)")
                    Object.Destroy(x.gameObject);
            });

            HudManager.Instance.transform.GetComponentsInChildren<Transform>().ToList().ForEach(x =>
            {
                if (x.gameObject.name == "KillButton(Clone)")
                    Object.Destroy(x.gameObject);

                if (x.gameObject.name == "AbilityButton(Clone)")
                    Object.Destroy(x.gameObject);
            });

            light.transform.SetParent(PlayerControl.LocalPlayer.transform);
            light.transform.localPosition = PlayerControl.LocalPlayer.Collider.offset;

            Camera.main!.GetComponent<FollowerCamera>().SetTarget(newPlayer);
            PlayerControl.LocalPlayer.MyPhysics.ResetMoveState(true);
            KillAnimation.SetMovement(PlayerControl.LocalPlayer, true);
        }

        public static void SwitchTo(int clientId)
        {
            byte? id = PlayerIdClientId.Keys.FirstOrDefault(x => PlayerIdClientId[x] == clientId);

            if (id != null)
                SwitchTo((byte)id);
        }
    }
}