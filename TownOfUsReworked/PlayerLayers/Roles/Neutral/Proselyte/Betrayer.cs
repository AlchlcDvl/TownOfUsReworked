using TownOfUsReworked.Data;
using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Custom;
using TownOfUsReworked.Classes;
using System.Linq;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Betrayer : NeutralRole
    {
        public CustomButton KillButton;
        public DateTime LastKilled;

        public Betrayer(PlayerControl player) : base(player)
        {
            Name = "Betrayer";
            StartText = "Those Guys Backs Are Ripe For Some Backstabbing";
            Objectives = $"- Kill anyone who opposes the {FactionName}";
            RoleType = RoleEnum.Betrayer;
            Color = CustomGameOptions.CustomNeutColors ? Colors.Betrayer : Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralPros;
            AlignmentName = NP;
            Type = LayerEnum.Betrayer;
            KillButton = new(this, "BetKill", AbilityTypes.Direct, "ActionSecondary", Kill);
            InspectorResults = InspectorResults.IsAggressive;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastKilled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.BetrayerKillCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Kill()
        {
            if (Utils.IsTooFar(Player, KillButton.TargetPlayer) || KillTimer() != 0f || Faction == Faction.Neutral)
                return;

            var interact = Utils.Interact(Player, KillButton.TargetPlayer, true);

            if (interact[3] || interact[0])
                LastKilled = DateTime.UtcNow;
            else if (interact[1])
                LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public override void UpdateHud(HudManager __instance)
        {
            if (Faction == Faction.Neutral)
                return;

            base.UpdateHud(__instance);
            var notMates = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction)).ToList();
            KillButton.Update("KILL", KillTimer(), CustomGameOptions.BetrayerKillCooldown, notMates);
        }
    }
}