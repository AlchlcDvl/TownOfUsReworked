namespace TownOfUsReworked.Debugger;

public sealed class TestingTab : BaseTab
{
    public override string Name => "Testing";

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

        BlockExposed = GUILayout.Toggle(BlockExposed, "Roleblocked Exposed");

        if (CustomPlayer.Local)
            CustomPlayer.Local.Collider.enabled = GUILayout.Toggle(CustomPlayer.Local.Collider.enabled, "Player Collider");

        if (Lobby() && IsLocalGame())
        {
            if (GUILayout.Button("Spawn Bot") && GameData.Instance.PlayerCount < GameSettings.LobbySize)
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

        if (!GUILayout.Button("Randomise Outfit"))
            return;

        var r = (byte)URandom.RandomRangeInt(0, 256);
        var g = (byte)URandom.RandomRangeInt(0, 256);
        var b = (byte)URandom.RandomRangeInt(0, 256);

        var outfit = new CustomOutfit()
        {
            ColorId = CustomColorManager.AllColors.Keys.AddItem(-2).Random(), // TODO: Transition works, name change remaining
            Color = new(r, g, b, 255), // TODO: Same as above
            HatId = HatManager.Instance.allHats.Random().ProductId, // TODO: Works, but alpha is still a problem
            // SkinId = HatManager.Instance.allSkins.Random().ProductId, // Works
            VisorId = HatManager.Instance.allVisors.Random().ProductId, // TODO: Same as hats
            // NamePlateId = HatManager.Instance.allNamePlates.Random().ProductId, // No need to check lmao
            PetId = HatManager.Instance.allPets.Random().ProductId, // Works
            PlayerName = "", // TODO: Reimplement name handling and its transition (make it letter by letter)
            // Size = URandom.RandomRange(Dwarf.DwarfScale.Value, Giant.GiantScale), // Works
            Speed = URandom.RandomRange(GameSettings.PlayerSpeed.Value, GameSettings.GhostSpeed), // TODO: Implement speed and its transition
            Alpha = URandom.RandomRange(0f, 1f) // Works
        };
        CustomPlayer.Local.GetComponent<AppearanceHandler>().OverrideOutfit(outfit, CustomPlayerOutfitType.Custom, 3f);
    }
}