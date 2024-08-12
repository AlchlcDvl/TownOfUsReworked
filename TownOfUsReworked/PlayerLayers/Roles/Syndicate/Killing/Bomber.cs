namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Bomber : Syndicate
{
    public CustomButton BombButton { get; set; }
    public CustomButton DetonateButton { get; set; }
    public List<Bomb> Bombs { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Bomber : CustomColorManager.Syndicate;
    public override string Name => "Bomber";
    public override LayerEnum Type => LayerEnum.Bomber;
    public override Func<string> StartText => () => "Make People Go Boom";
    public override Func<string> Description => () => $"- You can place bombs which can be detonated at any time to kill anyone within a {CustomGameOptions.BombRange}m radius\n" +
        CommonAbilities;

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.SyndicateKill;
        Bombs = [];
        BombButton = CreateButton(this, new SpriteName("Plant"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClick)Place, new Cooldown(CustomGameOptions.BombCd), "PLACE BOMB",
            (ConditionFunc)Condition);
        DetonateButton = CreateButton(this, new SpriteName("Detonate"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)Detonate, new Cooldown(CustomGameOptions.DetonateCd),
            "DETONATE", (UsableFunc)Bombs.Any);
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

        if (CustomGameOptions.BombsDetonateOnMeetingStart)
        {
            Bombs.ForEach(x => x.Detonate());
            Bombs.Clear();
        }
    }

    public void Place()
    {
        Bombs.Add(new(Player, HoldsDrive));
        BombButton.StartCooldown();

        if (CustomGameOptions.BombCooldownsLinked)
            DetonateButton.StartCooldown();
    }

    public void Detonate()
    {
        Bombs.ForEach(x => x.Detonate());
        Bombs.Clear();
        DetonateButton.StartCooldown();

        if (CustomGameOptions.BombCooldownsLinked)
            BombButton.StartCooldown();
    }

    public bool Condition() => !Bombs.Any(x => Vector2.Distance(Player.transform.position, x.Transform.position) < x.Size * 2);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        Bombs.ForEach(x => x.Update());
    }
}