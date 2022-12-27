using Hazel;
using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Werewolf : Role
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastMauled { get; set; }
        public bool WWWins { get; set; }

        public Werewolf(PlayerControl player) : base(player)
        {
            Name = "Werewolf";
            StartText = "Howl And Maul Everyone";
            AbilitiesText = "Kill everyone!\nFake Tasks:";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Werewolf : Colors.Neutral;
            LastMauled = DateTime.UtcNow;
            RoleType = RoleEnum.Werewolf;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = "Neutral (Killing)";
            IntroText = "Maul those who oppose you";
            Results = InspResults.JestJuggWWInv;
            IntroSound = null;
            Attack = AttackEnum.Basic;
            Defense = DefenseEnum.None;
            AttackString = "Basic";
            DefenseString = "None";
            AddToRoleHistory(RoleType);
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if (Utils.NKWins(RoleType))
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WerewolfWin,
                    SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public override void Wins()
        {
            WWWins = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        public float MaulTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMauled;
            var num = CustomGameOptions.MaulCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var murdTeam = new List<PlayerControl>();
            murdTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = murdTeam;
        }

        public static List<PlayerControl> FindClosestPlayers(PlayerControl player)
        {
            List<PlayerControl> playerControlList = new List<PlayerControl>();
            float maulRadius = CustomGameOptions.MaulRadius * 2;
            Vector2 truePosition = player.GetTruePosition();
            List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers;

            for (int index = 0; index < allPlayers.Count; ++index)
            {
                GameData.PlayerInfo playerInfo = allPlayers[index];

                if (!playerInfo.Disconnected)
                {
                    Vector2 vector2 = new Vector2(playerInfo.Object.GetTruePosition().x - truePosition.x, playerInfo.Object.GetTruePosition().y - truePosition.y);
                    float magnitude = ((Vector2) vector2).magnitude;

                    if (magnitude <= maulRadius)
                    {
                        PlayerControl playerControl = playerInfo.Object;
                        playerControlList.Add(playerControl);
                    }
                }
            }

            return playerControlList;
        }

        public void Maul(PlayerControl player2)
        {
            var closestPlayers = FindClosestPlayers(Player);

            foreach (var player in closestPlayers)
            {
                if (player.Is(RoleEnum.Pestilence) || player.IsVesting() || player.IsProtected())
                    continue;
                    
                if (player.PlayerId != ClosestPlayer.PlayerId && !player.Is(RoleType))
                    Utils.RpcMurderPlayer(player2, player);
                
                if (player.IsOnAlert())
                    Utils.RpcMurderPlayer(player, player2);
            }
        }
    }
}