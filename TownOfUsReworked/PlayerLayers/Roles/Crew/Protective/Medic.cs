namespace TownOfUsReworked.PlayerLayers.Roles;

public class Medic : Crew
{
    public PlayerControl ShieldedPlayer { get; set; }
    public PlayerControl ExShielded { get; set; }
    public CustomButton ShieldButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomCrewColors ? CustomColorManager.Medic : CustomColorManager.Crew;
    public override string Name => "Medic";
    public override LayerEnum Type => LayerEnum.Medic;
    public override Func<string> StartText => () => "Shield A Player To Protect Them";
    public override Func<string> Description => () => "- You can shield a player to give them Powerful defense" + (CustomGameOptions.NotificationShield is ShieldOptions.Everyone or
        ShieldOptions.Medic or ShieldOptions.SelfAndMedic ? "\n- If your target is attacked, you will be notified of it" : "");

    public Medic() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        ShieldedPlayer = null;
        ExShielded = null;
        Alignment = Alignment.CrewProt;
        ShieldButton = new(this, "Shield", AbilityTypes.Alive, "ActionSecondary", Protect, Exception);
        Data.Role.IntroSound = GetAudio("MedicIntro");
        return this;
    }

    public void Protect()
    {
        if (Interact(Player, ShieldButton.TargetPlayer) != CooldownType.Fail)
        {
            if (ShieldedPlayer == null)
            {
                ShieldedPlayer = ShieldButton.TargetPlayer;
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, MedicActionsRPC.Add, ShieldedPlayer);
            }
            else
            {
                ShieldedPlayer = null;
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, MedicActionsRPC.Remove);
            }
        }
    }

    public bool Exception(PlayerControl player)
    {
        if (ShieldedPlayer == null)
            return (player.Is(LayerEnum.Mayor) && player.GetRole<Mayor>().Revealed) || (player.Is(LayerEnum.Dictator) && player.GetRole<Dictator>().Revealed);
        else
            return ShieldedPlayer != player;
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        ShieldButton.Update2("SHIELD", ExShielded == null);
    }

    public override void ReadRPC(MessageReader reader) => ShieldedPlayer = (MedicActionsRPC)reader.ReadByte() == MedicActionsRPC.Add ? reader.ReadPlayer() : null;
}