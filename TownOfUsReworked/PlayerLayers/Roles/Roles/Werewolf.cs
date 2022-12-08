using Hazel;
using System;
using System.Linq;
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
        public List<PlayerControl> mauledPlayers = new List<PlayerControl>();
        public List<PlayerControl> closestPlayers = null;

        public Werewolf(PlayerControl player) : base(player)
        {
            Name = "Werwolf";
            StartText = "Howl And Maul Everyone";
            AbilitiesText = "Kill everyone!\nFake Tasks:";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Werewolf : Colors.Neutral;
            SubFaction = SubFaction.None;
            LastMauled = DateTime.UtcNow;
            RoleType = RoleEnum.Werewolf;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = "Neutral (Killing)";
            IntroText = "Maul those who oppose you";
            Results = InspResults.JestJuggWWInv;
            AddToRoleHistory(RoleType);
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                (x.Data.IsImpostor() | (x.Is(RoleAlignment.NeutralKill) | !x.Is(RoleEnum.Werewolf)) | x.Is(Faction.Crew))) == 0)
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

        public void Maul()
        {
            closestPlayers = FindClosestPlayers(Player);
            mauledPlayers = closestPlayers;

            foreach (var player in closestPlayers)
            {
                if (player.PlayerId != ClosestPlayer.PlayerId)
                    Utils.RpcMurderPlayer(Player, player);
            }
        }
    }
}