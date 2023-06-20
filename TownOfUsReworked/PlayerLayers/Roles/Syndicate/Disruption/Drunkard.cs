namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Drunkard : Syndicate
    {
        public CustomButton ConfuseButton;
        public bool Enabled;
        public DateTime LastConfused;
        public float TimeRemaining;
        public bool Confused => TimeRemaining > 0f;
        public float Modifier => Confused ? -1 : 1;
        public PlayerControl ConfusedPlayer;
        public CustomMenu ConfuseMenu;

        public Drunkard(PlayerControl player) : base(player)
        {
            Name = "Drunkard";
            StartText = () => "*Burp*";
            AbilitiesText = () => "- You can confuse a player\n- Confused players will have their controls reverse\n- With the Chaos Drive, you reverse everyone's controls\n" +
                $"{CommonAbilities}";
            Color = CustomGameOptions.CustomSynColors ? Colors.Drunkard : Colors.Syndicate;
            RoleType = RoleEnum.Drunkard;
            RoleAlignment = RoleAlignment.SyndicateDisrup;
            ConfuseMenu = new(Player, Click, Exception1);
            ConfusedPlayer = null;
            Type = LayerEnum.Drunkard;
            ConfuseButton = new(this, "Confuse", AbilityTypes.Effect, "Secondary", HitConfuse);
            InspectorResults = InspectorResults.HindersOthers;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public void Confuse()
        {
            if (!Enabled && (CustomPlayer.Local == ConfusedPlayer || HoldsDrive))
                Utils.Flash(Color);

            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance || (ConfusedPlayer == null && !HoldsDrive))
                TimeRemaining = 0f;
        }

        public void UnConfuse()
        {
            Enabled = false;
            LastConfused = DateTime.UtcNow;
            ConfusedPlayer = null;
        }

        public float ConfuseTimer()
        {
            var timespan = DateTime.UtcNow - LastConfused;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ConfuseCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Click(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                ConfusedPlayer = player;
            else if (interact[0])
                LastConfused = DateTime.UtcNow;
            else if (interact[1])
                LastConfused.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void HitConfuse()
        {
            if (ConfuseTimer() != 0f || Confused)
                return;

            if (HoldsDrive)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Confuse);
                writer.Write(PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.ConfuseDuration;
                Confuse();
            }
            else if (ConfusedPlayer == null)
                ConfuseMenu.Open();
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Confuse);
                writer.Write(PlayerId);
                writer.Write(ConfusedPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.ConfuseDuration;
                Confuse();
            }
        }

        public bool Exception1(PlayerControl player) => player == ConfusedPlayer || player == Player || (player.Is(Faction) && !CustomGameOptions.ConfuseImmunity);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var flag = ConfusedPlayer == null && !HoldsDrive;
            ConfuseButton.Update(flag ? "SET TARGET" : "CONFUSE", ConfuseTimer(), CustomGameOptions.ConfuseCooldown, Confused, TimeRemaining, CustomGameOptions.ConfuseDuration);

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (ConfusedPlayer != null && !HoldsDrive && !Confused)
                    ConfusedPlayer = null;

                Utils.LogSomething("Removed a target");
            }
        }
    }
}