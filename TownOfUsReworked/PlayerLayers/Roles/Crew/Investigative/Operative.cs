﻿namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Operative : Crew
{
    [NumberOption(MultiMenu2.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float BugCd { get; set; } = 25f;

    [NumberOption(MultiMenu2.LayerSubOptions, 0f, 15f, 0.5f, Format.Time)]
    public static float MinAmountOfTimeInBug { get; set; } = 0f;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool BugsRemoveOnNewRound { get; set; } = true;

    [NumberOption(MultiMenu2.LayerSubOptions, 1, 15, 1)]
    public static int MaxBugs { get; set; } = 5;

    [NumberOption(MultiMenu2.LayerSubOptions, 0.5f, 5f, 0.25f, Format.Distance)]
    public static float BugRange { get; set; } = 1.5f;

    [NumberOption(MultiMenu2.LayerSubOptions, 1, 5, 1)]
    public static int MinAmountOfPlayersInBug { get; set; } = 1;

    [StringOption(MultiMenu2.LayerSubOptions)]
    public static AdminDeadPlayers WhoSeesDead { get; set; } = AdminDeadPlayers.Nobody;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool PreciseOperativeInfo { get; set; } = false;

    public List<Bug> Bugs { get; set; }
    public List<LayerEnum> BuggedPlayers { get; set; }
    public CustomButton BugButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Operative : CustomColorManager.Crew;
    public override string Name => "Operative";
    public override LayerEnum Type => LayerEnum.Operative;
    public override Func<string> StartText => () => "Detect Which Roles Are Here";
    public override Func<string> Description => () => "- You can place bugs around the map\n- Upon triggering the bugs, the player's role will be included in a list to be shown in the next" +
        " meeting\n- You can see which colors are where on the admin table\n- On Vitals, the time of death for each player will be shown";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.CrewInvest;
        BuggedPlayers = [];
        Bugs = [];
        BugButton = CreateButton(this, "BUG", new SpriteName("Bug"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClick)PlaceBug, new Cooldown(CustomGameOptions.BugCd),
            CustomGameOptions.MaxBugs, (ConditionFunc)Condition);
    }

    public override void OnLobby()
    {
        base.OnLobby();
        Bugs.ForEach(x => x.Destroy());
        Bugs.Clear();
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);

        if (Dead)
            return;

        var message = "";

        if (BuggedPlayers.Count == 0)
            message = "No one triggered your bugs.";
        else if (BuggedPlayers.Count < CustomGameOptions.MinAmountOfPlayersInBug)
            message = "Not enough players triggered your bugs.";
        else if (BuggedPlayers.Count == 1)
        {
            var result = BuggedPlayers[0];
            var a_an = result is LayerEnum.Altruist or LayerEnum.Engineer or LayerEnum.Escort or LayerEnum.Operative or LayerEnum.Amnesiac or LayerEnum.Actor or LayerEnum.Arsonist or
                LayerEnum.Executioner or LayerEnum.Ambusher or LayerEnum.Enforcer or LayerEnum.Impostor or LayerEnum.Anarchist ? "n" : "";
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
            BuggedPlayers.ForEach(role => message += $"{GetLayers(role)[0]}, ");
            message = message.Remove(message.Length - 2);
        }

        if (HUD)
            Run("<color=#A7D1B3FF>〖 Bug Results 〗</color>", message);
    }

    public bool Condition() => !Bugs.Any(x => Vector2.Distance(Player.transform.position, x.Transform.position) < x.Size * 2);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        Bugs.ForEach(x => x.Update());
    }

    public void PlaceBug()
    {
        Bugs.Add(new(Player));
        BugButton.StartCooldown();
    }
}