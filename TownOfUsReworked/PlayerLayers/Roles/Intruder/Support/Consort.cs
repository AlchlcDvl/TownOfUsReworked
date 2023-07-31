namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Consort : Intruder
    {
        public DateTime LastBlock;
        public float TimeRemaining;
        public CustomButton BlockButton;
        public PlayerControl BlockTarget;
        public bool Enabled;
        public bool Blocking => TimeRemaining > 0f;
        public CustomMenu BlockMenu;

        public override Color32 Color => ClientGameOptions.CustomIntColors ? Colors.Consort : Colors.Intruder;
        public override string Name => "Consort";
        public override LayerEnum Type => LayerEnum.Consort;
        public override RoleEnum RoleType => RoleEnum.Consort;
        public override Func<string> StartText => () => "Roleblock The <color=#8CFFFFFF>Crew</color> From Progressing";
        public override Func<string> AbilitiesText => () => "- You can seduce players\n- Seduction blocks your target from being able to use their abilities for a short while\n- You are " +
            $"immune to blocks\n- If you block a <color=#336EFFFF>Serial Killer</color>, they will be forced to kill you\n{CommonAbilities}";
        public override InspectorResults InspectorResults => InspectorResults.HindersOthers;

        public Consort(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.IntruderSupport;
            RoleBlockImmune = true;
            BlockMenu = new(Player, Click, Exception1);
            BlockTarget = null;
            BlockButton = new(this, "ConsortRoleblock", AbilityTypes.Effect, "Secondary", Roleblock);
        }

        public void UnBlock()
        {
            Enabled = false;
            GetLayers(BlockTarget).ForEach(x => x.IsBlocked = false);
            BlockTarget = null;
            LastBlock = DateTime.UtcNow;
        }

        public void Block()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            GetLayers(BlockTarget).ForEach(x => x.IsBlocked = !GetRole(BlockTarget).RoleBlockImmune);

            if (IsDead || BlockTarget.Data.IsDead || BlockTarget.Data.Disconnected || Meeting || !BlockTarget.IsBlocked())
                TimeRemaining = 0f;
        }

        public float RoleblockTimer()
        {
            var timespan = DateTime.UtcNow - LastBlock;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ConsRoleblockCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Click(PlayerControl player)
        {
            var interact = Interact(Player, player);

            if (interact[3])
                BlockTarget = player;
            else if (interact[0])
                LastBlock = DateTime.UtcNow;
            else if (interact[1])
                LastBlock.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void Roleblock()
        {
            if (RoleblockTimer() != 0f)
                return;

            if (BlockTarget == null)
                BlockMenu.Open();
            else
            {
                TimeRemaining = CustomGameOptions.ConsRoleblockDuration;
                Block();
                CallRpc(CustomRPC.Action, ActionsRPC.ConsRoleblock, this, BlockTarget);
            }
        }

        public bool Exception1(PlayerControl player) => player == BlockTarget || player == Player || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            BlockButton.Update(BlockTarget == null ? "SET TARGET" : "ROLEBLOCK", RoleblockTimer(), CustomGameOptions.ConsRoleblockCooldown, Blocking, TimeRemaining,
                CustomGameOptions.ConsRoleblockDuration);

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (BlockTarget != null && !Blocking)
                    BlockTarget = null;

                LogSomething("Removed a target");
            }
        }
    }
}