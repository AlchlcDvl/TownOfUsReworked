namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Banshee)]
public sealed class Banshee : Syndicate, IGhosty
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number ScreamCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    private static Number ScreamDur = 10;

    private CustomButton ScreamButton { get; set; }
    public HashSet<byte> Blocked { get; } = [];
    public bool Caught { get; set; }
    public Vector3 LastPosition { get; set; }

    protected override UColor MainColor => CustomColorManager.Banshee;
    public override LayerEnum Type => LayerEnum.Banshee;
    public override Func<string> StartText { get; } = () => "AAAAAAAAAAAAAAAAAAAAAAAAA";
    public override Func<string> Description => () => "- You can scream loudly, blocking all players as long as you are not clicked";
    public override bool RoleBlockImmune => true; // Not taking chances

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Utility;
        Blocked.Clear();
        ScreamButton ??= new(this, new SpriteName("Scream"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)HitScream, new Cooldown(ScreamCd), new PostDeath(true),
            new Duration(ScreamDur), (EffectEndVoid)UnScream, "SCREAM", (UsableFunc)Usable, (EndFunc)EndEffect, (EffectStartVoid)StartScream);
        Player.gameObject.layer = LayerMask.NameToLayer("Players");
    }

    public override void BeforeMeeting()
    {
        if (!UninteractablePlayers.ContainsKey(PlayerId))
            LastPosition = Player.transform.position;
    }

    private void StartScream()
    {
        foreach (var player in AllPlayers())
        {
            if (!player.HasDied() && !Player.IsBuddyWith(player, Faction))
                Blocked.Add(player.PlayerId);
        }
    }

    private void UnScream()
    {
        BlockExposed = false;
        Blocked.Clear();
    }

    private void HitScream()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ScreamButton);
        ScreamButton.Begin();
    }

    private bool Usable() => !Caught;

    private bool EndEffect() => Caught;

    public bool CanBeClicked(PlayerControl clicker) => !clicker.Is(Faction);
}