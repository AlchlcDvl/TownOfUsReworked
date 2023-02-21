using InnerNet;
using UnityEngine;

namespace TownOfUsReworked.MCI
{
    public class MCIUtils
    {
        public const int MaxID = 100;

        public static int AvailableId()
        {
            for (int i = 2; i < MaxID; i++)
            {
                if (!InstanceControl.clients.ContainsKey(i))
                {
                    if (PlayerControl.LocalPlayer.OwnerId != i)
                        return i;
                }
            }

            return -1;
        }

        public static void CleanUpLoad()
        {
            if (GameData.Instance.AllPlayers.Count == 1)
            {
                InstanceControl.clients.Clear();
                InstanceControl.PlayerIdClientId.Clear();
            } 
        }

        public static PlayerControl CreatePlayerInstance(string name, int id = -1)
        {
            PlatformSpecificData samplePSD = new PlatformSpecificData();
            samplePSD.Platform = Platforms.StandaloneItch;
            samplePSD.PlatformName = "Robot";

            int sampleId = id;

            if (sampleId == -1)
                sampleId = AvailableId();

            var sampleC = new ClientData(sampleId, name + $"-{sampleId}", samplePSD, 5, "", "");
            PlayerControl playerControl = Object.Instantiate<PlayerControl>(AmongUsClient.Instance.PlayerPrefab, Vector3.zero, Quaternion.identity);
            playerControl.PlayerId = (byte)GameData.Instance.GetAvailableId();
            playerControl.FriendCode = sampleC.FriendCode;
            playerControl.Puid = sampleC.ProductUserId;
            sampleC.Character = playerControl;

            if (ShipStatus.Instance)
                ShipStatus.Instance.SpawnPlayer(playerControl, Palette.PlayerColors.Length, false);

            AmongUsClient.Instance.Spawn(playerControl, sampleC.Id, SpawnFlags.IsClientCharacter);
            GameData.Instance.AddPlayer(playerControl);
            
            playerControl.SetName(name + $" ({playerControl.PlayerId}:{sampleId})");
            playerControl.SetSkin(HatManager.Instance.allSkins[UnityEngine.Random.Range(0, HatManager.Instance.allSkins.Count)].ProdId, 0);
            playerControl.SetHat(HatManager.Instance.allHats[UnityEngine.Random.Range(0, HatManager.Instance.allSkins.Count)].ProdId, 0);
            playerControl.SetColor(UnityEngine.Random.Range(0, Palette.PlayerColors.Length));

            //PlayerControl.AllPlayerControls.Add(playerControl);
            InstanceControl.clients.Add(sampleId, sampleC);
            InstanceControl.PlayerIdClientId.Add(playerControl.PlayerId, sampleId);
            return playerControl;
        }
    }
}