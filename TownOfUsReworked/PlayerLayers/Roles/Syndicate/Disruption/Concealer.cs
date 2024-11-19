namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Concealer : Syndicate
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number ConcealCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static Number ConcealDur { get; set; } = new(10);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ConcealMates { get; set; } = false;

    public CustomButton ConcealButton { get; set; }
    public PlayerControl ConcealedPlayer { get; set; }
    public CustomPlayerMenu ConcealMenu { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Concealer : CustomColorManager.Syndicate;
    public override string Name => "Concealer";
    public override LayerEnum Type => LayerEnum.Concealer;
    public override Func<string> StartText => () => "Turn The <color=#8CFFFFFF>Crew</color> Invisible For Some Chaos";
    public override Func<string> Description => () => $"- You can turn {(HoldsDrive ? "everyone" : "a player")} invisible\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.SyndicateDisrup;
        ConcealMenu = new(Player, Click, Exception1);
        ConcealedPlayer = null;
        ConcealButton ??= new(this, new SpriteName("Conceal"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)HitConceal, new Cooldown(ConcealCd), (EffectVoid)Conceal,
            (LabelFunc)Label, new Duration(ConcealDur), (EffectEndVoid)UnConceal);
    }

    public void Conceal()
    {
        if (HoldsDrive)
            AllPlayers().ForEach(x => Invis(x, CustomPlayer.Local.Is(Faction.Syndicate)));
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

    public bool Exception1(PlayerControl player) => player == ConcealedPlayer || player == Player || (player.Is(Faction) && !ConcealMates && Faction is Faction.Intruder or Faction.Syndicate)
        || (player.Is(SubFaction) && SubFaction != SubFaction.None && !ConcealMates);

    public string Label() => ConcealedPlayer || HoldsDrive ? "CONCEAL" : "SET TARGET";

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (KeyboardJoystick.player.GetButton("Delete"))
        {
            if (ConcealedPlayer && !HoldsDrive && !ConcealButton.EffectActive)
                ConcealedPlayer = null;

            Message("Removed a target");
        }
    }

    public bool EndEffect() => (ConcealedPlayer && ConcealedPlayer.HasDied()) || (!HoldsDrive && Dead);

    public override void ReadRPC(MessageReader reader)
    {
        if (!HoldsDrive)
            ConcealedPlayer = reader.ReadPlayer();
    }
}