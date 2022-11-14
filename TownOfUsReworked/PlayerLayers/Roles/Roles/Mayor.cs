using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Mayor : Role
    {
        public List<byte> ExtraVotes = new List<byte>();
        public bool MayorWin;
        public int VoteBank { get; set; }
        public bool SelfVote { get; set; }
        public bool VotedOnce { get; set; }
        public PlayerVoteArea Abstain { get; set; }
        public bool CanVote => VoteBank > 0 && !SelfVote;

        public Mayor(PlayerControl player) : base(player)
        {
            Name = "Mayor";
            ImpostorText = () => "Save Your Votes To Mayor Dump Someone";
            TaskText = () => "Save your votes to vote multiple times";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Mayor : Colors.Crew;
            SubFaction = SubFaction.None;
            RoleType = RoleEnum.Mayor;
            VoteBank = CustomGameOptions.MayorVoteBank;
            Faction = Faction.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewSov;
            AlignmentName = () => "Crew (Sovereign)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            Results = InspResults.GFMayorRebelPest;
            AddToRoleHistory(RoleType);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }
    }
}