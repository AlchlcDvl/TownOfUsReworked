namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Whisperer : Neophyte
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number WhisperCd = 25;

    [ToggleOption]
    public static bool WhisperCdIncreases = false;

    [NumberOption(2.5f, 30f, 2.5f, Format.Time)]
    public static Number WhisperCdIncrease = 5;

    [NumberOption(0.5f, 5f, 0.25f, Format.Distance)]
    public static Number WhisperRadius = 1.5f;

    [NumberOption(5, 50, 5, Format.Percent)]
    public static Number WhisperRate = 5;

    [ToggleOption]
    public static bool WhisperRateDecreases = false;

    [NumberOption(5, 50, 5, Format.Percent)]
    public static Number WhisperRateDecrease = 5;

    [ToggleOption]
    public static bool WhispVent = false;

    [ToggleOption]
    public static bool PersuadedVent = false;

    public CustomButton WhisperButton { get; set; }
    public int WhisperCount { get; set; }
    public int ConversionCount { get; set; }
    public Dictionary<byte, byte> PlayerConversion { get; } = [];
    public int WhisperConversion { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Whisperer : FactionColor;
    public override LayerEnum Type => LayerEnum.Whisperer;
    public override Func<string> StartText => () => "PSST";
    public override Func<string> Description => () => "- You can whisper to players around, slowly bending them to your ideals\n- When a player reaches 100% conversion, they will " +
        "defect and join the <#F995FCFF>Cult</color>";
    public override AttackEnum AttackVal => AttackEnum.Basic;

    public override void Init()
    {
        base.Init();
        Objectives = () => "- Persuade or kill anyone who can oppose the <#F995FCFF>Cult</color>";
        SubFaction = SubFaction.Cult;
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
                RpcConvert(player, PlayerId, SubFaction.Cult);
                removals.Add(player);
            }
        }

        WhisperCount++;
        Members.ForEach(x => PlayerConversion.Remove(x));
        removals.ForEach(x => PlayerConversion.Remove(x));
        WhisperButton.StartCooldown();
        var writer = CallOpenRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, (byte)PlayerConversion.Count);

        if (writer == null)
            return;

        foreach (var (id, perc) in PlayerConversion)
        {
            writer.Write(id);
            writer.Write(perc);
        }

        writer.CloseRpc();
    }

    public float Difference() => WhisperCdIncreases ? (WhisperCdIncrease * WhisperCount) : 0;

    public override void ReadRPC(MessageReader reader)
    {
        var count = reader.ReadByte();

        while (count-- > 0)
            PlayerConversion[reader.ReadByte()] = reader.ReadByte();

        Members.ForEach(x => PlayerConversion.Remove(x));
    }
}