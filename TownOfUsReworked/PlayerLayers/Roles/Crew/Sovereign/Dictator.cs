namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Dictator : Crew
{
    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool RoundOneNoDictReveal { get; set; } = false;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool DictateAfterVoting { get; set; } = false;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool DictatorButton { get; set; } = true;

    public bool RoundOne { get; set; }
    public bool Revealed { get; set; }
    public List<byte> ToBeEjected { get; set; }
    public CustomButton RevealButton { get; set; }
    public bool Ejected { get; set; }
    public bool ToDie { get; set; }
    public CustomMeeting DictMenu { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Dictator : CustomColorManager.Crew;
    public override string Name => "Dictator";
    public override LayerEnum Type => LayerEnum.Dictator;
    public override Func<string> StartText => () => "You Have The Final Say";
    public override Func<string> Description => () => "- You can reveal yourself to the crew to eject up to 3 players for one meeting\n- When revealed, you cannot be protected";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.CrewSov;
        ToBeEjected = [];
        Ejected = false;
        ToDie = false;
        RevealButton = CreateButton(this, "REVEAL", new SpriteName("DictReveal"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClick)Reveal);
        DictMenu = new(Player, "DictActive", "DictDisabled", CustomGameOptions.DictateAfterVoting, SetActive, IsExempt, new(-0.4f, 0.03f, -1.3f));
    }

    public void Reveal()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.PublicReveal, Player);
        PublicReveal(Player);
    }

    public override void VoteComplete(MeetingHud __instance)
    {
        base.VoteComplete(__instance);
        DictMenu.HideButtons();

        if (ToBeEjected.Any() && !Ejected)
        {
            ToDie = ToBeEjected.Any(x => PlayerById(x).Is(Faction.Crew));
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, ToDie, ToBeEjected);
        }
    }

    private void SetActive(PlayerVoteArea voteArea, MeetingHud __instance)
    {
        if (__instance.state == MeetingHud.VoteStates.Discussion || !Revealed || Ejected || ToDie)
            return;

        var id = voteArea.TargetPlayerId;

        if (ToBeEjected.Contains(id))
        {
            ToBeEjected.Remove(id);
            DictMenu.Actives[id] = false;
        }
        else
        {
            ToBeEjected.Add(id);
            DictMenu.Actives[id] = true;
        }

        if (ToBeEjected.Count > 3)
        {
            DictMenu.Actives[ToBeEjected[0]] = false;
            ToBeEjected.Remove(ToBeEjected[0]);
        }
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        DictMenu.GenButtons(__instance, Revealed && !Ejected && !ToDie);
    }

    private bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = PlayerByVoteArea(voteArea);
        return player.HasDied() || (player == Player && Local) || Dead || !Revealed || Ejected;
    }

    public override void ConfirmVotePrefix(MeetingHud __instance)
    {
        base.ConfirmVotePrefix(__instance);
        DictMenu.Voted();

        if (ToBeEjected.Any() && !Ejected)
        {
            ToDie = ToBeEjected.Any(x => PlayerById(x).Is(Faction.Crew));
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, ToDie, ToBeEjected);
        }
    }

    public override void UpdateMeeting(MeetingHud __instance)
    {
        base.UpdateMeeting(__instance);
        DictMenu.Update(__instance);
    }

    public override void ReadRPC(MessageReader reader)
    {
        ToDie = reader.ReadBoolean();
        ToBeEjected = reader.ReadByteList();
    }
}