namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Shapeshifter)]
public sealed class Shapeshifter : Syndicate
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number ShapeshiftCd = 25f;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number ShapeshiftDur = 10;

    [ToggleOption]
    public static bool ShapeshiftMates = false;

    [ToggleOption]
    public static bool ShapeshiftSelf = false;

    private CustomButton ShapeshiftButton { get; set; }
    public PlayerControl ShapeshiftPlayer1 { get; private set; }
    public PlayerControl ShapeshiftPlayer2 { get; private set; }
    private CustomPlayerMenu ShapeshiftMenu1 { get; set; }
    private CustomPlayerMenu ShapeshiftMenu2 { get; set; }
    private bool ClickedAgain { get; set; }

    protected override UColor MainColor => CustomColorManager.Shapeshifter;
    public override LayerEnum Type => LayerEnum.Shapeshifter;
    public override Func<string> StartText { get; } = () => "Change Everyone's Appearances";
    public override Func<string> Description => () => $"- You can {(HoldsDrive ? "shuffle everyone's appearances" : "swap the appearances of 2 players")}\n{CommonAbilities}";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Disruption;
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
            using var writer = CreateWriter(CustomRPC.Action, ActionsRPC.ButtonAction, ShapeshiftButton);

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

    private bool CommonException(PlayerControl player) => (player == Player && !ShapeshiftSelf) || (player.Data.IsDead && !BodyByPlayer(player)) || (player.Is(Faction) && Faction.IsFactionedEvil()
        && !ShapeshiftMates);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

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

    public override void ReadRPC(NetData reader)
    {
        if (HoldsDrive)
            return;

        ShapeshiftPlayer1 = reader.ReadPlayer();
        ShapeshiftPlayer2 = reader.ReadPlayer();
    }

    private bool EndEffect() => ClickedAgain;
}