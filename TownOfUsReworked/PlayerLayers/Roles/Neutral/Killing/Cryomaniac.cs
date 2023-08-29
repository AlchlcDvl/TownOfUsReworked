namespace TownOfUsReworked.PlayerLayers.Roles;

public class Cryomaniac : Neutral
{
    public CustomButton FreezeButton { get; set; }
    public CustomButton DouseButton { get; set; }
    public CustomButton KillButton { get; set; }
    public List<byte> Doused { get; set; }
    public bool FreezeUsed { get; set; }
    public DateTime LastDoused { get; set; }
    public DateTime LastKilled { get; set; }
    public bool LastKiller => !CustomPlayer.AllPlayers.Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) ||
        x.Is(RoleAlignment.CrewKill) || x.Is(RoleAlignment.CrewAudit) || x.Is(RoleAlignment.NeutralPros) || x.Is(RoleAlignment.NeutralNeo) || (x.Is(RoleAlignment.NeutralKill) && x !=
        Player))) && CustomGameOptions.CryoLastKillerBoost;

    public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Cryomaniac : Colors.Neutral;
    public override string Name => "Cryomaniac";
    public override LayerEnum Type => LayerEnum.Cryomaniac;
    public override Func<string> StartText => () => "Who Likes Ice Cream?";
    public override Func<string> Description => () => "- You can douse players in coolant\n- Doused players can be frozen, which kills all of them at once at the start of the next " +
        $"meeting\n- People who interact with you will also get doused{(LastKiller ? "\n- You can kill normally" : "")}";
    public override InspectorResults InspectorResults => InspectorResults.SeeksToDestroy;
    public float DouseTimer => ButtonUtils.Timer(Player, LastDoused, CustomGameOptions.CryoDouseCd);
    public float KillTimer => ButtonUtils.Timer(Player, LastKilled, CustomGameOptions.CryoDouseCd);

    public Cryomaniac(PlayerControl player) : base(player)
    {
        Objectives = () => "- Freeze anyone who can oppose you";
        RoleAlignment = RoleAlignment.NeutralKill;
        Doused = new();
        DouseButton = new(this, "CryoDouse", AbilityTypes.Direct, "ActionSecondary", Douse, Exception);
        FreezeButton = new(this, "Freeze", AbilityTypes.Effect, "Secondary", Freeze);
        KillButton = new(this, "CryoKill", AbilityTypes.Direct, "Tertiary", Kill, Exception);
    }

    public void Kill()
    {
        if (IsTooFar(Player, KillButton.TargetPlayer) || KillTimer != 0f)
            return;

        var interact = Interact(Player, KillButton.TargetPlayer, true);

        if (interact[0] || interact[3])
            LastKilled = DateTime.UtcNow;
        else if (interact[1])
            LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
        else if (interact[2])
            LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
    }

    public void RpcSpreadDouse(PlayerControl source, PlayerControl target)
    {
        if (!source.Is(Type) || Doused.Contains(target.PlayerId) || source != Player)
            return;

        Doused.Add(target.PlayerId);
        CallRpc(CustomRPC.Action, ActionsRPC.FreezeDouse, this, target);
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);

        foreach (var cryo in GetRoles<Cryomaniac>(LayerEnum.Cryomaniac))
        {
            if (cryo.FreezeUsed)
            {
                foreach (var player in cryo.Doused)
                {
                    var player2 = PlayerById(player);

                    if (player2.Data.IsDead || player2.Data.Disconnected || player2.Is(LayerEnum.Pestilence) || player2.IsProtected())
                        continue;

                    RpcMurderPlayer(Player, player2, DeathReasonEnum.Frozen);
                }

                cryo.Doused.Clear();
                cryo.FreezeUsed = false;
            }
        }
    }

    public bool Exception(PlayerControl player) => Doused.Contains(player.PlayerId) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction
        is Faction.Intruder or Faction.Syndicate) || Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        DouseButton.Update("DOUSE", DouseTimer, CustomGameOptions.CryoDouseCd);
        KillButton.Update("KILL", KillTimer, CustomGameOptions.CryoDouseCd, true, LastKiller);
        FreezeButton.Update("FREEZE", true, Doused.Count > 0 && !FreezeUsed);
    }

    public void Douse()
    {
        if (IsTooFar(Player, DouseButton.TargetPlayer) || DouseTimer != 0f || Doused.Contains(DouseButton.TargetPlayer.PlayerId))
            return;

        var interact = Interact(Player, DouseButton.TargetPlayer, LastKiller);

        if (interact[3])
            RpcSpreadDouse(Player, DouseButton.TargetPlayer);

        if (interact[0])
            LastDoused = DateTime.UtcNow;
        else if (interact[1])
            LastDoused.AddSeconds(CustomGameOptions.ProtectKCReset);
        else if (interact[2])
            LastDoused.AddSeconds(CustomGameOptions.VestKCReset);
    }

    public void Freeze()
    {
        if (Doused.Count == 0 || FreezeUsed)
            return;

        FreezeUsed = true;
    }
}