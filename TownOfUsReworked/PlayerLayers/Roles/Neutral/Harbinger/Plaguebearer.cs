namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Plaguebearer : Harbinger<Pestilence>
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number InfectCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool PBVent { get; set; } = false;

    public List<byte> Infected { get; set; }
    public CustomButton InfectButton { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Plaguebearer : CustomColorManager.Neutral;
    public override string Name => "Plaguebearer";
    public override LayerEnum Type => LayerEnum.Plaguebearer;
    public override Func<string> StartText => () => "Spread Disease To Summon <#424242FF>Pestilence</color>";
    public override Func<string> Description => () => "- You can infect players\n- When all players are infected, you will turn into <#424242FF>Pestilence</color>\n- Infections can "
        + "spread via interaction between players";
    public override DefenseEnum DefenseVal => Infected.Count < AllPlayers().Count / 2 ? DefenseEnum.Basic : DefenseEnum.None;

    public override void Init()
    {
        base.Init();
        Objectives = () => "- Infect everyone to become <#424242FF>Pestilence</color>\n- Kill off anyone who can oppose you";
        Infected = [ Player.PlayerId ];
        InfectButton ??= new(this, new SpriteName("Infect"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClickPlayer)Infect, new Cooldown(InfectCd), "INFECT",
            (PlayerBodyExclusion)Exception);
    }

    public void RpcSpreadInfection(PlayerControl source, PlayerControl target)
    {
        if (Infected.Contains(source.PlayerId) == Infected.Contains(target.PlayerId))
            return;

        byte id = 0;
        var changed = false;

        if (Infected.Contains(source.PlayerId) || source.Is(LayerEnum.Plaguebearer))
        {
            id = target.PlayerId;
            changed = true;
        }
        else if (Infected.Contains(target.PlayerId) || target.Is(LayerEnum.Plaguebearer))
        {
            id = source.PlayerId;
            changed = true;
        }

        if (changed)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, id);
            Infected.Add(id);
        }
    }

    public void Infect(PlayerControl target) => InfectButton.StartCooldown(Interact(Player, target));

    public override bool CanTransform() => AllPlayers().Count(x => !x.HasDied()) <= Infected.Count;

    public bool Exception(PlayerControl player) => Infected.Contains(player.PlayerId);

    public override void ReadRPC(MessageReader reader) => Infected.Add(reader.ReadByte());
}