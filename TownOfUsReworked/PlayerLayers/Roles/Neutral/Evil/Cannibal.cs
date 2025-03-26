namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Cannibal)]
public sealed class Cannibal : Evil
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number EatCd = 25;

    [NumberOption(1, 5, 1)]
    private static Number BodiesNeeded = 2;

    [ToggleOption]
    private static bool EatArrows = false;

    [NumberOption(0f, 15f, 1f, Format.Time)]
    private static Number EatArrowDelay = 5;

    [ToggleOption]
    public static bool CannibalVent = false;

    private CustomButton EatButton { get; set; }
    private int EatNeed { get; set; }
    public bool Eaten { get; set; }
    private Dictionary<byte, PositionalArrow> BodyArrows { get; } = [];
    private bool EatWin => EatNeed == 0;
    private bool CanEat => !Eaten || !NeutralSettings.AvoidNeutralKingmakers;

    protected override UColor MainColor => CustomColorManager.Cannibal;
    public override LayerEnum Type => LayerEnum.Cannibal;
    public override Func<string> StartText => () => "Eat The Bodies Of The Dead";
    public override Func<string> Description => () => "- You can consume a body, making it disappear from the game" + (EatArrows ? "\n- When someone dies, you get an arrow pointing to their "
        + "body" : "");
    public override bool HasWon => EatWin;
    public override bool CanVent => base.CanVent && CannibalVent;
    protected override WinLose EndState => WinLose.CannibalWins;

    protected override void Init()
    {
        base.Init();
        Objectives = () => Eaten ? "- You are satiated" : $"- Eat {EatNeed} bod{(EatNeed == 1 ? "y" : "ies")}";
        BodyArrows.Clear();
        EatNeed = Mathf.Min(BodiesNeeded, GameData.Instance.PlayerCount / 2);
        EatButton ??= new(this, new SpriteName("Eat"), AbilityTypes.Body, KeybindType.ActionSecondary, (OnClickBody)Eat, new Cooldown(EatCd), "EAT", (UsableFunc)Usable);
    }

    private void DestroyArrow(byte targetPlayerId)
    {
        if (BodyArrows.Remove(targetPlayerId, out var arrow))
            arrow.Destroy();
    }

    public override void ClearArrows()
    {
        base.ClearArrows();
        BodyArrows.Values.DestroyAll();
        BodyArrows.Clear();
    }

    private bool Usable() => CanEat;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (!EatArrows || Dead)
            return;

        var validBodies = AllBodies().Where(x => KilledPlayers.Any(y => y.PlayerId == x.ParentId && y.KillAge <= EatArrowDelay));
        BodyArrows.Keys.Where(bodyArrow => validBodies.All(x => x.ParentId != bodyArrow)).ForEach(DestroyArrow);

        foreach (var body in validBodies)
        {
            if (!BodyArrows.ContainsKey(body.ParentId))
                BodyArrows[body.ParentId] = new(Player, body.TruePosition, Color);
        }
    }

    private void Eat(DeadBody target)
    {
        Spread(Player, PlayerByBody(target));
        CallRpc(CustomRPC.Action, ActionsRPC.FadeBody, target);
        EatButton.StartCooldown();
        EatNeed--;
        FadeBody(target);

        if (!EatWin || Eaten)
            return;

        Eaten = true;
        CallRpc(CustomRPC.WinLose, WinLose.CannibalWins, this);
    }
}