namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Phantom : Neutral, IGhosty
{
    [NumberOption(MultiMenu.LayerSubOptions, 1, 10, 1)]
    public static Number PhantomTasksRemaining { get; set; } = new(5);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool PhantomPlayersAlerted { get; set; } = false;

    public bool Caught { get; set; }
    public bool Faded { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Phantom : FactionColor;
    public override string Name => "Phantom";
    public override LayerEnum Type => LayerEnum.Phantom;
    public override Func<string> StartText => () => "Peek-A-Boo!";
    public override Func<string> Description => () => "- You end the game upon finishing your objective";

    public override void Init()
    {
        base.Init();
        Objectives = () => "- Finish your tasks without getting clicked";
        Alignment = Alignment.NeutralPros;
    }

    public override void UponTaskComplete(uint taskId)
    {
        if (TasksLeft == PhantomTasksRemaining && PhantomPlayersAlerted && !Caught)
            Flash(Color);
    }

    public override void UpdatePlayer() => (this as IGhosty).UpdateGhost();

    public override void CheckWin()
    {
        if (TasksDone && Faithful)
        {
            WinState = WinLose.PhantomWins;
            CallRpc(CustomRPC.WinLose, WinLose.PhantomWins);
        }
    }
}