namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Spellslinger : Syndicate, IHexer
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number SpellCd { get; set; } = 25f;

    [NumberOption(2.5f, 30f, 2.5f, Format.Time)]
    public static Number SpellCdIncrease { get; set; } = 5;

    public CustomButton SpellButton { get; set; }
    public List<byte> Spelled { get; } = [];
    public int SpellCount { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Spellslinger : FactionColor;
    public override LayerEnum Type => LayerEnum.Spellslinger;
    public override Func<string> StartText => () => "Place the <#8CFFFFFF>Crew</color> Under A Curse";
    public override Func<string> Description => () => $"- You can spellbind players\n- When all non-{FactionColorString}{Faction}</color> players are spelled the game ends in a " +
        $"{FactionColorString}{Faction}</color> win{(HoldsDrive ? "\n- Your spells don't trigger interaction sensitive roles and your cooldown does not increase" : "")}\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Power;
        Spelled.Clear();
        SpellCount = 0;
        SpellButton ??= new(this, new SpriteName("Spellbind"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Spell, new Cooldown(SpellCd), "SPELLBIND", (DifferenceFunc)Difference,
            (PlayerBodyExclusion)Exception1);
    }

    public void Spell(PlayerControl target)
    {
        var cooldown = Interact(Player, target, astral: HoldsDrive);

        if (cooldown != CooldownType.Fail)
        {
            Spelled.Add(target.PlayerId);
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, target.PlayerId);

            if (HoldsDrive)
                SpellCount = 0;
            else
                SpellCount++;

            if (AmongUsClient.Instance.AmHost)
                CheckEndGame.CheckEnd();
        }

        SpellButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => Spelled.Contains(player.PlayerId) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate);

    public override void ReadRPC(MessageReader reader)
    {
        Spelled.Add(reader.ReadByte());

        if (AmongUsClient.Instance.AmHost)
            CheckEndGame.CheckEnd();
    }

    public float Difference() => SpellCount * SpellCdIncrease;
}