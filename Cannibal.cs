namespace TownOfUsReworked.PlayerLayers.Roles;

public class Cannibal : Neutral
{
    public CustomButton EatButton { get; set; }
    public int EatNeed { get; set; }
    public bool Eaten { get; set; }
    public DateTime LastEaten { get; set; }
    public Dictionary<byte, CustomArrow> BodyArrows { get; set; }
    public bool EatWin => EatNeed == 0;
    public bool CanEat => !Eaten || (Eaten && !CustomGameOptions.AvoidNeutralEvilKingmakers);

    public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Cannibal : Colors.Neutral;
    public override string Name => "Cannibal";
    public override LayerEnum Type => LayerEnum.Cannibal;
    public override Func<string> StartText => () => "Eat The Bodies Of The Dead";
    public override Func<string> Description => () => "- You can consume a body, making it disappear from the game" + (CustomGameOptions.EatArrows ? "\n- When someone dies, you get "
        + "an arrow pointing to their body" : "");
    public override InspectorResults InspectorResults => InspectorResults.DealsWithDead;
    public float Timer => ButtonUtils.Timer(Player, LastEaten, CustomGameOptions.CannibalCd);

    public Cannibal(PlayerControl player) : base(player)
    {
        RoleAlignment = RoleAlignment.NeutralEvil;
        Objectives = () => Eaten ? "- You are satiated" : $"- Eat {EatNeed} bod{(EatNeed == 1 ? "y" : "ies")}";
        BodyArrows = new();
        EatNeed = CustomGameOptions.CannibalBodyCount >= CustomPlayer.AllPlayers.Count / 2 ? CustomPlayer.AllPlayers.Count / 2 : CustomGameOptions.CannibalBodyCount;
        EatButton = new(this, "Eat", AbilityTypes.Dead, "ActionSecondary", Eat);
    }

    public void DestroyArrow(byte targetPlayerId)
    {
        BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId).Value?.Destroy();
        BodyArrows.Remove(targetPlayerId);
    }

    public override void OnLobby()
    {
        base.OnLobby();
        BodyArrows.Values.ToList().DestroyAll();
        BodyArrows.Clear();
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        EatButton.Update("EAT", Timer, CustomGameOptions.CannibalCd, true, CanEat);

        if (CustomGameOptions.EatArrows && !IsDead)
        {
            var validBodies = AllBodies.Where(x => KilledPlayers.Any(y => y.PlayerId == x.ParentId && y.KillTime.AddSeconds(CustomGameOptions.EatArrowDelay) < DateTime.UtcNow));

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
            OnLobby();
    }

    public void Eat()
    {
        if (IsTooFar(Player, EatButton.TargetBody) || Timer != 0f)
            return;

        Spread(Player, PlayerByBody(EatButton.TargetBody));
        CallRpc(CustomRPC.Action, ActionsRPC.FadeBody, EatButton.TargetBody);
        LastEaten = DateTime.UtcNow;
        EatNeed--;
        Coroutines.Start(FadeBody(EatButton.TargetBody));

        if (EatWin && !Eaten)
        {
            Eaten = true;
            CallRpc(CustomRPC.WinLose, WinLoseRPC.CannibalWin, this);
        }
    }
}