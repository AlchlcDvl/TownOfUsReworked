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

    protected override UColor MainColor => CustomColorManager.Concealer;
    public override LayerEnum Type => LayerEnum.Concealer;
    public override Func<string> StartText { get; } = () => "Turn The <#8CFFFFFF>Crew</color> Invisible For Some Chaos";
    public override Func<string> Description => () => $"- You can turn {(HoldsDrive ? "everyone" : "a player")} invisible\n{CommonAbilities}";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Disruption;
        ConcealMenu = new(Player, Click, Color, Exception1);
        ConcealedPlayer = null;
        ConcealButton ??= new(this, new SpriteName("Conceal"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitConceal, new Cooldown(ConcealCd), (EffectVoid)Conceal,
            (LabelFunc)Label, new Duration(ConcealDur), (EffectEndVoid)UnConceal, (EndFunc)EndEffect);
    }

    public override void Reset(bool meeting, bool start) => ConcealedPlayer = null;

    private void Conceal()
    {
        if (HoldsDrive)
            AllPlayers().Do(x => Invis(x, LocalPlayer.GetFaction() is not (Faction.Crew or Faction.Neutral)));
        else
            Invis(ConcealedPlayer, LocalPlayer.GetFaction() is not (Faction.Crew or Faction.Neutral));
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
            using var writer = CreateWriter(CustomRPC.Action, ActionsRPC.ButtonAction, ConcealButton);

            if (writer is not null)
            {
                if (ConcealedPlayer)
                    writer.Write(ConcealedPlayer.PlayerId);

                writer.Send();
            }

            ConcealButton.Begin();
        }
        else
            ConcealMenu.Open();
    }

    private bool Exception1(PlayerControl player) => player == ConcealedPlayer || player == Player || (player.Is(Faction) && !ConcealMates && Faction is not (Faction.Crew or Faction.Neutral))
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

    public override void ReadRPC(NetData reader)
    {
        if (!HoldsDrive)
            ConcealedPlayer = reader.ReadPlayer();
    }
}