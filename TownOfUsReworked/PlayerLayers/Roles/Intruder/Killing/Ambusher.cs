namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Ambusher : Intruder
    {
        public bool Enabled;
        public DateTime LastAmbushed;
        public float TimeRemaining;
        public bool OnAmbush => TimeRemaining > 0f;
        public PlayerControl AmbushedPlayer;
        public CustomButton AmbushButton;

        public override Color32 Color => ClientGameOptions.CustomIntColors ? Colors.Ambusher : Colors.Intruder;
        public override string Name => "Ambusher";
        public override LayerEnum Type => LayerEnum.Ambusher;
        public override RoleEnum RoleType => RoleEnum.Ambusher;
        public override Func<string> StartText => () => "Spook The <color=#8CFFFFFF>Crew</color>";
        public override Func<string> AbilitiesText => () => "- You can ambush players\n- Ambushed players will be forced to be on alert and kill whoever interacts with them\n" +
            CommonAbilities;
        public override InspectorResults InspectorResults => InspectorResults.HindersOthers;

        public Ambusher(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.IntruderKill;
            AmbushedPlayer = null;
            AmbushButton = new(this, "Ambush", AbilityTypes.Direct, "Secondary", HitAmbush, Exception1);
        }

        public float AmbushTimer()
        {
            var timespan = DateTime.UtcNow - LastAmbushed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.AmbushCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Ambush()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (IsDead || AmbushedPlayer.Data.IsDead || AmbushedPlayer.Data.Disconnected || Meeting)
                TimeRemaining = 0f;
        }

        public void UnAmbush()
        {
            Enabled = false;
            LastAmbushed = DateTime.UtcNow;
            AmbushedPlayer = null;
        }

        public void HitAmbush()
        {
            if (AmbushTimer() != 0f || IsTooFar(Player, AmbushButton.TargetPlayer) || AmbushButton.TargetPlayer == AmbushedPlayer)
                return;

            var interact = Interact(Player, AmbushButton.TargetPlayer);

            if (interact[3])
            {
                TimeRemaining = CustomGameOptions.AmbushDuration;
                AmbushedPlayer = AmbushButton.TargetPlayer;
                CallRpc(CustomRPC.Action, ActionsRPC.Ambush, this, AmbushedPlayer);
                Ambush();
            }
            else if (interact[0])
                LastAmbushed = DateTime.UtcNow;
            else if (interact[1])
                LastAmbushed.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public bool Exception1(PlayerControl player) => player == AmbushedPlayer || (player.Is(Faction) && !CustomGameOptions.AmbushMates) || (player.Is(SubFaction) &&
            SubFaction != SubFaction.None && !CustomGameOptions.AmbushMates);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            AmbushButton.Update("AMBUSH", AmbushTimer(), CustomGameOptions.AmbushDuration, OnAmbush, TimeRemaining, CustomGameOptions.AmbushDuration);
        }
    }
}