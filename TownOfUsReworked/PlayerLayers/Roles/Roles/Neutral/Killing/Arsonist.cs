using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Arsonist : NeutralRole
    {
        public CustomButton IgniteButton;
        public CustomButton DouseButton;
        public bool LastKiller => !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) ||
            x.Is(RoleAlignment.CrewKill) || x.Is(RoleAlignment.CrewAudit) || x.Is(RoleAlignment.NeutralPros) || (x.Is(RoleAlignment.NeutralKill) && x != Player))) &&
            CustomGameOptions.ArsoLastKillerBoost;
        public PlayerControl ClosestPlayer;
        public List<byte> DousedPlayers = new();
        public DateTime LastDoused;
        public DateTime LastIgnited;
        public int DousedAlive => DousedPlayers.Count;

        public Arsonist(PlayerControl player) : base(player)
        {
            Name = "Arsonist";
            StartText = "PYROMANIAAAAAAAAAAAAAA";
            AbilitiesText = "- You can douse players in gasoline\n- Doused players can be ignited, killing them all at once\n- People who interact with you will also get doused";
            Objectives = "- Burn anyone who can oppose you";
            RoleType = RoleEnum.Arsonist;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = NK;
            Color = CustomGameOptions.CustomNeutColors ? Colors.Arsonist : Colors.Neutral;
            DousedPlayers = new();
            Type = LayerEnum.Arsonist;
            DouseButton = new(this, AssetManager.ArsoDouse, AbilityTypes.Effect, "ActionSecondary", Douse);
            IgniteButton = new(this, AssetManager.Ignite, AbilityTypes.Direct, "Secondary", Ignite);
        }

        public float DouseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastDoused;
            var num = Player.GetModifiedCooldown(CustomGameOptions.DouseCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float IgniteTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastIgnited;
            var num = Player.GetModifiedCooldown(CustomGameOptions.IgniteCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Ignite()
        {
            if (IgniteTimer() != 0f || DousedPlayers.Count == 0)
                return;

            foreach (var arso in GetRoles<Arsonist>(RoleEnum.Arsonist))
            {
                if (arso.Player != Player && !CustomGameOptions.ArsoIgniteAll)
                    continue;

                foreach (var playerId in arso.DousedPlayers)
                {
                    var player = Utils.PlayerById(playerId);

                    if (player?.Data.Disconnected == true || player.Data.IsDead || player.Is(RoleEnum.Pestilence) || player.IsProtected())
                        continue;

                    Utils.RpcMurderPlayer(Player, player, DeathReasonEnum.Ignited, false);
                }

                arso.DousedPlayers.Clear();
            }

            if (!LastKiller)
                LastIgnited = DateTime.UtcNow;

            if (CustomGameOptions.ArsoCooldownsLinked)
                LastDoused = DateTime.UtcNow;
        }

        public void Douse()
        {
            if (Utils.IsTooFar(Player, ClosestPlayer) || DouseTimer() != 0f || DousedPlayers.Contains(ClosestPlayer.PlayerId))
                return;

            var interact = Utils.Interact(Player, ClosestPlayer);

            if (interact[3])
                RpcSpreadDouse(Player, ClosestPlayer);

            if (interact[0])
            {
                LastDoused = DateTime.UtcNow;

                if (CustomGameOptions.ArsoCooldownsLinked && !LastKiller)
                    LastIgnited = DateTime.UtcNow;
            }
            else if (interact[1])
            {
                LastDoused.AddSeconds(CustomGameOptions.ProtectKCReset);

                if (CustomGameOptions.ArsoCooldownsLinked && !LastKiller)
                    LastIgnited.AddSeconds(CustomGameOptions.ProtectKCReset);
            }
        }

        public void RpcSpreadDouse(PlayerControl source, PlayerControl target)
        {
            if (!source.Is(RoleType) || DousedPlayers.Contains(target.PlayerId))
                return;

            _ = new WaitForSeconds(1f);
            DousedPlayers.Add(target.PlayerId);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Douse);
            writer.Write(Player.PlayerId);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var notDoused = PlayerControl.AllPlayerControls.ToArray().Where(player => !DousedPlayers.Contains(player.PlayerId)).ToList();
            DouseButton.Update("DOUSE", DouseTimer(), CustomGameOptions.DouseCd, notDoused);
            IgniteButton.Update("IGNITE", IgniteTimer(), CustomGameOptions.IgniteCd, true, DousedAlive > 0);
        }
    }
}
