using System;
using System.Collections.Generic;
using Hazel;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using UnityEngine;
using System.Linq;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Cryomaniac : NeutralRole
    {
        public CustomButton FreezeButton;
        public CustomButton DouseButton;
        public PlayerControl ClosestPlayer;
        public List<byte> DousedPlayers;
        public bool FreezeUsed;
        public DateTime LastDoused;
        public bool LastKiller => !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) ||
            x.Is(RoleAlignment.CrewKill) || x.Is(RoleAlignment.CrewAudit) || x.Is(RoleAlignment.NeutralPros) || (x.Is(RoleAlignment.NeutralKill) && x != Player)));
        public int DousedAlive => DousedPlayers.Count(x => Utils.PlayerById(x)?.Data?.IsDead == false && Utils.PlayerById(x)?.Data?.Disconnected == false);

        public Cryomaniac(PlayerControl player) : base(player)
        {
            Name = "Cryomaniac";
            StartText = "Who Likes Ice Cream?";
            AbilitiesText = "- You can douse players in coolant\n- Doused players can be frozen, which kills all of them at once at the start of the next meeting\n- People who " +
                "interact with you will also get doused";
            Objectives = "- Freeze anyone who can oppose you";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Cryomaniac : Colors.Neutral;
            RoleType = RoleEnum.Cryomaniac;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = NK;
            DousedPlayers = new();
            Type = LayerEnum.Cryomaniac;
            DouseButton = new(this, AssetManager.Placeholder, AbilityTypes.Direct, "ActionSecondary", Douse);
            FreezeButton = new(this, AssetManager.CryoFreeze, AbilityTypes.Effect, "Secondary", Freeze);
        }

        public float DouseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastDoused;
            var num = Player.GetModifiedCooldown(CustomGameOptions.DouseCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void RpcSpreadDouse(PlayerControl source, PlayerControl target)
        {
            if (!source.Is(RoleType) || DousedPlayers.Contains(target.PlayerId))
                return;

            _ = new WaitForSeconds(1f);
            DousedPlayers.Add(target.PlayerId);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.FreezeDouse);
            writer.Write(Player.PlayerId);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);

            foreach (var cryo in GetRoles<Cryomaniac>(RoleEnum.Cryomaniac))
            {
                if (cryo.FreezeUsed)
                {
                    foreach (var player in cryo.DousedPlayers)
                    {
                        var player2 = Utils.PlayerById(player);

                        if (player2.Data.IsDead || player2.Data.Disconnected || player2.Is(RoleEnum.Pestilence) || player2.IsProtected())
                            continue;

                        Utils.RpcMurderPlayer(cryo.Player, player2, DeathReasonEnum.Frozen);
                    }

                    cryo.DousedPlayers.Clear();
                    cryo.FreezeUsed = false;
                }
            }
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var notDoused = PlayerControl.AllPlayerControls.ToArray().Where(player => !DousedPlayers.Contains(player.PlayerId)).ToList();
            DouseButton.Update("DOUSE", DouseTimer(), CustomGameOptions.DouseCd, notDoused);
            FreezeButton.Update("FREEZE", 0, 1, true, DousedAlive > 0 && !FreezeUsed);
        }

        public void Douse()
        {
            if (Utils.IsTooFar(Player, ClosestPlayer) || DouseTimer() != 0f || DousedPlayers.Contains(ClosestPlayer.PlayerId))
                return;

            var interact = Utils.Interact(Player, ClosestPlayer, LastKiller);

            if (interact[3])
                RpcSpreadDouse(Player, ClosestPlayer);

            if (interact[0])
                LastDoused = DateTime.UtcNow;
            else if (interact[1])
                LastDoused.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastDoused.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public void Freeze()
        {
            if (DousedAlive <= 0 || FreezeUsed)
                return;

            FreezeUsed = true;
        }
    }
}
