using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using Reactor.Utilities;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Plaguebearer : NeutralRole
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastInfected;
        public List<byte> InfectedPlayers;
        public int InfectedAlive => InfectedPlayers.Count;
        public bool CanTransform => PlayerControl.AllPlayerControls.ToArray().Count(x => x != null && !x.Data.IsDead && !x.Data.Disconnected && !x.Data.Disconnected) <= InfectedAlive;
        public AbilityButton InfectButton;

        public Plaguebearer(PlayerControl player) : base(player)
        {
            Name = "Plaguebearer";
            StartText = "Spread Disease To Become Pestilence";
            AbilitiesText = "Infect everyone to become Pestilence\nFake Tasks:";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Plaguebearer : Colors.Neutral;
            RoleType = RoleEnum.Plaguebearer;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = NK;
            InfectedPlayers = new List<byte>();
        }

        public float InfectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInfected;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.InfectCd) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void RpcSpreadInfection(PlayerControl source, PlayerControl target)
        {
            if (InfectedPlayers.Contains(source.PlayerId) && InfectedPlayers.Contains(target.PlayerId))
                return;

            new WaitForSeconds(1f);

            if (InfectedPlayers.Contains(source.PlayerId) || source.Is(RoleEnum.Plaguebearer))
            {
                InfectedPlayers.Add(target.PlayerId);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Infect);
                writer.Write(Player.PlayerId);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            else if (InfectedPlayers.Contains(target.PlayerId) || target.Is(RoleEnum.Plaguebearer))
            {
                InfectedPlayers.Add(source.PlayerId);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Infect);
                writer.Write(Player.PlayerId);
                writer.Write(source.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }

        public void TurnPestilence()
        {
            var pb = Role.GetRole<Plaguebearer>(Player);
            var role = new Pestilence(Player);
            role.RoleHistory.Add(pb);
            role.RoleHistory.AddRange(pb.RoleHistory);
            Player.RegenTask();

            if (CustomGameOptions.PlayersAlerted)
                Coroutines.Start(Utils.FlashCoroutine(Color));

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Coroutines.Start(Utils.FlashCoroutine(Colors.Seer));
        }
    }
}