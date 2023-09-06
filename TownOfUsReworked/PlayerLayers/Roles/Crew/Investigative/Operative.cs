﻿namespace TownOfUsReworked.PlayerLayers.Roles;

public class Operative : Crew
{
    public List<Bug> Bugs { get; set; }
    public DateTime LastBugged { get; set; }
    public int UsesLeft { get; set; }
    public List<LayerEnum> BuggedPlayers { get; set; }
    public bool ButtonUsable => UsesLeft > 0;
    public CustomButton BugButton { get; set; }
    public Dictionary<byte, TMP_Text> PlayerNumbers { get; set; }
    public float Timer => ButtonUtils.Timer(Player, LastBugged, CustomGameOptions.BugCd);

    public override Color Color => ClientGameOptions.CustomCrewColors ? Colors.Operative : Colors.Crew;
    public override string Name => "Operative";
    public override LayerEnum Type => LayerEnum.Operative;
    public override Func<string> StartText => () => "Detect Which Roles Are Here";
    public override Func<string> Description => () => "- You can place bugs around the map\n- Upon triggering the bugs, the player's role will be included in a list to be shown in " +
        "the next meeting\n- You can see which colors are where on the admin table\n- On Vitals, the time of death for each player will be shown";
    public override InspectorResults InspectorResults => InspectorResults.DropsItems;

    public Operative(PlayerControl player) : base(player)
    {
        UsesLeft = CustomGameOptions.MaxBugs;
        Alignment = Alignment.CrewInvest;
        PlayerNumbers = new();
        BuggedPlayers = new();
        Bugs = new();
        BugButton = new(this, "Bug", AbilityTypes.Effect, "ActionSecondary", PlaceBug, true);
    }

    public override void OnLobby()
    {
        base.OnLobby();
        Bug.Clear(Bugs);
        Bugs.Clear();
    }

    public void GenNumbers()
    {
        if (!DataManager.Settings.Accessibility.ColorBlindMode)
            return;

        foreach (var voteArea in AllVoteAreas)
        {
            var targetId = voteArea.TargetPlayerId;
            var nameText = UObject.Instantiate(voteArea.NameText, voteArea.transform);
            nameText.transform.localPosition = new(-1.211f, -0.18f, -0.1f);
            nameText.text = $"{targetId}";
            PlayerNumbers.Add(targetId, nameText);
        }
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        GenNumbers();
        var message = "";

        if (BuggedPlayers.Count == 0)
            message = "No one triggered your bugs.";
        else if (BuggedPlayers.Count < CustomGameOptions.MinAmountOfPlayersInBug)
            message = "Not enough players triggered your bugs.";
        else if (BuggedPlayers.Count == 1)
        {
            var result = BuggedPlayers[0];
            var a_an = result is LayerEnum.Altruist or LayerEnum.Engineer or LayerEnum.Escort or LayerEnum.Inspector or LayerEnum.Operative or LayerEnum.Actor or LayerEnum.Amnesiac or
                LayerEnum.Arsonist or LayerEnum.Executioner or LayerEnum.Ambusher or LayerEnum.Enforcer or LayerEnum.Impostor or LayerEnum.Anarchist ? "n" : "";
            message = $"A{a_an} {result} triggered your bug.";
        }
        else if (CustomGameOptions.PreciseOperativeInfo)
        {
            message = "Your bugs returned the following results:";
            Bugs.ForEach(bug => message += $"\n{bug.GetResults()}");
        }
        else
        {
            message = "The following roles triggered your bugs: ";
            BuggedPlayers.Shuffle();
            BuggedPlayers.ForEach(role => message += $"{GetRoles(role)[0]}, ");
            message = message.Remove(message.Length - 2);
        }

        if (HUD)
            Run(HUD.Chat, "<color=#A7D1B3FF>〖 Bug Results 〗</color>", message);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        BugButton.Update("BUG", Timer, CustomGameOptions.BugCd, UsesLeft, ButtonUsable);
    }

    public void PlaceBug()
    {
        if (Timer != 0f || !ButtonUsable)
            return;

        UsesLeft--;
        LastBugged = DateTime.UtcNow;
        Bugs.Add(new(Player));
    }
}