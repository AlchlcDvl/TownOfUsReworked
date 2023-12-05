namespace TownOfUsReworked.PlayerLayers.Roles;

public class Banshee : Syndicate
{
    public CustomButton ScreamButton { get; set; }
    public List<byte> Blocked { get; set; }
    public bool Caught { get; set; }
    public bool Faded { get; set; }

    public override Color Color => ClientGameOptions.CustomSynColors ? Colors.Banshee : Colors.Syndicate;
    public override string Name => "Banshee";
    public override LayerEnum Type => LayerEnum.Banshee;
    public override Func<string> StartText => () => "AAAAAAAAAAAAAAAAAAAAAAAAA";
    public override Func<string> Description => () => "- You can scream loudly, blocking all players as long as you are not clicked";

    public Banshee(PlayerControl player) : base(player)
    {
        Alignment = Alignment.SyndicateUtil;
        Blocked = new();
        RoleBlockImmune = true; //Not taking chances
        ScreamButton = new(this, "Scream", AbilityTypes.Targetless, "ActionSecondary", HitScream, CustomGameOptions.ScreamCd, CustomGameOptions.ScreamDur, (CustomButton.EffectVoid)Scream,
            UnScream, true);
    }

    public void Scream()
    {
        foreach (var id in Blocked)
        {
            var player = PlayerById(id);

            foreach (var layer in GetLayers(player))
                layer.IsBlocked = !GetRole(player).RoleBlockImmune;
        }
    }

    public void UnScream()
    {
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
        if (Disconnected)
            return;

        Faded = true;
        Player.Visible = true;
        var color = new Color(1f, 1f, 1f, 0f);

        var maxDistance = Ship.MaxLightRadius * TownOfUsReworked.NormalOptions.CrewLightMod;
        var distance = (CustomPlayer.Local.GetTruePosition() - Player.GetTruePosition()).magnitude;

        var distPercent = distance / maxDistance;
        distPercent = Mathf.Max(0, distPercent - 1);

        var velocity = Player.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
        color.a = 0.07f + (velocity / Player.MyPhysics.TrueSpeed * 0.13f);
        color.a = Mathf.Lerp(color.a, 0, distPercent);

        if (Player.GetCustomOutfitType() != CustomPlayerOutfitType.PlayerNameOnly)
            Player.SetOutfit(CustomPlayerOutfitType.PlayerNameOnly, BlankOutfit(Player));

        Player.MyRend().color = color;
        Player.NameText().color = new(0f, 0f, 0f, 0f);
        Player.cosmetics.colorBlindText.color = new(0f, 0f, 0f, 0f);

        if (Local)
            Camouflage();
    }

    public void UnFade()
    {
        Player.MyRend().color = UColor.white;
        Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
        Faded = false;
        Player.MyPhysics.ResetMoveState();

        if (Local)
            DefaultOutfitAll();
    }

    public void HitScream()
    {
        foreach (var player in CustomPlayer.AllPlayers)
        {
            if (!player.HasDied() && !player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate)
                Blocked.Add(player.PlayerId);
        }

        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, ScreamButton);
        ScreamButton.Begin();
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        KillButton.Disable();
        ScreamButton.Update2("SCREAM", !Caught);
    }

    public override void TryEndEffect() => ScreamButton.Update3(Caught);

    public override void ReadRPC(MessageReader reader)
    {
        foreach (var player in CustomPlayer.AllPlayers)
        {
            if (!player.HasDied() && !player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate)
                Blocked.Add(player.PlayerId);
        }
    }
}