namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Dictator : Crew
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool RoundOneNoDictReveal { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool DictateAfterVoting { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool DictatorButton { get; set; } = true;

    [NumberOption(MultiMenu.LayerSubOptions, 0, 10, 1, ZeroIsInfinity = true)]
    public static Number MaxTribunals { get; set; } = new(2);

    public bool RoundOne { get; set; }
    public bool Revealed { get; set; }
    public bool Tribunal { get; set; }
    public PlayerControl ToBeEjected { get; set; }
    public CustomButton RevealButton { get; set; }
    public CustomMeeting DictMenu { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Dictator : CustomColorManager.Crew;
    public override string Name => "Dictator";
    public override LayerEnum Type => LayerEnum.Dictator;
    public override Func<string> StartText => () => "You Have The Final Say";
    public override Func<string> Description => () => "- You can reveal yourself to the crew to eject up to 3 players for one meeting\n- When revealed, you cannot be protected";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.CrewSov;
        RevealButton ??= new(this, "REVEAL", new SpriteName("DictReveal"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClick)Reveal, (UsableFunc)Usable);
        DictMenu = new(Player, "DictActive", "DictDisabled", DictateAfterVoting, SetActive, IsExempt, new(-0.4f, 0.03f, -1.3f));
    }

    public void Reveal()
    {
        if (!Revealed)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.PublicReveal, Player);
            PublicReveal(Player);
        }
        else
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, DictActionsRPC.Tribunal);

        Tribunal = true;
    }

    public override void VoteComplete(MeetingHud __instance)
    {
        DictMenu.HideButtons();

        if (ToBeEjected && !Dead)
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, DictActionsRPC.SelectToEject, ToBeEjected);
    }

    private void SetActive(PlayerVoteArea voteArea, MeetingHud __instance)
    {
        if (__instance.state == MeetingHud.VoteStates.Discussion || !Revealed || Dead)
            return;

        var id = voteArea.TargetPlayerId;
        var player = PlayerById(id);
        DictMenu.Actives[id] = !DictMenu.Actives[id];
        var prev = ToBeEjected;

        if (!prev)
            ToBeEjected = player;
        else if (prev == player)
            ToBeEjected = null;
        else if (prev)
            DictMenu.Actives[prev.PlayerId] = false;
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        DictMenu.GenButtons(__instance, Tribunal && !Dead);
    }

    private bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = PlayerByVoteArea(voteArea);
        return player.HasDied() || player.AmOwner || Dead || !Revealed;
    }

    public override void ConfirmVotePrefix(MeetingHud __instance) => DictMenu.Voted();

    public override void UpdateMeeting(MeetingHud __instance) => DictMenu.Update(__instance);

    public bool Usable() => !RoundOne;

    public override void ReadRPC(MessageReader reader)
    {
        var dictAction = reader.ReadEnum<DictActionsRPC>();

        switch (dictAction)
        {
            case DictActionsRPC.Tribunal:
                Flash(Color);
                Tribunal = true;
                break;

            case DictActionsRPC.SelectToEject:
                ToBeEjected = reader.ReadPlayer();
                break;

            default:
                Error($"Received unknown RPC - {(int)dictAction}");
                break;
        }
    }
}