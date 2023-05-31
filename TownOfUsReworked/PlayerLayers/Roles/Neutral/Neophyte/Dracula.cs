namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Dracula : NeutralRole
    {
        public DateTime LastBitten;
        public CustomButton BiteButton;
        public List<byte> Converted = new();

        public Dracula(PlayerControl player) : base(player)
        {
            Name = "Dracula";
            RoleType = RoleEnum.Dracula;
            StartText = () => "Lead The <color=#7B8968FF>Undead</color> To Victory";
            AbilitiesText = () => "- You can convert the <color=#8CFFFFFF>Crew</color> into your own sub faction\n- If the target cannot be converted or the number of alive " +
                $"<color=#7B8968FF>Undead</color> exceeds {CustomGameOptions.AliveVampCount}, you will kill them instead\n- Attempting to convert a <color=#C0C0C0FF>Vampire Hunter" +
                "</color> will force them to kill you";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Dracula : Colors.Neutral;
            Objectives = () => "- Convert or kill anyone who can oppose the <color=#7B8968FF>Undead</color>";
            SubFaction = SubFaction.Undead;
            RoleAlignment = RoleAlignment.NeutralNeo;
            SubFactionColor = Colors.Undead;
            Converted = new() { Player.PlayerId };
            Type = LayerEnum.Dracula;
            BiteButton = new(this, "Bite", AbilityTypes.Direct, "ActionSecondary", Convert);
            InspectorResults = InspectorResults.NewLens;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public float ConvertTimer()
        {
            var timespan = DateTime.UtcNow - LastBitten;
            var num = Player.GetModifiedCooldown(CustomGameOptions.BiteCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Convert()
        {
            if (Utils.IsTooFar(Player, BiteButton.TargetPlayer) || ConvertTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, BiteButton.TargetPlayer, false, true);

            if (interact[3])
                RoleGen.RpcConvert(BiteButton.TargetPlayer.PlayerId, Player.PlayerId, SubFaction.Undead, Converted.Count >= CustomGameOptions.AliveVampCount);

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