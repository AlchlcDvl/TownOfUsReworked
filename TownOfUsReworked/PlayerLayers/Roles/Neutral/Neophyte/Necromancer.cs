using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using System.Collections.Generic;
using System;
using UnityEngine;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;
using Hazel;
using TownOfUsReworked.Custom;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Necromancer : NeutralRole
    {
        public DeadBody ResurrectingBody;
        public bool Success;
        public CustomButton ResurrectButton;
        public CustomButton KillButton;
        public List<byte> Resurrected = new();
        public int ResurrectUsesLeft;
        public bool ResurrectButtonUsable => ResurrectUsesLeft > 0;
        public int KillUsesLeft;
        public bool KillButtonUsable => KillUsesLeft > 0;
        public DateTime LastKilled;
        public DateTime LastResurrected;
        public int ResurrectedCount;
        public int KillCount;
        public bool Resurrecting;
        public float TimeRemaining;
        public bool IsResurrecting => TimeRemaining > 0f;

        public Necromancer(PlayerControl player) : base(player)
        {
            Name = "Necromancer";
            StartText = "Resurrect The Dead Into Doing Your Bidding";
            AbilitiesText = "- You can resurrect a dead body and bring them into the <color=#E6108AFF>Reanimated</color>\n- You can kill players to speed up the process";
            Objectives = "- Resurrect or kill anyone who can oppose the <color=#E6108AFF>Reanimated</color>";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Necromancer : Colors.Neutral;
            RoleType = RoleEnum.Necromancer;
            RoleAlignment = RoleAlignment.NeutralNeo;
            AlignmentName = NN;
            Objectives = "- Resurrect the dead into helping you gain control of the crew";
            SubFaction = SubFaction.Reanimated;
            SubFactionColor = Colors.Reanimated;
            ResurrectUsesLeft = CustomGameOptions.ResurrectCount;
            KillUsesLeft = CustomGameOptions.NecroKillCount;
            ResurrectedCount = 0;
            KillCount = 0;
            Resurrected = new() { Player.PlayerId };
            Type = LayerEnum.Necromancer;
            ResurrectButton = new(this, "Ressurect", AbilityTypes.Dead, "ActionSecondary", HitResurrect, true);
            KillButton = new(this, "NecroKill", AbilityTypes.Direct, "Secondary", Kill, Exception, true);
            InspectorResults = InspectorResults.PreservesLife;
        }

        public float ResurrectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastResurrected;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ResurrectCooldown, ResurrectedCount * CustomGameOptions.ResurrectCooldownIncrease) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastKilled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.NecroKillCooldown, KillCount * CustomGameOptions.NecroKillCooldownIncrease) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Resurrect()
        {
            if (!Resurrecting && PlayerControl.LocalPlayer.PlayerId == ResurrectButton.TargetBody.ParentId)
            {
                Utils.Flash(Colors.Reanimated, CustomGameOptions.NecroResurrectDuration);

                if (CustomGameOptions.NecromancerTargetBody)
                    ResurrectButton.TargetBody?.gameObject.Destroy();
            }

            Resurrecting = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance || IsDead)
            {
                Success = false;
                TimeRemaining = 0f;
            }
        }

        public void UnResurrect()
        {
            Resurrecting = false;
            LastResurrected = DateTime.UtcNow;

            if (Success)
                FinishResurrect();
        }

        private void FinishResurrect()
        {
            var player = Utils.PlayerByBody(ResurrectingBody);
            var targetRole = GetRole(player);
            targetRole.DeathReason = DeathReasonEnum.Revived;
            targetRole.KilledBy = " By " + PlayerName;
            RoleGen.Convert(player.PlayerId, Player.PlayerId, SubFaction.Reanimated, false);
            ResurrectedCount++;
            ResurrectUsesLeft--;
            Utils.Revive(player);

            if (player.Is(ObjectifierEnum.Lovers) && CustomGameOptions.BothLoversDie)
            {
                var lover = player.GetOtherLover();
                var loverRole = GetRole(lover);
                loverRole.DeathReason = DeathReasonEnum.Revived;
                loverRole.KilledBy = " By " + PlayerName;
                RoleGen.Convert(lover.PlayerId, Player.PlayerId, SubFaction.Reanimated, false);
                Utils.Revive(lover);
            }
        }

        public bool Exception(PlayerControl player) => Resurrected.Contains(player.PlayerId) || player == Player.GetOtherLover() || player == Player.GetOtherRival() ||
            (player.Is(ObjectifierEnum.Mafia) && Player.Is(ObjectifierEnum.Mafia));

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            KillButton.Update("KILL", KillTimer(), CustomGameOptions.NecroKillCooldown + (KillCount * CustomGameOptions.NecroKillCooldownIncrease), KillUsesLeft, true, KillButtonUsable);
            ResurrectButton.Update("RESURRECT", ResurrectTimer(), CustomGameOptions.ResurrectCooldown + (ResurrectedCount * CustomGameOptions.ResurrectCooldownIncrease), ResurrectUsesLeft,
                IsResurrecting, TimeRemaining, CustomGameOptions.NecroResurrectDuration, true, ResurrectButtonUsable);
        }

        public void HitResurrect()
        {
            if (Utils.IsTooFar(Player, ResurrectButton.TargetBody) || ResurrectTimer() != 0f || !ResurrectButtonUsable)
                return;

            if (RoleGen.Convertible <= 0 || !Utils.PlayerByBody(ResurrectButton.TargetBody).Is(SubFaction.None))
            {
                Utils.Flash(new Color32(255, 0, 0, 255));
                LastResurrected = DateTime.UtcNow;
            }
            else
            {
                var playerId = ResurrectButton.TargetBody.ParentId;
                ResurrectingBody = ResurrectButton.TargetBody;
                var player = Utils.PlayerById(playerId);
                Utils.Spread(Player, player);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.NecromancerResurrect);
                writer.Write(PlayerId);
                writer.Write(playerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.NecroResurrectDuration;
                Success = true;
                Resurrect();

                if (CustomGameOptions.KillResurrectCooldownsLinked)
                    LastKilled = DateTime.UtcNow;
            }
        }

        public void Kill()
        {
            if (KillTimer() != 0f || Utils.IsTooFar(Player, KillButton.TargetPlayer) || !KillButtonUsable)
                return;

            var interact = Utils.Interact(Player, KillButton.TargetPlayer, true);

            if (interact[3])
            {
                KillCount++;
                KillUsesLeft--;
            }

            if (interact[0])
            {
                LastKilled = DateTime.UtcNow;

                if (CustomGameOptions.KillResurrectCooldownsLinked)
                    LastResurrected = DateTime.UtcNow;
            }
            else if (interact[1])
            {
                LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);

                if (CustomGameOptions.KillResurrectCooldownsLinked)
                    LastResurrected.AddSeconds(CustomGameOptions.ProtectKCReset);
            }
            else if (interact[2])
            {
                LastKilled.AddSeconds(CustomGameOptions.VestKCReset);

                if (CustomGameOptions.KillResurrectCooldownsLinked)
                    LastResurrected.AddSeconds(CustomGameOptions.VestKCReset);
            }
        }
    }
}