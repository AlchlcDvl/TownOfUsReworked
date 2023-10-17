namespace TownOfUsReworked.PlayerLayers.Roles;

public class Arsonist : Neutral
{
    public CustomButton IgniteButton { get; set; }
    public CustomButton DouseButton { get; set; }
    public bool LastKiller => !CustomPlayer.AllPlayers.Any(x => !x.HasDied() && (x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || x.Is(Alignment.CrewKill) || x.Is(Alignment.CrewAudit) ||
        x.Is(Alignment.NeutralPros) || x.Is(Alignment.NeutralNeo) || (x.Is(Alignment.NeutralKill) && x != Player))) && CustomGameOptions.ArsoLastKillerBoost;
    public List<byte> Doused { get; set; }

    public override Color Color => ClientGameOptions.CustomNeutColors ? Colors.Arsonist : Colors.Neutral;
    public override string Name => "Arsonist";
    public override LayerEnum Type => LayerEnum.Arsonist;
    public override Func<string> StartText => () => "PYROMANIAAAAAAAAAAAAAA";
    public override Func<string> Description => () => "- You can douse players in gasoline\n- Doused players can be ignited, killing them all at once\n- Players who interact with " +
        "you will get doused";

    public Arsonist(PlayerControl player) : base(player)
    {
        Objectives = () => "- Burn anyone who can oppose you";
        Alignment = Alignment.NeutralKill;
        Doused = new();
        DouseButton = new(this, "ArsoDouse", AbilityTypes.Target, "ActionSecondary", Douse, CustomGameOptions.ArsoDouseCd, Exception);
        IgniteButton = new(this, "Ignite", AbilityTypes.Targetless, "Secondary", Ignite, CustomGameOptions.IgniteCd);
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

                if (player == null || player.HasDied() || player.Is(Alignment.NeutralApoc) || player.IsProtected())
                    continue;

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
            IgniteButton.StartCooldown(CooldownType.Reset);

        if (CustomGameOptions.ArsoCooldownsLinked)
            DouseButton.StartCooldown(CooldownType.Reset);
    }

    public void Douse()
    {
        var interact = Interact(Player, DouseButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
            RpcSpreadDouse(Player, DouseButton.TargetPlayer);

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        DouseButton.StartCooldown(cooldown);

        if (CustomGameOptions.ArsoCooldownsLinked)
            IgniteButton.StartCooldown(CooldownType.Reset);
    }

    public void RpcSpreadDouse(PlayerControl source, PlayerControl target)
    {
        if (!source.Is(Type) || Doused.Contains(target.PlayerId) || source != Player)
            return;

        Doused.Add(target.PlayerId);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, target.PlayerId);
    }

    public bool Exception(PlayerControl player) => Doused.Contains(player.PlayerId) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction
        is Faction.Intruder or Faction.Syndicate) || Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        DouseButton.Update2("DOUSE");
        IgniteButton.Update2("IGNITE", Doused.Count > 0);
    }

    public override void ReadRPC(MessageReader reader) => Doused.Add(reader.ReadByte());
}