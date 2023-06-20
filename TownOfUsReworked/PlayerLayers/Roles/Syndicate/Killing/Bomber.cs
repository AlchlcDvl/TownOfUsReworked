namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Bomber : Syndicate
    {
        public DateTime LastPlaced;
        public DateTime LastDetonated;
        public CustomButton BombButton;
        public CustomButton DetonateButton;
        public List<Bomb> Bombs = new();

        public Bomber(PlayerControl player) : base(player)
        {
            Name = "Bomber";
            StartText = () => "Make People Go Boom";
            AbilitiesText = () => $"- You can place bombs which you can detonate at any time to kill anyone within a certain radius\n{CommonAbilities}";
            Color = CustomGameOptions.CustomSynColors ? Colors.Bomber : Colors.Syndicate;
            RoleType = RoleEnum.Bomber;
            RoleAlignment = RoleAlignment.SyndicateKill;
            Bombs = new();
            Type = LayerEnum.Bomber;
            BombButton = new(this, "Plant", AbilityTypes.Effect, "ActionSecondary", Place);
            DetonateButton = new(this, "Detonate", AbilityTypes.Effect, "Secondary", Detonate);
            InspectorResults = InspectorResults.DropsItems;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public float BombTimer()
        {
            var timespan = DateTime.UtcNow - LastPlaced;
            var num = Player.GetModifiedCooldown(CustomGameOptions.BombCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float DetonateTimer()
        {
            var timespan = DateTime.UtcNow - LastDetonated;
            var num = Player.GetModifiedCooldown(CustomGameOptions.DetonateCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
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

            Bombs.Add(new(Player.GetTruePosition(), HoldsDrive, Player));
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
