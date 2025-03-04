namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public sealed class Banshee : Syndicate, IGhosty
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number ScreamCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    private static Number ScreamDur = 10;

    private CustomButton ScreamButton { get; set; }
    public List<byte> Blocked { get; } = [];
    public bool Caught { get; set; }
    public bool Faded { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Banshee : FactionColor;
    public override LayerEnum Type => LayerEnum.Banshee;
    public override Func<string> StartText => () => "AAAAAAAAAAAAAAAAAAAAAAAAA";
    public override Func<string> Description => () => "- You can scream loudly, blocking all players as long as you are not clicked";
    public override bool RoleBlockImmune => true; // Not taking chances

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Utility;
        Blocked.Clear();
        ScreamButton ??= new(this, new SpriteName("Scream"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)HitScream, new Cooldown(ScreamCd), new PostDeath(true),
            new Duration(ScreamDur), (EffectEndVoid)UnScream, "SCREAM", (UsableFunc)Usable, (EndFunc)EndEffect);
        Player.gameObject.layer = LayerMask.NameToLayer("Players");
    }

    private void UnScream()
    {
        BlockExposed = false;
        Blocked.Clear();
    }

    private void HitScream()
    {
        foreach (var player in AllPlayers())
        {
            if (!player.HasDied() && !player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate)
                Blocked.Add(player.PlayerId);
        }

        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ScreamButton);
        ScreamButton.Begin();
    }

    private bool Usable() => !Caught;

    private bool EndEffect() => Caught;

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
        base.UpdatePlayer();
        (this as IGhosty).UpdateGhost();
    }
}