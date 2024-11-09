namespace TownOfUsReworked.MultiClientInstancing;

public static class MCIUtils
{
    public static readonly Dictionary<int, ClientData> Clients = [];
    public static readonly Dictionary<byte, int> PlayerClientIDs = [];

    private static int AvailableId()
    {
        for (var i = 1; i < 128; i++)
        {
            if (!AmongUsClient.Instance.allClients.Any(x => x.Id == i) && !Clients.ContainsKey(i) && CustomPlayer.Local.OwnerId != i)
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
        }
    }

    public static void CreatePlayerInstances(int count)
    {
        for (var i = 0; i < count; i++)
            CreatePlayerInstance();
    }

    public static void CreatePlayerInstance() => Coroutines.Start(CoCreatePlayerInstance());

    public static IEnumerator CoCreatePlayerInstance()
    {
        var sampleId = AvailableId();
        var sampleC = new ClientData(sampleId, $"Bot-{sampleId}", new()
        {
            Platform = Platforms.StandaloneWin10,
            PlatformName = "Bot"
        }, 1, "", "robotmodeactivate");

        AmongUsClient.Instance.GetOrCreateClient(sampleC);
        yield return AmongUsClient.Instance.CreatePlayer(sampleC);

        var colorId = CustomColorManager.AllColors.Random().ColorID;
        sampleC.Character.SetName($"Bot {sampleC.Character.PlayerId}");
        sampleC.Character.SetColor(colorId);
        sampleC.Character.SetSkin(HatManager.Instance.allSkins.Random().ProdId, colorId);
        sampleC.Character.SetNamePlate(HatManager.Instance.allNamePlates.Random().ProdId);
        sampleC.Character.SetPet(HatManager.Instance.allPets.Random().ProdId);
        sampleC.Character.SetVisor("visor_EmptyVisor", colorId);
        sampleC.Character.SetHat("hat_NoHat", colorId);

        Clients.Add(sampleId, sampleC);
        PlayerClientIDs.Add(sampleC.Character.PlayerId, sampleId);

        sampleC.Character.MyPhysics.ResetAnimState();
        sampleC.Character.MyPhysics.ResetMoveState();

        if (SubLoaded)
            ImpartSub(sampleC.Character);

        yield return sampleC.Character.MyPhysics.CoSpawnPlayer(Lobby());
    }

    public static void RemovePlayer(byte id)
    {
        if (id == 0)
            return;

        var clientId = Clients.FirstOrDefault(x => x.Value.Character.PlayerId == id).Key;
        Clients.Remove(clientId, out var outputData);
        PlayerClientIDs.Remove(id);
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

        // Setup new player
        var newPlayer = PlayerById(playerId);

        if (!newPlayer)
            return;

        if (ActiveTask())
            ActiveTask().Close();

        if (Meeting())
        {
            PlayerLayer.LocalLayers().ForEach(x => x.OnMeetingEnd(Meeting()));
            ButtonUtils.DisableAllButtons();
        }
        else
        {
            CustomPlayer.Local.DisableButtons();
            CustomPlayer.Local.DisableArrows();
        }

        PlayerLayer.LocalLayers().ForEach(x => x.ExitingLayer());

        CustomPlayer.Local.RpcCustomSnapTo(CustomPlayer.LocalCustom.Position);
        CustomPlayer.Local.moveable = false;

        var light = CustomPlayer.Local.lightSource;
        var savedPlayer = CustomPlayer.Local;

        var pos = CustomPlayer.LocalCustom.Position;
        var pos2 = newPlayer.transform.position;

        PlayerControl.LocalPlayer = newPlayer;
        newPlayer.lightSource = light;
        newPlayer.moveable = true;

        AmongUsClient.Instance.ClientId = newPlayer.OwnerId;
        AmongUsClient.Instance.HostId = newPlayer.OwnerId;

        HUD().SetHudActive(true);
        HUD().ShadowQuad.gameObject.SetActive(!newPlayer.Data.IsDead);

        light.transform.SetParent(CustomPlayer.LocalCustom.Transform, false);
        light.transform.localPosition = newPlayer.Collider.offset;

        HUD().PlayerCam.SetTarget(newPlayer);
        newPlayer.MyPhysics.ResetMoveState(true);
        KillAnimation.SetMovement(newPlayer, true);
        newPlayer.MyPhysics.inputHandler.enabled = true;

        if (Meeting())
        {
            PlayerLayer.LocalLayers().ForEach(x => x.OnMeetingStart(Meeting()));

            if (newPlayer.Data.IsDead)
                Meeting().SetForegroundForDead();
            else
                Meeting().SetForegroundForAlive();
        }
        else
        {
            newPlayer.EnableButtons();
            newPlayer.EnableArrows();
        }

        PlayerLayer.LocalLayers().ForEach(x => x.EnteringLayer());

        Chat().SetVisible(newPlayer.CanChat());
        newPlayer.RpcCustomSnapTo(pos2);
        savedPlayer.RpcCustomSnapTo(pos);
        Role.LocalRole?.UpdateButtons();
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