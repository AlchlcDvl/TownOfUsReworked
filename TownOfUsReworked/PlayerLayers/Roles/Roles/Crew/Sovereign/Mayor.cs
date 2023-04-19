using System.Collections.Generic;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using TMPro;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Mayor : CrewRole
    {
        public List<byte> ExtraVotes = new();
        public int VoteBank;
        public bool SelfVote;
        public bool VotedOnce;
        public PlayerVoteArea Abstain;
        public bool CanVote => VoteBank > 0 && !SelfVote;

        public Mayor(PlayerControl player) : base(player)
        {
            Name = "Mayor";
            StartText = "Save Your Votes To Vote Dump Someone";
            AbilitiesText = "- You can save your votes into your vote bank, so you can vote multiple times later\n- You can vote multiple times as long as you haven't abstained or " +
                "are the last player voting";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Mayor : Colors.Crew;
            RoleType = RoleEnum.Mayor;
            VoteBank = CustomGameOptions.MayorVoteBank;
            RoleAlignment = RoleAlignment.CrewSov;
            AlignmentName = CSv;
            InspectorResults = InspectorResults.LeadsTheGroup;
            ExtraVotes = new();
            Type = LayerEnum.Mayor;
        }

        public static void UpdateButton(Mayor role, MeetingHud __instance)
        {
            var skip = __instance.SkipVoteButton;
            role.Abstain.gameObject.SetActive(skip.gameObject.active && !role.VotedOnce);
            role.Abstain.voteComplete = skip.voteComplete;
            role.Abstain.GetComponent<SpriteRenderer>().enabled = skip.GetComponent<SpriteRenderer>().enabled;
            role.Abstain.GetComponentsInChildren<TextMeshPro>()[0].text = "Abstain";
        }

        public void GenButton(MeetingHud __instance)
        {
            Abstain = Object.Instantiate(__instance.SkipVoteButton, __instance.SkipVoteButton.transform.parent);
            Abstain.Parent = __instance;
            Abstain.SetTargetPlayerId(251);
            Abstain.transform.localPosition = __instance.SkipVoteButton.transform.localPosition + new Vector3(0f, -0.17f, 0f);
            __instance.SkipVoteButton.transform.localPosition += new Vector3(0f, 0.20f, 0f);
            UpdateButton(this, __instance);
        }

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);
            GenButton(__instance);
        }
    }
}