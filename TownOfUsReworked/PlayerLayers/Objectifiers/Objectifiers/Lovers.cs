namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Lovers : Objectifier
    {
        public PlayerControl OtherLover;
        public bool LoverDead => OtherLover == null || OtherLover.Data.IsDead || OtherLover.Data.Disconnected;
        public bool IsDeadLover => Player == null || IsDead || Disconnected;
        public bool LoversLose => LoverDead && IsDeadLover;
        public bool LoversAlive => !IsDeadLover && !LoverDead;

        public override Color32 Color => ClientGameOptions.CustomObjColors ? Colors.Lovers : Colors.Objectifier;
        public override string Name => "Lovers";
        public override string Symbol => "♥";
        public override LayerEnum Type => LayerEnum.Lovers;
        public override ObjectifierEnum ObjectifierType => ObjectifierEnum.Lovers;
        public override Func<string> TaskText => () => $"- Live to the final 3 with {OtherLover.name}";

        public Lovers(PlayerControl player) : base(player) {}
    }
}