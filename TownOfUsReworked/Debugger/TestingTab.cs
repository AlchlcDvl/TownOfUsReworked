namespace TownOfUsReworked.Debugger;

public class TestingTab : BaseTab
{
    public override string Name => "Testing";

    public override void OnGUI()
    {
        TownOfUsReworked.Persistence.Value = GUILayout.Toggle(TownOfUsReworked.Persistence.Value, "Bot Persistence");
        TownOfUsReworked.DisableTimeout.Value = GUILayout.Toggle(TownOfUsReworked.DisableTimeout.Value, "Disable Lobby Timeout");
        TownOfUsReworked.BlockBaseGameLogger.Value = GUILayout.Toggle(TownOfUsReworked.BlockBaseGameLogger.Value, "Block AU Logger");
        TownOfUsReworked.RedirectLogger.Value = GUILayout.Toggle(TownOfUsReworked.RedirectLogger.Value, "Redirect Logger");

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
                        MCIUtils.CleanUpLoad();
                        MCIUtils.CreatePlayerInstance();
                    }
                }

                if (GUILayout.Button("Remove Last Bot"))
                {
                    MCIUtils.RemovePlayer((byte)MCIUtils.Clients.Count);
                    DebuggerBehaviour.Instance.ControllingFigure = 0;
                }

                if (GUILayout.Button("Remove All Bots"))
                {
                    MCIUtils.RemoveAllPlayers();
                    DebuggerBehaviour.Instance.ControllingFigure = 0;
                }
            }
        }

        if (GUILayout.Button("Test Achievement"))
            CustomAchievementManager.UnlockAchievement("Test");
    }
}