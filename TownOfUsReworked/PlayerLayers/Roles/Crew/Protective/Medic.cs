namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Medic : Crew, IShielder
{
    [StringOption(MultiMenu.LayerSubOptions)]
    public static ShieldOptions ShowShielded { get; set; } = ShieldOptions.Medic;

    [StringOption(MultiMenu.LayerSubOptions)]
    public static ShieldOptions WhoGetsNotification { get; set; } = ShieldOptions.Medic;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ShieldBreaks { get; set; } = true;

    public PlayerControl ShieldedPlayer { get; set; }
    public bool ShieldBroken { get; set; }
    public CustomButton ShieldButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Medic: FactionColor;
    public override string Name => "Medic";
    public override LayerEnum Type => LayerEnum.Medic;
    public override Func<string> StartText => () => "Shield A Player To Protect Them";
    public override Func<string> Description => () => "- You can shield a player to give them Powerful defense" + (WhoGetsNotification is ShieldOptions.Everyone or ShieldOptions.Medic or
        ShieldOptions.ShieldedAndMedic ? "\n- If your target is attacked, you will be notified of it" : "");

    public override void Init()
    {
        base.Init();
        ShieldedPlayer = null;
        Alignment = Alignment.CrewProt;
        ShieldButton ??= new(this, "SHIELD", new SpriteName("Shield"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClickPlayer)Protect, (PlayerBodyExclusion)Exception,
            (UsableFunc)Usable);
    }

    public void Protect(PlayerControl target)
    {
        if (Interact(Player, target) != CooldownType.Fail)
        {
            if (ShieldedPlayer)
            {
                ShieldedPlayer = null;
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, MedicActionsRPC.Remove);
            }
            else
            {
                ShieldedPlayer = target;
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, MedicActionsRPC.Add, target);
            }
        }
    }

    public bool Exception(PlayerControl player)
    {
        if (ShieldedPlayer)
            return ShieldedPlayer != player;
        else
            return (player.TryGetLayer<Mayor>(out var mayor) && mayor.Revealed) || (player.TryGetLayer<Dictator>(out var dictator) && dictator.Revealed);
    }

    public bool Usable() => !ShieldBroken;

    public override void ReadRPC(MessageReader reader) => ShieldedPlayer = reader.ReadEnum<MedicActionsRPC>() == MedicActionsRPC.Add ? reader.ReadPlayer() : null;
}