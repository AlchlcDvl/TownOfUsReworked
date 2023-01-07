using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Arsonist : Role
    {
        private KillButton _igniteButton;
        public bool ArsonistWins;
        public bool LastKiller;
        public PlayerControl ClosestPlayerDouse;
        public PlayerControl ClosestPlayerIgnite;
        public List<byte> DousedPlayers = new List<byte>();
        public DateTime LastDoused;
        public DateTime LastIgnited;
        public int DousedAlive => DousedPlayers.Count(x => Utils.PlayerById(x) != null && Utils.PlayerById(x).Data != null &&
            !Utils.PlayerById(x).Data.IsDead);

        public Arsonist(PlayerControl player) : base(player)
        {
            Name = "Arsonist";
            StartText = "Gasoline + Bean + Lighter = Bonfire";
            AbilitiesText = "- You can douse players in gasoline.\n- Doused players can then be ignite to kill all doused players at once.";
            AttributesText = "- People who interact with you will also get doused.";
            RoleType = RoleEnum.Arsonist;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = "Neutral (Killing)";
            IntroText = "Ignite those who oppose you";
            Results = InspResults.ArsoCryoPBOpTroll;
            Color = IsRecruit ? Colors.Cabal : (CustomGameOptions.CustomNeutColors ? Colors.Arsonist : Colors.Neutral);
            Attack = AttackEnum.Unstoppable;
            AttackString = "Unstoppable";
            DefenseString = "Basic";
            Defense = DefenseEnum.Basic;
            RoleDescription = "You are an Arsonist! This means that you do not kill directly and instead, bide your time by dousing other players" +
                " and igniting them later for mass murder. Be careful though, as you need be next to someone to ignite and if anyone sees you ignite," +
                $" you are done for. There are currently {DousedAlive} players doused.";
            FactionDescription = NeutralFactionDescription;
            AlignmentDescription = NKDescription;
            Objectives = IsRecruit ? JackalWinCon : NKWinCon;
            AddToRoleHistory(RoleType);
        }

        public KillButton IgniteButton
        {
            get => _igniteButton;
            set
            {
                _igniteButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
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
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ArsonistWin,
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
                ArsonistWins = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            if (IsRecruit)
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.GoodRecruit);
            }

            __instance.teamToShow = team;
        }

        public float DouseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDoused;
            var num = CustomGameOptions.DouseCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public float IgniteTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastIgnited;
            var num = CustomGameOptions.IgniteCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Ignite()
        {
            System.Console.WriteLine("Ignite 1");

            foreach (var arso in Role.GetRoles(RoleEnum.Arsonist))
            {
                var arso2 = (Arsonist)arso;

                foreach (var playerId in arso2.DousedPlayers)
                {
                    var player = Utils.PlayerById(playerId);

                    if (player == null || player.Data.Disconnected || player.Data.IsDead || player.Is(RoleEnum.Pestilence))
                        continue;

                    Utils.RpcMurderPlayer(Player, player);
                }

                arso2.DousedPlayers.Clear();
            }
            
            System.Console.WriteLine("Ignite 2");
        }

        public void RpcSpreadDouse(PlayerControl source, PlayerControl target)
        {
            new WaitForSeconds(1f);

            if (DousedPlayers.Contains(source.PlayerId))
            {
                DousedPlayers.Add(target.PlayerId);

                unchecked
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Douse,
                        SendOption.Reliable, -1);
                    writer.Write(Player.PlayerId);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }
    }
}
