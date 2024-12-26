namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Coroner : Crew, IExaminer
{
    [NumberOption(MultiMenu.LayerSubOptions, 0f, 2f, 0.05f, Format.Time)]
    public static Number CoronerArrowDur { get; set; } = new(0.1f);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool CoronerReportRole { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool CoronerReportName { get; set; } = false;

    [NumberOption(MultiMenu.LayerSubOptions, 0.5f, 15f, 0.5f, Format.Time)]
    public static Number CoronerKillerNameTime { get; set; } = new(1);

    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number CompareCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number AutopsyCd { get; set; } = new(25);

    public Dictionary<byte, PositionalArrow> BodyArrows { get; set; }
    public List<byte> Reported { get; set; }
    public CustomButton CompareButton { get; set; }
    public List<DeadPlayer> ReferenceBodies { get; set; }
    public CustomButton AutopsyButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Coroner : FactionColor;
    public override string Name => "Coroner";
    public override LayerEnum Type => LayerEnum.Coroner;
    public override Func<string> StartText => () => "Examine The Dead For Information";
    public override Func<string> Description => () => "- You know when players die and will be notified to as to where their body is for a brief period of time\n- You will get a report " +
        "when you report a body\n- You can perform an autopsy on bodies, to get a reference\n- You can compare the autopsy reference with players to see if they killed the body you examined";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.CrewInvest;
        BodyArrows = [];
        Reported = [];
        ReferenceBodies = [];
        AutopsyButton ??= new(this, "AUTOPSY", new SpriteName("Autopsy"), AbilityTypes.Body, KeybindType.ActionSecondary, (OnClickBody)Autopsy, new Cooldown(AutopsyCd));
        CompareButton ??= new(this, "COMPARE", new SpriteName("Compare"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Compare, new Cooldown(CompareCd), (UsableFunc)Usable);
    }

    public void DestroyArrow(byte targetPlayerId)
    {
        BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId).Value?.Destroy();
        BodyArrows.Remove(targetPlayerId);
    }

    public override void ClearArrows()
    {
        base.ClearArrows();
        BodyArrows.Values.DestroyAll();
        BodyArrows.Clear();
    }

    public bool Usable() => ReferenceBodies.Any();

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Dead)
            return;

        var validBodies = AllBodies().Where(x => KilledPlayers.Any(y => y.PlayerId == x.ParentId && y.KillAge <= CoronerArrowDur));

        foreach (var bodyArrow in BodyArrows.Keys)
        {
            if (!validBodies.Any(x => x.ParentId == bodyArrow))
                DestroyArrow(bodyArrow);
        }

        foreach (var body in validBodies)
        {
            if (!BodyArrows.ContainsKey(body.ParentId))
                BodyArrows[body.ParentId] = new(Player, body.TruePosition, Color);
        }
    }

    public void Autopsy(DeadBody target)
    {
        Spread(Player, PlayerByBody(target));
        ReferenceBodies.AddRange(KilledPlayers.Where(x => x.PlayerId == target.ParentId));
        AutopsyButton.StartCooldown();
    }

    public void Compare(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Flash(ReferenceBodies.Any(x => target.PlayerId == x.KillerId) ? UColor.red : UColor.green);

        CompareButton.StartCooldown(cooldown);
    }

    public override void OnBodyReport(NetworkedPlayerInfo info)
    {
        if (info == null || !KilledPlayers.TryFinding(x => x.PlayerId == info.PlayerId, out var body))
            return;

        Reported.Add(info.PlayerId);
        body.Reporter = Player;
        var reportMsg = body.ParseBodyReport();

        if (IsNullEmptyOrWhiteSpace(reportMsg))
            return;

        // Only Coroner can see this
        if (HUD())
            Run("<#4D99E6FF>〖 Autopsy Results 〗</color>", reportMsg);
    }
}