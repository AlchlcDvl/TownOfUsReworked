namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Ambusher : Intruder
    {
        public bool Enabled { get; set; }
        public DateTime LastAmbushed { get; set; }
        public float TimeRemaining { get; set; }
        public bool OnAmbush => TimeRemaining > 0f;
        public PlayerControl AmbushedPlayer { get; set; }
        public CustomButton AmbushButton { get; set; }

        public override Color32 Color => ClientGameOptions.CustomIntColors ? Colors.Ambusher : Colors.Intruder;
        public override string Name => "Ambusher";
        public override LayerEnum Type => LayerEnum.Ambusher;
        public override RoleEnum RoleType => RoleEnum.Ambusher;
        public override Func<string> StartText => () => "Spook The <color=#8CFFFFFF>Crew</color>";
        public override Func<string> Description => () => "- You can ambush players\n- Ambushed players will be forced to be on alert and kill whoever interacts with them\n" +
            CommonAbilities;
        public override InspectorResults InspectorResults => InspectorResults.HindersOthers;
        public float Timer => ButtonUtils.Timer(Player, LastAmbushed, CustomGameOptions.AmbushCooldown);

        public Ambusher(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.IntruderKill;
            AmbushedPlayer = null;
            AmbushButton = new(this, "Ambush", AbilityTypes.Direct, "Secondary", HitAmbush, Exception1);
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
            if (Timer != 0f || IsTooFar(Player, AmbushButton.TargetPlayer) || AmbushButton.TargetPlayer == AmbushedPlayer)
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
            AmbushButton.Update("AMBUSH", Timer, CustomGameOptions.AmbushDuration, OnAmbush, TimeRemaining, CustomGameOptions.AmbushDuration);
        }
    }
}