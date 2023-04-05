using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Lovers : Objectifier
    {
        public PlayerControl OtherLover;

        public Lovers(PlayerControl player) : base(player)
        {
            Name = "Lover";
            SymbolName = "♥";
            TaskText = "- Live to the final 3 with your lover";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Lovers : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Lovers;
        }

        public bool LoverDead() => OtherLover?.Data?.IsDead == true || OtherLover?.Data?.Disconnected == true;

        public bool IsDeadLover() => Player?.Data?.IsDead == true || Player?.Data?.Disconnected == true;

        public bool LoversLose() => LoverDead() && IsDeadLover();

        public bool LoversAlive() => OtherLover?.Data?.IsDead == false && Player?.Data?.IsDead == false && OtherLover?.Data?.Disconnected == false && Player?.Data?.Disconnected == false;
    }
}