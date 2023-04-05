using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Random = UnityEngine.Random;
using System;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class BountyHunter : NeutralRole
    {
        public PlayerControl TargetPlayer;
        public PlayerControl ClosestPlayer;
        public bool TargetKilled;
        public bool ColorHintGiven;
        public bool LetterHintGiven;
        public bool TargetFound;
        public DateTime LastChecked;
        public AbilityButton GuessButton;
        public AbilityButton HuntButton;
        public bool ButtonUsable => UsesLeft > 0;
        public bool Failed => (UsesLeft <= 0 && !TargetFound) || (!TargetKilled && (TargetPlayer?.Data.IsDead == true || TargetPlayer?.Data.Disconnected == true));
        public int UsesLeft;

        public BountyHunter(PlayerControl player) : base(player)
        {
            Name = "Bounty Hunter";
            StartText = "Find And Kill Your Target";
            Objectives = "- Find and kill your target";
            AbilitiesText = "- You can guess a player to be your bounty\n- Upon finding the bounty, you can kill them\n- After your bounty has been killed by you, you can kill others as " +
                "many times as you want\n- If your target dies not by your hands, you will become a <color=#678D36FF>Troll</color>";
            Color = CustomGameOptions.CustomNeutColors ? Colors.BountyHunter : Colors.Neutral;
            RoleType = RoleEnum.BountyHunter;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = NE;
            UsesLeft = CustomGameOptions.BountyHunterGuesses;
        }

        public float CheckTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastChecked;
            var num = CustomGameOptions.BountyHunterCooldown * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
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
            var newRole = new Troll(Player);
            newRole.RoleUpdate(this);

            if (Player == PlayerControl.LocalPlayer)
                Utils.Flash(Colors.Troll, "Your target died so you have become a <color=#678D36FF>Troll</color>!");

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer, "Someone has changed their identity!");
        }
    }
}