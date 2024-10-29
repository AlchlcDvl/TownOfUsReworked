namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Shapeshifter : Syndicate
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number ShapeshiftCd { get; set; } = new(25f);

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static Number ShapeshiftDur { get; set; } = new(10);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ShapeshiftMates { get; set; } = false;

    public CustomButton ShapeshiftButton { get; set; }
    public PlayerControl ShapeshiftPlayer1 { get; set; }
    public PlayerControl ShapeshiftPlayer2 { get; set; }
    public CustomPlayerMenu ShapeshiftMenu1 { get; set; }
    public CustomPlayerMenu ShapeshiftMenu2 { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Shapeshifter : CustomColorManager.Syndicate;
    public override string Name => "Shapeshifter";
    public override LayerEnum Type => LayerEnum.Shapeshifter;
    public override Func<string> StartText => () => "Change Everyone's Appearances";
    public override Func<string> Description => () => $"- You can {(HoldsDrive ? "shuffle everyone's appearances" : "swap the appearances of 2 players")}\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.SyndicateDisrup;
        ShapeshiftPlayer1 = null;
        ShapeshiftPlayer2 = null;
        ShapeshiftMenu1 = new(Player, Click1, Exception1);
        ShapeshiftMenu2 = new(Player, Click2, Exception2);
        ShapeshiftButton ??= CreateButton(this, "Shapeshift", AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)HitShapeshift, new Cooldown(ShapeshiftCd), (EffectEndVoid)UnShapeshift,
            new Duration(ShapeshiftDur), (EffectVoid)Shift, (LabelFunc)Label);
    }

    public void Shift() => Shapeshift(ShapeshiftPlayer1, ShapeshiftPlayer2, HoldsDrive);

    public static void Shapeshift(PlayerControl player1, PlayerControl player2, bool drived)
    {
        if (!drived)
        {
            Morph(player1, player2);
            Morph(player2, player1);
        }
        else
        {
            if (!Shapeshifted)
            {
                Shapeshifted = true;
                var allPlayers = AllPlayers();
                var shuffledPlayers = AllPlayers();
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
    }

    public void UnShapeshift()
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

    public void Click1(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            ShapeshiftPlayer1 = player;
        else
            ShapeshiftButton.StartCooldown(cooldown);
    }

    public void Click2(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            ShapeshiftPlayer2 = player;
        else
            ShapeshiftButton.StartCooldown(cooldown);
    }

    public void HitShapeshift()
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

    public bool Exception1(PlayerControl player) => player == ShapeshiftPlayer2 || CommonException(player);

    public bool Exception2(PlayerControl player) => player == ShapeshiftPlayer1 || CommonException(player);

    public bool CommonException(PlayerControl player) => player == Player || (player.Data.IsDead && !BodyByPlayer(player)) || (player.Is(Faction) && Faction is Faction.Intruder or
        Faction.Syndicate && !ShapeshiftMates) || (player.Is(SubFaction) && SubFaction != SubFaction.None && !ShapeshiftMates);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (!HoldsDrive && !ShapeshiftButton.EffectActive)
            {
                if (ShapeshiftPlayer2)
                    ShapeshiftPlayer2 = null;
                else if (ShapeshiftPlayer1)
                    ShapeshiftPlayer1 = null;
            }

            Message("Removed a target");
        }
    }

    public string Label()
    {
        if (HoldsDrive)
            return "SHAPESHIFT";
        else if (!ShapeshiftPlayer1)
            return "FIRST TARGET";
        else if (!ShapeshiftPlayer2)
            return "SECOND TARGET";
        else
            return "SHAPESHIFT";
    }

    public override void ReadRPC(MessageReader reader)
    {
        if (!HoldsDrive)
        {
            ShapeshiftPlayer1 = reader.ReadPlayer();
            ShapeshiftPlayer2 = reader.ReadPlayer();
        }
    }
}