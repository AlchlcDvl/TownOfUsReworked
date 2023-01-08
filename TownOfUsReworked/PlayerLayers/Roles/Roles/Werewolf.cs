using Hazel;
using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Werewolf : Role
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastMauled { get; set; }
        public bool WWWins { get; set; }
        public bool CanMaul;

        public Werewolf(PlayerControl player) : base(player)
        {
            Name = "Werewolf";
            StartText = "Howl And Maul Everyone";
            AbilitiesText = "Kill everyone!";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Werewolf : Colors.Neutral;
            LastMauled = DateTime.UtcNow;
            RoleType = RoleEnum.Werewolf;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = "Neutral (Killing)";
            Results = InspResults.JestJuggWWInv;
            Attack = AttackEnum.Basic;
            AttackString = "Basic";
            AddToRoleHistory(RoleType);
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit)
            {
                if (Utils.CabalWin())
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CabalWin,
                        SendOption.Reliable, -1);
                    writer.Write(Player.PlayerId);
                    Wins();
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (Utils.NKWins(RoleType))
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
            if (IsRecruit)
                CabalWin = true;
            else
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
            var team = new List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            if (IsRecruit)
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.GoodRecruit);
            }

            __instance.teamToShow = team;
        }

        public void Maul(PlayerControl player2)
        {
            var closestPlayers = Utils.FindClosestPlayers(Player, CustomGameOptions.MaulRadius);

            foreach (var player in closestPlayers)
            {
                if (player.Is(RoleEnum.Pestilence) || player.IsVesting() || player.IsProtected())
                    continue;
                    
                if (Player.PlayerId != ClosestPlayer.PlayerId && !player.Is(RoleType))
                    Utils.RpcMurderPlayer(player2, player);
                
                if (player.IsOnAlert())
                    Utils.RpcMurderPlayer(player, player2);
            }
        }
    }
}