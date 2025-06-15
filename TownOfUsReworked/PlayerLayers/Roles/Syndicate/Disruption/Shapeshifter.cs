namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Shapeshifter)]
public sealed class Shapeshifter : Disruption
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number ShapeshiftCd = 25f;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    private static Number ShapeshiftDur = 10;

    [ToggleOption]
    private static bool ShapeshiftMates = false;

    [ToggleOption]
    private static bool ShapeshiftSelf = false;

    private CustomButton ShapeshiftButton;
    private PlayerControl ShapeshiftPlayer1;
    private PlayerControl ShapeshiftPlayer2;
    private CustomPlayerMenu ShapeshiftMenu1;
    private CustomPlayerMenu ShapeshiftMenu2;
    private bool ClickedAgain;

    protected override UColor MainColor => CustomColorManager.Shapeshifter;
    public override Layer Type => Layer.Shapeshifter;
    public override string StartText => "Change Everyone's Appearances";
    public override string Description => $"- You can {(HoldsDrive ? "shuffle everyone's appearances" : "swap the appearances of 2 players")}\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        ShapeshiftPlayer1 = null;
        ShapeshiftPlayer2 = null;
        ShapeshiftMenu1 = new(Player, Click1, Color, Exception1);
        ShapeshiftMenu2 = new(Player, Click2, Color, Exception2);
        ShapeshiftButton ??= new(this, new SpriteName("Shapeshift"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitShapeshift, new Cooldown(ShapeshiftCd), (LabelFunc)Label,
            (EffectEndVoid)UnShapeshift, new Duration(ShapeshiftDur), (EffectStartVoid)Shift, (EndFunc)EndEffect, (ClickedAgainVoid)OnClickedAgain);
    }

    public override void Reset(bool meeting, bool start) => ShapeshiftPlayer1 = ShapeshiftPlayer2 = null;

    private void Shift()
    {
        if (HoldsDrive)
        {
            var allPlayers = AllPlayers().ToList();
            var shuffledPlayers = AllPlayers().ToList();
            shuffledPlayers.Shuffle();

            for (var i = 0; i < allPlayers.Count; i++)
                shuffledPlayers[i].SetMimicked(allPlayers[i], ShapeshiftDur, EndEffect);
        }
        else
        {
            ShapeshiftPlayer1.SetMimicked(ShapeshiftPlayer2, ShapeshiftDur, EndEffect);
            ShapeshiftPlayer2.SetMimicked(ShapeshiftPlayer1, ShapeshiftDur, EndEffect);
        }
    }

    private void UnShapeshift()
    {
        ClickedAgain = false;
        ShapeshiftPlayer1 = null;
        ShapeshiftPlayer2 = null;
    }

    private void Click1(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            ShapeshiftPlayer1 = player;
        else
            ShapeshiftButton.StartCooldown(cooldown);
    }

    private void Click2(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            ShapeshiftPlayer2 = player;
        else
            ShapeshiftButton.StartCooldown(cooldown);
    }

    private void HitShapeshift()
    {
        if (HoldsDrive || (ShapeshiftPlayer1 && ShapeshiftPlayer2))
        {
            using var writer = CreateWriter(ReworkedRpc.Action, ActionsRpc.ButtonAction, ShapeshiftButton);

            if (writer is not null)
            {
                if (ShapeshiftPlayer1)
                    writer.Write(ShapeshiftPlayer1.PlayerId);

                if (ShapeshiftPlayer2)
                    writer.Write(ShapeshiftPlayer2.PlayerId);

                writer.Send();
            }

            ShapeshiftButton.Begin();
        }
        else if (!ShapeshiftPlayer1)
            ShapeshiftMenu1.Open();
        else if (!ShapeshiftPlayer2)
            ShapeshiftMenu2.Open();
    }

    private bool Exception1(PlayerControl player) => player == ShapeshiftPlayer2 || CommonException(player);

    private bool Exception2(PlayerControl player) => player == ShapeshiftPlayer1 || CommonException(player);

    private bool CommonException(PlayerControl player) => (player == Player && !ShapeshiftSelf) || (player.Data.IsDead && !BodyByPlayer(player)) || (!ShapeshiftMates && Player.IsBuddyWith(player, Faction));

    public override void UpdateHud(HudManager __instance)
    {
        if (HoldsDrive || !KeyboardJoystick.player.GetButtonDown("Delete"))
            return;

        if (!ShapeshiftButton.EffectActive)
        {
            if (ShapeshiftPlayer2)
                ShapeshiftPlayer2 = null;
            else if (ShapeshiftPlayer1)
                ShapeshiftPlayer1 = null;
        }

        Message("Removed a target");
    }

    private string Label()
    {
        if (HoldsDrive)
            return "SHAPESHIFT";

        if (!ShapeshiftPlayer1)
            return "FIRST TARGET";

        return !ShapeshiftPlayer2 ? "SECOND TARGET" : "SHAPESHIFT";
    }

    private void OnClickedAgain() => ClickedAgain = true;

    public override void ReadRPC(RpcReader reader)
    {
        if (HoldsDrive)
            return;

        ShapeshiftPlayer1 = reader.ReadPlayer();
        ShapeshiftPlayer2 = reader.ReadPlayer();
    }

    private bool EndEffect() => ClickedAgain;
}