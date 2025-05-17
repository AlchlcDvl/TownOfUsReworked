namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Phantom)]
public sealed class Phantom : Neutral, IGhosty
{
    [NumberOption(1, 10, 1)]
    public static Number PhantomTasksRemaining = 5;

    [ToggleOption]
    private static bool PhantomPlayersAlerted = false;

    public bool Caught { get; set; }
    public bool Faded { get; set; }

    protected override UColor MainColor => CustomColorManager.Phantom;
    public override LayerEnum Type => LayerEnum.Phantom;
    public override Func<string> StartText { get; } = () => "Peek-A-Boo!";
    public override Func<string> Description => () => "- You end the game upon finishing your objective";
    public override bool HasWon => TasksDone;
    public override WinLose EndState => WinLose.PhantomWins;

    protected override void Init()
    {
        base.Init();
        Objectives = () => "- Finish your tasks without getting clicked";
        Alignment = Alignment.Proselyte;
        RemoveTasks(Player);
        Player.gameObject.layer = LayerMask.NameToLayer("Players");
    }

    public override void UponTaskComplete(uint taskId)
    {
        if (TasksLeft == PhantomTasksRemaining && PhantomPlayersAlerted && !Caught)
            Flash(Color);
    }

    public override void UpdatePlayer()
    {
        base.UpdatePlayer();
        (this as IGhosty).UpdateGhost();
    }

    protected override void CheckWin(HashSet<byte> winnerIds)
    {
        if (!HasWon)
            return;

        if (NeutralEvilSettings.NeutralEvilsEndGame && !WinState.IsAny(EndState, WinLose.NeutralsWin))
            WinState = WinState is > WinLose.NobodyWins and < WinLose.NeutralsWin ? WinLose.NeutralsWin : EndState;

        winnerIds.Add(PlayerId);
    }

    public bool CanBeClicked(PlayerControl _) => TasksLeft <= PhantomTasksRemaining;
}