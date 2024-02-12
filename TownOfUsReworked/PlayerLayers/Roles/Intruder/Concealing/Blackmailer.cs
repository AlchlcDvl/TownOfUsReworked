namespace TownOfUsReworked.PlayerLayers.Roles;

public class Blackmailer : Intruder
{
    public CustomButton BlackmailButton { get; set; }
    public PlayerControl BlackmailedPlayer { get; set; }
    public bool ShookAlready { get; set; }
    public Sprite PrevOverlay { get; set; }
    public UColor? PrevColor { get; set; }

    public override UColor Color => ClientGameOptions.CustomIntColors ? CustomColorManager.Blackmailer : CustomColorManager.Intruder;
    public override string Name => "Blackmailer";
    public override LayerEnum Type => LayerEnum.Blackmailer;
    public override Func<string> StartText => () => "You Know Their Secrets";
    public override Func<string> Description => () => "- You can silence players to ensure they cannot hear what others say\n" + (CustomGameOptions.BMRevealed ? ("- Everyone will be " +
        "alerted at the start of the meeting that someone has been silenced ") : "") + (CustomGameOptions.WhispersNotPrivate ? "\n- You can read whispers during meetings" : "") +
        $"\n{CommonAbilities}";

    public Blackmailer() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Alignment = Alignment.IntruderConceal;
        BlackmailedPlayer = null;
        BlackmailButton = new(this, "Blackmail", AbilityTypes.Alive, "Secondary", Blackmail, CustomGameOptions.BlackmailCd, Exception1);
        return this;
    }

    public void Blackmail()
    {
        var cooldown = Interact(Player, BlackmailButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
        {
            BlackmailedPlayer = BlackmailButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, BlackmailedPlayer);
        }

        BlackmailButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => player == BlackmailedPlayer || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate && CustomGameOptions.BlackmailMates)
        || (player.Is(SubFaction) && SubFaction != SubFaction.None && CustomGameOptions.BlackmailMates);

    public override void ReadRPC(MessageReader reader) => BlackmailedPlayer = reader.ReadPlayer();

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        BlackmailButton.Update2("BLACKMAIL");
    }
}