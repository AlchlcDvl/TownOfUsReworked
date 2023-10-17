namespace TownOfUsReworked.PlayerLayers.Abilities;

public class ButtonBarry : Ability
{
    public bool ButtonUsed { get; set; }
    public CustomButton ButtonButton { get; set; }

    public override Color Color => ClientGameOptions.CustomAbColors ? Colors.ButtonBarry : Colors.Ability;
    public override string Name => "Button Barry";
    public override LayerEnum Type => LayerEnum.ButtonBarry;
    public override Func<string> Description => () => "- You can call a button from anywhere";

    public ButtonBarry(PlayerControl player) : base(player) => ButtonButton = new(this, "Button", AbilityTypes.Targetless, "Quarternary", Call, CustomGameOptions.ButtonCooldown);

    public void Call()
    {
        ButtonUsed = true;
        CallRpc(CustomRPC.Action, ActionsRPC.BarryButton, Player);
        FixExtentions.Fix();

        if (AmongUsClient.Instance.AmHost)
        {
            Player.RemainingEmergencies++;
            MeetingRoomManager.Instance.reporter = Player;
            MeetingRoomManager.Instance.target = null;
            AmongUsClient.Instance.DisconnectHandlers.AddUnique(MeetingRoomManager.Instance.Cast<IDisconnectHandler>());
            HUD.OpenMeetingRoom(Player);
            Player.RpcStartMeeting(null);
        }
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        ButtonButton.Update2("BUTTON", !ButtonUsed);
    }
}