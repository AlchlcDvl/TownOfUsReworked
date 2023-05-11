using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using TownOfUsReworked.Objects;
using Object = UnityEngine.Object;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Arsonist : NeutralRole
    {
        public CustomButton IgniteButton;
        public CustomButton DouseButton;
        public bool LastKiller => !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) ||
            x.Is(RoleAlignment.CrewKill) || x.Is(RoleAlignment.CrewAudit) || x.Is(RoleAlignment.NeutralPros) || (x.Is(RoleAlignment.NeutralKill) && x != Player))) &&
            CustomGameOptions.ArsoLastKillerBoost;
        public List<byte> Doused = new();
        public DateTime LastDoused;
        public DateTime LastIgnited;
        public int DousedAlive => Doused.Count;

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
            Doused = new();
            Type = LayerEnum.Arsonist;
            DouseButton = new(this, "ArsoDouse", AbilityTypes.Direct, "ActionSecondary", Douse, Exception);
            IgniteButton = new(this, "Ignite", AbilityTypes.Effect, "Secondary", Ignite);
            InspectorResults = InspectorResults.SeeksToDestroy;
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
            if (IgniteTimer() != 0f || Doused.Count == 0)
                return;

            foreach (var arso in GetRoles<Arsonist>(RoleEnum.Arsonist))
            {
                if (arso.Player != Player && !CustomGameOptions.ArsoIgniteAll)
                    continue;

                foreach (var playerId in arso.Doused)
                {
                    var player = Utils.PlayerById(playerId);

                    if (player?.Data.Disconnected == true || player.Data.IsDead || player.Is(RoleEnum.Pestilence) || player.IsProtected())
                        continue;

                    Utils.RpcMurderPlayer(Player, player, DeathReasonEnum.Ignited, false);
                }

                if (CustomGameOptions.IgnitionCremates)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Burn);
                    writer.Write(arso.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    foreach (var body in Object.FindObjectsOfType<DeadBody>())
                    {
                        if (arso.Doused.Contains(body.ParentId) && Utils.PlayerById(body.ParentId).Data.IsDead)
                        {
                            Coroutines.Start(Utils.FadeBody(body));
                            _ = new Ash(body.TruePosition);
                        }
                    }
                }

                arso.Doused.Clear();
            }

            if (!LastKiller)
                LastIgnited = DateTime.UtcNow;

            if (CustomGameOptions.ArsoCooldownsLinked)
                LastDoused = DateTime.UtcNow;
        }

        public void Douse()
        {
            if (Utils.IsTooFar(Player, DouseButton.TargetPlayer) || DouseTimer() != 0f || Doused.Contains(DouseButton.TargetPlayer.PlayerId))
                return;

            var interact = Utils.Interact(Player, DouseButton.TargetPlayer);

            if (interact[3])
                RpcSpreadDouse(Player, DouseButton.TargetPlayer);

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
            if (!source.Is(RoleType) || Doused.Contains(target.PlayerId))
                return;

            Doused.Add(target.PlayerId);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Douse);
            writer.Write(PlayerId);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public bool Exception(PlayerControl player) => Doused.Contains(player.PlayerId) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction
            is Faction.Intruder or Faction.Syndicate) || player == Player.GetOtherLover() || player == Player.GetOtherRival() || (player.Is(ObjectifierEnum.Mafia) &&
            Player.Is(ObjectifierEnum.Mafia));

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            DouseButton.Update("DOUSE", DouseTimer(), CustomGameOptions.DouseCd);
            IgniteButton.Update("IGNITE", IgniteTimer(), CustomGameOptions.IgniteCd, true, DousedAlive > 0);
        }
    }
}
