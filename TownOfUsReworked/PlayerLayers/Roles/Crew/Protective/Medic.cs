namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Medic)]
public sealed class Medic : Crew, IShielder
{
    [MultiSelectOption<ShieldOptions>(ShieldOptions.Nobody, ShieldOptions.Everyone)]
    public static MultiSelectValue<ShieldOptions> ShowShielded = ShieldOptions.Medic;

    [MultiSelectOption<ShieldOptions>(ShieldOptions.Nobody, ShieldOptions.Everyone)]
    public static MultiSelectValue<ShieldOptions> WhoGetsNotification = ShieldOptions.Medic;

    [ToggleOption]
    public static bool ShieldBreaks = true;

    public PlayerControl ShieldedPlayer { get; set; }
    public bool ShieldBroken { get; set; }
    private CustomButton ShieldButton { get; set; }

    public override UColor MainColor => CustomColorManager.Medic;
    public override LayerEnum Type => LayerEnum.Medic;
    public override Func<string> StartText => () => "Shield A Player To Protect Them";
    public override Func<string> Description => () => "- You can shield a player to give them Powerful defense" + (WhoGetsNotification.ContainsAny(ShieldOptions.Medic, ShieldOptions.Everyone) ?
        "\n- If your target is attacked, you will be notified of it" : "");

    protected override void Init()
    {
        base.Init();
        ShieldedPlayer = null;
        Alignment = Alignment.Protective;
        ShieldButton ??= new(this, "SHIELD", new SpriteName("Shield"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Protect, (PlayerBodyExclusion)Exception,
            (UsableFunc)Usable);
    }

    private void Protect(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            ShieldedPlayer = ShieldedPlayer ? null : target;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, ShieldedPlayer?.PlayerId ?? 255);
        }

        ShieldButton.StartCooldown(cooldown);
    }

    private bool Exception(PlayerControl player)
    {
        if (ShieldedPlayer)
            return ShieldedPlayer != player;

        return player.Is<IRevealer>(out var irev) && irev.Revealed;
    }

    private bool Usable() => !ShieldBroken;

    public override void ReadRPC(MessageReader reader) => ShieldedPlayer = reader.ReadPlayer();

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (ShieldedPlayer == player && ShowShielded.Contains(ShieldOptions.Medic))
            name += " <#006600FF>✚</color>";
    }
}