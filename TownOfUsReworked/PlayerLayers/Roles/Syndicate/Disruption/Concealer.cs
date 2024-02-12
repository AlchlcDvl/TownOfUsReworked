namespace TownOfUsReworked.PlayerLayers.Roles;

public class Concealer : Syndicate
{
    public CustomButton ConcealButton { get; set; }
    public PlayerControl ConcealedPlayer { get; set; }
    public CustomMenu ConcealMenu { get; set; }

    public override UColor Color => ClientGameOptions.CustomSynColors ? CustomColorManager.Concealer : CustomColorManager.Syndicate;
    public override string Name => "Concealer";
    public override LayerEnum Type => LayerEnum.Concealer;
    public override Func<string> StartText => () => "Turn The <color=#8CFFFFFF>Crew</color> Invisible For Some Chaos";
    public override Func<string> Description => () => $"- You can turn {(HoldsDrive ? "everyone" : "a player")} invisible\n{CommonAbilities}";

    public Concealer() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Alignment = Alignment.SyndicateDisrup;
        ConcealMenu = new(Player, Click, Exception1);
        ConcealedPlayer = null;
        ConcealButton = new(this, "Conceal", AbilityTypes.Targetless, "Secondary", HitConceal, CustomGameOptions.ConcealCd, CustomGameOptions.ConcealDur, (CustomButton.EffectVoid)Conceal,
            UnConceal);
        return this;
    }

    public void Conceal()
    {
        if (HoldsDrive)
            CustomPlayer.AllPlayers.ForEach(x => Invis(x, CustomPlayer.Local.Is(Faction.Syndicate)));
        else
            Invis(ConcealedPlayer, CustomPlayer.Local.Is(Faction.Syndicate));
    }

    public void UnConceal()
    {
        if (HoldsDrive)
            DefaultOutfitAll();
        else
            DefaultOutfit(ConcealedPlayer);

        ConcealedPlayer = null;
    }

    public void Click(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            ConcealedPlayer = player;
        else
            ConcealButton.StartCooldown(cooldown);
    }

    public void HitConceal()
    {
        if (HoldsDrive)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, ConcealButton);
            ConcealButton.Begin();
        }
        else if (ConcealedPlayer == null)
            ConcealMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, ConcealButton, ConcealedPlayer);
            ConcealButton.Begin();
        }
    }

    public bool Exception1(PlayerControl player) => player == ConcealedPlayer || player == Player || (player.Is(Faction) && !CustomGameOptions.ConcealMates && Faction is Faction.Intruder or
        Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None && !CustomGameOptions.ConcealMates);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        ConcealButton.Update2(ConcealedPlayer == null && !HoldsDrive ? "SET TARGET" : "CONCEAL");

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (ConcealedPlayer != null && !HoldsDrive && !ConcealButton.EffectActive)
                ConcealedPlayer = null;

            LogMessage("Removed a target");
        }
    }

    public override void TryEndEffect() => ConcealButton.Update3(!HoldsDrive && ConcealedPlayer != null && ConcealedPlayer.HasDied());

    public override void ReadRPC(MessageReader reader)
    {
        if (!HoldsDrive)
            ConcealedPlayer = reader.ReadPlayer();
    }
}