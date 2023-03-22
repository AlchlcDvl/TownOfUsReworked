using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Arsonist : NeutralRole
    {
        public AbilityButton IgniteButton;
        public AbilityButton DouseButton;
        public bool LastKiller => PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) ||
            x.Is(RoleAlignment.CrewKill) || x.Is(RoleAlignment.CrewAudit) || x.Is(RoleAlignment.NeutralPros) || (x.Is(RoleAlignment.NeutralKill) && x != Player))).ToList().Count() == 0;
        public PlayerControl ClosestPlayer = null;
        public List<byte> DousedPlayers = new List<byte>();
        public DateTime LastDoused;
        public DateTime LastIgnited;
        public int DousedAlive => DousedPlayers.Count;

        public Arsonist(PlayerControl player) : base(player)
        {
            Name = "Arsonist";
            StartText = "Gasoline + Bean + Lighter = Bonfire";
            AbilitiesText = "- You can douse players in gasoline.\n- Doused players can then be ignite to kill all doused players at once.\n- People who interact with you will also " +
                "get doused.";
            RoleType = RoleEnum.Arsonist;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = NK;
            Color = CustomGameOptions.CustomNeutColors ? Colors.Arsonist : Colors.Neutral;
            DousedPlayers = new List<byte>();
        }

        public float DouseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDoused;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.DouseCd) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public float IgniteTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastIgnited;
            var num = CustomGameOptions.IgniteCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Ignite()
        {
            foreach (var arso in Role.GetRoles(RoleEnum.Arsonist))
            {
                var arso2 = (Arsonist)arso;

                foreach (var playerId in arso2.DousedPlayers)
                {
                    var player = Utils.PlayerById(playerId);

                    if (player == null || player.Data.Disconnected || player.Data.IsDead || player.Is(RoleEnum.Pestilence) || player.IsProtected())
                        continue;

                    Utils.RpcMurderPlayer(Player, player, false);
                }

                arso2.DousedPlayers.Clear();
            }
        }

        public void RpcSpreadDouse(PlayerControl source, PlayerControl target)
        {
            new WaitForSeconds(1f);

            if (!source.Is(RoleType))
                return;

            DousedPlayers.Add(target.PlayerId);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Douse);
            writer.Write(Player.PlayerId);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }
}
