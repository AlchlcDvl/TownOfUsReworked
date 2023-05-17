namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Corrupted : Objectifier
    {
        public DateTime LastCorrupted;
        public CustomButton CorruptButton;

        public Corrupted(PlayerControl player) : base(player)
        {
            Name = "Corrupted";
            Symbol = "δ";
            TaskText = "- Corrupt everyone";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Corrupted : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Corrupted;
            Type = LayerEnum.Corrupted;
            CorruptButton = new(this, "CorruptedCorrupt", AbilityTypes.Direct, "Quarternary", Corrupt);
            Role.GetRole(Player).RoleAlignment = Role.GetRole(Player).RoleAlignment.GetNewAlignment(Faction.Neutral);
        }

        public float CorruptTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastCorrupted;
            var num = CustomGameOptions.CorruptedKillCooldown * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Corrupt()
        {
            if (CorruptTimer() != 0f || Utils.IsTooFar(Player, CorruptButton.TargetPlayer))
                return;

            var interact = Utils.Interact(Player, CorruptButton.TargetPlayer, true);

            if (interact[3] || interact[0])
                LastCorrupted = DateTime.UtcNow;
            else if (interact[1])
                LastCorrupted.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastCorrupted.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            CorruptButton.Update("CORRUPT", CorruptTimer(), CustomGameOptions.CorruptedKillCooldown);
        }
    }
}