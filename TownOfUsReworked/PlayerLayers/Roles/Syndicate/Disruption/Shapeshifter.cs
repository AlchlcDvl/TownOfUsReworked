namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Shapeshifter)]
public sealed class Shapeshifter : Syndicate, IShaper
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number ShapeshiftCd = 25f;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number ShapeshiftDur = 10;

    [ToggleOption]
    public static bool ShapeshiftMates = false;

    private CustomButton ShapeshiftButton { get; set; }
    public PlayerControl ShapeshiftPlayer1 { get; private set; }
    public PlayerControl ShapeshiftPlayer2 { get; private set; }
    private CustomPlayerMenu ShapeshiftMenu1 { get; set; }
    private CustomPlayerMenu ShapeshiftMenu2 { get; set; }

    public override UColor MainColor => CustomColorManager.Shapeshifter;
    public override LayerEnum Type => LayerEnum.Shapeshifter;
    public override Func<string> StartText => () => "Change Everyone's Appearances";
    public override Func<string> Description => () => $"- You can {(HoldsDrive ? "shuffle everyone's appearances" : "swap the appearances of 2 players")}\n{CommonAbilities}";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Disruption;
        ShapeshiftPlayer1 = null;
        ShapeshiftPlayer2 = null;
        ShapeshiftMenu1 = new(Player, Click1, Exception1);
        ShapeshiftMenu2 = new(Player, Click2, Exception2);
        ShapeshiftButton ??= new(this, new SpriteName("Shapeshift"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitShapeshift, new Cooldown(ShapeshiftCd),
            (EffectEndVoid)UnShapeshift, new Duration(ShapeshiftDur), (EffectVoid)Shift, (LabelFunc)Label);
    }

    public override void Reset(bool meeting, bool start) => ShapeshiftPlayer1 = ShapeshiftPlayer2 = null;

    private void Shift() => Shapeshift(ShapeshiftPlayer1, ShapeshiftPlayer2, HoldsDrive);

    public static void Shapeshift(PlayerControl player1, PlayerControl player2, bool drived)
    {
        if (!drived)
        {
            Morph(player1, player2);
            Morph(player2, player1);
        }
        else if (!Shapeshifted)
        {
            Shapeshifted = true;
            var allPlayers = AllPlayers().ToList();
            var shuffledPlayers = AllPlayers().ToList();
            shuffledPlayers.Shuffle();

            for (var i = 0; i < allPlayers.Count; i++)
            {
                var morphed = allPlayers[i];
                var morphTarget = shuffledPlayers[i];
                CachedMorphs.TryAdd(morphed.PlayerId, morphTarget.PlayerId);
            }
        }
        else
        {
            AllPlayers().ForEach(x =>
            {
                if (CachedMorphs.TryGetValue(x.PlayerId, out var target))
                    Morph(x, PlayerById(target));
            });
        }
    }

    private void UnShapeshift()
    {
        if (HoldsDrive)
            DefaultOutfitAll();
        else
        {
            DefaultOutfit(ShapeshiftPlayer1);
            DefaultOutfit(ShapeshiftPlayer2);
        }

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
        if (HoldsDrive)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ShapeshiftButton);
            ShapeshiftButton.Begin();
        }
        else if (!ShapeshiftPlayer1)
            ShapeshiftMenu1.Open();
        else if (!ShapeshiftPlayer2)
            ShapeshiftMenu2.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ShapeshiftButton, ShapeshiftPlayer1, ShapeshiftPlayer2);
            ShapeshiftButton.Begin();
        }
    }

    private bool Exception1(PlayerControl player) => player == ShapeshiftPlayer2 || CommonException(player);

    private bool Exception2(PlayerControl player) => player == ShapeshiftPlayer1 || CommonException(player);

    private bool CommonException(PlayerControl player) => player == Player || (player.Data.IsDead && !BodyByPlayer(player)) || (player.Is(Faction) && Faction is Faction.Intruder or
        Faction.Syndicate && !ShapeshiftMates) || (player.Is(SubFaction) && SubFaction != SubFaction.None && !ShapeshiftMates);

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

    public override void ReadRPC(MessageReader reader)
    {
        if (HoldsDrive)
            return;

        ShapeshiftPlayer1 = reader.ReadPlayer();
        ShapeshiftPlayer2 = reader.ReadPlayer();
    }
}