using static TownOfUsReworked.Languages.Language;
namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Escort : Crew
    {
        public PlayerControl BlockTarget;
        public bool Enabled;
        public DateTime LastBlock;
        public float TimeRemaining;
        public CustomButton BlockButton;
        public bool Blocking => TimeRemaining > 0f;

        public Escort(PlayerControl player) : base(player)
        {
            Name = GetString("Escort");
            RoleType = RoleEnum.Escort;
            StartText = () => GetString("EscortStartText");
            AbilitiesText = () => GetString("EscortAbilitiesText");
            Color = CustomGameOptions.CustomCrewColors ? Colors.Escort : Colors.Crew;
            RoleAlignment = RoleAlignment.CrewSupport;
            RoleBlockImmune = true;
            InspectorResults = InspectorResults.HindersOthers;
            Type = LayerEnum.Escort;
            BlockTarget = null;
            BlockButton = new(this, "EscortRoleblock", AbilityTypes.Direct, "ActionSecondary", Roleblock);

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
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

            if (Utils.Meeting || IsDead || BlockTarget.Data.IsDead || BlockTarget.Data.Disconnected)
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