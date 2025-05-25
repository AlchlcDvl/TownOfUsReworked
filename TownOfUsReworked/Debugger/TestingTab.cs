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
            ColorId = CustomColorManager.AllColors.Keys.AddItem(-2).Random(),
            HatId = HatManager.Instance.allHats.Random().ProductId,
            SkinId = HatManager.Instance.allSkins.Random().ProductId,
            VisorId = HatManager.Instance.allVisors.Random().ProductId,
            NamePlateId = HatManager.Instance.allNamePlates.Random().ProductId,
            PetId = HatManager.Instance.allPets.Random().ProductId,
            PlayerName = "",
            Size = URandom.RandomRange(Dwarf.DwarfScale.Value, Giant.GiantScale),
            Speed = URandom.RandomRange(GameSettings.PlayerSpeed.Value, GameSettings.GhostSpeed),
            Alpha = URandom.RandomRange(0f, 1f),
            Color = new(r, g, b, 255)
        };
        CustomPlayer.Local.GetComponent<AppearanceHandler>().OverrideOutfit(outfit, CustomPlayerOutfitType.Custom, 10f);
    }
}