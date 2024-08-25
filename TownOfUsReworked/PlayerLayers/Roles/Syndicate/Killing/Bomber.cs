namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Bomber : Syndicate
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float BombCd { get; set; } = 25f;

    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float DetonateCd { get; set; } = 25f;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool BombCooldownsLinked { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool BombsRemoveOnNewRound { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool BombsDetonateOnMeetingStart { get; set; } = false;

    [NumberOption(MultiMenu.LayerSubOptions, 0.5f, 5f, 0.25f, Format.Distance)]
    public static float BombRange { get; set; } = 1.5f;

    [NumberOption(MultiMenu.LayerSubOptions, 0.5f, 5f, 0.25f, Format.Distance)]
    public static float ChaosDriveBombRange { get; set; } = 0.5f;

    public CustomButton BombButton { get; set; }
    public CustomButton DetonateButton { get; set; }
    public List<Bomb> Bombs { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Bomber : CustomColorManager.Syndicate;
    public override string Name => "Bomber";
    public override LayerEnum Type => LayerEnum.Bomber;
    public override Func<string> StartText => () => "Make People Go Boom";
    public override Func<string> Description => () => $"- You can place bombs which can be detonated at any time to kill anyone within a {BombRange}m radius\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.SyndicateKill;
        Bombs = [];
        BombButton = CreateButton(this, new SpriteName("Plant"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClick)Place, new Cooldown(BombCd), "PLACE BOMB",
            (ConditionFunc)Condition);
        DetonateButton = CreateButton(this, new SpriteName("Detonate"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)Detonate, new Cooldown(DetonateCd), (UsableFunc)Bombs.Any,
            "DETONATE");
        Data.Role.IntroSound = GetAudio("BomberIntro");
    }

    public override void OnLobby()
    {
        base.OnLobby();
        Bombs.ForEach(x => x.Destroy());
        Bombs.Clear();
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);

        if (BombsDetonateOnMeetingStart)
        {
            Bombs.ForEach(x => x.Detonate());
            Bombs.Clear();
        }
    }

    public void Place()
    {
        Bombs.Add(new(Player, HoldsDrive));
        BombButton.StartCooldown();

        if (BombCooldownsLinked)
            DetonateButton.StartCooldown();
    }

    public void Detonate()
    {
        Bombs.ForEach(x => x.Detonate());
        Bombs.Clear();
        DetonateButton.StartCooldown();

        if (BombCooldownsLinked)
            BombButton.StartCooldown();
    }

    public bool Condition() => !Bombs.Any(x => Vector2.Distance(Player.transform.position, x.Transform.position) < x.Size * 2);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        Bombs.ForEach(x => x.Update());
    }
}