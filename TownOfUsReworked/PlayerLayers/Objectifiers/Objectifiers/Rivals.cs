namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Rivals : Objectifier
    {
        public PlayerControl OtherRival;
        public bool RivalDead => OtherRival == null || OtherRival.Data.IsDead || OtherRival.Data.Disconnected;
        public bool IsDeadRival => Player == null || IsDead || Disconnected;
        public bool BothRivalsDead => IsDeadRival && RivalDead;
        public bool IsWinningRival =>  RivalDead && !IsDeadRival;

        public override Color32 Color => ClientGameOptions.CustomObjColors ? Colors.Rivals : Colors.Objectifier;
        public override string Name => "Rivals";
        public override string Symbol => "α";
        public override LayerEnum Type => LayerEnum.Rivals;
        public override ObjectifierEnum ObjectifierType => ObjectifierEnum.Rivals;
        public override Func<string> TaskText => () => $"- Get {OtherRival.name} killed and then live to the final 2";

        public Rivals(PlayerControl player) : base(player) {}
    }
}