namespace TownOfUsReworked.PlayerLayers.Roles;

public class Bomber : Syndicate
{
    public DateTime LastPlaced { get; set; }
    public DateTime LastDetonated { get; set; }
    public CustomButton BombButton { get; set; }
    public CustomButton DetonateButton { get; set; }
    public List<Bomb> Bombs { get; set; }

    public override Color32 Color => ClientGameOptions.CustomSynColors ? Colors.Bomber : Colors.Syndicate;
    public override string Name => "Bomber";
    public override LayerEnum Type => LayerEnum.Bomber;
    public override Func<string> StartText => () => "Make People Go Boom";
    public override Func<string> Description => () => $"- You can place bombs which you can detonate at any time to kill anyone within a {CustomGameOptions.BombRange}m radius\n" +
        CommonAbilities;
    public override InspectorResults InspectorResults => InspectorResults.DropsItems;
    public float BombTimer => ButtonUtils.Timer(Player, LastPlaced, CustomGameOptions.BombCd);
    public float DetonateTimer => ButtonUtils.Timer(Player, LastDetonated, CustomGameOptions.DetonateCd);

    public Bomber(PlayerControl player) : base(player)
    {
        RoleAlignment = RoleAlignment.SyndicateKill;
        Bombs = new();
        BombButton = new(this, "Plant", AbilityTypes.Effect, "ActionSecondary", Place);
        DetonateButton = new(this, "Detonate", AbilityTypes.Effect, "Secondary", Detonate);
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
        if (BombTimer != 0f)
            return;

        Bombs.Add(new(Player, HoldsDrive));
        LastPlaced = DateTime.UtcNow;

        if (CustomGameOptions.BombCooldownsLinked)
            LastDetonated = DateTime.UtcNow;
    }

    public void Detonate()
    {
        if (DetonateTimer != 0f || Bombs.Count == 0)
            return;

        Bomb.DetonateBombs(Bombs);
        LastDetonated = DateTime.UtcNow;

        if (CustomGameOptions.BombCooldownsLinked)
            LastPlaced = DateTime.UtcNow;
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        BombButton.Update("PLACE", BombTimer, CustomGameOptions.BombCd);
        DetonateButton.Update("DETONATE", DetonateTimer, CustomGameOptions.DetonateCd, true, Bombs.Count > 0);
    }
}