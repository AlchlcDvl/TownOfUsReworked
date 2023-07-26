namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Grenadier : Intruder
    {
        public CustomButton FlashButton;
        public bool Enabled;
        public DateTime LastFlashed;
        public float TimeRemaining;
        private static List<PlayerControl> ClosestPlayers = new();
        private static Color32 NormalVision => new(212, 212, 212, 0);
        private static Color32 DimVision => new(212, 212, 212, 51);
        private static Color32 BlindVision => new(212, 212, 212, 255);
        public List<PlayerControl> FlashedPlayers = new();
        public bool Flashed => TimeRemaining > 0f;

        public override Color32 Color => ClientGameOptions.CustomIntColors ? Colors.Grenadier : Colors.Intruder;
        public override string Name => "Grenadier";
        public override LayerEnum Type => LayerEnum.Grenadier;
        public override RoleEnum RoleType => RoleEnum.Grenadier;
        public override Func<string> StartText => () => "Blind The <color=#8CFFFFFF>Crew</color> With Your Magnificent Figure";
        public override Func<string> AbilitiesText => () => $"- You can drop a flashbang, which blinds players around you\n{CommonAbilities}";
        public override InspectorResults InspectorResults => InspectorResults.DropsItems;

        public Grenadier(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.IntruderConceal;
            ClosestPlayers = new();
            FlashedPlayers = new();
            FlashButton = new(this, "Flash", AbilityTypes.Effect, "Secondary", HitFlash);
        }

        public float FlashTimer()
        {
            var timespan = DateTime.UtcNow - LastFlashed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.GrenadeCd) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Flash()
        {
            if (!Enabled)
            {
                ClosestPlayers = GetClosestPlayers(Player.GetTruePosition(), CustomGameOptions.FlashRadius);
                FlashedPlayers = ClosestPlayers;
            }

            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (Meeting)
                TimeRemaining = 0f;

            //To stop the scenario where the flash and sabotage are called at the same time.
            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var dummyActive = system.dummy.IsActive;
            var sabActive = system.specials.Any(s => s.IsActive);

            if (sabActive || dummyActive)
                return;

            foreach (var player in ClosestPlayers)
            {
                if (CustomPlayer.Local == player)
                {
                    HUD.FullScreen.enabled = true;
                    HUD.FullScreen.gameObject.active = true;

                    if (TimeRemaining > CustomGameOptions.GrenadeDuration - 0.5f)
                    {
                        var fade = (TimeRemaining - CustomGameOptions.GrenadeDuration) * (-2f);

                        if (ShouldPlayerBeBlinded(player))
                            HUD.FullScreen.color = Color32.Lerp(NormalVision, BlindVision, fade);
                        else if (ShouldPlayerBeDimmed(player))
                            HUD.FullScreen.color = Color32.Lerp(NormalVision, DimVision, fade);
                        else
                            HUD.FullScreen.color = NormalVision;
                    }
                    else if (TimeRemaining <= (CustomGameOptions.GrenadeDuration - 0.5f) && TimeRemaining >= 0.5f)
                    {
                        HUD.FullScreen.enabled = true;
                        HUD.FullScreen.gameObject.active = true;

                        if (ShouldPlayerBeBlinded(player))
                            HUD.FullScreen.color = BlindVision;
                        else if (ShouldPlayerBeDimmed(player))
                            HUD.FullScreen.color = DimVision;
                        else
                            HUD.FullScreen.color = NormalVision;
                    }
                    else if (TimeRemaining < 0.5f)
                    {
                        var fade2 = (TimeRemaining * -2.0f) + 1.0f;

                        if (ShouldPlayerBeBlinded(player))
                            HUD.FullScreen.color = Color32.Lerp(BlindVision, NormalVision, fade2);
                        else if (ShouldPlayerBeDimmed(player))
                            HUD.FullScreen.color = Color32.Lerp(DimVision, NormalVision, fade2);
                        else
                            HUD.FullScreen.color = NormalVision;
                    }
                    else
                    {
                        HUD.FullScreen.color = NormalVision;
                        TimeRemaining = 0f;
                    }

                    if (MapBehaviour.Instance)
                        MapBehaviour.Instance.Close();

                    if (Minigame.Instance)
                        Minigame.Instance.Close();
                }
            }
        }

        private static bool ShouldPlayerBeDimmed(PlayerControl player) => (player.Is(Faction.Intruder) || player.Data.IsDead) && !Meeting;

        private static bool ShouldPlayerBeBlinded(PlayerControl player) => !(player.Is(Faction.Intruder) || player.Data.IsDead || Meeting);

        public void UnFlash()
        {
            Enabled = false;
            LastFlashed = DateTime.UtcNow;
            HUD.FullScreen.color = NormalVision;
            FlashedPlayers.Clear();
            SetFullScreenHUD();
        }

        public void HitFlash()
        {
            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var dummyActive = system.dummy.IsActive;
            var sabActive = system.specials.Any(s => s.IsActive);

            if (sabActive || dummyActive || FlashTimer() != 0f)
                return;

            CallRpc(CustomRPC.Action, ActionsRPC.FlashGrenade, this);
            TimeRemaining = CustomGameOptions.GrenadeDuration;
            Flash();
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var dummyActive = system.dummy.IsActive;
            var sabActive = system.specials.Any(s => s.IsActive);
            var condition = !dummyActive && !sabActive;
            FlashButton.Update("FLASH", FlashTimer(), CustomGameOptions.GrenadeCd, Flashed, TimeRemaining, CustomGameOptions.GrenadeDuration, condition);
        }
    }
}
