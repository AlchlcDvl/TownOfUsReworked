namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Politician : Ability
    {
        public List<byte> ExtraVotes = new();
        public int VoteBank;
        public bool SelfVote;
        public bool VotedOnce;
        public PlayerVoteArea Abstain;
        public bool CanVote => VoteBank > 0 && !SelfVote;
        public bool CanKill;

        public Politician(PlayerControl player) : base(player)
        {
            Name = "Politician";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Politician : Colors.Ability;
            AbilityType = AbilityEnum.Politician;
            VoteBank = CustomGameOptions.PoliticianVoteBank;
            ExtraVotes = new();
            Type = LayerEnum.Politician;
            CanKill = player.Is(Faction.Intruder) || player.Is(Faction.Syndicate) || player.Is(RoleAlignment.NeutralKill) || player.Is(ObjectifierEnum.Corrupted) ||
                player.Is(ObjectifierEnum.Traitor) || player.Is(ObjectifierEnum.Fanatic) || player.Is(RoleAlignment.CrewKill);
            TaskText = () => $"- You can vote multiple times as long as you{(CanKill ? "" : " haven't abstained or")} are the last player voting\n" + (CanKill ? ("- When you kill players, "
                + "you take their") : "- You can abstain in meetings to gain more") + " votes for use later";

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public void UpdateButton(MeetingHud __instance)
        {
            if (CanKill)
                return;

            Abstain.gameObject.SetActive(__instance.SkipVoteButton.gameObject.active && !VotedOnce);
            Abstain.voteComplete = __instance.SkipVoteButton.voteComplete;
            Abstain.GetComponent<SpriteRenderer>().enabled = __instance.SkipVoteButton.GetComponent<SpriteRenderer>().enabled;
            Abstain.GetComponentsInChildren<TextMeshPro>()[0].text = "Abstain";
        }

        public void DestroyAbstain()
        {
            if (CanKill || Abstain == null)
                return;

            Abstain.ClearButtons();
            Abstain.gameObject.SetActive(false);
            Abstain.Destroy();
            Abstain = null;
            VotedOnce = false;
            ExtraVotes.Clear();
        }

        public void GenButton(MeetingHud __instance)
        {
            if (CanKill)
                return;

            Abstain = UObject.Instantiate(__instance.SkipVoteButton, __instance.SkipVoteButton.transform.parent);
            Abstain.Parent = __instance;
            Abstain.SetTargetPlayerId(251);
            Abstain.transform.localPosition = __instance.SkipVoteButton.transform.localPosition + new Vector3(0f, -0.17f, 0f);
            __instance.SkipVoteButton.transform.localPosition += new Vector3(0f, 0.17f, 0f);
            UpdateButton(__instance);
        }

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);
            GenButton(__instance);
        }

        public override void UpdateMeeting(MeetingHud __instance)
        {
            base.UpdateMeeting(__instance);

            if (__instance.state == MeetingHud.VoteStates.Discussion)
            {
                if (__instance.discussionTimer < CustomGameOptions.DiscussionTime || CanKill)
                    Abstain?.SetDisabled();
                else
                    Abstain?.SetEnabled();
            }

            UpdateButton(__instance);

            if (IsDead || __instance.TimerText.text.Contains("Can Vote"))
                return;

            __instance.TimerText.text = $"Can Vote: {VoteBank} time(s) | {__instance.TimerText.text}";
        }

        public override void VoteComplete(MeetingHud __instance)
        {
            base.VoteComplete(__instance);
            UpdateButton(__instance);
        }

        public override void ClearVote(MeetingHud __instance)
        {
            base.ClearVote(__instance);
            UpdateButton(__instance);
        }

        public override void ConfirmVotePostfix(MeetingHud __instance)
        {
            base.ConfirmVotePostfix(__instance);
            __instance.SkipVoteButton.gameObject.SetActive(CanVote);
            Abstain.ClearButtons();
            UpdateButton(__instance);
        }

        public override void ConfirmVotePrefix(MeetingHud __instance)
        {
            base.ConfirmVotePrefix(__instance);

            if (__instance.state != MeetingHud.VoteStates.Voted)
                return;

            __instance.state = MeetingHud.VoteStates.NotVoted;
        }

        public override void SelectVote(MeetingHud __instance, int id)
        {
            base.SelectVote(__instance, id);

            if (id != 251)
                Abstain?.ClearButtons();

            UpdateButton(__instance);
        }
    }
}