namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Whisperer)]
public sealed class Whisperer : Neophyte
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number WhisperCd = 25;

    [ToggleOption]
    private static bool WhisperCdIncreases = false;

    [NumberOption(2.5f, 30f, 2.5f, Format.Time)]
    private static Number WhisperCdIncrease = 5;

    [NumberOption(0.5f, 5f, 0.25f, Format.Distance)]
    private static Number WhisperRadius = 1.5f;

    [NumberOption(5, 50, 5, Format.Percent)]
    private static Number WhisperRate = 5;

    [ToggleOption]
    private static bool WhisperRateDecreases = false;

    [NumberOption(5, 50, 5, Format.Percent)]
    private static Number WhisperRateDecrease = 5;

    [ToggleOption]
    private static bool WhispVent = false;

    [ToggleOption]
    public static bool PersuadedVent = false;

    private CustomButton WhisperButton { get; set; }
    private int WhisperCount { get; set; }
    public Dictionary<byte, byte> PlayerConversion { get; } = [];
    private int WhisperConversion { get; set; }

    protected override UColor MainColor => CustomColorManager.Whisperer;
    public override LayerEnum Type => LayerEnum.Whisperer;
    public override Func<string> StartText { get; } = () => "PSST";
    public override Func<string> Description => () => "- You can whisper to players around, slowly bending them to your ideals\n- When a player reaches 100% conversion, they will " +
        "defect and join the <#F995FCFF>Cult</color>";
    public override AttackEnum AttackVal => AttackEnum.Basic;
    public override bool CanVent => base.CanVent && WhispVent;

    protected override void Init()
    {
        base.Init();
        Objectives = () => "- Persuade or kill anyone who can oppose the <#F995FCFF>Cult</color>";
        Faction = Faction.Cult;
        WhisperConversion = WhisperRate;
        WhisperButton ??= new(this, new SpriteName("Whisper"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Whisper, new Cooldown(WhisperCd), "WHISPER",
            (DifferenceFunc)Difference);
        PlayerConversion.Clear();
        AllPlayers().Do(x => PlayerConversion.Add(x.PlayerId, 100));
        Members.Do(x => PlayerConversion.Remove(x));
    }

    private void Whisper()
    {
        foreach (var player in GetClosestPlayers(Player, WhisperRadius, x => !Members.Contains(x.PlayerId)))
        {
            if (PlayerConversion.ContainsKey(player.PlayerId))
                PlayerConversion[player.PlayerId] -= (byte)WhisperConversion;
            else if (!Members.Contains(player.PlayerId))
                PlayerConversion.Add(player.PlayerId, (byte)(100 - WhisperConversion));
        }

        var removals = new List<byte>();

        foreach (var (player, stat) in PlayerConversion)
        {
            if (stat > 0)
                continue;

            if (WhisperRateDecreases)
                WhisperConversion -= WhisperRateDecrease;

            if (WhisperConversion < 2)
                WhisperConversion = 2;

            RpcConvert(player, PlayerId);
            removals.Add(player);
        }

        WhisperCount++;
        Members.Do(x => PlayerConversion.Remove(x));
        removals.ForEach(x => PlayerConversion.Remove(x));
        WhisperButton.StartCooldown();
        using var writer = CreateWriter(CustomRPC.Action, ActionsRPC.LayerAction, this, (byte)PlayerConversion.Count);

        if (writer is null)
            return;

        foreach (var (id, perc) in PlayerConversion)
        {
            writer.Write(id);
            writer.Write(perc);
        }

        writer.Send();
    }

    private float Difference() => WhisperCdIncreases ? (WhisperCdIncrease * WhisperCount) : 0;

    public override void ReadRPC(NetData reader)
    {
        var count = reader.ReadByte();

        while (count-- > 0)
            PlayerConversion[reader.ReadByte()] = reader.ReadByte();

        Members.Do(x => PlayerConversion.Remove(x));
    }

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        base.UpdatePlayerName(handler, player, meeting, ref name, ref color, ref revealed, ref removeFromConsig);

        if (PlayerConversion.TryGetValue(player.PlayerId, out var value))
            name += $" <#2D6AA5FF>{value}%</color>";
    }
}