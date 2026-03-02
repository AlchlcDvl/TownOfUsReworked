namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Concealer)]
public sealed class Concealer : Disruption
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number ConcealCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    private static Number ConcealDur = 10;

    [ToggleOption]
    private static bool ConcealMates = false;

    private CustomButton ConcealButton;
    private PlayerControl ConcealedPlayer;
    private CustomPlayerMenu ConcealMenu;
    private bool ClickedAgain;

    protected override UColor MainColor => CustomColorManager.Concealer;
    public override Layer Type => Layer.Concealer;
    public override string StartText => "Turn The <#8CFFFFFF>Crew</color> Invisible For Some Chaos";
    public override string Description => $"- You can turn {(HoldsDrive ? "everyone" : "a player")} invisible\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        ConcealMenu = new(Player, Click, Color, Exception1);
        ConcealedPlayer = null;
        ConcealButton ??= new(this, new SpriteName("Conceal"), ReworkedAbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitConceal, new Cooldown(ConcealCd), (EffectStartVoid)Conceal,
            (LabelFunc)Label, new Duration(ConcealDur), (EffectEndVoid)UnConceal, (EndFunc)EndEffect, (ClickedAgainVoid)ClickAgain);
    }

    public override void Reset(bool meeting, bool start) => ConcealedPlayer = null;

    private void Conceal()
    {
        var isBuddy = Player.IsBuddyWith(LocalPlayer, Handler.CurrentFaction);

        if (HoldsDrive)
            AllPlayers().Do(x => Invis(x, ConcealDur, EndEffect, isBuddy));
        else
            Invis(ConcealedPlayer, ConcealDur, EndEffect, isBuddy);
    }

    private void UnConceal()
    {
        ClickedAgain = false;
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

    private void ClickAgain() => ClickedAgain = true;

    private void HitConceal()
    {
        if (HoldsDrive || ConcealedPlayer)
        {
            using var writer = CreateWriter(ActionsRpc.ButtonAction, ConcealButton);

            if (writer is not null)
            {
                if (ConcealedPlayer)
                    writer.WriteByte(ConcealedPlayer.PlayerId);

                writer.Send();
            }

            ConcealButton.Begin();
        }
        else
            ConcealMenu.Open();
    }

    private bool Exception1(PlayerControl player) => player == ConcealedPlayer || player == Player || (!ConcealMates && Player.IsBuddyWith(player, Handler.CurrentFaction));

    private string Label() => ConcealedPlayer || HoldsDrive ? "CONCEAL" : "SET TARGET";

    public override void UpdateHud(HudManager __instance)
    {
        if (HoldsDrive || !KeyboardJoystick.player.GetButtonDown("Delete"))
            return;

        if (ConcealedPlayer && !ConcealButton.EffectActive)
            ConcealedPlayer = null;

        Message("Removed a target");
    }

    private bool EndEffect() => (ConcealedPlayer && ConcealedPlayer.HasDied()) || (!HoldsDrive && Dead) || ClickedAgain;

    public override void ReadRPC(RpcReader reader)
    {
        if (!HoldsDrive)
            ConcealedPlayer = reader.ReadPlayer();
    }
}