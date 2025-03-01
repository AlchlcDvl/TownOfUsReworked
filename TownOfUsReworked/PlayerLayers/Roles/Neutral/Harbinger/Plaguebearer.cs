namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Plaguebearer : Harbinger<Pestilence>
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number InfectCd = 25;

    [ToggleOption]
    public static bool PbVent = false;

    public List<byte> Infected { get; } = [];
    private CustomButton InfectButton { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Plaguebearer : FactionColor;
    public override LayerEnum Type => LayerEnum.Plaguebearer;
    public override Func<string> StartText => () => "Spread Disease To Summon <#424242FF>Pestilence</color>";
    public override Func<string> Description => () => "- You can infect players\n- When all players are infected, you will turn into <#424242FF>Pestilence</color>\n- Infections can "
        + "spread via interaction between players";
    public override DefenseEnum DefenseVal => Infected.Count < GameData.Instance.PlayerCount / 2 ? DefenseEnum.Basic : DefenseEnum.None;

    protected override void Init()
    {
        base.Init();
        Objectives = () => "- Infect everyone to become <#424242FF>Pestilence</color>\n- Kill off anyone who can oppose you";
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

        byte id = 0;
        var changed = false;

        if (Infected.Contains(source.PlayerId) || source.Is<Plaguebearer>())
        {
            id = target.PlayerId;
            changed = true;
        }
        else if (Infected.Contains(target.PlayerId) || target.Is<Plaguebearer>())
        {
            id = source.PlayerId;
            changed = true;
        }

        if (!changed)
            return;

        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, id);
        Infected.Add(id);
    }

    private void Infect(PlayerControl target) => InfectButton.StartCooldown(Interact(Player, target));

    protected override bool CanTransform() => AllPlayers().Count(x => !x.HasDied()) <= Infected.Count;

    private bool Exception(PlayerControl player) => Infected.Contains(player.PlayerId);

    public override void ReadRPC(MessageReader reader) => Infected.Add(reader.ReadByte());
}