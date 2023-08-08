namespace TownOfUsReworked.PlayerLayers.Roles;

public class Whisperer : Neutral
{
    public CustomButton WhisperButton { get; set; }
    public DateTime LastWhispered { get; set; }
    public int WhisperCount { get; set; }
    public int ConversionCount { get; set; }
    public Dictionary<byte, int> PlayerConversion { get; set; }
    public int WhisperConversion { get; set; }
    public List<byte> Persuaded { get; set; }

    public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Whisperer : Colors.Neutral;
    public override string Name => "Whisperer";
    public override LayerEnum Type => LayerEnum.Whisperer;
    public override Func<string> StartText => () => "PSST";
    public override Func<string> Description => () => "- You can whisper to players around, slowly bending them to your ideals\n- When a player reaches 100% conversion, they will " +
        "defect and join the <color=#F995FCFF>Sect</color>";
    public override InspectorResults InspectorResults => InspectorResults.BringsChaos;
    public float Timer => ButtonUtils.Timer(Player, LastWhispered, CustomGameOptions.WhisperCooldown, CustomGameOptions.WhisperCooldownIncreases ?
        (CustomGameOptions.WhisperCooldownIncrease * WhisperCount) : 0);

    public Whisperer(PlayerControl player) : base(player)
    {
        Objectives = () => "- Persuade or kill anyone who can oppose the <color=#F995FCFF>Sect</color>";
        RoleAlignment = RoleAlignment.NeutralNeo;
        SubFaction = SubFaction.Sect;
        SubFactionColor = Colors.Sect;
        WhisperConversion = CustomGameOptions.InitialWhisperRate;
        Persuaded = new() { Player.PlayerId };
        WhisperButton = new(this, "Whisper", AbilityTypes.Effect, "ActionSecondary", Whisper);
        SubFactionSymbol = "Λ";
        PlayerConversion = new();
        CustomPlayer.AllPlayers.ForEach(x => PlayerConversion.Add(x.PlayerId, 100));
    }

    public void Whisper()
    {
        if (Timer != 0f)
            return;

        var closestPlayers = GetClosestPlayers(Player.GetTruePosition(), CustomGameOptions.WhisperRadius);
        closestPlayers.Remove(Player);

        foreach (var player in closestPlayers)
        {
            if (PlayerConversion.ContainsKey(player.PlayerId))
                PlayerConversion[player.PlayerId] -= WhisperConversion;
            else if (!Persuaded.Contains(player.PlayerId))
                PlayerConversion.Add(player.PlayerId, 100 - WhisperConversion);
        }

        var removals = new List<byte>();

        foreach (var (player, stat) in PlayerConversion)
        {
            if (stat <= 0)
            {
                if (CustomGameOptions.WhisperRateDecreases)
                    WhisperConversion -= CustomGameOptions.WhisperRateDecrease;

                if (WhisperConversion < 2)
                    WhisperConversion = 2;

                ConversionCount++;
                RoleGen.RpcConvert(player, PlayerId, SubFaction.Sect);
                removals.Add(player);
            }
        }

        Persuaded.ForEach(x => PlayerConversion.Remove(x));
        LastWhispered = DateTime.UtcNow;
        WhisperCount++;
        removals.ForEach(x => PlayerConversion.Remove(x));
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        WhisperButton.Update("WHISPER", Timer, CustomGameOptions.WhisperCooldown, CustomGameOptions.WhisperCooldownIncreases ? (CustomGameOptions.WhisperCooldownIncrease *
            WhisperCount) : 0);
    }
}