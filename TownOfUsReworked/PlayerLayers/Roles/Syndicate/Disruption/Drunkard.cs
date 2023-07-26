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

        public override Color32 Color => ClientGameOptions.CustomSynColors ? Colors.Drunkard : Colors.Syndicate;
        public override string Name => "Drunkard";
        public override LayerEnum Type => LayerEnum.Drunkard;
        public override RoleEnum RoleType => RoleEnum.Drunkard;
        public override Func<string> StartText => () => "<i>Burp</i>";
        public override Func<string> AbilitiesText => () => $"- You can confuse {(HoldsDrive ? "everyone" : "a player")}\n- Confused players will have their controls reverse\n" +
            CommonAbilities;
        public override InspectorResults InspectorResults => InspectorResults.HindersOthers;

        public Drunkard(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.SyndicateDisrup;
            ConfuseMenu = new(Player, Click, Exception1);
            ConfusedPlayer = null;
            ConfuseButton = new(this, "Confuse", AbilityTypes.Effect, "Secondary", HitConfuse);
        }

        public void Confuse()
        {
            if (!Enabled && (CustomPlayer.Local == ConfusedPlayer || HoldsDrive))
                Flash(Color);

            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (Meeting || (ConfusedPlayer == null && !HoldsDrive))
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
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Click(PlayerControl player)
        {
            var interact = Interact(Player, player);

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
                TimeRemaining = CustomGameOptions.ConfuseDuration;
                Confuse();
                CallRpc(CustomRPC.Action, ActionsRPC.Confuse, this);
            }
            else if (ConfusedPlayer == null)
                ConfuseMenu.Open();
            else
            {
                CallRpc(CustomRPC.Action, ActionsRPC.Confuse, this, ConfusedPlayer);
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

                LogSomething("Removed a target");
            }
        }
    }
}