namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Rivals : Objectifier
    {
        public PlayerControl OtherRival;
        public bool RivalDead => OtherRival?.Data?.IsDead == true || OtherRival?.Data?.Disconnected == true;
        public bool IsDeadRival => IsDead || Disconnected;
        public bool BothRivalsDead => IsDeadRival && RivalDead;
        public bool IsWinningRival =>  RivalDead && !IsDeadRival;

        public Rivals(PlayerControl player) : base(player)
        {
            Name = "Rival";
            Symbol = "α";
            TaskText = "- Get your rival killed and then live to the final 2.";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Rivals : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Rivals;
            Type = LayerEnum.Rivals;
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (!__instance.Chat.isActiveAndEnabled)
                __instance.Chat.SetVisible(CustomGameOptions.RivalsChat);
        }
    }
}