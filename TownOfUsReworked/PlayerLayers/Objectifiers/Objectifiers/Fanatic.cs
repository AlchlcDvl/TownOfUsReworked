using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers
{
    public class Fanatic : Objectifier
    {
        public Role former;
        public bool Turned = false;
        public string Side;

        public Fanatic(PlayerControl player) : base(player)
        {
            Name = "Fanatic";
            SymbolName = "♠";
            TaskText = Turned
                ? "You now side with the <color=#" + Role.GetRole(Player).ColorString + $"{Side}</color>!"
                : "You are <color=#8BFDFDFF>Crew</color>, for now, that is....";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Fanatic : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Fanatic;
            Hidden = !CustomGameOptions.FanaticKnows;
        }

        public void TurnFanatic(PlayerControl fanatic, Faction faction)
        {
            var fanaticRole = Role.GetRole(fanatic);
            fanaticRole.Faction = faction;
            Turned = true;

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
                Coroutines.Start(Utils.FlashCoroutine(Colors.Mystic));
        }
    }
}