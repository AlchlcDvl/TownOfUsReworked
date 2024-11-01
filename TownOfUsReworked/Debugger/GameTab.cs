namespace TownOfUsReworked.Debugger;

public class GameTab : BaseTab
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
        Role.SyndicateHasChaosDrive = GUILayout.Toggle(Role.SyndicateHasChaosDrive, "Chaos Drive");

        if (Role.SyndicateHasChaosDrive)
            RoleGen.AssignChaosDrive();
        else
            Role.DriveHolder = null;

        if (GUILayout.Button("Next Player"))
        {
            DebuggerBehaviour.Instance.ControllingFigure = CycleByte(AllPlayers().Count - 1, 0,DebuggerBehaviour.Instance. ControllingFigure, true);
            MCIUtils.SwitchTo(DebuggerBehaviour.Instance.ControllingFigure);
        }
        else if (GUILayout.Button("Previous Player"))
        {
            DebuggerBehaviour.Instance.ControllingFigure = CycleByte(AllPlayers().Count - 1, 0, DebuggerBehaviour.Instance.ControllingFigure, false);
            MCIUtils.SwitchTo(DebuggerBehaviour.Instance.ControllingFigure);
        }

        if (GUILayout.Button("End Game"))
            CheckEndGame.PerformStalemate();

        if (GUILayout.Button("Fix All Sabotages"))
        {
            FixExtentions.Fix();
            DefaultOutfitAll();
        }

        if (GUILayout.Button("Complete Tasks"))
            CustomPlayer.Local.myTasks.ForEach(x => CustomPlayer.Local.RpcCompleteTask(x.Id));

        if (GUILayout.Button("Complete Everyone's Tasks"))
            AllPlayers().ForEach(x => x.myTasks.ForEach(y => x.RpcCompleteTask(y.Id)));

        if (GUILayout.Button("Redo Intro Sequence"))
        {
            HUD().StartCoroutine(HUD().CoFadeFullScreen(UColor.clear, UColor.black));
            HUD().StartCoroutine(HUD().CoShowIntro());
        }

        if (Meeting())
        {
            if (GUILayout.Button("End Meeting"))
                Meeting().RpcClose();
        }
        else
        {
            if (GUILayout.Button("Start Meeting"))
                CallMeeting(CustomPlayer.Local);
        }

        if (GUILayout.Button("Kill Self"))
            RpcMurderPlayer(CustomPlayer.Local);

        if (GUILayout.Button("Kill All"))
            AllPlayers().ForEach(x => RpcMurderPlayer(x));

        if (GUILayout.Button("Revive Self"))
            CustomPlayer.Local.Revive();

        if (GUILayout.Button("Revive All"))
            AllPlayers().ForEach(x => x.Revive());

        if (GUILayout.Button("Log Dump"))
        {
            Message(CustomPlayer.Local.Data.PlayerName);
            PlayerLayer.LocalLayers().ForEach(Message);
            Message("Is Dead - " + CustomPlayer.Local.HasDied());
            Message("Location - " + CustomPlayer.LocalCustom.Position);
        }

        if (GUILayout.Button("Flash"))
        {
            var r = (byte)URandom.RandomRangeInt(0, 256);
            var g = (byte)URandom.RandomRangeInt(0, 256);
            var b = (byte)URandom.RandomRangeInt(0, 256);
            Flash(new Color32(r, g, b, 255));
        }
    }
}