namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Crusader)]
public sealed class Crusader : Syndicate
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number CrusadeCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number CrusadeDur = 10;

    [NumberOption(0.5f, 5f, 0.25f, Format.Distance)]
    private static Number ChaosDriveCrusadeRadius = 1.5f;

    [ToggleOption]
    public static bool CrusadeMates = false;

    public PlayerControl CrusadedPlayer { get; private set; }
    public CustomButton CrusadeButton { get; private set; }

    protected override UColor MainColor => CustomColorManager.Crusader;
    public override LayerEnum Type => LayerEnum.Crusader;
    public override Func<string> StartText { get; } = () => "Cleanse This Land Of The Unholy Filth";
    public override Func<string> Description => () => "- You can crusade players\n- Crusaded players will be forced to be on alert, and will kill whoever interacts with then" +
        $"{(HoldsDrive ? $"\n- Crusaded players will also kill anyone within a {ChaosDriveCrusadeRadius}m radius" : "")}\n{CommonAbilities}";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Killing;
        CrusadedPlayer = null;
        CrusadeButton ??= new(this, new SpriteName("Crusade"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Crusade, new Cooldown(CrusadeCd), "CRUSADE", new Duration(CrusadeDur),
            (EffectEndVoid)UnCrusade, (PlayerBodyExclusion)Exception1, (EndFunc)EndEffect);
    }

    public override void Reset(bool meeting, bool start) => CrusadedPlayer = null;

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (CrusadedPlayer == player)
            name += " <#DF7AE8FF>τ</color>";
    }

    private void UnCrusade() => CrusadedPlayer = null;

    private void Crusade(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            CrusadedPlayer = target;
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, CrusadeButton, CrusadedPlayer);
            CrusadeButton.Begin();
        }
        else
            CrusadeButton.StartCooldown(cooldown);
    }

    public void RadialCrusade(PlayerControl player2)
    {
        foreach (var player in GetClosestPlayers(player2, ChaosDriveCrusadeRadius))
        {
            Spread(player2, player);

            if (player.IsVesting() || player.IsProtected() || player2.IsLinkedTo(player) || player.IsShielded() || Exception1(player))
                continue;

            if (!player.Is(Alignment.Deity))
                player2.RpcMurderPlayer(player, DeathReasonEnum.Crusaded, false);

            if (player.IsOnAlert() || player.Is<Pestilence>())
                player.RpcMurderPlayer(player2);
            else if (player.IsAmbushed())
                player.RpcMurderPlayer(player2, DeathReasonEnum.Ambushed);
            else if (player.IsCrusaded())
                player.RpcMurderPlayer(player2, DeathReasonEnum.Crusaded);
        }
    }

    public bool Exception1(PlayerControl player) => player == CrusadedPlayer || (!CrusadeMates && ((player.Is(Faction) && Faction is not (Faction.Crew or Faction.Neutral)) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None)));

    private bool EndEffect() => (CrusadedPlayer && CrusadedPlayer.HasDied()) || Dead;

    public override void ReadRPC(NetData reader) => CrusadedPlayer = reader.ReadPlayer();
}