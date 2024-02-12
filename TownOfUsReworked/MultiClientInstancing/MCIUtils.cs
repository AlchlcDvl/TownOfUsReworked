namespace TownOfUsReworked.MultiClientInstancing;

public static class MCIUtils
{
    public static readonly Dictionary<int, ClientData> Clients = new();
    public static readonly Dictionary<byte, int> PlayerClientIDs = new();
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
            PlayerClientIDs.Clear();
            SavedPositions.Clear();
        }
    }

    public static void CreatePlayerInstances(int count)
    {
        for (var i = 0; i < count; i++)
            CreatePlayerInstance();
    }

    public static void CreatePlayerInstance()
    {
        var sampleId = AvailableId();
        var sampleC = new ClientData(sampleId, $"Bot-{sampleId}", new()
        {
            Platform = Platforms.StandaloneWin10,
            PlatformName = "Bot"
        }, 1, "", "robotmodeactivate");

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
        PlayerClientIDs.Add(sampleC.Character.PlayerId, sampleId);
        SavedPositions.Remove(sampleC.Character.PlayerId);

        if (SubLoaded)
            ImpartSub(sampleC.Character);
    }

    public static void RemovePlayer(byte id)
    {
        if (id == 0)
            return;

        var clientId = Clients.FirstOrDefault(x => x.Value.Character.PlayerId == id).Key;
        Clients.Remove(clientId, out var outputData);
        PlayerClientIDs.Remove(id);
        SavedPositions.Remove(id);
        AmongUsClient.Instance.RemovePlayer(clientId, DisconnectReasons.Custom);
        AmongUsClient.Instance.allClients.Remove(outputData);
    }

    public static void RemoveAllPlayers()
    {
        PlayerClientIDs.Keys.ForEach(RemovePlayer);
        SwitchTo(0);
    }

    public static void SwitchTo(byte playerId)
    {
        if (!TownOfUsReworked.MCIActive)
            return;

        if (ActiveTask)
            ActiveTask.Close();

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

        SavedPositions[CustomPlayer.Local.PlayerId] = CustomPlayer.LocalCustom.Position;

        PlayerLayer.LocalLayers.ForEach(x => x.ExitingLayer());

        CustomPlayer.Local.RpcCustomSnapTo(CustomPlayer.LocalCustom.Position);
        CustomPlayer.Local.moveable = false;

        var light = CustomPlayer.Local.lightSource;
        var savedId = CustomPlayer.Local.PlayerId;

        //Setup new player
        var newPlayer = PlayerById(playerId);

        if (newPlayer == null)
            return;

        PlayerControl.LocalPlayer = newPlayer;
        newPlayer.lightSource = light;
        newPlayer.moveable = true;

        AmongUsClient.Instance.ClientId = newPlayer.OwnerId;
        AmongUsClient.Instance.HostId = newPlayer.OwnerId;

        HUD.SetHudActive(true);
        HUD.ShadowQuad.gameObject.SetActive(!newPlayer.Data.IsDead);

        light.transform.SetParent(CustomPlayer.LocalCustom.Transform, false);
        light.transform.localPosition = newPlayer.Collider.offset;

        Camera.main.GetComponent<FollowerCamera>().SetTarget(newPlayer);
        newPlayer.MyPhysics.ResetMoveState(true);
        KillAnimation.SetMovement(newPlayer, true);
        newPlayer.MyPhysics.inputHandler.enabled = true;

        if (Meeting)
        {
            PlayerLayer.LocalLayers.ForEach(x => x.OnMeetingStart(Meeting));

            if (newPlayer.Data.IsDead)
                Meeting.SetForegroundForDead();
            else
                Meeting.SetForegroundForAlive();
        }
        else
        {
            newPlayer.EnableButtons();
            newPlayer.EnableArrows();
        }

        PlayerLayer.LocalLayers.ForEach(x => x.EnteringLayer());

        Chat.SetVisible(newPlayer.CanChat());

        if (SavedPositions.TryGetValue(playerId, out var pos))
            newPlayer.RpcCustomSnapTo(pos);

        if (SavedPositions.TryGetValue(savedId, out var pos2))
            PlayerById(savedId).RpcCustomSnapTo(pos2);
    }

    public static void SetForegroundForAlive(this MeetingHud __instance)
    {
        __instance.amDead = false;
        __instance.SkipVoteButton.gameObject.SetActive(true);
        __instance.SkipVoteButton.AmDead = false;

        if (CacheGlassSprite.Cache)
            __instance.Glass.sprite = CacheGlassSprite.Cache;
    }
}