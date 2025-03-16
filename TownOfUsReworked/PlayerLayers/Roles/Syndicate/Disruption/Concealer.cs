namespace TownOfUsReworked.PlayerLayers.Roles;

// FIXME: Doesn't actually make people invisible
[LayerHeaderOption(LayerEnum.Concealer)]
public sealed class Concealer : Syndicate
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number ConcealCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number ConcealDur = 10;

    [ToggleOption]
    public static bool ConcealMates = false;

    private CustomButton ConcealButton { get; set; }
    private PlayerControl ConcealedPlayer { get; set; }
    private CustomPlayerMenu ConcealMenu { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Concealer : FactionColor;
    public override LayerEnum Type => LayerEnum.Concealer;
    public override Func<string> StartText => () => "Turn The <#8CFFFFFF>Crew</color> Invisible For Some Chaos";
    public override Func<string> Description => () => $"- You can turn {(HoldsDrive ? "everyone" : "a player")} invisible\n{CommonAbilities}";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Disruption;
        ConcealMenu = new(Player, Click, Exception1);
        ConcealedPlayer = null;
        ConcealButton ??= new(this, new SpriteName("Conceal"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitConceal, new Cooldown(ConcealCd), (EffectVoid)Conceal,
            (LabelFunc)Label, new Duration(ConcealDur), (EffectEndVoid)UnConceal, (EndFunc)EndEffect);
    }

    public override void Reset(bool meeting, bool start) => ConcealedPlayer = null;

    private void Conceal()
    {
        if (HoldsDrive)
            AllPlayers().ForEach(x => Invis(x, CustomPlayer.Local.Is(Faction.Syndicate)));
        else
            Invis(ConcealedPlayer, CustomPlayer.Local.Is(Faction.Syndicate));
    }

    private void UnConceal()
    {
        if (HoldsDrive)
            DefaultOutfitAll();
        else
            DefaultOutfit(ConcealedPlayer);

        ConcealedPlayer = null;
    }

    private void Click(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            ConcealedPlayer = player;
        else
            ConcealButton.StartCooldown(cooldown);
    }

    private void HitConceal()
    {
        if (HoldsDrive || ConcealedPlayer)
        {
            var writer = CallOpenRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ConcealButton);

            if (ConcealedPlayer)
                writer.Write(ConcealedPlayer.PlayerId);

            writer.CloseRpc();
            ConcealButton.Begin();
        }
        else
            ConcealMenu.Open();
    }

    private bool Exception1(PlayerControl player) => player == ConcealedPlayer || player == Player || (player.Is(Faction) && !ConcealMates && Faction is Faction.Intruder or Faction.Syndicate)
        || (player.Is(SubFaction) && SubFaction != SubFaction.None && !ConcealMates);

    private string Label() => ConcealedPlayer || HoldsDrive ? "CONCEAL" : "SET TARGET";

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (HoldsDrive || !KeyboardJoystick.player.GetButtonDown("Delete"))
            return;

        if (ConcealedPlayer && !ConcealButton.EffectActive)
            ConcealedPlayer = null;

        Message("Removed a target");
    }

    private bool EndEffect() => (ConcealedPlayer && ConcealedPlayer.HasDied()) || (!HoldsDrive && Dead);

    public override void ReadRPC(MessageReader reader)
    {
        if (!HoldsDrive)
            ConcealedPlayer = reader.ReadPlayer();
    }
}