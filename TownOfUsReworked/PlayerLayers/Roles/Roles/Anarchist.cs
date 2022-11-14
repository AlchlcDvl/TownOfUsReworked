using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using System;
using TownOfUsReworked.Extensions;
using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Anarchist : Role
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastKill { get; set; }

        public Anarchist(PlayerControl player) : base(player)
        {
            Name = "Anarchist";
            Faction = Faction.Syndicate;
            RoleType = RoleEnum.Anarchist;
            ImpostorText = () => "Imagine Being Boring Anarchist";
            TaskText = () => "Imagine Being Boring Anarchist";
            Color = Colors.Syndicate;
            FactionName = "Syndicate";
            FactionColor = Colors.Syndicate;
            RoleAlignment = RoleAlignment.SyndicateUtil;
            AlignmentName = () => "Syndicate (Utility)";
            IntroText = "Cause choas and kill your opposition";
            Results = InspResults.CrewImpAnMurd;
            SubFaction = SubFaction.None;
            AddToRoleHistory(RoleType);
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKill;
            var num = CustomGameOptions.PossessCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var synTeam = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Syndicate))
                    synTeam.Add(player);
            }
            __instance.teamToShow = synTeam;
        }
    }
}