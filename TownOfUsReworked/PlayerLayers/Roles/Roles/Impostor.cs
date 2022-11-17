using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Impostor : Role
    {
        public bool ImpWin;

        public Impostor(PlayerControl player) : base(player)
        {
            Name = "Impostor";
            Faction = Faction.Intruders;
            RoleType = RoleEnum.Impostor;
            SubFaction = SubFaction.None;
            ImpostorText = () => "Imagine Being Boring Impostor";
            TaskText = () => "Imagine Being Boring Impostor";
            Color = Colors.Intruder;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderUtil;
            AlignmentName = () => "Intruder (Utility)";
            IntroText = "Kill those who oppose you";
            Results = InspResults.CrewImpAnMurd;
            IntroSound = TownOfUsReworked.ImpostorIntro;
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var synTeam = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Intruders))
                    synTeam.Add(player);
            }
            __instance.teamToShow = synTeam;
        }
    }
}