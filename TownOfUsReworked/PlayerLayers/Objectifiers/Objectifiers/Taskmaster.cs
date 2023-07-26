namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Taskmaster : Objectifier
    {
        public bool Revealed => TasksLeft <= CustomGameOptions.TMTasksRemaining;
        public bool WinTasksDone;

        public override Color32 Color => ClientGameOptions.CustomObjColors ? Colors.Taskmaster : Colors.Objectifier;
        public override string Name => "Taskmaster";
        public override string Symbol => "µ";
        public override LayerEnum Type => LayerEnum.Taskmaster;
        public override ObjectifierEnum ObjectifierType => ObjectifierEnum.Taskmaster;
        public override Func<string> TaskText => () => "- Finish your tasks before the game ends";

        public Taskmaster(PlayerControl player) : base(player) {}
    }
}