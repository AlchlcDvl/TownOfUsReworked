namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Arsonist)]
public sealed class Arsonist : OKilling, IDouser
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number ArsoDouseCd = 25;

    [NumberOption(5f, 60f, 2.5f, Format.Time)]
    private static Number IgniteCd = 25;

    [ToggleOption]
    private static bool ArsoLastKillerBoost = false;

    [ToggleOption]
    private static bool ArsoIgniteAll = false;

    [ToggleOption]
    private static bool ArsoCooldownsLinked = false;

    [ToggleOption]
    private static bool IgnitionCremates = false;

    [ToggleOption]
    private static bool ArsoVent = false;

    [NumberOption(0f, 10f, 0.5f, Format.Time, zeroIsInf: true)]
    private static Number IgniteRadius = 25;

    private CustomButton IgniteButton;
    private CustomButton DouseButton;

    private bool LastKiller => !AllPlayers().Any(x => !x.HasDied() && (x.GetFaction() is not (Faction.Crew or Faction.Outcast) || x.GetAlignment() is Alignment.Killing or Alignment.Proselyte
        or Alignment.Neophyte) && x != Player) && ArsoLastKillerBoost;
    public HashSet<byte> Doused { get; } = [];

    protected override UColor MainColor => CustomColorManager.Arsonist;
    public override LayerEnum Type => LayerEnum.Arsonist;
    public override string StartText => "PYROMANIAAAAAAAAAAAAAA";
    public override string Description => "- You can douse players in gasoline\n- Doused players can be ignited, killing them all at once\n- Players who interact with you will " +
        "get doused";
    public override AttackEnum AttackVal => AttackEnum.Unstoppable;
    public override DefenseEnum DefenseVal => Doused.Count is 1 or 2 ? DefenseEnum.Basic : DefenseEnum.None;
    public override bool CanVent => base.CanVent && ArsoVent;
    protected override Faction ActualFaction => Faction.Arsonist;

    public override void Init()
    {
        base.Init();
        Objectives = () => "- Burn anyone who can oppose you";
        Doused.Clear();
        DouseButton ??= new(this, new SpriteName("ArsoDouse"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Douse, new Cooldown(ArsoDouseCd), "DOUSE",
            (PlayerBodyExclusion)Exception);
        IgniteButton ??= new(this, new SpriteName("Ignite"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)Ignite, new Cooldown(IgniteCd), "IGNITE",
            (UsableFunc)Doused.Any);
    }

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (Doused.Contains(player.PlayerId))
            name += " <#EE7600FF>Ξ</color>";
    }

    private void Ignite()
    {
        Play("Ignite");
        var disappear = new List<byte>();
        var arsos = ArsoIgniteAll ? GetLayers<Arsonist>() : [this];
        var toIgnite = arsos.SelectMany(x => x.Doused);

        if (IgniteRadius != 0)
            toIgnite = GetClosestPlayers(Player, IgniteRadius, x => toIgnite.Contains(x.PlayerId)).Select(x => x.PlayerId);

        foreach (var id in toIgnite)
        {
            var player = PlayerById(id);

            if (player.HasDied() || player.TryReversingDouses<Cryomaniac>() || !CanAttack(AttackVal, player.GetDefenseValue()))
                continue;

            Player.RpcMurderPlayer(player, DeathReasonEnum.Ignited, false);
            disappear.Add(id);
            arsos.Do(x => x.Doused.Remove(id));
        }

        if (IgnitionCremates)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.Burn, disappear);

            foreach (var body in AllBodies())
            {
                if (disappear.Contains(body.ParentId))
                    Ash.CreateAsh(body);
            }
        }

        if (!LastKiller)
            IgniteButton.StartCooldown();

        if (ArsoCooldownsLinked)
            DouseButton.StartCooldown();
    }

    private void Douse(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            RpcSpreadDouse(Player, target);

        DouseButton.StartCooldown(cooldown);

        if (ArsoCooldownsLinked)
            IgniteButton.StartCooldown();
    }

    public void RpcSpreadDouse(PlayerControl source, PlayerControl target)
    {
        if (!source.Is<Arsonist>() || Doused.Contains(target.PlayerId) || source != Player)
            return;

        Doused.Add(target.PlayerId);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, target.PlayerId);
    }

    private bool Exception(PlayerControl player) => Doused.Contains(player.PlayerId) || (player.Is(Faction) && Faction is not
        (Faction.Crew or Faction.Outcast)) || Player.IsLinkedTo(player);

    public override void ReadRPC(RpcReader reader)
    {
        var arsoAction = reader.Read<DouseActionsRPC>();

        switch (arsoAction)
        {
            case DouseActionsRPC.Douse:
            {
                Doused.Add(reader.ReadByte());
                break;
            }
            case DouseActionsRPC.UnDouse:
            {
                Doused.Remove(reader.ReadByte());
                break;
            }
            default:
            {
                Failure($"Received unknown RPC - {arsoAction}");
                break;
            }
        }
    }
}