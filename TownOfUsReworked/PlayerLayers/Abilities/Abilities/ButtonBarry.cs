namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class ButtonBarry : Ability
    {
        public bool ButtonUsed;
        public CustomButton ButtonButton;
        public DateTime LastButtoned;
        public bool ButtonUsable => !ButtonUsed && Player.RemainingEmergencies > 0;

        public override Color32 Color => ClientGameOptions.CustomAbColors ? Colors.ButtonBarry : Colors.Ability;
        public override string Name => "Button Barry";
        public override LayerEnum Type => LayerEnum.ButtonBarry;
        public override AbilityEnum AbilityType => AbilityEnum.ButtonBarry;
        public override Func<string> TaskText => () => "- You can call a button from anywhere";

        public ButtonBarry(PlayerControl player) : base(player) => ButtonButton = new(this, "Button", AbilityTypes.Effect, "Quarternary", Call);

        public float StartTimer()
        {
            var timespan = DateTime.UtcNow - LastButtoned;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ButtonCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Call()
        {
            if (!ButtonUsable || StartTimer() != 0f)
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
            ButtonButton.Update("BUTTON", StartTimer(), CustomGameOptions.ButtonCooldown, true, ButtonUsable);
        }
    }
}