namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Arsonist : NKilling
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number ArsoDouseCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 60f, 2.5f, Format.Time)]
    public static Number IgniteCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ArsoLastKillerBoost { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ArsoIgniteAll { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ArsoCooldownsLinked { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool IgnitionCremates { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ArsoVent { get; set; } = false;

    public CustomButton IgniteButton { get; set; }
    public CustomButton DouseButton { get; set; }
    public bool LastKiller => !AllPlayers().Any(x => !x.HasDied() && (x.GetFaction() is Faction.Intruder or Faction.Syndicate || x.GetAlignment() is Alignment.CrewKill or Alignment.NeutralPros
        or Alignment.NeutralNeo or Alignment.NeutralKill) && x != Player) && ArsoLastKillerBoost;
    public List<byte> Doused { get; } = [];

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Arsonist : FactionColor;
    public override LayerEnum Type => LayerEnum.Arsonist;
    public override Func<string> StartText => () => "PYROMANIAAAAAAAAAAAAAA";
    public override Func<string> Description => () => "- You can douse players in gasoline\n- Doused players can be ignited, killing them all at once\n- Players who interact with you will " +
        "get doused";
    public override AttackEnum AttackVal => AttackEnum.Unstoppable;
    public override DefenseEnum DefenseVal => Doused.Count is 1 or 2 ? DefenseEnum.Basic : DefenseEnum.None;

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

    public void Ignite()
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
                    RpcMurderPlayer(Player, player, DeathReasonEnum.Ignited, false);
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

    public void Douse(PlayerControl target)
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

    public bool Exception(PlayerControl player) => Doused.Contains(player.PlayerId) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is
        Faction.Intruder or Faction.Syndicate) || Player.IsLinkedTo(player);

    public override void ReadRPC(MessageReader reader)
    {
        var arsoAction = reader.ReadEnum<DouseActionsRPC>();

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