namespace TownOfUsReworked.Debugger;

public sealed class TestingTab : BaseTab
{
    public override string Name => "Testing";
    private static bool MoveToNext;

    public override void OnGUI()
    {
        TownOfUsReworked.Persistence.Value = GUILayout.Toggle(TownOfUsReworked.Persistence.Value, "Bot Persistence");
        TownOfUsReworked.DisableTimeout.Value = GUILayout.Toggle(TownOfUsReworked.DisableTimeout.Value, "Disable Lobby Timeout");
        TownOfUsReworked.BlockBaseGameLogger.Value = GUILayout.Toggle(TownOfUsReworked.BlockBaseGameLogger.Value, "Block AU Logger");
        TownOfUsReworked.RedirectLogger.Value = GUILayout.Toggle(TownOfUsReworked.RedirectLogger.Value, "Redirect Logger");
        TownOfUsReworked.LogFromUnity.Value = GUILayout.Toggle(TownOfUsReworked.LogFromUnity.Value, "Log From Unity");
        var hiddenBlock = GUILayout.Toggle(HiddenBlock, "Roleblocked");

        if (hiddenBlock != HiddenBlock)
            HiddenBlock = hiddenBlock;

        BlockExposed = GUILayout.Toggle(BlockExposed, "Roleblock Exposed");

        if (Lobby() && IsLocalGame())
        {
            if (GUILayout.Button("Spawn Bot") && GameData.Instance.PlayerCount < GameOptions.LobbySize.Value)
            {
                MciUtils.CleanUpLoad();
                MciUtils.CreatePlayerInstance();
            }

            if (GUILayout.Button("Remove Last Bot"))
            {
                MciUtils.RemovePlayer((byte)MciUtils.Clients.Count);
                Debugging.Instance.ControllingFigure = 0;
            }

            if (GUILayout.Button("Remove All Bots"))
            {
                MciUtils.RemoveAllPlayers();
                Debugging.Instance.ControllingFigure = 0;
            }
        }

        if (GUILayout.Button("Test Achievement"))
            CustomAchievementManager.UnlockAchievement("Test");

        if (!LocalPlayer)
            return;

        LocalPlayer.Collider.enabled = GUILayout.Toggle(LocalPlayer.Collider.enabled, "Player Collider");

        if (GUILayout.Button("Randomise Outfit"))
            LocalPlayer.OverrideOutfit(GenerateRandomOutfit(), CustomPlayerOutfitType.Custom, 5f, ShouldMove);

        if (GUILayout.Button("Queue Random Outfit"))
            LocalPlayer.QueueOutfit(GenerateRandomOutfit(), CustomPlayerOutfitType.Custom, 5f, ShouldMove);

        if (GUILayout.Button("Next Outfit"))
            MoveToNext = true;
    }

    private static bool ShouldMove()
    {
        if (!MoveToNext)
            return false;

        MoveToNext = false;
        return true;
    }

    private static CustomOutfit GenerateRandomOutfit() => new()
    {
        ColorId = CustomColorManager.AllColors.Keys.AddItem(-2).Random(),
        Color = new((byte)URandom.RandomRangeInt(0, 256), (byte)URandom.RandomRangeInt(0, 256), (byte)URandom.RandomRangeInt(0, 256), 255),
        HatId = HatManager.Instance.allHats.Random().ProductId,
        SkinId = HatManager.Instance.allSkins.Random().ProductId,
        VisorId = HatManager.Instance.allVisors.Random().ProductId,
        NamePlateId = HatManager.Instance.allNamePlates.Random().ProductId,
        PetId = HatManager.Instance.allPets.Random().ProductId,
        PlayerName = GetRandomisedName(),
        Size = URandom.RandomRange(0.3f, 3f),
        Speed = URandom.RandomRange(0.25f, 10f),
        Alpha = URandom.RandomRange(0f, 1f)
    };
}