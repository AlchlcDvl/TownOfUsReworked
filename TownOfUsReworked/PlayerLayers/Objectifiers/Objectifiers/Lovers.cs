namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Lovers : Objectifier
    {
        public PlayerControl OtherLover;
        public bool LoverDead => OtherLover == null || OtherLover.Data.IsDead || OtherLover.Data.Disconnected;
        public bool IsDeadLover => Player == null || IsDead || Disconnected;
        public bool LoversLose => LoverDead && IsDeadLover;
        public bool LoversAlive => !IsDeadLover && !LoverDead;

        public Lovers(PlayerControl player) : base(player)
        {
            Name = "Lover";
            Symbol = "♥";
            TaskText = () => $"- Live to the final 3 with {OtherLover.name}";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Lovers : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Lovers;
            Type = LayerEnum.Lovers;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (!__instance.Chat.isActiveAndEnabled)
                __instance.Chat.SetVisible(CustomGameOptions.LoversChat);
        }
    }
}