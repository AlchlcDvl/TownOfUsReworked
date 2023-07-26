namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Corrupted : Objectifier
    {
        public DateTime LastCorrupted;
        public CustomButton CorruptButton;

        public override Color32 Color => ClientGameOptions.CustomObjColors ? Colors.Corrupted : Colors.Objectifier;
        public override string Name => "Corrupted";
        public override string Symbol => "δ";
        public override LayerEnum Type => LayerEnum.Corrupted;
        public override ObjectifierEnum ObjectifierType => ObjectifierEnum.Corrupted;
        public override Func<string> TaskText => () => "- Corrupt everyone";

        public Corrupted(PlayerControl player) : base(player)
        {
            CorruptButton = new(this, "Corrupt", AbilityTypes.Direct, "Quarternary", Corrupt);
            Role.GetRole(Player).RoleAlignment = Role.GetRole(Player).RoleAlignment.GetNewAlignment(Faction.Neutral);
        }

        public float CorruptTimer()
        {
            var timespan = DateTime.UtcNow - LastCorrupted;
            var num = Player.GetModifiedCooldown(CustomGameOptions.CorruptedKillCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Corrupt()
        {
            if (CorruptTimer() != 0f || IsTooFar(Player, CorruptButton.TargetPlayer))
                return;

            var interact = Interact(Player, CorruptButton.TargetPlayer, true);

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