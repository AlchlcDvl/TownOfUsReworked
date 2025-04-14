namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Void)]
public sealed class Void : Deity
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number BanishCd = 25;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number ExtractCd = 25;

    [ToggleOption]
    private static bool VoidVent = true;

    private CustomButton ExtractButton { get; set; }
    private CustomButton BanishButton { get; set; }

    public static readonly HashSet<byte> ToBeExtracted = [];

    protected override UColor MainColor => CustomColorManager.Void;
    public override LayerEnum Type { get; } = LayerEnum.Void;
    public override Func<string> Description => () => "- Anyone who either you interact with or interacts with you will lose their role\n- Your victim's bodies cannot be reported" +
        CommonAbilities;
    public override bool CanVent => base.CanVent && VoidVent;

    protected override void Init()
    {
        base.Init();
        ExtractButton ??= new(this, new SpriteName("Extract"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Extract, (PlayerBodyExclusion)Exception, "EXTRACT",
            new Cooldown(ExtractCd));
        BanishButton ??= new(this, new SpriteName("Banish"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Banish, (PlayerBodyExclusion)Exception, "BANISH",
            new Cooldown(BanishCd));
    }

    public void Banish(PlayerControl target) => BanishButton.StartCooldown(Interact(Player, target, true));

    public void Extract(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            ToBeExtracted.Add(target.PlayerId);
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, target.PlayerId);
        }

        ExtractButton.StartCooldown(cooldown);
    }

    private bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);

    public override void ReadRPC(NetData reader) => ToBeExtracted.Add(reader.ReadByte());
}