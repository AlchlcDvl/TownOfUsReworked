namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Whisperer : Neutral
    {
        public CustomButton WhisperButton;
        public DateTime LastWhispered;
        public int WhisperCount;
        public int ConversionCount;
        public Dictionary<byte, int> PlayerConversion = new();
        public float WhisperConversion;
        public List<byte> Persuaded = new();

        public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Whisperer : Colors.Neutral;
        public override string Name => "Whisperer";
        public override LayerEnum Type => LayerEnum.Whisperer;
        public override RoleEnum RoleType => RoleEnum.Whisperer;
        public override Func<string> StartText => () => "PSST";
        public override Func<string> AbilitiesText => () => "- You can whisper to players around, slowly bending them to your ideals\n- When a player reaches 100% conversion, they will " +
            "defect and join the <color=#F995FCFF>Sect</color>";
        public override InspectorResults InspectorResults => InspectorResults.BringsChaos;

        public Whisperer(PlayerControl player) : base(player)
        {
            Objectives = () => "- Persuade or kill anyone who can oppose the <color=#F995FCFF>Sect</color>";
            RoleAlignment = RoleAlignment.NeutralNeo;
            SubFaction = SubFaction.Sect;
            SubFactionColor = Colors.Sect;
            PlayerConversion = new();
            WhisperConversion = CustomGameOptions.InitialWhisperRate;
            Persuaded = new() { Player.PlayerId };
            WhisperButton = new(this, "Whisper", AbilityTypes.Effect, "ActionSecondary", Whisper);
            SubFactionSymbol = "Λ";
        }

        public float WhisperTimer()
        {
            var timespan = DateTime.UtcNow - LastWhispered;
            var num = Player.GetModifiedCooldown(CustomGameOptions.WhisperCooldown, CustomGameOptions.WhisperCooldownIncreases ? (CustomGameOptions.WhisperCooldownIncrease * WhisperCount) :
                0) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public Dictionary<byte, int> GetPlayers()
        {
            var playerList = new Dictionary<byte, int>();

            foreach (var player in CustomPlayer.AllPlayers)
            {
                if (Player != player)
                    playerList.Add(player.PlayerId, 100);
            }

            return playerList;
        }

        public void Whisper()
        {
            if (WhisperTimer() != 0f)
                return;

            var truePosition = Player.GetTruePosition();
            var closestPlayers = GetClosestPlayers(truePosition, CustomGameOptions.WhisperRadius);
            closestPlayers.Remove(Player);

            if (PlayerConversion.Count == 0)
                PlayerConversion = GetPlayers();

            var oldStats = PlayerConversion;
            PlayerConversion.Clear();

            foreach (var (player, stats) in oldStats)
            {
                var stat = stats;

                if (closestPlayers.Contains(PlayerById(player)))
                    stat -= (int)WhisperConversion;

                if (!PlayerById(player).Data.IsDead)
                    PlayerConversion.Add(player, stat);
            }

            var removals = new Dictionary<byte, int>();

            foreach (var playerConversion in PlayerConversion)
            {
                if (playerConversion.Value <= 0)
                {
                    ConversionCount++;

                    if (CustomGameOptions.WhisperRateDecreases)
                        WhisperConversion -= CustomGameOptions.WhisperRateDecrease;

                    if (WhisperConversion < 2.5f)
                        WhisperConversion = 2.5f;

                    RoleGen.RpcConvert(playerConversion.Key, Player.PlayerId, SubFaction.Sect);
                    removals.Add(playerConversion.Key, playerConversion.Value);
                }
            }

            LastWhispered = DateTime.UtcNow;
            WhisperCount++;

            foreach (var (key, _) in removals)
                PlayerConversion.Remove(key);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            WhisperButton.Update("WHISPER", WhisperTimer(), CustomGameOptions.WhisperCooldown, CustomGameOptions.WhisperCooldownIncreases ? (CustomGameOptions.WhisperCooldownIncrease *
                WhisperCount) : 0);
        }
    }
}