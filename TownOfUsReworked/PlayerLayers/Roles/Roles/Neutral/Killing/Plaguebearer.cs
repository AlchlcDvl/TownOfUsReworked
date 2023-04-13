using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Plaguebearer : NeutralRole
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastInfected;
        public List<byte> InfectedPlayers = new();
        public int InfectedAlive => InfectedPlayers.Count;
        public bool CanTransform => PlayerControl.AllPlayerControls.ToArray().Count(x => x?.Data.IsDead == false && !x.Data.Disconnected) <= InfectedAlive;
        public AbilityButton InfectButton;

        public Plaguebearer(PlayerControl player) : base(player)
        {
            Name = "Plaguebearer";
            StartText = "Spread Disease To Become Pestilence";
            AbilitiesText = "- You can infect players\n- When all players are infected, you will turn into <color=#424242FF>Pestilence</color>";
            Objectives = "- Infect or kill anyone who can oppose you\n- Infect everyone to become <color=#424242FF>Pestilence</color>";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Plaguebearer : Colors.Neutral;
            RoleType = RoleEnum.Plaguebearer;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = NK;
            InfectedPlayers = new();
        }

        public float InfectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastInfected;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.InfectCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void RpcSpreadInfection(PlayerControl source, PlayerControl target)
        {
            if (InfectedPlayers.Contains(source.PlayerId) && InfectedPlayers.Contains(target.PlayerId))
                return;

            _ = new WaitForSeconds(1f);

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
            InfectButton.gameObject.SetActive(false);
            var role = new Pestilence(Player);
            role.RoleUpdate(this);

            if (Player == PlayerControl.LocalPlayer)
                Utils.Flash(Colors.Pestilence, "Everyone has been infected, you feel your body transforming!");

            if (CustomGameOptions.PlayersAlerted)
                Utils.Flash(Colors.Pestilence, "A plague has spread within the crew, summoning <color=#424242FF>Pestilence</color>!");
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer, "Someone has changed their identity!");
        }
    }
}