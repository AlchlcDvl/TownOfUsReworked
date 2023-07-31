namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Bomber : Syndicate
    {
        public DateTime LastPlaced;
        public DateTime LastDetonated;
        public CustomButton BombButton;
        public CustomButton DetonateButton;
        public List<Bomb> Bombs = new();

        public override Color32 Color => ClientGameOptions.CustomSynColors ? Colors.Bomber : Colors.Syndicate;
        public override string Name => "Bomber";
        public override LayerEnum Type => LayerEnum.Bomber;
        public override RoleEnum RoleType => RoleEnum.Bomber;
        public override Func<string> StartText => () => "Make People Go Boom";
        public override Func<string> AbilitiesText => () => $"- You can place bombs which you can detonate at any time to kill anyone within a {CustomGameOptions.BombRange}m radius\n" +
            CommonAbilities;
        public override InspectorResults InspectorResults => InspectorResults.DropsItems;

        public Bomber(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.SyndicateKill;
            Bombs = new();
            BombButton = new(this, "Plant", AbilityTypes.Effect, "ActionSecondary", Place);
            DetonateButton = new(this, "Detonate", AbilityTypes.Effect, "Secondary", Detonate);
        }

        public float BombTimer()
        {
            var timespan = DateTime.UtcNow - LastPlaced;
            var num = Player.GetModifiedCooldown(CustomGameOptions.BombCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public float DetonateTimer()
        {
            var timespan = DateTime.UtcNow - LastDetonated;
            var num = Player.GetModifiedCooldown(CustomGameOptions.DetonateCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public override void OnLobby()
        {
            base.OnLobby();
            Bomb.Clear(Bombs);
            Bombs.Clear();
        }

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);

            if (CustomGameOptions.BombsDetonateOnMeetingStart)
                Bomb.DetonateBombs(Bombs);
        }

        public void Place()
        {
            if (BombTimer() != 0f)
                return;

            Bombs.Add(new(Player, HoldsDrive));
            LastPlaced = DateTime.UtcNow;

            if (CustomGameOptions.BombCooldownsLinked)
                LastDetonated = DateTime.UtcNow;
        }

        public void Detonate()
        {
            if (DetonateTimer() != 0f || Bombs.Count == 0)
                return;

            Bomb.DetonateBombs(Bombs);
            LastDetonated = DateTime.UtcNow;

            if (CustomGameOptions.BombCooldownsLinked)
                LastPlaced = DateTime.UtcNow;
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            BombButton.Update("PLACE", BombTimer(), CustomGameOptions.BombCooldown);
            DetonateButton.Update("DETONATE", DetonateTimer(), CustomGameOptions.DetonateCooldown, true, Bombs.Count > 0);
        }
    }
}
