namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Coroner : Crew, IExaminer
{
    [NumberOption(0f, 2f, 0.05f, Format.Time)]
    public static Number CoronerArrowDur = 0.1f;

    [ToggleOption]
    public static bool CoronerReportRole = false;

    [ToggleOption]
    public static bool CoronerReportName = false;

    [NumberOption(0.5f, 15f, 0.5f, Format.Time)]
    public static Number CoronerKillerNameTime = 1;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number CompareCd = 25;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number AutopsyCd = 25;

    public CustomButton CompareButton { get; set; }
    public CustomButton AutopsyButton { get; set; }

    public List<byte> Reported { get; } = [];
    public List<DeadPlayer> ReferenceBodies { get; } = [];
    public Dictionary<byte, PositionalArrow> BodyArrows { get; } = [];

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Coroner : FactionColor;
    public override LayerEnum Type => LayerEnum.Coroner;
    public override Func<string> StartText => () => "Examine The Dead For Information";
    public override Func<string> Description => () => "- You know when players die and will be notified to as to where their body is for a brief period of time\n- You will get a report " +
        "when you report a body\n- You can perform an autopsy on bodies, to get a reference\n- You can compare the autopsy reference with players to see if they killed the body you examined";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Investigative;
        BodyArrows.Clear();
        Reported.Clear();
        ReferenceBodies.Clear();
        AutopsyButton ??= new(this, "AUTOPSY", new SpriteName("Autopsy"), AbilityTypes.Body, KeybindType.ActionSecondary, (OnClickBody)Autopsy, new Cooldown(AutopsyCd));
        CompareButton ??= new(this, "COMPARE", new SpriteName("Compare"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Compare, new Cooldown(CompareCd),
            (UsableFunc)ReferenceBodies.Any);
    }

    public void DestroyArrow(byte targetPlayerId)
    {
        if (BodyArrows.TryGetValue(targetPlayerId, out var arrow))
        {
            arrow.Destroy();
            BodyArrows.Remove(targetPlayerId);
        }
    }

    public override void ClearArrows()
    {
        base.ClearArrows();
        BodyArrows.Values.DestroyAll();
        BodyArrows.Clear();
    }

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
        if (info == null || !ReferenceBodies.TryFinding(x => x.PlayerId == info.PlayerId, out var body))
            return;

        Reported.Add(info.PlayerId);
        body.Reporter = Player;
        var reportMsg = body.ParseBodyReport();

        if (!IsNullEmptyOrWhiteSpace(reportMsg))
            Run("<#4D99E6FF>〖 Autopsy Results 〗</color>", reportMsg);
    }
}