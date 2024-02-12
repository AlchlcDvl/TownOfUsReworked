namespace TownOfUsReworked.PlayerLayers.Roles;

public class Plaguebearer : Neutral
{
    public List<byte> Infected { get; set; }
    public bool CanTransform => CustomPlayer.AllPlayers.Count(x => !x.HasDied()) <= Infected.Count || CustomGameOptions.PestSpawn;
    public CustomButton InfectButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomNeutColors ? CustomColorManager.Plaguebearer : CustomColorManager.Neutral;
    public override string Name => "Plaguebearer";
    public override LayerEnum Type => LayerEnum.Plaguebearer;
    public override Func<string> StartText => () => "Spread Disease To Summon <color=#424242FF>Pestilence</color>";
    public override Func<string> Description => () => "- You can infect players\n- When all players are infected, you will turn into <color=#424242FF>Pestilence</color>\n- Infections spread"
        + " via interaction between players";
    public override DefenseEnum DefenseVal => Infected.Count < CustomPlayer.AllPlayers.Count / 2 ? DefenseEnum.Basic : DefenseEnum.None;

    public Plaguebearer() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Objectives = () => "- Infect everyone to become <color=#424242FF>Pestilence</color>\n- Kill off anyone who can oppose you";
        Alignment = Alignment.NeutralHarb;
        Infected = new() { Player.PlayerId };
        InfectButton = new(this, "Infect", AbilityTypes.Alive, "ActionSecondary", Infect, CustomGameOptions.InfectCd, Exception);
        return this;
    }

    public void RpcSpreadInfection(PlayerControl source, PlayerControl target)
    {
        if ((Infected.Contains(source.PlayerId) && Infected.Contains(target.PlayerId)) || (!Infected.Contains(source.PlayerId) && !Infected.Contains(target.PlayerId)))
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
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, id);
            Infected.Add(id);
        }
    }

    public void Infect() => InfectButton.StartCooldown(Interact(Player, InfectButton.TargetPlayer));

    public void TurnPestilence()
    {
        new Pestilence().Start<Role>(Player).RoleUpdate(this);

        if (CustomGameOptions.PlayersAlerted)
            Flash(Color);

        foreach (var player in CustomPlayer.AllPlayers)
        {
            if (!player.Is(Alignment.NeutralApoc) || !player.Is(Alignment.NeutralHarb))
                Pestilence.Infected[player.PlayerId] = 1;
        }
    }

    public bool Exception(PlayerControl player) => Infected.Contains(player.PlayerId);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        InfectButton.Update2("INFECT");

        if (CanTransform && !IsDead)
        {
            CallRpc(CustomRPC.Change, TurnRPC.TurnPestilence, this);
            TurnPestilence();
        }
    }

    public override void ReadRPC(MessageReader reader) => Infected.Add(reader.ReadByte());
}