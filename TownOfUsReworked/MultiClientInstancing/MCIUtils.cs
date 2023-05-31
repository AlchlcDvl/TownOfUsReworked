namespace TownOfUsReworked.MultiClientInstancing
{
    [HarmonyPatch]
    public static class MCIUtils
    {
        public static readonly Dictionary<int, ClientData> Clients = new();
        public static readonly Dictionary<byte, int> PlayerIdClientId = new();

        public static int AvailableId()
        {
            for (var i = 1; i < 128; i++)
            {
                if (!Clients.ContainsKey(i) && PlayerControl.LocalPlayer.OwnerId != i)
                    return i;
            }

            return -1;
        }

        public static void CleanUpLoad()
        {
            if (GameData.Instance.AllPlayers.Count == 1)
            {
                Clients.Clear();
                PlayerIdClientId.Clear();
            }
        }

        public static PlayerControl CreatePlayerInstance()
        {
            var samplePSD = new PlatformSpecificData()
            {
                Platform = Platforms.StandaloneWin10,
                PlatformName = "Robot"
            };

            var sampleId = AvailableId();
            var sampleC = new ClientData(sampleId, $"Robot-{sampleId}", samplePSD, 5, "", "");

            AmongUsClient.Instance.CreatePlayer(sampleC);
            AmongUsClient.Instance.allClients.Add(sampleC);

            sampleC.Character.SetName($"Robot {sampleC.Character.PlayerId}");
            sampleC.Character.SetSkin(HatManager.Instance.allSkins[URandom.Range(0, HatManager.Instance.allSkins.Count)].ProdId, 0);
            sampleC.Character.SetNamePlate(HatManager.Instance.allNamePlates[URandom.Range(0, HatManager.Instance.allNamePlates.Count)].ProdId);
            sampleC.Character.SetHat("hat_NoHat", 0);
            sampleC.Character.SetColor(URandom.Range(0, Palette.PlayerColors.Length));

            Clients.Add(sampleId, sampleC);
            PlayerIdClientId.Add(sampleC.Character.PlayerId, sampleId);
            return sampleC.Character;
        }

        public static void RemovePlayer(byte id)
        {
            if (id == 0)
                return;

            var clientId = Clients.FirstOrDefault(x => x.Value.Character.PlayerId == id).Key;
            Clients.Remove(clientId, out var outputData);
            PlayerIdClientId.Remove(id);
            AmongUsClient.Instance.RemovePlayer(clientId, DisconnectReasons.ExitGame);
            AmongUsClient.Instance.allClients.Remove(outputData);
        }

        public static void RemoveAllPlayers()
        {
            foreach (var playerId in PlayerIdClientId.Keys)
                RemovePlayer(playerId);

            SwitchTo(AmongUsClient.Instance.allClients[0].Character.PlayerId);
        }

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

            HudManager.Instance.ShadowQuad.gameObject.SetActive(!newPlayer.Data.IsDead);

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