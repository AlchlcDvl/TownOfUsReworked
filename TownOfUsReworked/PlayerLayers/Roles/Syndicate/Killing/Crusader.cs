namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Crusader : Syndicate
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float CrusadeCd { get; set; } = 25f;

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static float CrusadeDur { get; set; } = 10f;

    [NumberOption(MultiMenu.LayerSubOptions, 0.5f, 5f, 0.25f, Format.Distance)]
    public static float ChaosDriveCrusadeRadius { get; set; } = 1.5f;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool CrusadeMates { get; set; } = false;

    public PlayerControl CrusadedPlayer { get; set; }
    public CustomButton CrusadeButton { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Crusader : CustomColorManager.Syndicate;
    public override string Name => "Crusader";
    public override LayerEnum Type => LayerEnum.Crusader;
    public override Func<string> StartText => () => "Cleanse This Land Of The Unholy Filth";
    public override Func<string> Description => () => "- You can crusade players\n- Crusaded players will be forced to be on alert, and will kill whoever interacts with then" +
        $"{(HoldsDrive ? $"\n- Crusaded players will also kill anyone within a {ChaosDriveCrusadeRadius}m radies" : "")}\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.SyndicateKill;
        CrusadedPlayer = null;
        CrusadeButton = CreateButton(this, new SpriteName("Crusade"), AbilityTypes.Alive, KeybindType.Secondary, (OnClick)Crusade, new Cooldown(CrusadeCd), "CRUSADE",
            new Duration(CrusadeDur), (EffectEndVoid)UnCrusade, (PlayerBodyExclusion)Exception1, (EndFunc)EndEffect);
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
        foreach (var player in GetClosestPlayers(player2.transform.position, ChaosDriveCrusadeRadius))
        {
            Spread(player2, player);

            if (player.IsVesting() || player.IsProtected() || player2.IsLinkedTo(player) || player.IsShielded() || (player.Is(Faction.Syndicate) && !CrusadeMates))
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

    public bool Exception1(PlayerControl player) => player == CrusadedPlayer || (player.Is(Faction) && !CrusadeMates && Faction is Faction.Intruder or Faction.Syndicate) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None && !CrusadeMates);

    public bool EndEffect() => (CrusadedPlayer && CrusadedPlayer.HasDied()) || Dead;

    public override void ReadRPC(MessageReader reader) => CrusadedPlayer = reader.ReadPlayer();
}