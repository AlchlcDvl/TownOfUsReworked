using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Patches;
using UnityEngine;
using TownOfUsReworked.Extensions;
using Hazel;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Altruist : CrewRole
    {
        public CustomButton ReviveButton;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public bool Reviving;
        public float TimeRemaining;
        public bool IsReviving => TimeRemaining > 0f;
        public DeadBody RevivingBody;
        public bool Success;
        public DateTime LastRevived;

        public Altruist(PlayerControl player) : base(player)
        {
            Name = "Altruist";
            StartText = "Sacrifice Yourself To Save Another";
            AbilitiesText = $"- You can revive a dead body\n- Reviving someone takes {CustomGameOptions.AltReviveDuration}s\n- If a meeting is called during your revive, the revive fails";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Altruist : Colors.Crew;
            RoleType = RoleEnum.Altruist;
            RoleAlignment = RoleAlignment.CrewProt;
            AlignmentName = CP;
            InspectorResults = InspectorResults.MeddlesWithDead;
            Type = LayerEnum.Altruist;
            UsesLeft = CustomGameOptions.ReviveCount;
            ReviveButton = new(this, "Revive", AbilityTypes.Dead, "ActionSecondary", HitRevive, true);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            ReviveButton.Update("REVIVE", ReviveTimer(), CustomGameOptions.ReviveCooldown, UsesLeft, IsReviving, TimeRemaining, CustomGameOptions.AltReviveDuration, true, ButtonUsable);
        }

        public override void OnLobby()
        {
            Arrow?.Destroy();
            Arrow?.gameObject?.Destroy();
            Target = null;
        }

        public float ReviveTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastRevived;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ReviveCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Revive()
        {
            if (!Reviving && PlayerControl.LocalPlayer.PlayerId == ReviveButton.TargetBody.ParentId)
            {
                Utils.Flash(Color);

                if (CustomGameOptions.AltruistTargetBody)
                    ReviveButton.TargetBody?.gameObject.Destroy();
            }

            Reviving = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance || Player.Data.IsDead)
            {
                Success = false;
                TimeRemaining = 0f;
            }
        }

        public void UnRevive()
        {
            Reviving = false;
            LastRevived = DateTime.UtcNow;

            if (Success)
                FinishRevive();
        }

        private void FinishRevive()
        {
            var position = RevivingBody.TruePosition;
            var player = Utils.PlayerById(RevivingBody.ParentId);
            var targetRole = GetRole(player);
            targetRole.DeathReason = DeathReasonEnum.Revived;
            targetRole.KilledBy = " By " + PlayerName;
            player.Revive();
            player.Data.SetImpostor(player.Data.IsImpostor());
            UsesLeft--;
            Utils.ReassignPostmortals(player);
            Utils.RecentlyKilled.Remove(player);
            Murder.KilledPlayers.Remove(Murder.KilledPlayers.Find(x => x.PlayerId == player.PlayerId));
            player.NetTransform.SnapTo(new Vector2(position.x, position.y + 0.3636f));
            RevivingBody?.gameObject.Destroy();

            if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer == player)
                SubmergedCompatibility.ChangeFloor(player.transform.position.y > -7);

            if (player.Is(ObjectifierEnum.Lovers) && CustomGameOptions.BothLoversDie)
            {
                var lover = player.GetOtherLover();
                var body = Utils.BodyById(lover.PlayerId);
                var position2 = body.TruePosition;
                lover.Revive();
                Murder.KilledPlayers.Remove(Murder.KilledPlayers.Find(x => x.PlayerId == lover.PlayerId));
                body?.gameObject.Destroy();
                var loverRole = GetRole(lover);
                loverRole.DeathReason = DeathReasonEnum.Revived;
                loverRole.KilledBy = " By " + PlayerName;
                RoleGen.Convert(lover.PlayerId, Player.PlayerId, SubFaction.Reanimated, false);
                Utils.RecentlyKilled.Remove(lover);
                lover.Data.SetImpostor(lover.Data.IsImpostor());
                Utils.ReassignPostmortals(lover);
                lover.NetTransform.SnapTo(new Vector2(position2.x, position2.y + 0.3636f));

                if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer == lover)
                    SubmergedCompatibility.ChangeFloor(lover.transform.position.y > -7);
            }

            if (UsesLeft == 0)
                Utils.RpcMurderPlayer(Player, Player);
        }

        public void HitRevive()
        {
            if (Utils.IsTooFar(Player, ReviveButton.TargetBody) || ReviveTimer() != 0f || !ButtonUsable)
                return;

            var playerId = ReviveButton.TargetBody.ParentId;
            RevivingBody = ReviveButton.TargetBody;
            var player = Utils.PlayerById(playerId);
            Utils.Spread(Player, player);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.AltruistRevive);
            writer.Write(Player.PlayerId);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            TimeRemaining = CustomGameOptions.AltReviveDuration;
            Success = true;
            Revive();
        }
    }
}