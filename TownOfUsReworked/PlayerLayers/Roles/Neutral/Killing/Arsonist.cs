namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Arsonist)]
public sealed class Arsonist : NKilling
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

    private CustomButton IgniteButton { get; set; }
    private CustomButton DouseButton { get; set; }
    private bool LastKiller => !AllPlayers().Any(x => !x.HasDied() && (x.GetFaction() is not (Faction.Crew or Faction.Neutral) || x.GetAlignment() is Alignment.Killing or Alignment.Proselyte
        or Alignment.Neophyte) && x != Player) && ArsoLastKillerBoost;
    public List<byte> Doused { get; } = [];

    protected override UColor MainColor => CustomColorManager.Arsonist;
    public override LayerEnum Type { get; } = LayerEnum.Arsonist;
    public override Func<string> StartText { get; } = () => "PYROMANIAAAAAAAAAAAAAA";
    public override Func<string> Description => () => "- You can douse players in gasoline\n- Doused players can be ignited, killing them all at once\n- Players who interact with you will " +
        "get doused";
    public override AttackEnum AttackVal => AttackEnum.Unstoppable;
    public override DefenseEnum DefenseVal => Doused.Count is 1 or 2 ? DefenseEnum.Basic : DefenseEnum.None;
    public override bool CanVent => base.CanVent && ArsoVent;

    protected override void Init()
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

        foreach (var arso in GetLayers<Arsonist>())
        {
            if (arso.Player != Player && !ArsoIgniteAll)
                continue;

            foreach (var playerId in arso.Doused)
            {
                var player = PlayerById(playerId);

                if (player.HasDied() || player.TryIgnitingFrozen())
                    continue;

                if (CanAttack(AttackVal, player.GetDefenseValue()))
                {
                    Player.RpcMurderPlayer(player, DeathReasonEnum.Ignited, false);
                    disappear.Add(playerId);
                }
            }

            arso.Doused.Clear();
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

    private bool Exception(PlayerControl player) => Doused.Contains(player.PlayerId) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is not
        (Faction.Crew or Faction.Neutral)) || Player.IsLinkedTo(player);

    public override void ReadRPC(NetData reader)
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