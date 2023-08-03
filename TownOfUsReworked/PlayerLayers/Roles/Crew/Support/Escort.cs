namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Escort : Crew
    {
        public PlayerControl BlockTarget { get; set; }
        public bool Enabled { get; set; }
        public DateTime LastBlocked { get; set; }
        public float TimeRemaining { get; set; }
        public CustomButton BlockButton { get; set; }
        public bool Blocking => TimeRemaining > 0f;
        public float Timer => ButtonUtils.Timer(Player, LastBlocked, CustomGameOptions.EscRoleblockCooldown);

        public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Escort : Colors.Crew;
        public override string Name => "Escort";
        public override LayerEnum Type => LayerEnum.Escort;
        public override RoleEnum RoleType => RoleEnum.Escort;
        public override Func<string> StartText => () => "Roleblock Players From Harming The <color=#8CFFFFFF>Crew</color>";
        public override Func<string> Description => () => "- You can seduce players\n- Seduction blocks your target from being able to use their abilities for a short while\n- You are " +
            "immune to blocks\n- If you attempt to block a <color=#336EFFFF>Serial Killer</color>, they will be forced to kill you";
        public override InspectorResults InspectorResults => InspectorResults.HindersOthers;

        public Escort(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.CrewSupport;
            RoleBlockImmune = true;
            BlockTarget = null;
            BlockButton = new(this, "EscortRoleblock", AbilityTypes.Direct, "ActionSecondary", Roleblock);
        }

        public void UnBlock()
        {
            Enabled = false;
            GetLayers(BlockTarget).ForEach(x => x.IsBlocked = false);
            BlockTarget = null;
            LastBlocked = DateTime.UtcNow;
        }

        public void Block()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            GetLayers(BlockTarget).ForEach(x => x.IsBlocked = !GetRole(BlockTarget).RoleBlockImmune);

            if (Meeting || IsDead || BlockTarget.Data.IsDead || BlockTarget.Data.Disconnected)
                TimeRemaining = 0f;
        }

        public void Roleblock()
        {
            if (Timer != 0f || IsTooFar(Player, BlockButton.TargetPlayer))
                return;

            var interact = Interact(Player, BlockButton.TargetPlayer);

            if (interact[3])
            {
                TimeRemaining = CustomGameOptions.EscRoleblockDuration;
                BlockTarget = BlockButton.TargetPlayer;
                Block();
                CallRpc(CustomRPC.Action, ActionsRPC.EscRoleblock, this, BlockTarget);
            }
            else if (interact[0])
                LastBlocked = DateTime.UtcNow;
            else if (interact[1])
                LastBlocked.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public bool Exception(PlayerControl player) => player == BlockTarget;

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            BlockButton.Update("SEDUCE", Timer, CustomGameOptions.EscRoleblockCooldown, Blocking, TimeRemaining, CustomGameOptions.EscRoleblockDuration);
        }
    }
}