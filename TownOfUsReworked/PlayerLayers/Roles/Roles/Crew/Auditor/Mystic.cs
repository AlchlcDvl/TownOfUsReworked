using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using System;
using System.Linq;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Mystic : CrewRole
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastRevealed;
        public static bool ConvertedDead => !PlayerControl.AllPlayerControls.ToArray().Any(x => x?.Data.IsDead == false && !x.Data.Disconnected && !x.Is(SubFaction.None));
        public AbilityButton RevealButton;

        public Mystic(PlayerControl player) : base(player)
        {
            Name = "Mystic";
            RoleType = RoleEnum.Mystic;
            Color = CustomGameOptions.CustomCrewColors ? Colors.Mystic : Colors.Crew;
            RoleAlignment = RoleAlignment.CrewAudit;
            AlignmentName = CA;
            StartText = "You Know When Converts Happen";
            AbilitiesText = "- You can investigate players to see if they have been converted.\n- Whenever someone has been converted, you will be alerted to it.\n- When all converted" +
                " and converters die, you will become a <color=#71368AFF>Seer.</color>";
            InspectorResults = InspectorResults.TracksOthers;
        }

        public float RevealTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastRevealed;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.RevealCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void TurnSeer()
        {
            var role = new Seer(Player);
            role.RoleUpdate(this);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Coroutines.Start(Utils.FlashCoroutine(Colors.Seer));
        }
    }
}