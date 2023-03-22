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
        public bool ConvertedDead => PlayerControl.AllPlayerControls.ToArray().Count(x => x != null && !x.Data.IsDead && !x.Data.Disconnected && !x.Is(SubFaction.None)) == 0;
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
            var timeSpan = utcNow - LastRevealed;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.RevealCooldown) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void TurnSeer()
        {
            var mystic = Role.GetRole<Mystic>(Player);
            var role = new Seer(Player);
            role.RoleUpdate(mystic);
            Player.RegenTask();

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Coroutines.Start(Utils.FlashCoroutine(Colors.Seer));
        }
    }
}