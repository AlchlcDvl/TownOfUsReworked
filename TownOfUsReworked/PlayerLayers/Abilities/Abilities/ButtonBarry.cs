namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class ButtonBarry : Ability
    {
        public bool ButtonUsed { get; set; }
        public CustomButton ButtonButton { get; set; }
        public DateTime LastButtoned { get; set; }
        public bool ButtonUsable => !ButtonUsed && Player.RemainingEmergencies > 0;
        public float Timer => ButtonUtils.Timer(Player, LastButtoned, CustomGameOptions.ButtonCooldown);

        public override Color32 Color => ClientGameOptions.CustomAbColors ? Colors.ButtonBarry : Colors.Ability;
        public override string Name => "Button Barry";
        public override LayerEnum Type => LayerEnum.ButtonBarry;
        public override AbilityEnum AbilityType => AbilityEnum.ButtonBarry;
        public override Func<string> Description => () => "- You can call a button from anywhere";

        public ButtonBarry(PlayerControl player) : base(player) => ButtonButton = new(this, "Button", AbilityTypes.Effect, "Quarternary", Call);

        public void Call()
        {
            if (!ButtonUsable || Timer != 0f)
                return;

            ButtonUsed = true;
            CallRpc(CustomRPC.Action, ActionsRPC.BarryButton, Player);
            FixExtentions.Fix();

            if (AmongUsClient.Instance.AmHost)
            {
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
            ButtonButton.Update("BUTTON", Timer, CustomGameOptions.ButtonCooldown, true, ButtonUsable);
        }
    }
}