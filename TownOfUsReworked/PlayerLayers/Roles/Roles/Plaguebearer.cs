using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using Reactor.Utilities;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Plaguebearer : Role
    {
        public PlayerControl ClosestPlayer;
        public List<byte> InfectedPlayers = new List<byte>();
        public DateTime LastInfected;
        public bool PlaguebearerWins { get; set; }
        public int InfectedAlive => InfectedPlayers.Count(x => Utils.PlayerById(x) != null && Utils.PlayerById(x).Data != null && !Utils.PlayerById(x).Data.IsDead);
        public bool CanTransform => (PlayerControl.AllPlayerControls.ToArray().Count(x => x != null && !x.Data.IsDead) <= InfectedAlive) | CustomGameOptions.PestSpawn;

        public Plaguebearer(PlayerControl player) : base(player)
        {
            Name = "Plaguebearer";
            ImpostorText = () => "Spread Disease To Become Pestilence";
            TaskText = () => "Infect everyone to become Pestilence\nFake Tasks:";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Plaguebearer : Colors.Neutral;
            SubFaction = SubFaction.None;
            RoleType = RoleEnum.Plaguebearer;
            Faction = Faction.Neutral;
            InfectedPlayers.Add(player.PlayerId);
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = () => "Neutral (Killing)";
            IntroText = "Infect everyone";
            Results = InspResults.ArsoCryoPBOpTroll;
            AddToRoleHistory(RoleType);
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                (x.Data.IsImpostor() | (x.Is(RoleAlignment.NeutralKill) && (!x.Is(RoleEnum.Pestilence) | !x.Is(RoleEnum.Plaguebearer))) |
                x.Is(RoleAlignment.NeutralNeo) | x.Is(RoleAlignment.NeutralPros) | x.Is(Faction.Crew))) == 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PlaguebearerWin,
                    SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public void Wins()
        {
            PlaguebearerWins = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var plaguebearerTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            plaguebearerTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = plaguebearerTeam;
        }

        public float InfectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInfected;
            var num = CustomGameOptions.InfectCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void RpcSpreadInfection(PlayerControl source, PlayerControl target)
        {
            new WaitForSeconds(1f);

            if (InfectedPlayers.Contains(source.PlayerId))
            {
                InfectedPlayers.Add(target.PlayerId);

                unchecked
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Infect,
                        SendOption.Reliable, -1);
                    writer.Write(Player.PlayerId);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
            else if (InfectedPlayers.Contains(target.PlayerId))
            {
                InfectedPlayers.Add(source.PlayerId);

                unchecked
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Infect,
                        SendOption.Reliable, -1);
                    writer.Write(Player.PlayerId);
                    writer.Write(source.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }

        public void TurnPestilence()
        {
            RoleDictionary.Remove(Player.PlayerId);
            var role = new Pestilence(Player);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (CustomGameOptions.PlayersAlerted)
                    Coroutines.Start(Utils.FlashCoroutine(Colors.Pestilence));
                
                if (player == PlayerControl.LocalPlayer)
                    role.RegenTask();
            }
        }
    }
}