namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Escort : CrewRole
    {
        public PlayerControl BlockTarget;
        public bool Enabled;
        public DateTime LastBlock;
        public float TimeRemaining;
        public CustomButton BlockButton;
        public bool Blocking => TimeRemaining > 0f;

        public Escort(PlayerControl player) : base(player)
        {
            Name = "Escort";
            RoleType = RoleEnum.Escort;
            StartText = "Roleblock Players From Harming The <color=#8CFFFFFF>Crew</color>";
            AbilitiesText = "- You can seduce players\n- Seduction blocks your target from being able to use their abilities for a short while\n- You are immune to blocks\n" +
                "- If you attempt to block a <color=#336EFFFF>Serial Killer</color>, they will be forced to kill you";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Escort : Colors.Crew;
            RoleAlignment = RoleAlignment.CrewSupport;
            RoleBlockImmune = true;
            InspectorResults = InspectorResults.HindersOthers;
            Type = LayerEnum.Escort;
            BlockTarget = null;
            BlockButton = new(this, "EscortRoleblock", AbilityTypes.Direct, "ActionSecondary", Roleblock);
        }

        public void UnBlock()
        {
            Enabled = false;

            foreach (var layer in GetLayers(BlockTarget))
                layer.IsBlocked = false;

            BlockTarget = null;
            LastBlock = DateTime.UtcNow;
        }

        public void Block()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            foreach (var layer in GetLayers(BlockTarget))
                layer.IsBlocked = !GetRole(BlockTarget).RoleBlockImmune;

            if (MeetingHud.Instance || IsDead || BlockTarget.Data.IsDead || BlockTarget.Data.Disconnected)
                TimeRemaining = 0f;
        }

        public float RoleblockTimer()
        {
            var timespan = DateTime.UtcNow - LastBlock;
            var num = Player.GetModifiedCooldown(CustomGameOptions.EscRoleblockCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Roleblock()
        {
            if (RoleblockTimer() != 0f || Utils.IsTooFar(Player, BlockButton.TargetPlayer))
                return;

            var interact = Utils.Interact(Player, BlockButton.TargetPlayer);

            if (interact[3])
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.EscRoleblock);
                writer.Write(PlayerId);
                writer.Write(BlockButton.TargetPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.EscRoleblockDuration;
                BlockTarget = BlockButton.TargetPlayer;
                Block();
            }
            else if (interact[0])
                LastBlock = DateTime.UtcNow;
            else if (interact[1])
                LastBlock.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public bool Exception(PlayerControl player) => player == BlockTarget;

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            BlockButton.Update("ROLEBLOCK", RoleblockTimer(), CustomGameOptions.EscRoleblockCooldown, Blocking, TimeRemaining, CustomGameOptions.EscRoleblockDuration);
        }
    }
}