namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public sealed class Phantom : Neutral, IGhosty
{
    [NumberOption(1, 10, 1)]
    public static Number PhantomTasksRemaining = 5;

    [ToggleOption]
    private static bool PhantomPlayersAlerted = false;

    public bool Caught { get; set; }
    public bool Faded { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Phantom : FactionColor;
    public override LayerEnum Type => LayerEnum.Phantom;
    public override Func<string> StartText => () => "Peek-A-Boo!";
    public override Func<string> Description => () => "- You end the game upon finishing your objective";

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

        if (!AmongUsClient.Instance.AmHost || !TasksDone || !Faithful)
            return;

        WinState = WinLose.PhantomWins;
        CallRpc(CustomRPC.WinLose, WinLose.PhantomWins);
    }

    public override void UpdatePlayer()
    {
        base.UpdatePlayer();
        (this as IGhosty).UpdateGhost();
    }
}