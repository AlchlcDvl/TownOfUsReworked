namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Cannibal)]
public sealed class Cannibal : Harbinger<Gluttony>
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number EatCd = 25;

    [NumberOption(1, 5, 1)]
    private static Number BodiesNeeded = 3;

    [ToggleOption]
    private static bool EatArrows = false;

    [NumberOption(0f, 15f, 1f, Format.Time)]
    private static Number EatArrowDelay = 5;

    [ToggleOption]
    public static bool CannibalVent = false;

    private CustomButton EatButton { get; set; }
    private int EatNeed { get; set; }
    private Dictionary<byte, PositionalArrow> BodyArrows { get; } = [];

    protected override UColor MainColor => CustomColorManager.Cannibal;
    public override LayerEnum Type => LayerEnum.Cannibal;
    public override Func<string> StartText { get; } = () => "Eat The Bodies Of The Dead";
    public override Func<string> Description => () => "- You can consume a body, making it disappear" + (EatArrows ? "\n- When someone dies, you get an arrow pointing to their body" : "");
    public override bool CanVent => base.CanVent && CannibalVent;
    public override DefenseEnum DefenseVal => EatNeed <= 2 ? DefenseEnum.None : DefenseEnum.Basic;

    public override void Init()
    {
        base.Init();
        Objectives = () => $"- Eat {EatNeed} bod{(EatNeed == 1 ? "y" : "ies")} to bring forth <#A7C596FF>Gluttony</color>";
        BodyArrows.Clear();
        EatNeed = Mathf.Min(BodiesNeeded, GameData.Instance.PlayerCount / 2);
        EatButton ??= new(this, new SpriteName("Eat"), AbilityTypes.Body, KeybindType.ActionSecondary, (OnClickBody)Eat, new Cooldown(EatCd), "EAT");
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
        if (!EatArrows || Dead)
            return;

        var validBodies = AllBodies().Where(x => KilledPlayers.Any(y => y.PlayerId == x.ParentId && y.KillAge <= EatArrowDelay));
        BodyArrows.Keys.Where(bodyArrow => validBodies.All(x => x.ParentId != bodyArrow)).Do(DestroyArrow);

        foreach (var body in validBodies)
        {
            if (!BodyArrows.ContainsKey(body.ParentId))
                BodyArrows[body.ParentId] = new(Player, body.TruePosition, Color);
        }
    }

    private void Eat(DeadBody target)
    {
        Spread(Player, PlayerByBody(target));
        EatButton.StartCooldown();
        EatNeed--;
        FadeBody(target);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, target);
    }

    public override void ReadRPC(RpcReader reader)
    {
        var id = reader.ReadByte();
        Spread(Player, PlayerById(id));
        FadeBody(BodyById(id));
        EatNeed--;
    }

    protected override bool CanTransform() => EatNeed == 0;
}