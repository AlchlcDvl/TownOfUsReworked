namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Democrat)]
public sealed class Democrat : Sovereign
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number CampaignCd = 25;

    [ToggleOption]
    private static bool RoundOneNoCampaigning = false;

    [ToggleOption]
    public static bool DemocratButton = true;

    public readonly HashSet<byte> Campaigned = [];

    private CustomButton RevealButton;
    private CustomButton CampaignButton;
    private bool RoundOne;

    protected override UColor MainColor => CustomColorManager.Democrat;
    public override Layer Type => Layer.Democrat;
    public override string StartText => "Start A Campaign To Get Elected!";
    public override string Description => "- You can plead your cause to players, campaigning them for their vote\n- When everyone is campaigned, you can reveal yourself to be the " +
        "<#704FA8FF>Mayor</color>";

    public override void Init()
    {
        Campaigned.Clear();
        RevealButton ??= new(this, new SpriteName("MayorReveal"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Reveal, (UsableFunc)Usable2, "REVEAL");
        CampaignButton ??= new(this, "CAMPAIGN", new SpriteName("Campaign"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Campaign, new Cooldown(CampaignCd),
            (UsableFunc)Usable1, (PlayerBodyExclusion)Exception);
    }

    public override void Reset(bool meeting, bool start) => RoundOne = start && RoundOneNoCampaigning;

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (Campaigned.Contains(player.PlayerId))
            name += " <#1A3270FF>°</color>";
    }

    private void Reveal()
    {
        CallRpc(ActionsRpc.PublicReveal, Player);
        PublicReveal(Player);
    }

    private void Campaign(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            Campaigned.Add(target.PlayerId);
            PerformRpcAction(target.PlayerId);
        }

        CampaignButton.StartCooldown(cooldown);
    }

    private bool Usable1() => !RoundOne && !GetLayers<Mayor>().Any(x => !x.TrulyDead && x.Revealed);

    private bool Usable2() => Usable1() && AllPlayers().Where(x => x.Is(Faction.Crew) && !x.HasDied()).All(x => Campaigned.Contains(x.PlayerId));

    public override void OnReveal() => new Mayor().RoleUpdate(this);

    private bool Exception(PlayerControl player) => Campaigned.Contains(player.PlayerId);

    public override void ReadRPC(RpcReader reader) => Campaigned.Add(reader.ReadByte());

    public override void OnMeetingStart(MeetingHud __instance) => Campaigned.RemoveAll(x => PlayerById(x).HasDied());
}