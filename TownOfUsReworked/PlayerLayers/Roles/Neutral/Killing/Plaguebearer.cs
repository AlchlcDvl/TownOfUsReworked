using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Custom;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Plaguebearer : NeutralRole
    {
        public DateTime LastInfected;
        public List<byte> InfectedPlayers = new();
        public int InfectedAlive => InfectedPlayers.Count;
        public bool CanTransform => PlayerControl.AllPlayerControls.ToArray().Count(x => x?.Data.IsDead == false && !x.Data.Disconnected) <= InfectedAlive;
        public CustomButton InfectButton;

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
            Type = LayerEnum.Plaguebearer;
            InfectButton = new(this, "Infect", AbilityTypes.Direct, "ActionSecondary", Infect);
            InspectorResults = InspectorResults.SeeksToDestroy;
        }

        public float InfectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastInfected;
            var num = Player.GetModifiedCooldown(CustomGameOptions.InfectCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void RpcSpreadInfection(PlayerControl source, PlayerControl target)
        {
            if (InfectedPlayers.Contains(source.PlayerId) && InfectedPlayers.Contains(target.PlayerId))
                return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Infect);
            writer.Write(Player.PlayerId);

            if (InfectedPlayers.Contains(source.PlayerId) || source.Is(RoleEnum.Plaguebearer))
            {
                InfectedPlayers.Add(target.PlayerId);
                writer.Write(target.PlayerId);
            }
            else if (InfectedPlayers.Contains(target.PlayerId) || target.Is(RoleEnum.Plaguebearer))
            {
                InfectedPlayers.Add(source.PlayerId);
                writer.Write(source.PlayerId);
            }

            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public void Infect()
        {
            if (Utils.IsTooFar(Player, InfectButton.TargetPlayer) || InfectTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, InfectButton.TargetPlayer);

            if (interact[3])
                RpcSpreadInfection(Player, InfectButton.TargetPlayer);

            if (interact[0])
                LastInfected = DateTime.UtcNow;
            else if (interact[1])
                LastInfected.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void TurnPestilence()
        {
            var newRole = new Pestilence(Player);
            newRole.RoleUpdate(this);

            if (Player == PlayerControl.LocalPlayer || CustomGameOptions.PlayersAlerted)
                Utils.Flash(Colors.Pestilence);
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var notInfected = PlayerControl.AllPlayerControls.ToArray().Where(player => !InfectedPlayers.Contains(player.PlayerId)).ToList();
            InfectButton.Update("INFECT", InfectTimer(), CustomGameOptions.InfectCd, notInfected, true, !CanTransform);
            InfectedPlayers.RemoveAll(x => Utils.PlayerById(x).Data.IsDead || Utils.PlayerById(x).Data.Disconnected);

            if (CanTransform && PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList().Count > 1 && !IsDead)
            {
                TurnPestilence();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnPestilence);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}