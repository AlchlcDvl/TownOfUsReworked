namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Banshee : Syndicate, IGhosty
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number ScreamCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number ScreamDur = 10;

    public CustomButton ScreamButton { get; set; }
    public List<byte> Blocked { get; } = [];
    public bool Caught { get; set; }
    public bool Faded { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Banshee : FactionColor;
    public override LayerEnum Type => LayerEnum.Banshee;
    public override Func<string> StartText => () => "AAAAAAAAAAAAAAAAAAAAAAAAA";
    public override Func<string> Description => () => "- You can scream loudly, blocking all players as long as you are not clicked";
    public override bool RoleBlockImmune => true; // Not taking chances

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Utility;
        Blocked.Clear();
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

    public override void UpdatePlayer() => (this as IGhosty).UpdateGhost();
}