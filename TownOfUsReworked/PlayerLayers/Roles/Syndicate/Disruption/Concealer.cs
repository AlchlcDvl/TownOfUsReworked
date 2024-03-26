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

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.SyndicateDisrup;
        ConcealMenu = new(Player, Click, Exception1);
        ConcealedPlayer = null;
        ConcealButton = CreateButton(this, new SpriteName("Conceal"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)HitConceal, new Cooldown(CustomGameOptions.ConcealCd),
            (LabelFunc)Label, new Duration(CustomGameOptions.ConcealDur), (EffectVoid)Conceal, (EffectEndVoid)UnConceal);
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
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ConcealButton);
            ConcealButton.Begin();
        }
        else if (!ConcealedPlayer)
            ConcealMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ConcealButton, ConcealedPlayer);
            ConcealButton.Begin();
        }
    }

    public bool Exception1(PlayerControl player) => player == ConcealedPlayer || player == Player || (player.Is(Faction) && !CustomGameOptions.ConcealMates && Faction is Faction.Intruder or
        Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None && !CustomGameOptions.ConcealMates);

    public string Label() => ConcealedPlayer || HoldsDrive ? "CONCEAL" : "SET TARGET";

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (ConcealedPlayer && !HoldsDrive && !ConcealButton.EffectActive)
                ConcealedPlayer = null;

            LogMessage("Removed a target");
        }
    }

    public bool EndEffect() => (ConcealedPlayer && ConcealedPlayer.HasDied()) || (!HoldsDrive && Dead);

    public override void ReadRPC(MessageReader reader)
    {
        if (!HoldsDrive)
            ConcealedPlayer = reader.ReadPlayer();
    }
}