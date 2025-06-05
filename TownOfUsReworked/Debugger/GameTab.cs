namespace TownOfUsReworked.Debugger;

public sealed class GameTab : BaseTab
{
    public override string Name => "Game";

    public override void OnGUI()
    {
        if (!IsInGame())
        {
            GUILayout.Label("Start a game");
            return;
        }

        TownOfUsReworked.SameVote.Value = GUILayout.Toggle(TownOfUsReworked.SameVote.Value, "All Bots Vote");

        if (GUILayout.Button("Next Player"))
        {
            Debugging.Instance.ControllingFigure = CycleByte(GameData.Instance.PlayerCount - 1, 0, Debugging.Instance.ControllingFigure, true);
            MciUtils.SwitchTo(Debugging.Instance.ControllingFigure);
        }
        else if (GUILayout.Button("Previous Player"))
        {
            Debugging.Instance.ControllingFigure = CycleByte(GameData.Instance.PlayerCount - 1, 0, Debugging.Instance.ControllingFigure, false);
            MciUtils.SwitchTo(Debugging.Instance.ControllingFigure);
        }

        if (GUILayout.Button("End Game (Lose)"))
            CheckEndGame.PerformStalemate();

        if (GUILayout.Button("End Game (Win)"))
        {
            WinState = WinLose.EveryoneWins;
            AllPlayers().Do(x => LayerHandler.Handlers[x.PlayerId].Winner = true);
        }

        if (Syndicate.DriveHolder)
        {
            if (GUILayout.Button("Take Away Chaos Drive"))
            {
                Syndicate.DriveHolder = null;
                Syndicate.SyndicateHasChaosDrive = false;
            }
        }
        else if (GUILayout.Button("Assign Chaos Drive"))
            AssignChaosDrive();

        if (GUILayout.Button("Fix All Sabotages"))
            FixUtils.Fix();

        if (GUILayout.Button("Reset Self"))
        {
            LocalPlayer.MyPhysics.ResetAnimState();
            LocalPlayer.MyPhysics.ResetMoveState();
        }

        if (GUILayout.Button("Kill Animation"))
            HUD().KillOverlay.ShowKillAnimation(AllPlayers().Random().Data, LocalPlayer.Data);

        if (GUILayout.Button("Complete Tasks"))
            LocalPlayer.myTasks.ForEach(x => LocalPlayer.CompleteTask(x.Id));

        if (GUILayout.Button("Complete Everyone's Tasks"))
            AllPlayers().Do(x => x.myTasks.ForEach(y => x.CompleteTask(y.Id)));

        if (GUILayout.Button("Redo Intro Sequence"))
        {
            var hud = HUD();
            hud.StartCoroutine(hud.CoFadeFullScreen(UColor.clear, UColor.black));
            hud.StartCoroutine(hud.CoShowIntro());
        }

        if (Meeting())
        {
            if (GUILayout.Button("End Meeting"))
                Meeting().RpcClose();
        }
        else if (GUILayout.Button("Start Meeting"))
            CallMeeting(LocalPlayer);

        if (GUILayout.Button("Kill Self"))
            LocalPlayer.Suicide();

        if (GUILayout.Button("Kill All"))
            AllPlayers().Do(x => x.Suicide());

        if (GUILayout.Button("Kill All But Me"))
            AllPlayers().Where(x => !x.AmOwner).Do(x => x.Suicide());

        if (GUILayout.Button("Revive Self"))
            LocalPlayer.Revive();

        if (GUILayout.Button("Revive All"))
            AllPlayers().Do(x => x.Revive());

        if (GUILayout.Button("Revive All But Me"))
            AllPlayers().Where(x => !x.AmOwner).Do(x => x.Revive());

        if (GUILayout.Button("Log Dump"))
        {
            Critical(LocalPlayer.name);
            PlayerLayer.LocalLayers().Do(x => Critical(x));
            Critical("Is Dead - " + LocalPlayer.HasDied());
            Critical("Location - " + LocalPlayer.transform.position);
        }

        if (!GUILayout.Button("Flash"))
            return;

        var r = (byte)URandom.RandomRangeInt(0, 256);
        var g = (byte)URandom.RandomRangeInt(0, 256);
        var b = (byte)URandom.RandomRangeInt(0, 256);
        Flash(new Color32(r, g, b, 255));
    }
}