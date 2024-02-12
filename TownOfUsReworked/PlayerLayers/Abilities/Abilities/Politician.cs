namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Politician : Ability
{
    public List<byte> ExtraVotes { get; set; }
    public int VoteBank { get; set; }
    public bool SelfVote { get; set; }
    public bool VotedOnce { get; set; }
    public PlayerVoteArea Abstain { get; set; }
    public bool CanVote => VoteBank > 0 && !SelfVote;
    public bool CanKill => Player.CanKill();

    public override UColor Color => ClientGameOptions.CustomAbColors ? CustomColorManager.Politician : CustomColorManager.Ability;
    public override string Name => "Politician";
    public override LayerEnum Type => LayerEnum.Politician;
    public override Func<string> Description => () => $"- You can vote multiple times as long as you{(CanKill ? "" : " haven't abstained or")} are the last player voting\n- You can " +
        (CanKill ? "players to take their" : "abstain in meetings to gain more") + " votes for use later";

    public Politician() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        VoteBank = CustomGameOptions.PoliticianVoteBank;
        ExtraVotes = new();
        return this;
    }

    private void UpdateButton(MeetingHud __instance)
    {
        if (CanKill || !Abstain)
            return;

        Abstain.gameObject.SetActive(__instance.SkipVoteButton.gameObject.active && !VotedOnce);
        Abstain.voteComplete = __instance.SkipVoteButton.voteComplete;
        Abstain.GetComponent<SpriteRenderer>().enabled = __instance.SkipVoteButton.GetComponent<SpriteRenderer>().enabled;
        Abstain.GetComponentsInChildren<TextMeshPro>()[0].text = "Abstain";
    }

    public void DestroyAbstain()
    {
        if (CanKill || !Abstain)
            return;

        Abstain.ClearButtons();
        Abstain.gameObject.SetActive(false);
        Abstain.Destroy();
        Abstain = null;
        VotedOnce = false;
        ExtraVotes.Clear();
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);

        if (CanKill || !Abstain)
            return;

        Abstain = UObject.Instantiate(__instance.SkipVoteButton, __instance.SkipVoteButton.transform.parent);
        Abstain.Parent = __instance;
        Abstain.SetTargetPlayerId(251);
        Abstain.transform.localPosition = __instance.SkipVoteButton.transform.localPosition + new Vector3(0f, -0.17f, 0f);
        __instance.SkipVoteButton.transform.localPosition += new Vector3(0f, 0.17f, 0f);
        UpdateButton(__instance);
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

    public override void ReadRPC(MessageReader reader)
    {
        var polAction = (PoliticianActionsRPC)reader.ReadByte();

        switch(polAction)
        {
            case PoliticianActionsRPC.Remove:
                ExtraVotes = reader.ReadByteList();
                VoteBank -= ExtraVotes.Count;
                break;

            case PoliticianActionsRPC.Add:
                VoteBank -= reader.ReadInt32();
                break;

            default:
                LogError($"Received unknown RPC - {polAction}");
                break;
        }
    }
}