namespace TownOfUsReworked.PlayerLayers.Roles;

public class Crusader : Syndicate
{
    public bool Enabled { get; set; }
    public DateTime LastCrusaded { get; set; }
    public float TimeRemaining { get; set; }
    public bool OnCrusade => TimeRemaining > 0f;
    public PlayerControl CrusadedPlayer { get; set; }
    public CustomButton CrusadeButton { get; set; }

    public override Color Color => ClientGameOptions.CustomSynColors ? Colors.Crusader : Colors.Syndicate;
    public override string Name => "Crusader";
    public override LayerEnum Type => LayerEnum.Crusader;
    public override Func<string> StartText => () => "Cleanse This Land Of The Unholy Filth";
    public override Func<string> Description => () => "- You can crusade players\n- Crusaded players will be forced to be on alert, and will kill whoever interacts with then" +
        $"{(HoldsDrive ? $"\n- Crusaded players will also kill anyone within a {CustomGameOptions.ChaosDriveCrusadeRadius}m radies" : "")}\n{CommonAbilities}";
    public override InspectorResults InspectorResults => InspectorResults.PreservesLife;
    public float Timer => ButtonUtils.Timer(Player, LastCrusaded, CustomGameOptions.CrusadeCd);

    public Crusader(PlayerControl player) : base(player)
    {
        Alignment = Alignment.SyndicateKill;
        CrusadedPlayer = null;
        CrusadeButton = new(this, "Crusade", AbilityTypes.Direct, "ActionSecondary", HitCrusade, Exception1);
    }

    public void Crusade()
    {
        Enabled = true;
        TimeRemaining -= Time.deltaTime;

        if (IsDead || CrusadedPlayer.HasDied() || Meeting)
            TimeRemaining = 0f;
    }

    public void UnCrusade()
    {
        Enabled = false;
        LastCrusaded = DateTime.UtcNow;
        CrusadedPlayer = null;
    }

    public static void RadialCrusade(PlayerControl player2)
    {
        foreach (var player in GetClosestPlayers(player2.GetTruePosition(), CustomGameOptions.ChaosDriveCrusadeRadius))
        {
            Spread(player2, player);

            if (player.IsVesting() || player.IsProtected() || player2.IsLinkedTo(player) || player.IsShielded() || player.IsRetShielded() || (player.Is(Faction.Syndicate) &&
                !CustomGameOptions.CrusadeMates))
            {
                continue;
            }

            if (!player.Is(LayerEnum.Pestilence))
                RpcMurderPlayer(player2, player, DeathReasonEnum.Crusaded, false);

            if (player.IsOnAlert() || player.Is(LayerEnum.Pestilence))
                RpcMurderPlayer(player, player2);
            else if (player.IsAmbushed() || player.IsGFAmbushed())
                RpcMurderPlayer(player, player2, DeathReasonEnum.Ambushed);
            else if (player.IsCrusaded() || player.IsRebCrusaded())
                RpcMurderPlayer(player, player2, DeathReasonEnum.Crusaded);
        }
    }

    public void HitCrusade()
    {
        if (Timer != 0f || IsTooFar(Player, CrusadeButton.TargetPlayer))
            return;

        var interact = Interact(Player, CrusadeButton.TargetPlayer);

        if (interact.AbilityUsed)
        {
            CrusadedPlayer = CrusadeButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.Crusade, this, CrusadedPlayer);
            TimeRemaining = CustomGameOptions.CrusadeDur;
        }
        else if (interact.Reset)
            LastCrusaded = DateTime.UtcNow;
        else if (interact.Protected)
            LastCrusaded.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public bool Exception1(PlayerControl player) => player == CrusadedPlayer || (player.Is(Faction) && !CustomGameOptions.CrusadeMates) || (player.Is(SubFaction) && SubFaction !=
        SubFaction.None && !CustomGameOptions.CrusadeMates);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        CrusadeButton.Update("CRUSADE", Timer, CustomGameOptions.CrusadeCd, OnCrusade, TimeRemaining, CustomGameOptions.CrusadeDur);
    }
}