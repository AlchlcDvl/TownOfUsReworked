namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Dracula : Neutral
    {
        public DateTime LastBitten;
        public CustomButton BiteButton;
        public List<byte> Converted = new();
        public int AliveCount => Converted.Count(x => PlayerById(x) != null && !PlayerById(x).Data.IsDead && !PlayerById(x).Data.Disconnected);

        public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Dracula : Colors.Neutral;
        public override string Name => "Dracula";
        public override LayerEnum Type => LayerEnum.Dracula;
        public override RoleEnum RoleType => RoleEnum.Dracula;
        public override Func<string> StartText => () => "Lead The <color=#7B8968FF>Undead</color> To Victory";
        public override Func<string> AbilitiesText => () => "- You can convert the <color=#8CFFFFFF>Crew</color> into your own sub faction\n- If the target cannot be converted or the " +
            $"number of alive <color=#7B8968FF>Undead</color> exceeds {CustomGameOptions.AliveVampCount}, you will kill them instead\n- Attempting to convert a <color=#C0C0C0FF>Vampire " +
            "Hunter</color> will force them to kill you";
        public override InspectorResults InspectorResults => InspectorResults.NewLens;

        public Dracula(PlayerControl player) : base(player)
        {
            Objectives = () => "- Convert or kill anyone who can oppose the <color=#7B8968FF>Undead</color>";
            SubFaction = SubFaction.Undead;
            RoleAlignment = RoleAlignment.NeutralNeo;
            SubFactionColor = Colors.Undead;
            Converted = new() { Player.PlayerId };
            BiteButton = new(this, "Bite", AbilityTypes.Direct, "ActionSecondary", Convert);
            SubFactionSymbol = "γ";
        }

        public float ConvertTimer()
        {
            var timespan = DateTime.UtcNow - LastBitten;
            var num = Player.GetModifiedCooldown(CustomGameOptions.BiteCd) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Convert()
        {
            if (IsTooFar(Player, BiteButton.TargetPlayer) || ConvertTimer() != 0f)
                return;

            var interact = Interact(Player, BiteButton.TargetPlayer, false, true);

            if (interact[3])
                RoleGen.RpcConvert(BiteButton.TargetPlayer.PlayerId, Player.PlayerId, SubFaction.Undead, AliveCount >= CustomGameOptions.AliveVampCount);

            if (interact[0])
                LastBitten = DateTime.UtcNow;
            else if (interact[1])
                LastBitten.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastBitten.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public bool Exception(PlayerControl player) => Converted.Contains(player.PlayerId);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            BiteButton.Update("BITE", ConvertTimer(), CustomGameOptions.BiteCd);
        }
    }
}