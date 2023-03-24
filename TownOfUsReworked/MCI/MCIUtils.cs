using InnerNet;
using UnityEngine;
using System.Linq;

namespace TownOfUsReworked.MCI
{
    public static class MCIUtils
    {
        public const int MaxID = 100;

        public static int AvailableId()
        {
            for (int i = 2; i < MaxID; i++)
            {
                if (!InstanceControl.Clients.ContainsKey(i) && PlayerControl.LocalPlayer.OwnerId != i)
                    return i;
            }

            return -1;
        }

        public static void CleanUpLoad()
        {
            if (GameData.Instance.AllPlayers.Count == 1)
            {
                InstanceControl.Clients.Clear();
                InstanceControl.PlayerIdClientId.Clear();
            }
        }

        public static PlayerControl CreatePlayerInstance()
        {
            PlatformSpecificData samplePSD = new()
            {
                Platform = Platforms.StandaloneWin10,
                PlatformName = "Robot"
            };

            var sampleId = AvailableId();
            var sampleC = new ClientData(sampleId, $"Robot-{sampleId}", samplePSD, 5, "", "");

            AmongUsClient.Instance.CreatePlayer(sampleC);
            AmongUsClient.Instance.allClients.Add(sampleC);

            sampleC.Character.SetName($"Robot {sampleC.Character.PlayerId}");
            sampleC.Character.SetSkin(HatManager.Instance.allSkins[Random.Range(0, HatManager.Instance.allSkins.Count)].ProdId, 0);
            sampleC.Character.SetHat(HatManager.Instance.allHats[Random.Range(0, HatManager.Instance.allSkins.Count)].ProdId, 0);
            sampleC.Character.SetColor(Random.Range(0, Palette.PlayerColors.Length));

            InstanceControl.Clients.Add(sampleId, sampleC);
            InstanceControl.PlayerIdClientId.Add(sampleC.Character.PlayerId, sampleId);
            return sampleC.Character;
        }

        public static void RemovePlayer(byte id)
        {
            int clientId = InstanceControl.Clients.FirstOrDefault(x => x.Value.Character.PlayerId == id).Key;
            InstanceControl.Clients.Remove(clientId, out ClientData outputData);
            InstanceControl.PlayerIdClientId.Remove(id);
            AmongUsClient.Instance.RemovePlayer(clientId, DisconnectReasons.ExitGame);
            AmongUsClient.Instance.allClients.Remove(outputData);
        }

        public static void RemoveAllPlayers()
        {
            foreach (byte playerId in InstanceControl.PlayerIdClientId.Keys)
                RemovePlayer(playerId);

            InstanceControl.SwitchTo(AmongUsClient.Instance.allClients[0].Character.PlayerId);
            TownOfUsReworked.MCIActive = false;
        }
    }
}