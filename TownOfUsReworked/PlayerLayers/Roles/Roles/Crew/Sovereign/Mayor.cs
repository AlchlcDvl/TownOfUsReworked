using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Mayor : CrewRole
    {
        public List<byte> ExtraVotes;
        public int VoteBank;
        public bool SelfVote;
        public bool VotedOnce;
        public PlayerVoteArea Abstain;
        public bool CanVote => VoteBank > 0 && !SelfVote;

        public Mayor(PlayerControl player) : base(player)
        {
            Name = "Mayor";
            StartText = "Save Your Votes To Vote Dump Someone";
            AbilitiesText = "- You can save your votes into your vote bank, so you can vote multiple times later.\n- You can vote multiple times as long as you haven't abstained or " +
                $"are the last player voting.\n- You currently have {VoteBank} votes in your vote bank.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Mayor : Colors.Crew;
            RoleType = RoleEnum.Mayor;
            VoteBank = CustomGameOptions.MayorVoteBank;
            RoleAlignment = RoleAlignment.CrewSov;
            AlignmentName = CSv;
            RoleDescription = "Your are a Mayor! You are the Crew who can forge votes to gain the upper hand! Save your votes to dump them on someone who you think is an evildoer!";
            InspectorResults = InspectorResults.LeadsTheGroup;
            ExtraVotes = new List<byte>();
        }
    }
}