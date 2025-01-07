namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Whisperer : Neophyte
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number WhisperCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool WhisperCdIncreases { get; set; } = false;

    [NumberOption(MultiMenu.LayerSubOptions, 2.5f, 30f, 2.5f, Format.Time)]
    public static Number WhisperCdIncrease { get; set; } = new(5);

    [NumberOption(MultiMenu.LayerSubOptions, 0.5f, 5f, 0.25f, Format.Distance)]
    public static Number WhisperRadius { get; set; } = new(1.5f);

    [NumberOption(MultiMenu.LayerSubOptions, 5, 50, 5, Format.Percent)]
    public static Number WhisperRate { get; set; } = new(5);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool WhisperRateDecreases { get; set; } = false;

    [NumberOption(MultiMenu.LayerSubOptions, 5, 50, 5, Format.Percent)]
    public static Number WhisperRateDecrease { get; set; } = new(5);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool WhispVent { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool PersuadedVent { get; set; } = false;

    public CustomButton WhisperButton { get; set; }
    public int WhisperCount { get; set; }
    public int ConversionCount { get; set; }
    public Dictionary<byte, byte> PlayerConversion { get; } = [];
    public int WhisperConversion { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Whisperer : FactionColor;
    public override string Name => "Whisperer";
    public override LayerEnum Type => LayerEnum.Whisperer;
    public override Func<string> StartText => () => "PSST";
    public override Func<string> Description => () => "- You can whisper to players around, slowly bending them to your ideals\n- When a player reaches 100% conversion, they will " +
        "defect and join the <#F995FCFF>Sect</color>";
    public override AttackEnum AttackVal => AttackEnum.Basic;

    public override void Init()
    {
        base.Init();
        Objectives = () => "- Persuade or kill anyone who can oppose the <#F995FCFF>Sect</color>";
        SubFaction = SubFaction.Sect;
        WhisperConversion = WhisperRate;
        WhisperButton ??= new(this, new SpriteName("Whisper"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Whisper, new Cooldown(WhisperCd), "WHISPER",
            (DifferenceFunc)Difference);
        PlayerConversion.Clear();
        AllPlayers().ForEach(x => PlayerConversion.Add(x.PlayerId, 100));
        Members.ForEach(x => PlayerConversion.Remove(x));
    }

    public void Whisper()
    {
        var closestPlayers = GetClosestPlayers(Player, WhisperRadius, x => !Members.Contains(x.PlayerId));

        foreach (var player in closestPlayers)
        {
            if (PlayerConversion.ContainsKey(player.PlayerId))
                PlayerConversion[player.PlayerId] -= (byte)WhisperConversion;
            else if (!Members.Contains(player.PlayerId))
                PlayerConversion.Add(player.PlayerId, (byte)(100 - WhisperConversion));
        }

        var removals = new List<byte>();

        foreach (var (player, stat) in PlayerConversion)
        {
            if (stat <= 0)
            {
                if (WhisperRateDecreases)
                    WhisperConversion -= WhisperRateDecrease;

                if (WhisperConversion < 2)
                    WhisperConversion = 2;

                ConversionCount++;
                RpcConvert(player, PlayerId, SubFaction.Sect);
                removals.Add(player);
            }
        }

        WhisperCount++;
        Members.ForEach(x => PlayerConversion.Remove(x));
        removals.ForEach(x => PlayerConversion.Remove(x));
        WhisperButton.StartCooldown();
        var writer = CallOpenRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, (byte)PlayerConversion.Count);

        foreach (var (id, perc) in PlayerConversion)
        {
            writer.Write(id);
            writer.Write(perc);
        }

        writer.EndRpc();
    }

    public float Difference() => WhisperCdIncreases ? (WhisperCdIncrease * WhisperCount) : 0;

    public override void ReadRPC(MessageReader reader)
    {
        var count = reader.ReadByte();

        for (var i = 0; i <= count; i++)
            PlayerConversion[reader.ReadByte()] = reader.ReadByte();

        Members.ForEach(x => PlayerConversion.Remove(x));
    }
}