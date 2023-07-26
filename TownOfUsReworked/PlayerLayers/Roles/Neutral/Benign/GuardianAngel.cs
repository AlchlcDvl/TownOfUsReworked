namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class GuardianAngel : Neutral
    {
        public bool Enabled;
        public DateTime LastProtected;
        public float TimeRemaining;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public PlayerControl TargetPlayer;
        public bool TargetAlive => !Disconnected && !TargetPlayer.Data.IsDead && !TargetPlayer.Data.Disconnected;
        public bool Protecting => TimeRemaining > 0f;
        public CustomButton ProtectButton;
        public CustomButton GraveProtectButton;
        public int Rounds;
        public CustomButton TargetButton;
        public bool Failed => TargetPlayer == null && Rounds > 2;

        public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.GuardianAngel : Colors.Neutral;
        public override string Name => "Guardian Angel";
        public override LayerEnum Type => LayerEnum.GuardianAngel;
        public override RoleEnum RoleType => RoleEnum.GuardianAngel;
        public override Func<string> StartText => () => "Find Someone To Protect";
        public override Func<string> AbilitiesText => () => TargetPlayer == null ? "- You can select a player to be your target" : ($"- You can protect {TargetPlayer?.name} from death for "
            + $"a short while\n- If {TargetPlayer?.name} dies, you will become a <color=#DDDD00FF>Survivor</color>");
        public override InspectorResults InspectorResults => InspectorResults.PreservesLife;

        public GuardianAngel(PlayerControl player) : base(player)
        {
            Objectives = () => TargetPlayer == null ? "- Find a target to protect" : $"- Have {TargetPlayer?.name} live to the end of the game";
            UsesLeft = CustomGameOptions.MaxProtects;
            RoleAlignment = RoleAlignment.NeutralBen;
            TargetPlayer = null;
            ProtectButton = new(this, "Protect", AbilityTypes.Effect, "ActionSecondary", HitProtect, true);
            TargetButton = new(this, "GATarget", AbilityTypes.Direct, "ActionSecondary", SelectTarget);
            Rounds = 0;

            if (CustomGameOptions.ProtectBeyondTheGrave)
                GraveProtectButton = new(this, "Protect", AbilityTypes.Effect, "ActionSecondary", HitProtect, true, true);
        }

        public void SelectTarget()
        {
            if (TargetPlayer != null)
                return;

            TargetPlayer = TargetButton.TargetPlayer;
            CallRpc(CustomRPC.Target, TargetRPC.SetGATarget, this, TargetPlayer);
        }

        public float ProtectTimer()
        {
            var timespan = DateTime.UtcNow - LastProtected;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ProtectCd) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Protect()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (Meeting)
                TimeRemaining = 0f;
        }

        public void TurnSurv()
        {
            var newRole = new Survivor(Player) { UsesLeft = UsesLeft };
            newRole.RoleUpdate(this);

            if (Local)
                Flash(Colors.Survivor);

            if (CustomPlayer.Local.Is(RoleEnum.Seer))
                Flash(Colors.Seer);
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
            CallRpc(CustomRPC.Action, ActionsRPC.GAProtect, this);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            ProtectButton.Update("PROTECT", ProtectTimer(), CustomGameOptions.ProtectCd, UsesLeft, Protecting, TimeRemaining, CustomGameOptions.ProtectDuration, true, !Failed &&
                TargetPlayer != null && TargetAlive);
            TargetButton.Update("WATCH", true, TargetPlayer == null);

            if (CustomGameOptions.ProtectBeyondTheGrave)
            {
                GraveProtectButton.Update("PROTECT", ProtectTimer(), CustomGameOptions.ProtectCd, UsesLeft, Protecting, TimeRemaining, CustomGameOptions.ProtectDuration, true, IsDead &&
                    !Failed && TargetPlayer != null && TargetAlive);
            }

            if ((Failed || (TargetPlayer != null && !TargetAlive)) && !IsDead)
            {
                CallRpc(CustomRPC.Change, TurnRPC.TurnSurv, this);
                TurnSurv();
            }
        }
    }
}