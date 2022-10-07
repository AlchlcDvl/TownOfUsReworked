using System.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Mayor : Role
    {
        public List<byte> ExtraVotes = new List<byte>();

        public Mayor(PlayerControl player) : base(player)
        {
            Name = "Mayor";
            ImpostorText = () => "Save Your Votes To Mayor Dump Someone";
            TaskText = () => "Save your votes to vote multiple times";
            if (CustomGameOptions.CustomCrewColors) Color = Patches.Colors.Mayor;
            else Color = Patches.Colors.Crew;
            RoleType = RoleEnum.Mayor;
            VoteBank = CustomGameOptions.MayorVoteBank;
            Faction = Faction.Crewmates;
            FactionName = "Crew";
            FactionColor = Patches.Colors.Crew;
            Alignment = Alignment.CrewSov;
            AlignmentName = "Crew (Sovereign)";
            AddToRoleHistory(RoleType);
        }

        public int VoteBank { get; set; }
        public bool SelfVote { get; set; }

        public bool VotedOnce { get; set; }

        public PlayerVoteArea Abstain { get; set; }

        public bool CanVote => VoteBank > 0 && !SelfVote;
    }
}