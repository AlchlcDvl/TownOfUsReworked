namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public sealed class Democrat : Role, IRevealer
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number CampaignCd = 25;

    [ToggleOption]
    public static bool RoundOneNoCampaigning = false;

    [ToggleOption]
    public static bool DemocratButton = true;

    public List<byte> Campaigned { get; } = [];
    private CustomButton RevealButton { get; set; }
    private CustomButton CampaignButton { get; set; }
    public bool Revealed { get; set; }
    public bool RoundOne { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Democrat : FactionColor;
    public override LayerEnum Type => LayerEnum.Democrat;
    public override Func<string> StartText => () => "Start A Campaign To Get Elected!";
    public override Func<string> Description => () => "- You can plead your cause to players, campaigning them for their vote\n- When everyone is campaigned, you can reveal yourself to be the " +
        "<#704FA8FF>Mayor</color>";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Sovereign;
        RevealButton ??= new(this, new SpriteName("MayorReveal"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Reveal, (UsableFunc)Usable, "REVEAL");
        CampaignButton ??= new(this, "CAMPAIGN", new SpriteName("Campaign"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Campaign, new Cooldown(CampaignCd),
            (UsableFunc)Usable, (PlayerBodyExclusion)Exception);
    }

    public override void Reset(bool meeting, bool start) => RoundOne = start && RoundOneNoCampaigning;

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (Campaigned.Contains(player.PlayerId))
            name += " <#1A3270FF>°</color>";
    }

    private void Reveal()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.PublicReveal, Player);
        PublicReveal(Player);
    }

    private void Campaign(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            Campaigned.Add(target.PlayerId);
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, target.PlayerId);
        }

        CampaignButton.StartCooldown(cooldown);
    }

    private bool Usable() => !RoundOne && !GetLayers<Mayor>().Any(x => !x.TrulyDead && x.Revealed) && AllPlayers().Count(x => !x.HasDied()) <= Campaigned.Count;

    public void OnReveal() => new Mayor().RoleUpdate(this);

    private bool Exception(PlayerControl player) => Campaigned.Contains(player.PlayerId);

    public override void ReadRPC(MessageReader reader) => Campaigned.Add(reader.ReadByte());

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        Campaigned.RemoveAll(x => PlayerById(x).HasDied());
    }
}