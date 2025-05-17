namespace TownOfUsReworked.Utils;

// Graciously taken from MyDragonBreath and modified to be more optimised and functional

public static class MciUtils
{
    public static readonly Dictionary<int, ClientData> Clients = [];
    public static readonly Dictionary<byte, int> PlayerClientIDs = [];

    public static int GetAvailableId(bool mci)
    {
        for (var i = 1; i < 251; i++)
        {
            if (IsAvailable(i, mci))
                return i;
        }

        return -1;
    }

    private static bool IsAvailable(int i, bool mci) => mci
        ? !AmongUsClient.Instance.allClients.Any(x => x.Id == i) && !Clients.ContainsKey(i) && CustomPlayer.Local.OwnerId != i
        : !GameData.Instance.AllPlayers.Any(p => p.PlayerId == i) && !GameData.Instance.PlayerQueue.Any(p => p.PlayerId == i);

    public static void CleanUpLoad()
    {
        if (GameData.Instance.AllPlayers.Count != 1)
            return;

        Clients.Clear();
        PlayerClientIDs.Clear();
    }

    public static void CreatePlayerInstances(int count)
    {
        for (var i = 0; i < count; i++)
            CreatePlayerInstance();
    }

    public static void CreatePlayerInstance() => Coroutines.Start(CoCreatePlayerInstance());

    private static IEnumerator CoCreatePlayerInstance()
    {
        var sampleId = GetAvailableId(true);
        var sampleC = new ClientData(sampleId, $"Bot-{sampleId}", new()
        {
#if ANDROID
            Platform = Platforms.Android,
#else
            Platform = Platforms.StandaloneWin10,
#endif
            PlatformName = "Bot"
        }, 1, "", "robotmodeactivate");

        AmongUsClient.Instance.GetOrCreateClient(sampleC);
        yield return AmongUsClient.Instance.CreatePlayer(sampleC);

        var colorId = CustomColorManager.AllColors.Keys.Random();
        sampleC.Character.SetName($"Bot {sampleC.Character.PlayerId}");
        sampleC.Character.SetColor(colorId);
        sampleC.Character.SetSkin(HatManager.Instance.allSkins.Random().ProdId, colorId);
        sampleC.Character.SetNamePlate(HatManager.Instance.allNamePlates.Random().ProdId);
        sampleC.Character.SetPet(HatManager.Instance.allPets.Random().ProdId);
        sampleC.Character.SetVisor("visor_EmptyVisor", colorId);
        sampleC.Character.SetHat("hat_NoHat", colorId);

        sampleC.Character.isDummy = true;

        Clients.Add(sampleId, sampleC);
        PlayerClientIDs.Add(sampleC.Character.PlayerId, sampleId);

        sampleC.Character.MyPhysics.ResetAnimState();
        sampleC.Character.MyPhysics.ResetMoveState();

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
        SwitchTo(0);
        PlayerClientIDs.Keys.Do(RemovePlayer);
    }

    public static void SwitchTo(byte playerId)
    {
        if (!TownOfUsReworked.MciActive)
            return;

        Debugging.Instance.ControllingFigure = playerId;

        // Setup new player
        var newPlayer = PlayerById(playerId);

        if (!newPlayer)
            return;

        if (IsInGame())
        {
            if (ActiveTask())
                ActiveTask().Close();

            if (Meeting())
            {
                PlayerLayer.LocalLayers().Do(x => x.OnMeetingEnd(Meeting()));
                ButtonUtils.DisableAllButtons();
            }
            else
                CustomPlayer.Local.DisableButtons();

            PlayerLayer.LocalLayers().Do(x => x.ExitingLayer());
        }

        CustomPlayer.Local.CustomSnapTo(CustomPlayer.Local.transform.position);
        CustomPlayer.Local.MyPhysics.ResetMoveState();
        CustomPlayer.Local.MyPhysics.ResetAnimState();
        CustomPlayer.Local.moveable = false;

        var light = CustomPlayer.Local.lightSource;
        var savedPlayer = CustomPlayer.Local;

        var pos = CustomPlayer.Local.transform.position;
        var pos2 = newPlayer.transform.position;

        PlayerControl.LocalPlayer = newPlayer;
        newPlayer.lightSource = light;
        newPlayer.moveable = true;

        AmongUsClient.Instance.ClientId = newPlayer.OwnerId;
        AmongUsClient.Instance.HostId = newPlayer.OwnerId;

        var hud = HUD();

        hud.SetHudActive(true);
        hud.ShadowQuad.gameObject.SetActive(!newPlayer.Data.IsDead);

        light.transform.SetParent(CustomPlayer.Local.transform, false);
        light.transform.localPosition = newPlayer.Collider.offset;

        hud.PlayerCam.SetTarget(newPlayer);
        newPlayer.MyPhysics.ResetMoveState();
        newPlayer.MyPhysics.ResetAnimState();
        KillAnimation.SetMovement(newPlayer, true);
        newPlayer.MyPhysics.inputHandler.enabled = true;

        if (IsInGame())
        {
            if (Meeting())
            {
                PlayerLayer.LocalLayers().Do(x => x.OnMeetingStart(Meeting()));

                if (newPlayer.Data.IsDead)
                    Meeting().SetForegroundForDead();
                else
                    Meeting().SetForegroundForAlive();
            }
            else
                newPlayer.EnableButtons();

            PlayerLayer.LocalLayers().Do(x => x.EnteringLayer());
        }

        Chat()?.SetVisible(newPlayer.CanChat());

        // Weird quirk of the game that got introduced when Fungle got added, there's a weird snap to being invoked somewhere causing previous bodies to clip out of bounds
        newPlayer.CustomSnapTo(pos2);
        savedPlayer.CustomSnapTo(pos);
    }

    private static void SetForegroundForAlive(this MeetingHud meeting)
    {
        meeting.amDead = false;
        meeting.SkipVoteButton.gameObject.SetActive(true);
        meeting.SkipVoteButton.AmDead = false;

        if (MeetingPatches.Cache)
            meeting.Glass.sprite = MeetingPatches.Cache;
    }
}