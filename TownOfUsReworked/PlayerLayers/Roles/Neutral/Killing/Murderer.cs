using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Custom;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Murderer : NeutralRole
    {
        public DateTime LastKilled;
        public CustomButton MurderButton;

        public Murderer(PlayerControl player) : base(player)
        {
            Name = "Murderer";
            StartText = "Imagine Getting Boring Murderer";
            Objectives = "- Murder anyone who can oppose you";
            AbilitiesText = "- You can kill";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Murderer : Colors.Neutral;
            RoleType = RoleEnum.Murderer;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = NK;
            Type = LayerEnum.Murderer;
            MurderButton = new(this, "Murder", AbilityTypes.Direct, "ActionSecondary", Murder, Exception);
            InspectorResults = InspectorResults.IsBasic;
        }

        public float MurderTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastKilled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.MurdKCD) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Murder()
        {
            if (Utils.IsTooFar(Player, MurderButton.TargetPlayer) || MurderTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, MurderButton.TargetPlayer, true);

            if (interact[3] || interact[0])
                LastKilled = DateTime.UtcNow;
            else if (interact[1])
                LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
            player == Player.GetOtherLover() || player == Player.GetOtherRival() || (player.Is(ObjectifierEnum.Mafia) && Player.Is(ObjectifierEnum.Mafia));

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            MurderButton.Update("MURDER", MurderTimer(), CustomGameOptions.MurdKCD);
        }
    }
}