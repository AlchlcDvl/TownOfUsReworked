namespace TownOfUsReworked.PlayerLayers.Roles;

public class Bomber : Syndicate
{
    public CustomButton BombButton { get; set; }
    public CustomButton DetonateButton { get; set; }
    public List<Bomb> Bombs { get; set; }

    public override Color Color => ClientGameOptions.CustomSynColors ? Colors.Bomber : Colors.Syndicate;
    public override string Name => "Bomber";
    public override LayerEnum Type => LayerEnum.Bomber;
    public override Func<string> StartText => () => "Make People Go Boom";
    public override Func<string> Description => () => $"- You can place bombs which can be detonated at any time to kill anyone within a {CustomGameOptions.BombRange}m radius\n" +
        CommonAbilities;

    public Bomber(PlayerControl player) : base(player)
    {
        Alignment = Alignment.SyndicateKill;
        Bombs = new();
        BombButton = new(this, "Plant", AbilityTypes.Targetless, "ActionSecondary", Place, CustomGameOptions.BombCd);
        DetonateButton = new(this, "Detonate", AbilityTypes.Targetless, "Secondary", Detonate, CustomGameOptions.DetonateCd);
        player.Data.Role.IntroSound = GetAudio("BomberIntro");
    }

    public override void OnLobby()
    {
        base.OnLobby();
        Bomb.Clear(Bombs);
        Bombs.Clear();
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);

        if (CustomGameOptions.BombsDetonateOnMeetingStart)
            Bomb.DetonateBombs(Bombs);
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
        Bomb.DetonateBombs(Bombs);
        DetonateButton.StartCooldown();

        if (CustomGameOptions.BombCooldownsLinked)
            BombButton.StartCooldown();
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        BombButton.Update2("PLACE BOMB", condition: !Bombs.Any(x => Vector2.Distance(Player.transform.position, x.Transform.position) < x.Size * 2));
        DetonateButton.Update2("DETONATE", Bombs.Count > 0);
    }
}