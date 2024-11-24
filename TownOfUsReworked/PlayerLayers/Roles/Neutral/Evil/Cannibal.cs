namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Cannibal : Neutral
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
    public Dictionary<byte, CustomArrow> BodyArrows { get; set; }
    public bool EatWin => EatNeed == 0;
    public bool CanEat => !Eaten || (Eaten && !NeutralSettings.AvoidNeutralKingmakers);

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Cannibal : CustomColorManager.Neutral;
    public override string Name => "Cannibal";
    public override LayerEnum Type => LayerEnum.Cannibal;
    public override Func<string> StartText => () => "Eat The Bodies Of The Dead";
    public override Func<string> Description => () => "- You can consume a body, making it disappear from the game" + (EatArrows ? "\n- When someone dies, you get an arrow pointing to their "
        + "body" : "");

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.NeutralEvil;
        Objectives = () => Eaten ? "- You are satiated" : $"- Eat {EatNeed} bod{(EatNeed == 1 ? "y" : "ies")}";
        BodyArrows = [];
        EatNeed = Math.Min(BodiesNeeded, AllPlayers().Count / 2);
        EatButton ??= new(this, new SpriteName("Eat"), AbilityTypes.Dead, KeybindType.ActionSecondary, (OnClickBody)Eat, new Cooldown(EatCd), "EAT", (UsableFunc)Usable);
    }

    public void DestroyArrow(byte targetPlayerId)
    {
        BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId).Value?.Destroy();
        BodyArrows.Remove(targetPlayerId);
    }

    public override void Deinit()
    {
        base.Deinit();
        BodyArrows.Values.ToList().DestroyAll();
        BodyArrows.Clear();
    }

    public bool Usable() => CanEat;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (EatArrows && !Dead)
        {
            var validBodies = AllBodies().Where(x => KilledPlayers.Any(y => y.PlayerId == x.ParentId && y.KillTime.AddSeconds(EatArrowDelay) < DateTime.UtcNow));

            foreach (var bodyArrow in BodyArrows.Keys)
            {
                if (!validBodies.Any(x => x.ParentId == bodyArrow))
                    DestroyArrow(bodyArrow);
            }

            foreach (var body in validBodies)
            {
                if (!BodyArrows.ContainsKey(body.ParentId))
                    BodyArrows.Add(body.ParentId, new(Player, Color));

                BodyArrows[body.ParentId]?.Update(body.TruePosition);
            }
        }
        else if (BodyArrows.Count != 0)
            Deinit();
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