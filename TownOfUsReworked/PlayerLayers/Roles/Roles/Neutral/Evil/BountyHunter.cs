using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Random = UnityEngine.Random;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class BountyHunter : NeutralRole
    {
        public PlayerControl TargetPlayer = null;
        public PlayerControl ClosestPlayer;
        public bool TargetKilled;
        public bool ColorHintGiven;
        public bool LetterHintGiven;
        public bool TargetFound;
        public DateTime LastChecked;
        public AbilityButton GuessButton;
        public AbilityButton HuntButton;
        public bool ButtonUsable => UsesLeft > 0;
        public bool Failed => (UsesLeft <= 0 && !TargetFound) || (!TargetKilled && (TargetPlayer == null || TargetPlayer.Data.IsDead || TargetPlayer.Data.Disconnected));
        public int UsesLeft;

        public BountyHunter(PlayerControl player) : base(player)
        {
            Name = "Bounty Hunter";
            StartText = "Find And Kill Your Target";
            Objectives = "- Find And Kill your target.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.BountyHunter : Colors.Neutral;
            RoleType = RoleEnum.BountyHunter;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = NE;
            UsesLeft = CustomGameOptions.BountyHunterGuesses;
        }

        public float CheckTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastChecked;
            var num = CustomGameOptions.BountyHunterCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Kaboom()
        {
            var allPlayers = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.Disconnected || player.Data.IsDead || player == Player)
                    continue;
                
                allPlayers.Add(player);
                PlayerControl unfortunate1 = null;
                PlayerControl unfortunate2 = null;

                while (unfortunate1 == null || unfortunate2 == null || unfortunate1 == unfortunate2)
                {
                    var target1 = Random.RandomRangeInt(0, allPlayers.Count);
                    var target2 = Random.RandomRangeInt(0, allPlayers.Count);

                    unfortunate1 = allPlayers[target1];
                    unfortunate2 = allPlayers[target2];
                }

                Utils.RpcMurderPlayer(unfortunate1, unfortunate2, true);
                Utils.RpcMurderPlayer(unfortunate2, unfortunate2, true);
            }
        }

        public void TurnTroll()
        {
            var bh = Role.GetRole(Player);
            var newRole = new Troll(Player);
            newRole.RoleUpdate(bh);
            Player.RegenTask();
        }
    }
}