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
    public override Func<string> Description => () => $"- You can place a spell on players\n- When all non-{FactionColorString}{Faction}</color> players are spelled the game ends in a " +
        $"{FactionColorString}{Faction}</color> win{(HoldsDrive ? "\n- Your spells don't trigger interaction sensitive roles and your cooldown does not increase" : "")}\n{CommonAbilities}";

    public Spellslinger(PlayerControl player) : base(player)
    {
        Alignment = Alignment.SyndicatePower;
        Spelled = new();
        SpellCount = 0;
        SpellButton = new(this, "Spell", AbilityTypes.Alive, "Secondary", HitSpell, CustomGameOptions.SpellCd, Exception1);
    }

    public void Spell(PlayerControl player)
    {
        if (player.Is(Faction) || Spelled.Contains(player.PlayerId))
            return;

        Spelled.Add(player.PlayerId);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, player.PlayerId);

        if (!HoldsDrive)
            SpellCount++;
        else
            SpellCount = 0;
    }

    public void HitSpell()
    {
        var cooldown = Interact(Player, SpellButton.TargetPlayer, astral: HoldsDrive);

        if (cooldown != CooldownType.Fail)
            Spell(SpellButton.TargetPlayer);

        SpellButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => Spelled.Contains(player.PlayerId);

    public override void ReadRPC(MessageReader reader) => Spelled.Add(reader.ReadByte());

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        SpellButton.Update2("SPELL", difference: SpellCount * CustomGameOptions.SpellCdIncrease);
    }
}