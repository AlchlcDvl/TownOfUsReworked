namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Whisperer : NeutralRole
    {
        public CustomButton WhisperButton;
        public DateTime LastWhispered;
        public int WhisperCount;
        public int ConversionCount;
        public List<(byte, int)> PlayerConversion = new();
        public float WhisperConversion;
        public List<byte> Persuaded = new();

        public Whisperer(PlayerControl player) : base(player)
        {
            Name = "Whisperer";
            Color = Colors.Whisperer;
            AbilitiesText = () => "- You can whisper to players around, slowly bending them to your ideals\n- When a player reaches 100% conversion, they will defect and join the " +
                "<color=#F995FCFF>Sect</color>";
            Objectives = () => "- Persuade or kill anyone who can oppose the <color=#F995FCFF>Sect</color>";
            RoleType = RoleEnum.Whisperer;
            RoleAlignment = RoleAlignment.NeutralNeo;
            SubFaction = SubFaction.Sect;
            SubFactionColor = Colors.Sect;
            PlayerConversion = new();
            WhisperConversion = CustomGameOptions.InitialWhisperRate;
            Persuaded = new() { Player.PlayerId };
            Type = LayerEnum.Whisperer;
            WhisperButton = new(this, "Whisper", AbilityTypes.Effect, "ActionSecondary", Whisper);
            InspectorResults = InspectorResults.BringsChaos;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public float WhisperTimer()
        {
            var timespan = DateTime.UtcNow - LastWhispered;
            var num = Player.GetModifiedCooldown(CustomGameOptions.WhisperCooldown, CustomGameOptions.WhisperCooldownIncrease * WhisperCount) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public List<(byte, int)> GetPlayers()
        {
            var playerList = new List<(byte, int)>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (Player != player)
                    playerList.Add((player.PlayerId, 100));
            }

            return playerList;
        }

        public void Whisper()
        {
            if (WhisperTimer() != 0f)
                return;

            var truePosition = Player.GetTruePosition();
            var closestPlayers = Utils.GetClosestPlayers(truePosition, CustomGameOptions.WhisperRadius);
            closestPlayers.Remove(Player);

            if (PlayerConversion.Count == 0)
                PlayerConversion = GetPlayers();

            var oldStats = PlayerConversion;
            PlayerConversion.Clear();

            foreach (var conversionRate in oldStats)
            {
                var player = conversionRate.Item1;
                var stats = conversionRate.Item2;

                if (closestPlayers.Contains(Utils.PlayerById(player)))
                    stats -= (int)WhisperConversion;

                if (!Utils.PlayerById(player).Data.IsDead)
                    PlayerConversion.Add((player, stats));
            }

            var removals = new List<(byte, int)>();

            foreach (var playerConversion in PlayerConversion)
            {
                if (playerConversion.Item2 <= 0)
                {
                    ConversionCount++;

                    if (CustomGameOptions.WhisperRateDecreases)
                        WhisperConversion -= CustomGameOptions.WhisperRateDecrease;

                    if (WhisperConversion < 2.5f)
                        WhisperConversion = 2.5f;

                    RoleGen.RpcConvert(playerConversion.Item1, Player.PlayerId, SubFaction.Sect);
                    removals.Add(playerConversion);
                }
            }

            PlayerConversion.RemoveRange(removals);
            LastWhispered = DateTime.UtcNow;
            WhisperCount++;
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            WhisperButton.Update("WHISPER", WhisperTimer(), CustomGameOptions.WhisperCooldown + (WhisperCount * CustomGameOptions.WhisperCooldownIncrease));
        }
    }
}