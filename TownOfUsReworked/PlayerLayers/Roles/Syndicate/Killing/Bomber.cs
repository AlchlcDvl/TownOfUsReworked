namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Bomber)]
public sealed class Bomber : SKilling
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number BombCd = 25;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number DetonateCd = 25;

    [ToggleOption]
    private static bool BombCooldownsLinked = false;

    [ToggleOption]
    private static bool BombsRemoveOnNewRound = false;

    [ToggleOption]
    private static bool BombsDetonateOnMeetingStart = false;

    [ToggleOption]
    private static bool ShowBomb = true;

    [ToggleOption]
    private static bool BombAlert = true;

    [NumberOption(0.5f, 5f, 0.25f, Format.Distance)]
    public static Number BombRange = 1.5f;

    [NumberOption(0.5f, 5f, 0.25f, Format.Distance)]
    public static Number ChaosDriveBombRange = 0.5f;

    private CustomButton BombButton;
    private CustomButton DetonateButton;
    private readonly List<Bomb> Bombs = [];

    protected override UColor MainColor => CustomColorManager.Bomber;
    public override Layer Type => Layer.Bomber;
    public override string StartText => "Make People Go Boom";
    public override string Description => $"- You can place bombs which can be detonated at any time to kill anyone within a {BombRange}m radius\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        Bombs.Clear();
        BombButton ??= new(this, new SpriteName("Plant"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Place, new Cooldown(BombCd), "PLACE BOMB",
            (ConditionFunc)Condition);
        DetonateButton ??= new(this, new SpriteName("Detonate"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)Detonate, new Cooldown(DetonateCd), (UsableFunc)Bombs.Any,
            "DETONATE");
    }

    public override void Reset(bool meeting, bool start)
    {
        if (!BombsRemoveOnNewRound || !meeting)
            return;

        Bombs.ForEach(x => x?.gameObject?.Destroy());
        Bombs.Clear();
    }

    protected override void Deinit()
    {
        Bombs.ForEach(x => x?.gameObject?.Destroy());
        Bombs.Clear();
    }

    public override void BeforeMeeting()
    {
        if (!BombsDetonateOnMeetingStart)
            return;

        Bombs.ForEach(x => x.Detonate());
        Bombs.Clear();
    }

    private void Place()
    {
        Bombs.Add(Bomb.CreateBomb(Player, HoldsDrive));
        BombButton.StartCooldown();

        if (BombCooldownsLinked)
            DetonateButton.StartCooldown();

        if (ShowBomb)
            PerformRpcAction(BomberActionsRpc.DropBomb);
    }

    private void Detonate()
    {
        Bombs.ForEach(x => x.Detonate());
        Bombs.Clear();
        DetonateButton.StartCooldown();

        if (BombCooldownsLinked)
            BombButton.StartCooldown();

        PerformRpcAction(BomberActionsRpc.Explode);
        Play("Bomb");
    }

    private bool Condition() => !Bombs.Any(x => Vector2.Distance(Player.transform.position, x.transform.position) < x.Size * 2);

    public override void ReadRPC(RpcReader reader)
    {
        var bombAction = reader.Read<BomberActionsRpc>();

        switch (bombAction)
        {
            case BomberActionsRpc.DropBomb:
            {
                if (Player.IsBuddyWith(LocalPlayer, Handler.CurrentFaction) && !LocalPlayer.IsOtherRival(Player))
                    Bombs.Add(Bomb.CreateBomb(Player, HoldsDrive));

                break;
            }
            case BomberActionsRpc.Explode:
            {
                if (BombAlert)
                    Play("Bomb");

                Bombs.ForEach(x => x.gameObject.Destroy());
                Bombs.Clear();
                break;
            }
            default:
            {
                Failure($"Received unknown RPC - {bombAction}");
                break;
            }
        }
    }
}