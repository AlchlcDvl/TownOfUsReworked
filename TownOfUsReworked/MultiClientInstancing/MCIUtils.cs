namespace TownOfUsReworked.MultiClientInstancing;

public static class MCIUtils
{
    public static readonly Dictionary<int, ClientData> Clients = new();
    public static readonly Dictionary<byte, int> PlayerIdClientIDs = new();
    public static readonly Dictionary<byte, Vector3> SavedPositions = new();

    public static int AvailableId()
    {
        for (var i = 1; i < 128; i++)
        {
            if (!Clients.ContainsKey(i) && CustomPlayer.Local.OwnerId != i)
                return i;
        }

        return -1;
    }

    public static void CleanUpLoad()
    {
        if (GameData.Instance.AllPlayers.Count == 1)
        {
            Clients.Clear();
            PlayerIdClientIDs.Clear();
        }
    }

    public static PlayerControl CreatePlayerInstance()
    {
        var samplePSD = new PlatformSpecificData()
        {
            Platform = Platforms.StandaloneWin10,
            PlatformName = "Bot"
        };

        var sampleId = AvailableId();
        var sampleC = new ClientData(sampleId, $"Bot-{sampleId}", samplePSD, 1, "", "robotmodeactivate");

        AmongUsClient.Instance.CreatePlayer(sampleC);
        AmongUsClient.Instance.allClients.Add(sampleC);

        sampleC.Character.SetName($"Bot {sampleC.Character.PlayerId}");
        sampleC.Character.SetSkin(HatManager.Instance.allSkins[URandom.RandomRangeInt(0, HatManager.Instance.allSkins.Count)].ProdId, 0);
        sampleC.Character.SetNamePlate(HatManager.Instance.allNamePlates[URandom.RandomRangeInt(0, HatManager.Instance.allNamePlates.Count)].ProdId);
        sampleC.Character.SetPet(HatManager.Instance.allPets[URandom.RandomRangeInt(0, HatManager.Instance.allPets.Count)].ProdId);
        sampleC.Character.SetHat("hat_NoHat", 0);
        sampleC.Character.SetColor(URandom.RandomRangeInt(0, Palette.PlayerColors.Length));
        sampleC.Character.MyPhysics.ResetMoveState();

        Clients.Add(sampleId, sampleC);
        PlayerIdClientIDs.Add(sampleC.Character.PlayerId, sampleId);

        if (SubLoaded)
            ImpartSub(sampleC.Character);

        return sampleC.Character;
    }

    public static void RemovePlayer(byte id)
    {
        if (id == 0)
            return;

        var clientId = Clients.FirstOrDefault(x => x.Value.Character.PlayerId == id).Key;
        Clients.Remove(clientId, out var outputData);
        PlayerIdClientIDs.Remove(id);
        SavedPositions.Remove(id);
        AmongUsClient.Instance.RemovePlayer(clientId, DisconnectReasons.Custom);
        AmongUsClient.Instance.allClients.Remove(outputData);
    }

    public static void RemoveAllPlayers()
    {
        PlayerIdClientIDs.Keys.ForEach(RemovePlayer);
        SwitchTo(0);
    }

    public static void SwitchTo(byte playerId)
    {
        if (!TownOfUsReworked.MCIActive)
            return;

        if (Meeting)
        {
            PlayerLayer.LocalLayers.ForEach(x => x.OnMeetingEnd(Meeting));
            ButtonUtils.DisableAllButtons();
        }
        else
        {
            CustomPlayer.Local.DisableButtons();
            CustomPlayer.Local.DisableArrows();
        }

        if (!SavedPositions.ContainsKey(CustomPlayer.Local.PlayerId))
            SavedPositions.Add(CustomPlayer.Local.PlayerId, CustomPlayer.LocalCustom.Position);
        else
            SavedPositions[CustomPlayer.Local.PlayerId] = CustomPlayer.LocalCustom.Position;

        PlayerLayer.LocalLayers.ForEach(x => x.ExitingLayer());

        CustomPlayer.Local.NetTransform.RpcSnapTo(CustomPlayer.LocalCustom.Position);
        CustomPlayer.Local.moveable = false;

        var light = CustomPlayer.Local.lightSource;
        var savedId = CustomPlayer.Local.PlayerId;

        //Setup new player
        var newPlayer = PlayerById(playerId);

        if (newPlayer == null)
            return;

        PlayerControl.LocalPlayer = newPlayer;
        CustomPlayer.Local.lightSource = light;
        CustomPlayer.Local.moveable = true;

        AmongUsClient.Instance.ClientId = newPlayer.OwnerId;
        AmongUsClient.Instance.HostId = newPlayer.OwnerId;

        HUD.SetHudActive(true);
        HUD.ShadowQuad.gameObject.SetActive(!newPlayer.Data.IsDead);

        light.transform.SetParent(CustomPlayer.LocalCustom.Transform);
        light.transform.localPosition = CustomPlayer.Local.Collider.offset;

        Camera.main!.GetComponent<FollowerCamera>().SetTarget(newPlayer);
        CustomPlayer.Local.MyPhysics.ResetMoveState(true);
        KillAnimation.SetMovement(CustomPlayer.Local, true);

        if (Meeting)
            PlayerLayer.LocalLayers.ForEach(x => x.OnMeetingStart(Meeting));
        else
        {
            CustomPlayer.Local.EnableButtons();
            CustomPlayer.Local.EnableArrows();
        }

        PlayerLayer.LocalLayers.ForEach(x => x.EnteringLayer());

        HUD.Chat.SetVisible(CustomPlayer.Local.CanChat());

        if (SavedPositions.TryGetValue(playerId, out var pos))
            CustomPlayer.Local.NetTransform.RpcSnapTo(pos);

        if (SavedPositions.TryGetValue(savedId, out var pos2))
            PlayerById(savedId).NetTransform.RpcSnapTo(pos2);
    }

    public static void SetForegroundForAlive(this MeetingHud __instance)
    {
        __instance.amDead = false;
        __instance.SkipVoteButton.gameObject.SetActive(true);
        __instance.SkipVoteButton.AmDead = false;
        __instance.Glass.gameObject.SetActive(false);
    }
}