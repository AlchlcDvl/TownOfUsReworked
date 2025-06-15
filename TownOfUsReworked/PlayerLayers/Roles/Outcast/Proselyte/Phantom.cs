namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Phantom)]
public sealed class Phantom : Proselyte, IGhosty
{
    [NumberOption(1, 10, 1)]
    public static Number PhantomTasksRemaining = 5;

    [ToggleOption]
    private static bool PhantomPlayersAlerted = false;

    public bool Caught { get; set; }
    public Vector3 LastPosition { get; set; }

    protected override UColor MainColor => CustomColorManager.Phantom;
    public override Layer Type => Layer.Phantom;
    public override string StartText => "Peek-A-Boo!";
    public override string Description => "- You end the game upon finishing your objective";
    public override bool HasWon => TasksDone;
    protected override WinLose EndState => WinLose.PhantomWins;

    public override void Init()
    {
        base.Init();
        Objectives = () => "- Finish your tasks without getting clicked";
        RemoveTasks(Player);
        Player.gameObject.layer = LayerMask.NameToLayer("Players");
    }

    public override void BeforeMeeting()
    {
        if (!UninteractablePlayers.ContainsKey(PlayerId))
            LastPosition = Player.transform.position;
    }

    public override void UponTaskComplete(uint taskId)
    {
        if (TasksLeft == PhantomTasksRemaining && PhantomPlayersAlerted && !Caught)
            Flash(Color);
    }

    protected override void CheckWin(HashSet<byte> winnerIds)
    {
        if (!HasWon)
            return;

        if (OutcastEvilSettings.OutcastEvilsEndGame && !WinState.IsAny(EndState, WinLose.OutcastsWin))
            WinState = WinState is > WinLose.NobodyWins and < WinLose.OutcastsWin ? WinLose.OutcastsWin : EndState;

        winnerIds.Add(PlayerId);
    }

    public bool CanBeClicked(PlayerControl _) => TasksLeft <= PhantomTasksRemaining;
}