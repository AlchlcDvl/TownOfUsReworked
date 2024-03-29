namespace TownOfUsReworked.PlayerLayers.Roles;

public class Crusader : Syndicate
{
    public PlayerControl CrusadedPlayer { get; set; }
    public CustomButton CrusadeButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomSynColors ? CustomColorManager.Crusader : CustomColorManager.Syndicate;
    public override string Name => "Crusader";
    public override LayerEnum Type => LayerEnum.Crusader;
    public override Func<string> StartText => () => "Cleanse This Land Of The Unholy Filth";
    public override Func<string> Description => () => "- You can crusade players\n- Crusaded players will be forced to be on alert, and will kill whoever interacts with then" +
        $"{(HoldsDrive ? $"\n- Crusaded players will also kill anyone within a {CustomGameOptions.ChaosDriveCrusadeRadius}m radies" : "")}\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.SyndicateKill;
        CrusadedPlayer = null;
        CrusadeButton = CreateButton(this, new SpriteName("Crusade"), AbilityTypes.Alive, KeybindType.Secondary, (OnClick)Crusade, new Cooldown(CustomGameOptions.CrusadeCd), "CRUSADE",
            new Duration(CustomGameOptions.CrusadeDur), (EffectEndVoid)UnCrusade, (PlayerBodyExclusion)Exception1, (EndFunc)EndEffect);
    }

    public void UnCrusade() => CrusadedPlayer = null;

    public void Crusade()
    {
        var cooldown = Interact(Player, CrusadeButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
        {
            CrusadedPlayer = CrusadeButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, CrusadeButton, CrusadedPlayer);
            CrusadeButton.Begin();
        }
        else
            CrusadeButton.StartCooldown(cooldown);
    }

    public static void RadialCrusade(PlayerControl player2)
    {
        foreach (var player in GetClosestPlayers(player2.transform.position, CustomGameOptions.ChaosDriveCrusadeRadius))
        {
            Spread(player2, player);

            if (player.IsVesting() || player.IsProtected() || player2.IsLinkedTo(player) || player.IsShielded() || (player.Is(Faction.Syndicate) && !CustomGameOptions.CrusadeMates))
                continue;

            if (!player.Is(LayerEnum.Pestilence))
                RpcMurderPlayer(player2, player, DeathReasonEnum.Crusaded, false);

            if (player.IsOnAlert() || player.Is(LayerEnum.Pestilence))
                RpcMurderPlayer(player, player2);
            else if (player.IsAmbushed())
                RpcMurderPlayer(player, player2, DeathReasonEnum.Ambushed);
            else if (player.IsCrusaded())
                RpcMurderPlayer(player, player2, DeathReasonEnum.Crusaded);
        }
    }

    public bool Exception1(PlayerControl player) => player == CrusadedPlayer || (player.Is(Faction) && !CustomGameOptions.CrusadeMates && Faction is Faction.Intruder or Faction.Syndicate) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None && !CustomGameOptions.CrusadeMates);

    public bool EndEffect() => (CrusadedPlayer && CrusadedPlayer.HasDied()) || Dead;

    public override void ReadRPC(MessageReader reader) => CrusadedPlayer = reader.ReadPlayer();
}