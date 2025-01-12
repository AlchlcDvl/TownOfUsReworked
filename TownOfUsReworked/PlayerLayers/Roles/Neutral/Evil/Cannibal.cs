namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Cannibal : Evil
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number EatCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 1, 5, 1)]
    public static Number BodiesNeeded { get; set; } = new(1);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool EatArrows { get; set; } = false;

    [NumberOption(MultiMenu.LayerSubOptions, 0f, 15f, 1f, Format.Time)]
    public static Number EatArrowDelay { get; set; } = new(5);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool CannibalVent { get; set; } = false;

    public CustomButton EatButton { get; set; }
    public int EatNeed { get; set; }
    public bool Eaten { get; set; }
    public Dictionary<byte, PositionalArrow> BodyArrows { get; } = [];
    public bool EatWin => EatNeed == 0;
    public bool CanEat => !Eaten || (Eaten && !NeutralSettings.AvoidNeutralKingmakers);

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Cannibal : FactionColor;
    public override LayerEnum Type => LayerEnum.Cannibal;
    public override Func<string> StartText => () => "Eat The Bodies Of The Dead";
    public override Func<string> Description => () => "- You can consume a body, making it disappear from the game" + (EatArrows ? "\n- When someone dies, you get an arrow pointing to their "
        + "body" : "");
    public override bool HasWon => EatWin;
    public override WinLose EndState => WinLose.CannibalWins;

    public override void Init()
    {
        base.Init();
        Objectives = () => Eaten ? "- You are satiated" : $"- Eat {EatNeed} bod{(EatNeed == 1 ? "y" : "ies")}";
        BodyArrows.Clear();
        EatNeed = Mathf.Min(BodiesNeeded, GameData.Instance.PlayerCount / 2);
        EatButton ??= new(this, new SpriteName("Eat"), AbilityTypes.Body, KeybindType.ActionSecondary, (OnClickBody)Eat, new Cooldown(EatCd), "EAT", (UsableFunc)Usable);
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

    public bool Usable() => CanEat;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (!EatArrows || Dead)
            return;

        var validBodies = AllBodies().Where(x => KilledPlayers.Any(y => y.PlayerId == x.ParentId && y.KillAge <= EatArrowDelay));

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

    public void Eat(DeadBody target)
    {
        Spread(Player, PlayerByBody(target));
        CallRpc(CustomRPC.Action, ActionsRPC.FadeBody, target);
        EatButton.StartCooldown();
        EatNeed--;
        FadeBody(target);

        if (EatWin && !Eaten)
        {
            Eaten = true;
            CallRpc(CustomRPC.WinLose, WinLose.CannibalWins, this);
        }
    }
}