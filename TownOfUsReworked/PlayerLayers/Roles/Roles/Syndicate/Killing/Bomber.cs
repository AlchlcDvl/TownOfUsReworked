using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using System.Collections.Generic;
using TownOfUsReworked.Objects;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Bomber : SyndicateRole
    {
        public DateTime LastPlaced;
        public DateTime LastDetonated;
        public CustomButton BombButton;
        public CustomButton DetonateButton;
        public List<Bomb> Bombs = new();

        public Bomber(PlayerControl player) : base(player)
        {
            Name = "Bomber";
            StartText = "Make People Go Boom";
            AbilitiesText = $"- You can place bombs which you can detonate at any time to kill anyone within a certain radius\n{AbilitiesText}";
            Color = CustomGameOptions.CustomSynColors ? Colors.Bomber : Colors.Syndicate;
            RoleType = RoleEnum.Bomber;
            RoleAlignment = RoleAlignment.SyndicateKill;
            AlignmentName = SyK;
            Bombs = new();
            Type = LayerEnum.Bomber;
            BombButton = new(this, AssetManager.Plant, AbilityTypes.Effect, "Secondary", Place);
            DetonateButton = new(this, AssetManager.Detonate, AbilityTypes.Effect, "Tertiary", Detonate);
        }

        public float BombTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastPlaced;
            var num = Player.GetModifiedCooldown(CustomGameOptions.BombCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float DetonateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastDetonated;
            var num = Player.GetModifiedCooldown(CustomGameOptions.DetonateCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);

            if (CustomGameOptions.BombsDetonateOnMeetingStart)
                Bombs.DetonateBombs();
        }

        public void Place()
        {
            if (BombTimer() != 0f)
                return;

            Bombs.Add(BombExtensions.CreateBomb(Player.GetTruePosition(), HoldsDrive));
            LastPlaced = DateTime.UtcNow;

            if (CustomGameOptions.BombCooldownsLinked)
                LastDetonated = DateTime.UtcNow;
        }

        public void Detonate()
        {
            if (DetonateTimer() != 0f || Bombs.Count == 0)
                return;

            Bombs.DetonateBombs();
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
