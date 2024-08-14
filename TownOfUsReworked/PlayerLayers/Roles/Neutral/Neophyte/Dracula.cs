namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Dracula : Neutral
{
    [NumberOption(MultiMenu2.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float BiteCd { get; set; } = 25f;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool DracVent { get; set; } = false;

    [NumberOption(MultiMenu2.LayerSubOptions, 1, 14, 1)]
    public static int AliveVampCount { get; set; } = 3;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool UndeadVent { get; set; } = false;

    public CustomButton BiteButton { get; set; }
    public List<byte> Converted { get; set; }
    public static int AliveCount => CustomPlayer.AllPlayers.Count(x => !x.HasDied());

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Dracula : CustomColorManager.Neutral;
    public override string Name => "Dracula";
    public override LayerEnum Type => LayerEnum.Dracula;
    public override Func<string> StartText => () => "Lead The <color=#7B8968FF>Undead</color> To Victory";
    public override Func<string> Description => () => "- You can convert the <color=#8CFFFFFF>Crew</color> into your own sub faction\n- If the target cannot be converted or the " +
        $"number of alive <color=#7B8968FF>Undead</color> exceeds {CustomGameOptions.AliveVampCount}, you will kill them instead\n- Attempting to convert a <color=#C0C0C0FF>Vampire " +
        "Hunter</color> will force them to kill you";
    public override AttackEnum AttackVal => AttackEnum.Basic;

    public override void Init()
    {
        BaseStart();
        Objectives = () => "- Convert or kill anyone who can oppose the <color=#7B8968FF>Undead</color>";
        SubFaction = SubFaction.Undead;
        Alignment = Alignment.NeutralNeo;
        SubFactionColor = CustomColorManager.Undead;
        Converted = [ Player.PlayerId ];
        BiteButton = CreateButton(this, new SpriteName("Bite"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Convert, new Cooldown(CustomGameOptions.BiteCd), "BITE",
            (PlayerBodyExclusion)Exception);
    }

    public void Convert()
    {
        var cooldown = Interact(Player, BiteButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
            RoleGen.RpcConvert(BiteButton.TargetPlayer.PlayerId, Player.PlayerId, SubFaction.Undead, AliveCount >= CustomGameOptions.AliveVampCount);

        BiteButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => Converted.Contains(player.PlayerId);
}