using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Werewolf : NeutralRole
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastMauled;
        public bool CanMaul;
        public AbilityButton MaulButton;

        public Werewolf(PlayerControl player) : base(player)
        {
            Name = "Werewolf";
            StartText = "Howl And Maul Everyone";
            AbilitiesText = "Kill everyone!";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Werewolf : Colors.Neutral;
            LastMauled = DateTime.UtcNow;
            RoleType = RoleEnum.Werewolf;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = NK;
        }

        public float MaulTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMauled;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.MaulCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Maul(PlayerControl player2)
        {
            foreach (var player in Utils.GetClosestPlayers(Player.GetTruePosition(), CustomGameOptions.MaulRadius))
            {
                Utils.Spread(Player, player);

                if (player.IsVesting() || player.IsProtected() || Player.IsOtherRival(player) || Player == player || ClosestPlayer == player)
                    continue;

                if (!player.Is(RoleEnum.Pestilence))
                    Utils.RpcMurderPlayer(player2, player, false);

                if (player.IsOnAlert() || player.Is(RoleEnum.Pestilence))
                    Utils.RpcMurderPlayer(player, player2, false);
            }
        }
    }
}