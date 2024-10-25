namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Drunkard : Syndicate
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number ConfuseCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static Number ConfuseDur { get; set; } = new(10);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ConfuseImmunity { get; set; } = true;

    public CustomButton ConfuseButton { get; set; }
    public float Modifier => ConfuseButton.EffectActive ? -1 : 1;
    public PlayerControl ConfusedPlayer { get; set; }
    public CustomPlayerMenu ConfuseMenu { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Drunkard : CustomColorManager.Syndicate;
    public override string Name => "Drunkard";
    public override LayerEnum Type => LayerEnum.Drunkard;
    public override Func<string> StartText => () => "<i>Burp</i>";
    public override Func<string> Description => () => $"- You can confuse {(HoldsDrive ? "everyone" : "a player")}\n- Confused players will have their controls reverse\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.SyndicateDisrup;
        ConfuseMenu = new(Player, Click, Exception1);
        ConfusedPlayer = null;
        ConfuseButton ??= CreateButton(this, new SpriteName("Confuse"), AbilityType.Targetless, KeybindType.Secondary, (OnClick)HitConfuse, new Cooldown(ConfuseCd), (LabelFunc)Label,
            new Duration(ConfuseDur), (EffectStartVoid)StartConfusion, (EffectEndVoid)UnConfuse, (EndFunc)EndEffect);
    }

    public void StartConfusion()
    {
        if (CustomPlayer.Local == ConfusedPlayer || HoldsDrive)
            Flash(CustomColorManager.Drunkard);
    }

    public void UnConfuse() => ConfusedPlayer = null;

    public void Click(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            ConfusedPlayer = player;
        else
            ConfuseButton.StartCooldown(cooldown);
    }

    public void HitConfuse()
    {
        if (HoldsDrive)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ConfuseButton);
            ConfuseButton.Begin();
        }
        else if (!ConfusedPlayer)
            ConfuseMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ConfuseButton, ConfusedPlayer);
            ConfuseButton.Begin();
        }
    }

    public bool Exception1(PlayerControl player) => player == ConfusedPlayer || player == Player || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate &&
        ConfuseImmunity) || (player.Is(SubFaction) && SubFaction != SubFaction.None && ConfuseImmunity);

    public string Label() => ConfusedPlayer || HoldsDrive ? "CONFUSE" : "SET TARGET";

    public bool EndEffect() => (ConfusedPlayer && ConfusedPlayer.HasDied()) || (!HoldsDrive && Dead);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (ConfusedPlayer && !HoldsDrive && !ConfuseButton.EffectActive)
                ConfusedPlayer = null;

            Message("Removed a target");
        }
    }

    public override void ReadRPC(MessageReader reader)
    {
        if (!HoldsDrive)
            ConfusedPlayer = reader.ReadPlayer();
    }
}