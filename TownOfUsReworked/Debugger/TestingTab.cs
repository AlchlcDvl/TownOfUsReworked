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
        BlockExposed = GUILayout.Toggle(BlockExposed, "Roleblocked");

        if (CustomPlayer.Local)
            CustomPlayer.Local.Collider.enabled = GUILayout.Toggle(CustomPlayer.Local.Collider.enabled, "Player Collider");

        if (Lobby())
        {
            if (IsLocalGame())
            {
                if (GUILayout.Button("Spawn Bot"))
                {
                    if (GameData.Instance.PlayerCount < GameSettings.LobbySize)
                    {
                        MciUtils.CleanUpLoad();
                        MciUtils.CreatePlayerInstance();
                    }
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
        }

        if (GUILayout.Button("Test Achievement"))
            CustomAchievementManager.UnlockAchievement("Test");
    }
}