namespace TownOfUsReworked.PlayerLayers.Roles;

public class Plaguebearer : Neutral
{
    public DateTime LastInfected { get; set; }
    public List<byte> Infected { get; set; }
    public bool CanTransform => CustomPlayer.AllPlayers.Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= Infected.Count || CustomGameOptions.PestSpawn;
    public CustomButton InfectButton { get; set; }

    public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Plaguebearer : Colors.Neutral;
    public override string Name => "Plaguebearer";
    public override LayerEnum Type => LayerEnum.Plaguebearer;
    public override Func<string> StartText => () => "Spread Disease To Summon <color=#424242FF>Pestilence</color>";
    public override Func<string> Description => () => "- You can infect players\n- When all players are infected, you will turn into <color=#424242FF>Pestilence</color>d\n- Non-" +
        "infected players will get infected if they interact with you or someone who's infected or are interacted with by an infected player";
    public override InspectorResults InspectorResults => InspectorResults.SeeksToDestroy;
    public float Timer => ButtonUtils.Timer(Player, LastInfected, CustomGameOptions.InfectCd);

    public Plaguebearer(PlayerControl player) : base(player)
    {
        Objectives = () => "- Infect everyone to become <color=#424242FF>Pestilence</color>\n- Kill off anyone who can oppose you";
        RoleAlignment = RoleAlignment.NeutralHarb;
        Infected = new() { Player.PlayerId };
        InfectButton = new(this, "Infect", AbilityTypes.Direct, "ActionSecondary", Infect, Exception);
    }

    public void RpcSpreadInfection(PlayerControl source, PlayerControl target)
    {
        if ((Infected.Contains(source.PlayerId) && Infected.Contains(target.PlayerId)) || (!Infected.Contains(source.PlayerId) && !Infected.Contains(target.PlayerId)))
            return;

        var id = (byte)0;

        if (Infected.Contains(source.PlayerId) || source.Is(LayerEnum.Plaguebearer))
            id = target.PlayerId;
        else if (Infected.Contains(target.PlayerId) || target.Is(LayerEnum.Plaguebearer))
            id = source.PlayerId;

        if (id != 0)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.Infect, this, id);
            Infected.Add(id);
        }
    }

    public void Infect()
    {
        if (IsTooFar(Player, InfectButton.TargetPlayer) || Timer != 0f)
            return;

        var interact = Interact(Player, InfectButton.TargetPlayer);

        if (interact[3])
            RpcSpreadInfection(Player, InfectButton.TargetPlayer);

        if (interact[0])
            LastInfected = DateTime.UtcNow;
        else if (interact[1])
            LastInfected.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public void TurnPestilence()
    {
        var newRole = new Pestilence(Player);
        newRole.RoleUpdate(this);

        if (Local || CustomGameOptions.PlayersAlerted)
            Flash(Colors.Pestilence);

        if (CustomPlayer.Local.Is(LayerEnum.Seer) && !CustomGameOptions.PlayersAlerted)
            Flash(Colors.Seer);
    }

    public bool Exception(PlayerControl player) => Infected.Contains(player.PlayerId);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        InfectButton.Update("INFECT", Timer, CustomGameOptions.InfectCd, true, !CanTransform);

        if (CanTransform && !IsDead)
        {
            CallRpc(CustomRPC.Change, TurnRPC.TurnPestilence, this);
            TurnPestilence();
        }
    }
}