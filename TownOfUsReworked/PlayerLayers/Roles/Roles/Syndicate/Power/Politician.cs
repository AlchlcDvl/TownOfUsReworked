using System.Collections.Generic;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Politician : SyndicateRole
    {
        public List<byte> ExtraVotes = new();
        public int VoteBank;
        public bool VotedOnce;
        public bool CanVote => VoteBank > 0;

        public Politician(PlayerControl player) : base(player)
        {
            Name = "Politician";
            StartText = "Hunt Down Others For Votes";
            AbilitiesText = $"- You gain a vote for each kill you make\n{AbilitiesText}";
            Color = CustomGameOptions.CustomSynColors ? Colors.Politician : Colors.Syndicate;
            RoleType = RoleEnum.Politician;
            RoleAlignment = RoleAlignment.SyndicatePower;
            VoteBank = CustomGameOptions.MayorVoteBank;
            AlignmentName = SP;
            ExtraVotes = new();
        }
    }
}