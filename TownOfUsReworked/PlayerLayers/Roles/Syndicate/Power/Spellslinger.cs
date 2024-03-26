namespace TownOfUsReworked.PlayerLayers.Roles;

public class Spellslinger : Syndicate
{
    public CustomButton SpellButton { get; set; }
    public List<byte> Spelled { get; set; }
    public int SpellCount { get; set; }

    public override UColor Color => ClientGameOptions.CustomSynColors ? CustomColorManager.Spellslinger : CustomColorManager.Syndicate;
    public override string Name => "Spellslinger";
    public override LayerEnum Type => LayerEnum.Spellslinger;
    public override Func<string> StartText => () => "Place the <color=#8CFFFFFF>Crew</color> Under A Curse";
    public override Func<string> Description => () => $"- You can spellbind players\n- When all non-{FactionColorString}{Faction}</color> players are spelled the game ends in a " +
        $"{FactionColorString}{Faction}</color> win{(HoldsDrive ? "\n- Your spells don't trigger interaction sensitive roles and your cooldown does not increase" : "")}\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.SyndicatePower;
        Spelled = [];
        SpellCount = 0;
        SpellButton = CreateButton(this, new SpriteName("Spellbind"), AbilityTypes.Alive, KeybindType.Secondary, (OnClick)HitSpell, new Cooldown(CustomGameOptions.SpellCd), "SPELLBIND",
            (PlayerBodyExclusion)Exception1, (DifferenceFunc)Difference);
    }

    public void HitSpell()
    {
        var cooldown = Interact(Player, SpellButton.TargetPlayer, astral: HoldsDrive);

        if (cooldown != CooldownType.Fail)
        {
            Spelled.Add(SpellButton.TargetPlayer.PlayerId);
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, SpellButton.TargetPlayer.PlayerId);

            if (HoldsDrive)
                SpellCount = 0;
            else
                SpellCount++;
        }

        SpellButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => Spelled.Contains(player.PlayerId) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate);

    public override void ReadRPC(MessageReader reader) => Spelled.Add(reader.ReadByte());

    public float Difference() => SpellCount * CustomGameOptions.SpellCdIncrease;
}