namespace TownOfUsReworked.PlayerLayers.Abilities;

[LayerHeaderOption(Layer.Politician)]
public sealed class Politician : Ability
{
    [NumberOption(0, 10, 1)]
    private static Number PoliticianVoteBank = 0;

    [ToggleOption]
    public static bool PoliticianButton = true;

    public int VoteBank;
    public bool SelfVote;
    public bool VotedOnce;
    public PlayerVoteArea Abstain;

    public List<byte> ExtraVotes { get; } = [];

    public bool CanVote => VoteBank > 0 && !SelfVote;
    public bool CanKill => Player.CanKill();

    protected override UColor MainColor => CustomColorManager.Politician;
    public override Layer Type => Layer.Politician;
    public override string Description => $"- You can vote multiple times as long as you{(CanKill ? "" : " haven't abstained or")} are the last player voting\n- You can " +
        (CanKill ? "players to take their" : "abstain in meetings to gain more") + " votes for use later";

    public override void Init()
    {
        VoteBank = PoliticianVoteBank;
        ExtraVotes.Clear();
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
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
        if (__instance.state == MeetingHud.VoteStates.Discussion)
        {
            if (__instance.discussionTimer < GameOptions.DiscussionTime || CanKill)
                Abstain?.SetDisabled();
            else
                Abstain?.SetEnabled();
        }

        UpdateButton(__instance);

        if (Dead || __instance.TimerText.text.Contains("Can Vote"))
            return;

        __instance.TimerText.text = $"Can Vote: {VoteBank} time(s) | {__instance.TimerText.text}";
    }

    public override void VoteComplete(MeetingHud __instance) => UpdateButton(__instance);

    public override void ClearVote(MeetingHud __instance) => UpdateButton(__instance);

    public override void ConfirmVotePostfix(MeetingHud __instance)
    {
        __instance.SkipVoteButton.gameObject.SetActive(CanVote);
        Abstain.ClearButtons();
        UpdateButton(__instance);
    }

    public override void ConfirmVotePrefix(MeetingHud __instance)
    {
        if (__instance.state == MeetingHud.VoteStates.Voted)
            __instance.state = MeetingHud.VoteStates.NotVoted;
    }

    public override void SelectVote(MeetingHud __instance, int id)
    {
        if (id != 251)
            Abstain?.ClearButtons();

        UpdateButton(__instance);
    }

    public override void ReadRPC(RpcReader reader)
    {
        var polAction = reader.Read<PoliticianActionsRpc>();

        switch(polAction)
        {
            case PoliticianActionsRpc.Remove:
            {
                ExtraVotes.Clear();
                ExtraVotes.AddRange(reader.ReadValues<byte>());
                VoteBank -= ExtraVotes.Count;
                break;
            }
            case PoliticianActionsRpc.Add:
            {
                VoteBank -= reader.ReadInt();
                break;
            }
            default:
            {
                Failure($"Received unknown RPC - {polAction}");
                break;
            }
        }
    }

    public override void OnKill(PlayerControl victim) => VoteBank++;

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
}