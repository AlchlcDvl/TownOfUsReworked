namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Plaguebearer)]
public sealed class Plaguebearer : Harbinger<Pestilence>
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number InfectCd = 25;

    [ToggleOption]
    private static bool PbVent = false;

    public HashSet<byte> Infected { get; } = [];
    private CustomButton InfectButton { get; set; }

    protected override UColor MainColor => CustomColorManager.Plaguebearer;
    public override LayerEnum Type => LayerEnum.Plaguebearer;
    public override Func<string> StartText { get; } = () => "Spread Disease To Summon <#424242FF>Pestilence</color>";
    public override Func<string> Description => () => "- You can infect players\n- When all players are infected, you will turn into <#424242FF>Pestilence</color>\n- Infections can "
        + "spread via interaction between players" + CommonAbilities;
    public override DefenseEnum DefenseVal => Infected.Count < GameData.Instance.PlayerCount / 2 ? DefenseEnum.Basic : DefenseEnum.None;
    public override bool CanVent => base.CanVent && PbVent;

    public override void Init()
    {
        base.Init();
        Infected.Clear();
        Infected.Add(PlayerId);
        InfectButton ??= new(this, new SpriteName("Infect"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Infect, new Cooldown(InfectCd), "INFECT",
            (PlayerBodyExclusion)Exception);
    }

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (Infected.Contains(player.PlayerId))
            name += " <#CFFE61FF>ρ</color>";
    }

    public void RpcSpreadInfection(PlayerControl source, PlayerControl target)
    {
        if (Infected.Contains(source.PlayerId) == Infected.Contains(target.PlayerId))
            return;

        byte id;

        if (Infected.Contains(source.PlayerId) || source.Is<Plaguebearer>())
            id = target.PlayerId;
        else if (Infected.Contains(target.PlayerId) || target.Is<Plaguebearer>())
            id = source.PlayerId;
        else
            return;

        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, id);
        Infected.Add(id);
    }

    private void Infect(PlayerControl target) => InfectButton.StartCooldown(Interact(Player, target));

    protected override bool CanTransform() => AllPlayers().Count(x => !x.HasDied()) <= Infected.Count;

    private bool Exception(PlayerControl player) => Infected.Contains(player.PlayerId);

    public override void ReadRPC(RpcReader reader) => Infected.Add(reader.ReadByte());
}