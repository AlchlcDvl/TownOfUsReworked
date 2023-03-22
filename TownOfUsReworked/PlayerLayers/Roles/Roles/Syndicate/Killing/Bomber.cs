using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Collections.Generic;
using TownOfUsReworked.Objects;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Bomber : SyndicateRole
    {
        public DateTime LastPlaced;
        public DateTime LastDetonated;
        public AbilityButton BombButton;
        public AbilityButton DetonateButton;
        public List<Bomb> Bombs;

        public Bomber(PlayerControl player) : base(player)
        {
            Name = "Bomber";
            StartText = "Make People Go Boom";
            AbilitiesText = "- You can place bombs which you can detonate at any time to kill anyone within a certain radius.\n- Your bombs can even kill you and your fellow Syndicate " +
                "so be careful when making people explode.";
            Color = CustomGameOptions.CustomSynColors ? Colors.Bomber : Colors.Syndicate;
            RoleType = RoleEnum.Bomber;
            RoleAlignment = RoleAlignment.SyndicateKill;
            AlignmentName = SyK;
            Bombs = new List<Bomb>();
        }

        public float BombTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastPlaced;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.BombCooldown, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public float DetonateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDetonated;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.DetonateCooldown, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
