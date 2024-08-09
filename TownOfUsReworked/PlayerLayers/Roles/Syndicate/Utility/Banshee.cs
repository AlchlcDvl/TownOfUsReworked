namespace TownOfUsReworked.PlayerLayers.Roles;

public class Banshee : Syndicate
{
    public CustomButton ScreamButton { get; set; }
    public List<byte> Blocked { get; set; }
    public bool Caught { get; set; }
    public bool Faded { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Banshee : CustomColorManager.Syndicate;
    public override string Name => "Banshee";
    public override LayerEnum Type => LayerEnum.Banshee;
    public override Func<string> StartText => () => "AAAAAAAAAAAAAAAAAAAAAAAAA";
    public override Func<string> Description => () => "- You can scream loudly, blocking all players as long as you are not clicked";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.SyndicateUtil;
        Blocked = [];
        RoleBlockImmune = true; // Not taking chances
        ScreamButton = CreateButton(this, new SpriteName("Scream"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClick)HitScream, new Cooldown(CustomGameOptions.ScreamCd),
            new Duration(CustomGameOptions.ScreamDur), (EffectVoid)Scream, (EffectEndVoid)UnScream, new PostDeath(true), "SCREAM", (UsableFunc)Usable, (EndFunc)EndEffect);
    }

    public void Scream() => Blocked.ForEach(y => PlayerById(y).GetLayers().ForEach(x => x.IsBlocked = !PlayerById(y).GetRole().RoleBlockImmune));

    public void UnScream()
    {
        Blocked.ForEach(y => PlayerById(y).GetLayers().ForEach(x => x.IsBlocked = false));
        Blocked.Clear();
    }

    public void Fade()
    {
        if (Disconnected)
            return;

        Faded = true;
        var color = new UColor(1f, 1f, 1f, 0f);

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

        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ScreamButton);
        ScreamButton.Begin();
    }

    public bool Usable() => !Caught;

    public bool EndEffect() => Caught;

    public override void ReadRPC(MessageReader reader)
    {
        foreach (var player in CustomPlayer.AllPlayers)
        {
            if (!player.HasDied() && !player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate)
                Blocked.Add(player.PlayerId);
        }
    }
}