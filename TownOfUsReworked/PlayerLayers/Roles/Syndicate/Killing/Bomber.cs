namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Bomber : Syndicate
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number BombCd = 25;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number DetonateCd = 25;

    [ToggleOption]
    public static bool BombCooldownsLinked = false;

    [ToggleOption]
    public static bool BombsRemoveOnNewRound = false;

    [ToggleOption]
    public static bool BombsDetonateOnMeetingStart = false;

    [ToggleOption]
    public static bool ShowBomb = true;

    [ToggleOption]
    public static bool BombAlert = true;

    [NumberOption(0.5f, 5f, 0.25f, Format.Distance)]
    public static Number BombRange = 1.5f;

    [NumberOption(0.5f, 5f, 0.25f, Format.Distance)]
    public static Number ChaosDriveBombRange = 0.5f;

    public CustomButton BombButton { get; set; }
    public CustomButton DetonateButton { get; set; }
    public List<Bomb> Bombs { get; } = [];

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Bomber : FactionColor;
    public override LayerEnum Type => LayerEnum.Bomber;
    public override Func<string> StartText => () => "Make People Go Boom";
    public override Func<string> Description => () => $"- You can place bombs which can be detonated at any time to kill anyone within a {BombRange}m radius\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Killing;
        Bombs.Clear();
        BombButton ??= new(this, new SpriteName("Plant"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Place, new Cooldown(BombCd), "PLACE BOMB",
            (ConditionFunc)Condition);
        DetonateButton ??= new(this, new SpriteName("Detonate"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)Detonate, new Cooldown(DetonateCd), (UsableFunc)Bombs.Any,
            "DETONATE");
    }

    public override void Deinit()
    {
        base.Deinit();
        Bombs.ForEach(x => x?.gameObject?.Destroy());
        Bombs.Clear();
    }

    public override void BeforeMeeting()
    {
        if (BombsDetonateOnMeetingStart)
        {
            Bombs.ForEach(x => x.Detonate());
            Bombs.Clear();
        }
    }

    public void Place()
    {
        Bombs.Add(Bomb.CreateBomb(Player, HoldsDrive));
        BombButton.StartCooldown();

        if (BombCooldownsLinked)
            DetonateButton.StartCooldown();

        if (ShowBomb)
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, BomberActionsRPC.DropBomb);
    }

    public void Detonate()
    {
        Bombs.ForEach(x => x.Detonate());
        Bombs.Clear();
        DetonateButton.StartCooldown();

        if (BombCooldownsLinked)
            BombButton.StartCooldown();

        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, BomberActionsRPC.Explode);
        Play("Bomb");
    }

    public bool Condition() => !Bombs.Any(x => Vector2.Distance(Player.transform.position, x.transform.position) < x.Size * 2);

    public override void ReadRPC(MessageReader reader)
    {
        if (reader.ReadEnum<BomberActionsRPC>() == BomberActionsRPC.DropBomb)
        {
            if ((CustomPlayer.Local.GetFaction() is Faction.Syndicate or Faction.Intruder && Faction is Faction.Syndicate or Faction.Intruder) || CustomPlayer.Local.IsOtherLover(Player) ||
                CustomPlayer.Local.IsOtherLink(Player))
            {
                Bombs.Add(Bomb.CreateBomb(Player, HoldsDrive));
            }
        }
        else
        {
            if (BombAlert)
                Play("Bomb");

            Bombs.ForEach(x => x.gameObject.Destroy());
            Bombs.Clear();
        }
    }
}