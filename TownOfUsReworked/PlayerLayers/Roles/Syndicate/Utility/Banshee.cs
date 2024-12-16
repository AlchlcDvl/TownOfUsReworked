namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Banshee : Syndicate
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number ScreamCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static Number ScreamDur { get; set; } = new(10);

    public CustomButton ScreamButton { get; set; }
    public List<byte> Blocked { get; set; }
    public bool Caught { get; set; }
    public bool Faded { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Banshee: FactionColor;
    public override string Name => "Banshee";
    public override LayerEnum Type => LayerEnum.Banshee;
    public override Func<string> StartText => () => "AAAAAAAAAAAAAAAAAAAAAAAAA";
    public override Func<string> Description => () => "- You can scream loudly, blocking all players as long as you are not clicked";
    public override bool RoleBlockImmune => true; // Not taking chances

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.SyndicateUtil;
        Blocked = [];
        ScreamButton ??= new(this, new SpriteName("Scream"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)HitScream, new Cooldown(ScreamCd), new PostDeath(true),
            new Duration(ScreamDur), (EffectVoid)Scream, (EffectEndVoid)UnScream, "SCREAM", (UsableFunc)Usable, (EndFunc)EndEffect);
    }

    public void Scream() => Blocked.ForEach(y => PlayerById(y).GetLayers().ForEach(x => x.IsBlocked = !PlayerById(y).GetRole().RoleBlockImmune));

    public void UnScream()
    {
        foreach (var id in Blocked)
        {
            var blocked = PlayerById(id);
            blocked.GetLayers().ForEach(x => x.IsBlocked = false);
            blocked.GetButtons().ForEach(x => x.BlockExposed = false);

            if (blocked.AmOwner)
                Patches.Blocked.BlockExposed = false;
        }

        Blocked.Clear();
    }

    public void Fade()
    {
        if (Disconnected)
            return;

        Faded = true;
        var color = new UColor(1f, 1f, 1f, 0f);

        var maxDistance = Ship().MaxLightRadius * TownOfUsReworked.NormalOptions.CrewLightMod;
        var distance = (CustomPlayer.Local.GetTruePosition() - Player.GetTruePosition()).magnitude;

        var distPercent = distance / maxDistance;
        distPercent = Mathf.Max(0, distPercent - 1);

        var velocity = Player.GetComponent<Rigidbody2D>().velocity.magnitude;
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
        foreach (var player in AllPlayers())
        {
            if (!player.HasDied() && !player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate)
                Blocked.Add(player.PlayerId);
        }

        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ScreamButton);
        ScreamButton.Begin();
    }

    public bool Usable() => !Caught;

    public bool EndEffect() => Caught;

    public override void ReadRPC(MessageReader reader)
    {
        foreach (var player in AllPlayers())
        {
            if (!player.HasDied() && !player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate)
                Blocked.Add(player.PlayerId);
        }
    }

    public override void UpdatePlayer()
    {
        if (!Caught)
            Fade();
        else if (Faded)
            UnFade();
    }
}