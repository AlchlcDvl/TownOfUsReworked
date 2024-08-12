namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Whisperer : Neutral
{
    public CustomButton WhisperButton { get; set; }
    public int WhisperCount { get; set; }
    public int ConversionCount { get; set; }
    public Dictionary<byte, int> PlayerConversion { get; set; }
    public int WhisperConversion { get; set; }
    public List<byte> Persuaded { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Whisperer : CustomColorManager.Neutral;
    public override string Name => "Whisperer";
    public override LayerEnum Type => LayerEnum.Whisperer;
    public override Func<string> StartText => () => "PSST";
    public override Func<string> Description => () => "- You can whisper to players around, slowly bending them to your ideals\n- When a player reaches 100% conversion, they will " +
        "defect and join the <color=#F995FCFF>Sect</color>";
    public override AttackEnum AttackVal => AttackEnum.Basic;

    public override void Init()
    {
        BaseStart();
        Objectives = () => "- Persuade or kill anyone who can oppose the <color=#F995FCFF>Sect</color>";
        Alignment = Alignment.NeutralNeo;
        SubFaction = SubFaction.Sect;
        SubFactionColor = CustomColorManager.Sect;
        WhisperConversion = CustomGameOptions.WhisperRate;
        Persuaded = [ Player.PlayerId ];
        WhisperButton = CreateButton(this, new SpriteName("Whisper"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClick)Whisper, new Cooldown(CustomGameOptions.WhisperCd),
            "WHISPER", (DifferenceFunc)Difference);
        PlayerConversion = [];
        CustomPlayer.AllPlayers.ForEach(x => PlayerConversion.Add(x.PlayerId, 100));
        Persuaded.ForEach(x => PlayerConversion.Remove(x));
    }

    public void Whisper()
    {
        var closestPlayers = GetClosestPlayers(Player.transform.position, CustomGameOptions.WhisperRadius);
        closestPlayers.RemoveAll(x => x == Player || Persuaded.Contains(x.PlayerId));

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

        WhisperCount++;
        Persuaded.ForEach(x => PlayerConversion.Remove(x));
        removals.ForEach(x => PlayerConversion.Remove(x));
        WhisperButton.StartCooldown();
        var writer = CallOpenRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, PlayerConversion.Count);

        foreach (var (id, perc) in PlayerConversion)
        {
            writer.Write(id);
            writer.Write(perc);
        }

        writer.EndRpc();
    }

    public float Difference() => CustomGameOptions.WhisperCdIncreases ? (CustomGameOptions.WhisperCdIncrease * WhisperCount) : 0;

    public override void ReadRPC(MessageReader reader)
    {
        var count = reader.ReadInt32();

        for (var i = 0; i <= count; i++)
            PlayerConversion[reader.ReadByte()] = reader.ReadInt32();

        Persuaded.ForEach(x => PlayerConversion.Remove(x));
    }
}