using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Custom;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Murderer : NeutralRole
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastKilled;
        public CustomButton MurderButton;

        public Murderer(PlayerControl player) : base(player)
        {
            Name = "Murderer";
            StartText = "Imagine Getting Boring Murderer";
            Objectives = "- Murder anyone who can oppose you";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Murderer : Colors.Neutral;
            RoleType = RoleEnum.Murderer;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = NK;
            Type = LayerEnum.Murderer;
            MurderButton = new(this, AssetManager.Placeholder, AbilityTypes.Direct, "ActionSecondary", Murder);
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
            if (Utils.IsTooFar(Player, ClosestPlayer) || MurderTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, ClosestPlayer, true);

            if (interact[3] || interact[0])
                LastKilled = DateTime.UtcNow;
            else if (interact[1])
                LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            MurderButton.Update("MURDER", MurderTimer(), CustomGameOptions.MurdKCD);
        }
    }
}