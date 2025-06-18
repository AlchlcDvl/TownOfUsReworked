namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Drunkard)]
public sealed class Drunkard : Disruption
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number ConfuseCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    private static Number ConfuseDur = 10;

    [ToggleOption]
    private static bool ConfuseImmunity = true;

    public CustomButton ConfuseButton;
    public PlayerControl ConfusedPlayer;
    private CustomPlayerMenu ConfuseMenu;

    protected override UColor MainColor => CustomColorManager.Drunkard;
    public override Layer Type => Layer.Drunkard;
    public override string StartText => "<i>Burp</i>";
    public override string Description => $"- You can confuse {(HoldsDrive ? "everyone" : "a player")}\n- Confused players will have their controls reverse\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        ConfuseMenu = new(Player, Click, Color, Exception1);
        ConfusedPlayer = null;
        ConfuseButton ??= new(this, new SpriteName("Confuse"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitConfuse, new Cooldown(ConfuseCd), (LabelFunc)Label,
            new Duration(ConfuseDur), (EffectStartVoid)StartConfusion, (EffectEndVoid)UnConfuse, (EndFunc)EndEffect);
    }

    private void StartConfusion()
    {
        if (ConfusedPlayer.AmOwner || HoldsDrive)
            Flash(CustomColorManager.Drunkard);
    }

    private void UnConfuse() => ConfusedPlayer = null;

    private void Click(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            ConfusedPlayer = player;
        else
            ConfuseButton.StartCooldown(cooldown);
    }

    private void HitConfuse()
    {
        if (HoldsDrive || ConfusedPlayer)
        {
            using var writer = CreateWriter(ActionsRpc.ButtonAction, ConfusedPlayer);

            if (writer is not null)
            {
                if (ConfusedPlayer)
                    writer.Write(ConfusedPlayer.PlayerId);

                writer.Send();
            }

            ConfuseButton.Begin();
        }
        else
            ConfuseMenu.Open();
    }

    private bool Exception1(PlayerControl player) => player == ConfusedPlayer || player == Player || (ConfuseImmunity && Player.IsBuddyWith(player, Handler.CurrentFaction));

    private string Label() => ConfusedPlayer || HoldsDrive ? "CONFUSE" : "SET TARGET";

    private bool EndEffect() => (ConfusedPlayer && ConfusedPlayer.HasDied()) || (!HoldsDrive && Dead);

    public override void UpdateHud(HudManager __instance)
    {
        if (HoldsDrive || !KeyboardJoystick.player.GetButtonDown("Delete"))
            return;

        if (ConfusedPlayer && !ConfuseButton.EffectActive)
            ConfusedPlayer = null;

        Message("Removed a target");
    }

    public override void ReadRPC(RpcReader reader)
    {
        if (!HoldsDrive)
            ConfusedPlayer = reader.ReadPlayer();
    }
}