using InnerNet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.MCI
{
    public static class InstanceControl
    {
        public static Dictionary<int, ClientData> clients = new();
        public static Dictionary<byte, int> PlayerIdClientId = new();
        public static bool MCIActive = false;

        public static void SwitchTo(byte playerId)
        {
            if (!MCIActive)
                return;

            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(PlayerControl.LocalPlayer.transform.position);
            PlayerControl.LocalPlayer.moveable = false;

            Object.Destroy(PlayerControl.LocalPlayer.lightSource);
            HudManager.Instance.KillButton.buttonLabelText.gameObject.SetActive(false);

            //Setup new player
            var newPlayer = Utils.PlayerById(playerId);
            PlayerControl.LocalPlayer = newPlayer;
            PlayerControl.LocalPlayer.moveable = true;

            AmongUsClient.Instance.ClientId = newPlayer.OwnerId;
            AmongUsClient.Instance.HostId = newPlayer.OwnerId;

            HudManager.Instance.SetHudActive(true);

            //Hacky "fix" for twix and Det
            HudManager.Instance.KillButton.transform.parent.GetComponentsInChildren<Transform>().ToList().ForEach(x =>
            {
                if (x.gameObject.name == "KillButton(Clone)")
                    Object.Destroy(x.gameObject);
            });

            HudManager.Instance.KillButton.transform.GetComponentsInChildren<Transform>().ToList().ForEach((x) =>
            {
                if (x.gameObject.name == "KillTimer_TMP(Clone)")
                    Object.Destroy(x.gameObject);
            });

            HudManager.Instance.transform.GetComponentsInChildren<Transform>().ToList().ForEach((x) =>
            {
                if (x.gameObject.name == "KillButton(Clone)")
                    Object.Destroy(x.gameObject);
            });

            PlayerControl.LocalPlayer.lightSource = Object.Instantiate(PlayerControl.LocalPlayer.LightPrefab);
            PlayerControl.LocalPlayer.lightSource.transform.SetParent(PlayerControl.LocalPlayer.transform);
            PlayerControl.LocalPlayer.lightSource.transform.localPosition = PlayerControl.LocalPlayer.Collider.offset;
            PlayerControl.LocalPlayer.lightSource.Initialize(PlayerControl.LocalPlayer.Collider.offset * 0.5f);
            
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