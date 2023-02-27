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
                if (!InstanceControl.clients.ContainsKey(i) && PlayerControl.LocalPlayer.OwnerId != i)
                    return i;
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
            PlatformSpecificData samplePSD = new()
            {
                Platform = Platforms.StandaloneWin10,
                PlatformName = "Robot"
            };

            int sampleId = id;

            if (sampleId == -1)
                sampleId = AvailableId();

            var sampleC = new ClientData(sampleId, name + $"-{sampleId}", samplePSD, 5, "", "");

            AmongUsClient.Instance.CreatePlayer(sampleC);
            AmongUsClient.Instance.allClients.Add(sampleC);

            sampleC.Character.SetName(name + $" {{{sampleC.Character.PlayerId}:{sampleId}}}");
            sampleC.Character.SetSkin(HatManager.Instance.allSkins[Random.Range(0, HatManager.Instance.allSkins.Count)].ProdId, 0);
            sampleC.Character.SetHat(HatManager.Instance.allHats[UnityEngine.Random.Range(0, HatManager.Instance.allSkins.Count)].ProdId, 0);
            sampleC.Character.SetColor(Random.Range(0, Palette.PlayerColors.Length));

            InstanceControl.clients.Add(sampleId, sampleC);
            InstanceControl.PlayerIdClientId.Add(sampleC.Character.PlayerId, sampleId);
            return sampleC.Character;
        }
    }
}