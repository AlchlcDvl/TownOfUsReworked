namespace TownOfUsReworked.PlayerLayers.Roles;

public class Medic : Crew
{
    public PlayerControl ShieldedPlayer { get; set; }
    public bool ShieldBroken { get; set; }
    public CustomButton ShieldButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomCrewColors ? CustomColorManager.Medic : CustomColorManager.Crew;
    public override string Name => "Medic";
    public override LayerEnum Type => LayerEnum.Medic;
    public override Func<string> StartText => () => "Shield A Player To Protect Them";
    public override Func<string> Description => () => "- You can shield a player to give them Powerful defense" + (CustomGameOptions.NotificationShield is ShieldOptions.Everyone or
        ShieldOptions.Medic or ShieldOptions.SelfAndMedic ? "\n- If your target is attacked, you will be notified of it" : "");

    public override void Init()
    {
        BaseStart();
        ShieldedPlayer = null;
        Alignment = Alignment.CrewProt;
        ShieldButton = CreateButton(this, "SHIELD", new SpriteName("Shield"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Protect, (PlayerBodyExclusion)Exception,
            (UsableFunc)Usable);
        Data.Role.IntroSound = GetAudio("MedicIntro");
    }

    public void Protect()
    {
        if (Interact(Player, ShieldButton.TargetPlayer) != CooldownType.Fail)
        {
            if (ShieldedPlayer)
            {
                ShieldedPlayer = null;
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, MedicActionsRPC.Remove);
            }
            else
            {
                ShieldedPlayer = ShieldButton.TargetPlayer;
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, MedicActionsRPC.Add, ShieldedPlayer);
            }
        }
    }

    public bool Exception(PlayerControl player)
    {
        if (ShieldedPlayer)
            return ShieldedPlayer != player;
        else
            return (player.Is(LayerEnum.Mayor) && player.GetRole<Mayor>().Revealed) || (player.Is(LayerEnum.Dictator) && player.GetRole<Dictator>().Revealed);
    }

    public bool Usable() => !ShieldBroken;

    public override void ReadRPC(MessageReader reader) => ShieldedPlayer = (MedicActionsRPC)reader.ReadByte() == MedicActionsRPC.Add ? reader.ReadPlayer() : null;
}