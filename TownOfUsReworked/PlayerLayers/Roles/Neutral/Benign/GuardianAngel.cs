namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class GuardianAngel : NeutralRole
    {
        public bool Enabled;
        public DateTime LastProtected;
        public float TimeRemaining;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public PlayerControl TargetPlayer;
        public bool TargetAlive => Player != null && TargetPlayer != null && !Disconnected && !TargetPlayer.Data.IsDead && !TargetPlayer.Data.Disconnected;
        public bool Protecting => TimeRemaining > 0f;
        public CustomButton ProtectButton;
        public CustomButton GraveProtectButton;

        public GuardianAngel(PlayerControl player) : base(player)
        {
            Name = "Guardian Angel";
            StartText = "Protect Your Target With Your Life";
            Objectives = "- Have your target live to the end of the game";
            Color = CustomGameOptions.CustomNeutColors ? Colors.GuardianAngel : Colors.Neutral;
            RoleType = RoleEnum.GuardianAngel;
            UsesLeft = CustomGameOptions.MaxProtects;
            RoleAlignment = RoleAlignment.NeutralBen;
            AlignmentName = NB;
            AbilitiesText = "- You can protect your target from death for a short while\n- If your target dies, you will be a <color=#DDDD00FF>Survivor</color>";
            InspectorResults = InspectorResults.PreservesLife;
            Type = LayerEnum.GuardianAngel;
            TargetPlayer = null;
            ProtectButton = new(this, "Protect", AbilityTypes.Effect, "ActionSecondary", HitProtect, true);

            if (CustomGameOptions.ProtectBeyondTheGrave)
                GraveProtectButton = new(this, "Protect", AbilityTypes.Effect, "ActionSecondary", HitProtect, true, true);
        }

        public float ProtectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastProtected;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ProtectCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Protect()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void TurnSurv()
        {
            var newRole = new Survivor(Player) { UsesLeft = UsesLeft };
            newRole.RoleUpdate(this);

            if (Player == PlayerControl.LocalPlayer)
                Utils.Flash(Colors.Survivor);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer);
        }

        public void UnProtect()
        {
            Enabled = false;
            LastProtected = DateTime.UtcNow;
        }

        public void HitProtect()
        {
            if (!ButtonUsable || ProtectTimer() != 0f || !TargetAlive || Protecting)
                return;

            TimeRemaining = CustomGameOptions.ProtectDuration;
            UsesLeft--;
            Protect();
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.GAProtect);
            writer.Write(PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            ProtectButton.Update("PROTECT", ProtectTimer(), CustomGameOptions.ProtectCd, UsesLeft, Protecting, TimeRemaining, CustomGameOptions.ProtectDuration, true, TargetAlive);

            if (CustomGameOptions.ProtectBeyondTheGrave)
            {
                GraveProtectButton.Update("PROTECT", ProtectTimer(), CustomGameOptions.ProtectCd, UsesLeft, Protecting, TimeRemaining, CustomGameOptions.ProtectDuration, true, IsDead &&
                    TargetAlive);
            }

            if (!TargetAlive && !IsDead)
            {
                TurnSurv();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnSurv);
                writer.Write(PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}