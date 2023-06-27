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

        public GuardianAngel(PlayerControl player) : base(player)
        {
            Name = "Guardian Angel";
            StartText = () => $"Protect {TargetPlayer?.name} With Your Life";
            Objectives = () => TargetPlayer == null ? "- Find a target to protect" : $"- Have {TargetPlayer?.name} live to the end of the game";
            Color = CustomGameOptions.CustomNeutColors ? Colors.GuardianAngel : Colors.Neutral;
            RoleType = RoleEnum.GuardianAngel;
            UsesLeft = CustomGameOptions.MaxProtects;
            RoleAlignment = RoleAlignment.NeutralBen;
            AbilitiesText = () => "- You can protect your target from death for a short while\n- If your target dies, you will be a <color=#DDDD00FF>Survivor</color>";
            InspectorResults = InspectorResults.PreservesLife;
            Type = LayerEnum.GuardianAngel;
            TargetPlayer = null;
            ProtectButton = new(this, "Protect", AbilityTypes.Effect, "ActionSecondary", HitProtect, true);
            TargetButton = new(this, "GATarget", AbilityTypes.Direct, "ActionSecondary", SelectTarget);
            Rounds = 0;

            if (CustomGameOptions.ProtectBeyondTheGrave)
                GraveProtectButton = new(this, "Protect", AbilityTypes.Effect, "ActionSecondary", HitProtect, true, true);

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public void SelectTarget()
        {
            if (TargetPlayer != null)
                return;

            TargetPlayer = TargetButton.TargetPlayer;
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Target, SendOption.Reliable);
            writer.Write((byte)TargetRPC.SetGATarget);
            writer.Write(PlayerId);
            writer.Write(TargetPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public float ProtectTimer()
        {
            var timespan = DateTime.UtcNow - LastProtected;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ProtectCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Protect()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (Utils.Meeting)
                TimeRemaining = 0f;
        }

        public void TurnSurv()
        {
            var newRole = new Survivor(Player) { UsesLeft = UsesLeft };
            newRole.RoleUpdate(this);

            if (Local && !IntroCutscene.Instance)
                Utils.Flash(Colors.Survivor);

            if (CustomPlayer.Local.Is(RoleEnum.Seer) && !IntroCutscene.Instance)
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
            ProtectButton.Update("PROTECT", ProtectTimer(), CustomGameOptions.ProtectCd, UsesLeft, Protecting, TimeRemaining, CustomGameOptions.ProtectDuration, true, !Failed &&
                TargetPlayer != null && TargetAlive);
            TargetButton.Update("SET TARGET", 0, 1, true, TargetPlayer == null);

            if (CustomGameOptions.ProtectBeyondTheGrave)
            {
                GraveProtectButton.Update("PROTECT", ProtectTimer(), CustomGameOptions.ProtectCd, UsesLeft, Protecting, TimeRemaining, CustomGameOptions.ProtectDuration, true, IsDead &&
                    !Failed && TargetPlayer != null && TargetAlive);
            }

            if ((Failed || (TargetPlayer != null && !TargetAlive)) && !IsDead)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnSurv);
                writer.Write(PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TurnSurv();
            }
        }
    }
}