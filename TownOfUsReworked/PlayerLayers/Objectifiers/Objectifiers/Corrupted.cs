using TownOfUsReworked.CustomOptions;
using System;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Corrupted : Objectifier
    {
        public DateTime LastKilled;
        public PlayerControl ClosestPlayer;
        public CustomButton KillButton;

        public Corrupted(PlayerControl player) : base(player)
        {
            Name = "Corrupted";
            SymbolName = "δ";
            TaskText = "- Kill everyone";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Corrupted : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Corrupted;
            Type = LayerEnum.Corrupted;
            KillButton = new(this, AssetManager.CorruptedKill, AbilityTypes.Direct, "Quarternary", Kill);
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastKilled;
            var num = CustomGameOptions.CorruptedKillCooldown * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Kill()
        {
            if (KillTimer() != 0f || Utils.IsTooFar(Player, ClosestPlayer))
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
            KillButton.Update("KILL", KillTimer(), CustomGameOptions.CorruptedKillCooldown);
        }
    }
}