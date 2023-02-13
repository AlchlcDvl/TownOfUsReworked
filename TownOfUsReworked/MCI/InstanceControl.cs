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
        public const int MaxID = 100;

        public static int AvailableId()
        {
            for (int i = 2; i < MaxID; i++)
            {
                if (!clients.ContainsKey(i))
                {
                    if (PlayerControl.LocalPlayer.OwnerId != i)
                        return i;
                }
            }

            return -1;
        }

        public static void SwitchTo(byte playerId)
        {
            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(PlayerControl.LocalPlayer.transform.position);
            PlayerControl.LocalPlayer.moveable = false;
            
            //Setup new player
            var newPlayer = Utils.PlayerById(playerId);

            newPlayer.lightSource = Object.Instantiate(PlayerControl.LocalPlayer.LightPrefab, newPlayer.transform);
            newPlayer.lightSource.Initialize(newPlayer.Collider.offset);

            newPlayer.moveable = true;
            newPlayer.MyPhysics.ResetMoveState();
            KillAnimation.SetMovement(newPlayer, true);
            newPlayer.MyPhysics.inputHandler.enabled = true;
            
            //Assign new player
            PlayerControl.LocalPlayer = newPlayer;
            AmongUsClient.Instance.ClientId = newPlayer.OwnerId;
            AmongUsClient.Instance.HostId = newPlayer.OwnerId;

            DestroyableSingleton<HudManager>.Instance.SetHudActive(true);

            //Hacky "fix" for twix and Det
            DestroyableSingleton<HudManager>.Instance.KillButton.transform.parent.GetComponentsInChildren<Transform>().ToList().ForEach(x =>
            {
                if (x.gameObject.name == "KillButton(Clone)")
                    Object.Destroy(x.gameObject);
            });

            DestroyableSingleton<HudManager>.Instance.transform.GetComponentsInChildren<Transform>().ToList().ForEach(x =>
            {
                if (x.gameObject.name == "KillButton(Clone)")
                    Object.Destroy(x.gameObject);
            });
            
            Camera.main!.GetComponent<FollowerCamera>().SetTarget(newPlayer);
        }

        public static void SwitchTo(int clientId)
        {
            byte? id = PlayerIdClientId.Keys.FirstOrDefault(x => PlayerIdClientId[x] == clientId);

            if (id != null)
                SwitchTo((byte)id);
        }
    }
}