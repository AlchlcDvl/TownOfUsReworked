namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Rivals : Objectifier
    {
        public PlayerControl OtherRival;
        public bool RivalDead => OtherRival == null || OtherRival.Data.IsDead || OtherRival.Data.Disconnected;
        public bool IsDeadRival => Player == null || IsDead || Disconnected;
        public bool BothRivalsDead => IsDeadRival && RivalDead;
        public bool IsWinningRival =>  RivalDead && !IsDeadRival;

        public Rivals(PlayerControl player) : base(player)
        {
            Name = "Rival";
            Symbol = "α";
            TaskText = () => $"- Get {OtherRival.name} killed and then live to the final 2";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Rivals : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Rivals;
            Type = LayerEnum.Rivals;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (!__instance.Chat.isActiveAndEnabled)
                __instance.Chat.SetVisible(CustomGameOptions.RivalsChat);
        }
    }
}