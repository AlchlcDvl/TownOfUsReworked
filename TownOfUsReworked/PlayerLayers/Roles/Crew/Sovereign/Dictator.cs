namespace TownOfUsReworked.PlayerLayers.Roles;

public class Dictator : Crew
{
    public bool RoundOne { get; set; }
    public bool Revealed { get; set; }
    public List<byte> ToBeEjected { get; set; }
    public CustomButton RevealButton { get; set; }
    public bool Ejected { get; set; }
    public bool ToDie { get; set; }
    public CustomMeeting DictMenu { get; set; }

    public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Dictator : Colors.Crew;
    public override string Name => "Dictator";
    public override LayerEnum Type => LayerEnum.Dictator;
    public override Func<string> StartText => () => "You Have The Final Say";
    public override Func<string> Description => () => "- You can reveal yourself to the crew to eject up to 3 players for one meeting\n- When revealed, you cannot be protected";
    public override InspectorResults InspectorResults => InspectorResults.Manipulative;

    public Dictator(PlayerControl player) : base(player)
    {
        RoleAlignment = RoleAlignment.CrewSov;
        ToBeEjected = new();
        Ejected = false;
        ToDie = false;
        RevealButton = new(this, "DictReveal", AbilityTypes.Effect, "ActionSecondary", Reveal);
        DictMenu = new(Player, "DictActive", "DictDisabled", MeetingTypes.Toggle, CustomGameOptions.DictateAfterVoting, SetActive, IsExempt) { Position = new(-0.4f, 0.03f, -1.3f) };
    }

    public void Reveal()
    {
        if (RoundOne)
            return;

        CallRpc(CustomRPC.Action, ActionsRPC.DictatorReveal, this);
        Revealed = true;
        Flash(Color);
        BreakShield(Player, true);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        RevealButton.Update("REVEAL", !Revealed, !Revealed && !RoundOne);
    }

    public override void VoteComplete(MeetingHud __instance)
    {
        base.VoteComplete(__instance);
        DictMenu.HideButtons();

        if (ToBeEjected.Count > 0 && !Ejected)
        {
            ToDie = ToBeEjected.Any(x => PlayerById(x).Is(Faction.Crew));
            CallRpc(CustomRPC.Action, ActionsRPC.SetExiles, this, ToDie, ToBeEjected.ToArray());
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
        return player.Data.IsDead || player.Data.Disconnected || (player == Player && player == CustomPlayer.Local) || IsDead || !Revealed || Ejected;
    }

    public override void ConfirmVotePrefix(MeetingHud __instance)
    {
        base.ConfirmVotePrefix(__instance);
        DictMenu.Voted();

        if (ToBeEjected.Count > 0 && !Ejected)
        {
            ToDie = ToBeEjected.Any(x => PlayerById(x).Is(Faction.Crew));
            CallRpc(CustomRPC.Action, ActionsRPC.SetExiles, this, ToDie, ToBeEjected.ToArray());
        }
    }

    public override void UpdateMeeting(MeetingHud __instance)
    {
        base.UpdateMeeting(__instance);
        DictMenu.Update();
    }
}