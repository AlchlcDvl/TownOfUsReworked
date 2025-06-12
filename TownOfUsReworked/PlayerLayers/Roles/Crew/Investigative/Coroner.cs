namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Coroner)]
public sealed class Coroner : Investigative, IExaminer
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

    private CustomButton CompareButton { get; set; }
    private CustomButton AutopsyButton { get; set; }

    public HashSet<byte> Reported { get; } = [];
    private List<DeadPlayer> ReferenceBodies { get; } = [];
    private Dictionary<byte, PositionalArrow> BodyArrows { get; } = [];

    protected override UColor MainColor => CustomColorManager.Coroner;
    public override LayerEnum Type => LayerEnum.Coroner;
    public override Func<string> StartText { get; } = () => "Examine The Dead For Information";
    public override Func<string> Description => () => "- You know when players die and will be notified to as to where their body is for a brief period of time\n- You will get a report " +
        "when you report a body\n- You can perform an autopsy on bodies, to get a reference\n- You can compare the autopsy reference with players to see if they killed the body you examined";

    public override void Init()
    {
        base.Init();
        BodyArrows.Clear();
        Reported.Clear();
        ReferenceBodies.Clear();
        AutopsyButton ??= new(this, "AUTOPSY", new SpriteName("Autopsy"), AbilityTypes.Body, KeybindType.ActionSecondary, (OnClickBody)Autopsy, new Cooldown(AutopsyCd));
        CompareButton ??= new(this, "COMPARE", new SpriteName("Compare"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Compare, new Cooldown(CompareCd),
            (UsableFunc)ReferenceBodies.Any);
    }

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (!Reported.Contains(player.PlayerId) || revealed || !meeting)
            return;

        var role = handler.CurrentRole;
        color = role.Color;
        name += $"\n{(CoronerReportRole ? role : role.Faction)}";
        revealed = true;
    }

    private void DestroyArrow(byte targetPlayerId)
    {
        if (BodyArrows.Remove(targetPlayerId, out var arrow))
            arrow.Destroy();
    }

    public override void ClearArrows()
    {
        BodyArrows.Values.DestroyAll();
        BodyArrows.Clear();
    }

    public override void UpdateHud(HudManager __instance)
    {
        if (Dead)
            return;

        var validBodies = AllBodies().Where(x => KilledPlayers.Any(y => y.PlayerId == x.ParentId && y.KillAge <= CoronerArrowDur));
        BodyArrows.Keys.Where(bodyArrow => validBodies.All(x => x.ParentId != bodyArrow)).Do(DestroyArrow);

        foreach (var body in validBodies)
        {
            if (!BodyArrows.ContainsKey(body.ParentId))
                BodyArrows[body.ParentId] = new(Player, body.TruePosition, Color);
        }
    }

    private void Autopsy(DeadBody target)
    {
        Spread(Player, PlayerByBody(target));
        ReferenceBodies.AddRange(KilledPlayers.Where(x => x.PlayerId == target.ParentId));
        AutopsyButton.StartCooldown();
    }

    private void Compare(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Flash(ReferenceBodies.Any(x => target.PlayerId == x.KillerId) ? UColor.red : UColor.green);

        CompareButton.StartCooldown(cooldown);
    }

    public override void OnBodyReport(NetworkedPlayerInfo info)
    {
        if (info is null || !ReferenceBodies.TryFinding(x => x.PlayerId == info.PlayerId, out var body))
            return;

        Reported.Add(info.PlayerId);
        var reportMsg = body.ParseBodyReport(Player);

        if (!IsNullEmptyOrWhiteSpace(reportMsg))
            Run("<#4D99E6FF>〖 Autopsy Results 〗</color>", reportMsg);
    }
}