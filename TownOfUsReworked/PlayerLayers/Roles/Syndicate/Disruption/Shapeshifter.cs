namespace TownOfUsReworked.PlayerLayers.Roles;

public class Shapeshifter : Syndicate
{
    public CustomButton ShapeshiftButton { get; set; }
    public PlayerControl ShapeshiftPlayer1 { get; set; }
    public PlayerControl ShapeshiftPlayer2 { get; set; }
    public CustomMenu ShapeshiftMenu1 { get; set; }
    public CustomMenu ShapeshiftMenu2 { get; set; }

    public override UColor Color => ClientGameOptions.CustomSynColors ? CustomColorManager.Shapeshifter : CustomColorManager.Syndicate;
    public override string Name => "Shapeshifter";
    public override LayerEnum Type => LayerEnum.Shapeshifter;
    public override Func<string> StartText => () => "Change Everyone's Appearances";
    public override Func<string> Description => () => $"- You can {(HoldsDrive ? "shuffle everyone's appearances" : "swap the appearances of 2 players")}\n{CommonAbilities}";

    public Shapeshifter(PlayerControl player) : base(player)
    {
        Alignment = Alignment.SyndicateDisrup;
        ShapeshiftPlayer1 = null;
        ShapeshiftPlayer2 = null;
        ShapeshiftMenu1 = new(Player, Click1, Exception1);
        ShapeshiftMenu2 = new(Player, Click2, Exception2);
        ShapeshiftButton = new(this, "Shapeshift", AbilityTypes.Targetless, "Secondary", HitShapeshift, CustomGameOptions.ShapeshiftCd, CustomGameOptions.ShapeshiftDur,
            (CustomButton.EffectVoid)Shift, UnShapeshift);
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
                var allPlayers = CustomPlayer.AllPlayers;
                var shuffledPlayers = CustomPlayer.AllPlayers;
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
                CustomPlayer.AllPlayers.ForEach(x =>
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
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, ShapeshiftButton);
            ShapeshiftButton.Begin();
        }
        else if (ShapeshiftPlayer1 == null)
            ShapeshiftMenu1.Open();
        else if (ShapeshiftPlayer2 == null)
            ShapeshiftMenu2.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, ShapeshiftButton, ShapeshiftPlayer1, ShapeshiftPlayer2);
            ShapeshiftButton.Begin();
        }
    }

    public bool Exception1(PlayerControl player) => player == Player || player == ShapeshiftPlayer2 || (player.Data.IsDead && BodyByPlayer(player) == null) || (player.Is(Faction) &&
        !CustomGameOptions.ShapeshiftMates && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None &&
        !CustomGameOptions.ShapeshiftMates);

    public bool Exception2(PlayerControl player) => player == Player || player == ShapeshiftPlayer1 || (player.Data.IsDead && BodyByPlayer(player) == null) || (player.Is(Faction) &&
        !CustomGameOptions.ShapeshiftMates && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None &&
        !CustomGameOptions.ShapeshiftMates);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        ShapeshiftButton.Update2(Label());

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (!HoldsDrive && !ShapeshiftButton.EffectActive)
            {
                if (ShapeshiftPlayer2 != null)
                    ShapeshiftPlayer2 = null;
                else if (ShapeshiftPlayer1 != null)
                    ShapeshiftPlayer1 = null;
            }

            LogMessage("Removed a target");
        }
    }

    public string Label()
    {
        if (HoldsDrive)
            return "SHAPESHIFT";
        else if (ShapeshiftPlayer1 == null)
            return "FIRST TARGET";
        else if (ShapeshiftPlayer2 == null)
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