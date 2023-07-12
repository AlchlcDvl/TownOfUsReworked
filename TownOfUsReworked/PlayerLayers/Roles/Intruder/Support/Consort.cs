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

        public Consort(PlayerControl player) : base(player)
        {
            Name = "Consort";
            RoleType = RoleEnum.Consort;
            StartText = () => "Roleblock The <color=#8CFFFFFF>Crew</color> From Progressing";
            AbilitiesText = () => "- You can seduce players\n- Seduction blocks your target from being able to use their abilities for a short while\n- You are immune to blocks\n" +
                $"- If you block a <color=#336EFFFF>Serial Killer</color>, they will be forced to kill you\n{CommonAbilities}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Consort : Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            RoleBlockImmune = true;
            BlockMenu = new(Player, Click, Exception1);
            Type = LayerEnum.Consort;
            BlockTarget = null;
            BlockButton = new(this, "ConsortRoleblock", AbilityTypes.Effect, "Secondary", Roleblock);
            InspectorResults = InspectorResults.HindersOthers;

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
            Utils.DefaultOutfit(Player);
        }

        public void Block()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            foreach (var layer in GetLayers(BlockTarget))
                layer.IsBlocked = !GetRole(BlockTarget).RoleBlockImmune;

            if (IsDead || BlockTarget.Data.IsDead || BlockTarget.Data.Disconnected || Utils.Meeting)
                TimeRemaining = 0f;
        }

        public float RoleblockTimer()
        {
            var timespan = DateTime.UtcNow - LastBlock;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ConsRoleblockCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Click(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

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
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.ConsRoleblock);
                writer.Write(PlayerId);
                writer.Write(BlockTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.ConsRoleblockDuration;
                Block();
            }
        }

        public bool Exception1(PlayerControl player) => player == BlockTarget || player == Player || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var flag = BlockTarget == null;
            BlockButton.Update(flag ? "SET TARGET" : "ROLEBLOCK", RoleblockTimer(), CustomGameOptions.ConsRoleblockCooldown, Blocking, TimeRemaining,
                CustomGameOptions.ConsRoleblockDuration);
        }
    }
}