namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Spellslinger)]
public sealed class Spellslinger : SHead
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number SpellCd = 25f;

    [NumberOption(2.5f, 30f, 2.5f, Format.Time)]
    private static Number SpellCdIncrease = 5;

    private CustomButton SpellButton;
    public readonly HashSet<byte> Spelled = [];

    protected override UColor MainColor => CustomColorManager.Spellslinger;
    public override Layer Type => Layer.Spellslinger;
    public override string StartText => "Place the <#8CFFFFFF>Crew</color> Under A Curse";
    public override string Description => $"- You can spellbind players\n- When all non-{FactionColorString}{Faction}</color> players are spelled the game ends in a " +
        $"{FactionColorString}{Faction}</color> win{(HoldsDrive ? "\n- Your spells don't trigger interaction sensitive roles and your cooldown does not increase" : "")}\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        Spelled.Clear();
        SpellButton ??= new(this, new SpriteName("Spellbind"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Spell, new Cooldown(SpellCd), "SPELLBIND", (DifferenceFunc)Difference,
            (PlayerBodyExclusion)Exception1);
    }

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (Spelled.Contains(player.PlayerId))
            name += " <#0028F5FF>ø</color>";
    }

    private void Spell(PlayerControl target)
    {
        var cooldown = Interact(Player, target, astral: HoldsDrive);

        if (cooldown != CooldownType.Fail)
        {
            Spelled.Add(target.PlayerId);
            CallRpc(ReworkedRpc.Action, ActionsRpc.LayerAction, this, target.PlayerId);

            if (AmongUsClient.Instance.AmHost)
                CheckEndGame.CheckSpellWin(this);
        }

        SpellButton.StartCooldown(cooldown);
    }

    private bool Exception1(PlayerControl player) => Spelled.Contains(player.PlayerId) || Player.IsBuddyWith(player, Faction);

    public override void ReadRPC(RpcReader reader)
    {
        Spelled.Add(reader.ReadByte());

        if (AmongUsClient.Instance.AmHost)
            CheckEndGame.CheckSpellWin(this);
    }

    private float Difference() => HoldsDrive ? 0 : (Spelled.Count * SpellCdIncrease);
}