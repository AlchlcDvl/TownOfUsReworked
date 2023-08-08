namespace TownOfUsReworked.PlayerLayers.Roles;

public class Banshee : Syndicate
{
    public CustomButton ScreamButton { get; set; }
    public bool Enabled { get; set; }
    public DateTime LastScreamed { get; set; }
    public float TimeRemaining { get; set; }
    public bool Screaming => TimeRemaining > 0f;
    public List<byte> Blocked { get; set; }
    public bool Caught { get; set; }
    public bool Faded { get; set; }

    public override Color32 Color => ClientGameOptions.CustomSynColors ? Colors.Banshee : Colors.Syndicate;
    public override string Name => "Banshee";
    public override LayerEnum Type => LayerEnum.Banshee;
    public override Func<string> StartText => () => "AAAAAAAAAAAAAAAAAAAAAAAAA";
    public override Func<string> Description => () => "- You can scream loudly, blocking all players as long as you are not clicked";
    public override InspectorResults InspectorResults => InspectorResults.Ghostly;
    public float Timer => ButtonUtils.Timer(Player, LastScreamed, CustomGameOptions.ScreamCooldown, true);

    public Banshee(PlayerControl player) : base(player)
    {
        RoleAlignment = RoleAlignment.SyndicateUtil;
        Blocked = new();
        RoleBlockImmune = true; //Not taking chances
        ScreamButton = new(this, "Scream", AbilityTypes.Effect, "ActionSecondary", HitScream);
    }

    public void Scream()
    {
        if (!Enabled)
        {
            foreach (var player8 in CustomPlayer.AllPlayers)
            {
                if (!player8.Data.IsDead && !player8.Data.Disconnected && !player8.Is(Faction.Syndicate))
                    Blocked.Add(player8.PlayerId);
            }
        }

        Enabled = true;
        TimeRemaining -= Time.deltaTime;

        foreach (var id in Blocked)
        {
            var player = PlayerById(id);

            foreach (var layer in GetLayers(player))
                layer.IsBlocked = !GetRole(player).RoleBlockImmune;
        }

        if (Meeting || Caught)
            TimeRemaining = 0f;
    }

    public void UnScream()
    {
        Enabled = false;
        LastScreamed = DateTime.UtcNow;

        foreach (var id in Blocked)
        {
            var player = PlayerById(id);

            if (player?.Data.Disconnected == true)
                continue;

            foreach (var layer in GetLayers(player))
                layer.IsBlocked = false;
        }

        Blocked.Clear();
    }

    public void Fade()
    {
        Faded = true;
        Player.Visible = true;
        var color = new Color(1f, 1f, 1f, 0f);

        var maxDistance = ShipStatus.Instance.MaxLightRadius * TownOfUsReworked.NormalOptions.CrewLightMod;
        var distance = (CustomPlayer.Local.GetTruePosition() - Player.GetTruePosition()).magnitude;

        var distPercent = distance / maxDistance;
        distPercent = Mathf.Max(0, distPercent - 1);

        var velocity = Player.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
        color.a = 0.07f + (velocity / Player.MyPhysics.TrueSpeed * 0.13f);
        color.a = Mathf.Lerp(color.a, 0, distPercent);

        if (Player.GetCustomOutfitType() != CustomPlayerOutfitType.PlayerNameOnly)
        {
            Player.SetOutfit(CustomPlayerOutfitType.PlayerNameOnly, new GameData.PlayerOutfit()
            {
                ColorId = Player.GetDefaultOutfit().ColorId,
                HatId = "",
                SkinId = "",
                VisorId = "",
                PlayerName = ""
            });
        }

        Player.MyRend().color = color;
        Player.NameText().color = new(0f, 0f, 0f, 0f);
        Player.cosmetics.colorBlindText.color = new(0f, 0f, 0f, 0f);
    }

    public void HitScream()
    {
        if (Timer != 0f)
            return;

        TimeRemaining = CustomGameOptions.ScreamDuration;
        Scream();
        CallRpc(CustomRPC.Action, ActionsRPC.Scream, this);

        foreach (var player in CustomPlayer.AllPlayers)
        {
            if (!player.Data.IsDead && !player.Data.Disconnected && !player.Is(Faction.Syndicate))
                Blocked.Add(player.PlayerId);
        }
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        ScreamButton.Update("SCREAM", Timer, CustomGameOptions.ScreamCooldown, Screaming, TimeRemaining, CustomGameOptions.ScreamDuration, true, !Caught);
    }
}