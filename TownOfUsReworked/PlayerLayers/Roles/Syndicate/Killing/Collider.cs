using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using System;
using TownOfUsReworked.Custom;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Collider : SyndicateRole
    {
        public CustomButton PositiveButton;
        public CustomButton NegativeButton;
        public DateTime LastPositive;
        public DateTime LastNegative;
        public PlayerControl Positive;
        public PlayerControl Negative;

        public Collider(PlayerControl player) : base(player)
        {
            Name = "Collider";
            RoleType = RoleEnum.Collider;
            StartText = "FUUUUUUUUUUUUUUUUUUUUUUUUUUSION!";
            RoleAlignment = RoleAlignment.SyndicateKill;
            AlignmentName = SyK;
            Type = LayerEnum.Collider;
            InspectorResults = InspectorResults.MovesAround;
            Positive = null;
            Negative = null;
            PositiveButton = new(this, "Positive", AbilityTypes.Direct, "ActionSecondary", SetPositive, Exception1);
            NegativeButton = new(this, "Negative", AbilityTypes.Direct, "Secondary", SetNegative, Exception2);
        }

        public float PositiveTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastPositive;
            var num = Player.GetModifiedCooldown(CustomGameOptions.CollideCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float NegativeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastNegative;
            var num = Player.GetModifiedCooldown(CustomGameOptions.CollideCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void SetPositive()
        {
            if (HoldsDrive || Utils.IsTooFar(Player, PositiveButton.TargetPlayer) || PositiveTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, PositiveButton.TargetPlayer);

            if (interact[3])
                Positive = PositiveButton.TargetPlayer;

            if (interact[0])
                LastPositive = DateTime.UtcNow;
            else if (interact[1])
                LastPositive.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void SetNegative()
        {
            if (HoldsDrive || Utils.IsTooFar(Player, NegativeButton.TargetPlayer) || NegativeTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, NegativeButton.TargetPlayer);

            if (interact[3])
                Negative = NegativeButton.TargetPlayer;

            if (interact[0])
                LastNegative = DateTime.UtcNow;
            else if (interact[1])
                LastNegative.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public bool Exception1(PlayerControl player) => player == Negative || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None);

        public bool Exception2(PlayerControl player) => player == Positive || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            PositiveButton.Update("SET POSITIVE", PositiveTimer(), CustomGameOptions.CollideCooldown);
            NegativeButton.Update("SET NEGATIVE", NegativeTimer(), CustomGameOptions.CollideCooldown);
            var range = CustomGameOptions.CollideRange + (HoldsDrive ? CustomGameOptions.CollideRangeIncrease : 0);

            if (Utils.GetDistBetweenPlayers(Positive, Negative) <= range)
            {
                Utils.RpcMurderPlayer(Player, Positive, DeathReasonEnum.Collided, false);
                Utils.RpcMurderPlayer(Player, Negative, DeathReasonEnum.Collided, false);
            }
        }

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);
            Positive = null;
            Negative = null;
        }
    }
}