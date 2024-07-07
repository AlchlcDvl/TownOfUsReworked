namespace TownOfUsReworked.PlayerLayers.Roles;

public class Arsonist : Neutral
{
    public CustomButton IgniteButton { get; set; }
    public CustomButton DouseButton { get; set; }
    public bool LastKiller => !CustomPlayer.AllPlayers.Any(x => !x.HasDied() && (x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || x.Is(Alignment.CrewKill) || x.Is(Alignment.CrewAudit) ||
        x.Is(Alignment.NeutralPros) || x.Is(Alignment.NeutralNeo) || (x.Is(Alignment.NeutralKill) && x != Player))) && CustomGameOptions.ArsoLastKillerBoost;
    public List<byte> Doused { get; set; }

    public override UColor Color => ClientGameOptions.CustomNeutColors ? CustomColorManager.Arsonist : CustomColorManager.Neutral;
    public override string Name => "Arsonist";
    public override LayerEnum Type => LayerEnum.Arsonist;
    public override Func<string> StartText => () => "PYROMANIAAAAAAAAAAAAAA";
    public override Func<string> Description => () => "- You can douse players in gasoline\n- Doused players can be ignited, killing them all at once\n- Players who interact with " +
        "you will get doused";
    public override AttackEnum AttackVal => AttackEnum.Unstoppable;
    public override DefenseEnum DefenseVal => Doused.Count is 1 or 2 ? DefenseEnum.Basic : DefenseEnum.None;

    public override void Init()
    {
        BaseStart();
        Objectives = () => "- Burn anyone who can oppose you";
        Alignment = Alignment.NeutralKill;
        Doused = [];
        DouseButton = CreateButton(this, new SpriteName("ArsoDouse"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Douse, new Cooldown(CustomGameOptions.ArsoDouseCd), "DOUSE",
            (PlayerBodyExclusion)Exception);
        IgniteButton = CreateButton(this, new SpriteName("Ignite"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)Ignite, new Cooldown(CustomGameOptions.IgniteCd), "IGNITE",
            (UsableFunc)Doused.Any);
    }

    public void Ignite()
    {
        foreach (var arso in GetLayers<Arsonist>())
        {
            if (arso.Player != Player && !CustomGameOptions.ArsoIgniteAll)
                continue;

            foreach (var playerId in arso.Doused)
            {
                var player = PlayerById(playerId);

                if (CanAttack(AttackVal, player.GetDefenseValue()))
                    RpcMurderPlayer(Player, player, DeathReasonEnum.Ignited, false);
            }

            if (CustomGameOptions.IgnitionCremates)
            {
                CallRpc(CustomRPC.Action, ActionsRPC.Burn, arso);

                foreach (var body in AllBodies)
                {
                    if (arso.Doused.Contains(body.ParentId))
                        Ash.CreateAsh(body);
                }
            }

            arso.Doused.Clear();
        }

        if (!LastKiller)
            IgniteButton.StartCooldown();

        if (CustomGameOptions.ArsoCooldownsLinked)
            DouseButton.StartCooldown();
    }

    public void Douse()
    {
        var cooldown = Interact(Player, DouseButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
            RpcSpreadDouse(Player, DouseButton.TargetPlayer);

        DouseButton.StartCooldown(cooldown);

        if (CustomGameOptions.ArsoCooldownsLinked)
            IgniteButton.StartCooldown();
    }

    public void RpcSpreadDouse(PlayerControl source, PlayerControl target)
    {
        if (!source.Is(Type) || Doused.Contains(target.PlayerId) || source != Player)
            return;

        Doused.Add(target.PlayerId);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, target.PlayerId);
    }

    public bool Exception(PlayerControl player) => Doused.Contains(player.PlayerId) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is
        Faction.Intruder or Faction.Syndicate) || Player.IsLinkedTo(player);

    public override void ReadRPC(MessageReader reader) => Doused.Add(reader.ReadByte());
}