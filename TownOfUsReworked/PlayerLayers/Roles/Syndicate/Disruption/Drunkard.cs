namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Drunkard : Syndicate
{
    public CustomButton ConfuseButton { get; set; }
    public float Modifier => ConfuseButton.EffectActive ? -1 : 1;
    public PlayerControl ConfusedPlayer { get; set; }
    public CustomMenu ConfuseMenu { get; set; }

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
        ConfuseButton = CreateButton(this, new SpriteName("Confuse"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)HitConfuse, new Cooldown(CustomGameOptions.ConfuseCd),
            new Duration(CustomGameOptions.ConfuseDur), (EffectStartVoid)StartConfusion, (EffectEndVoid)UnConfuse, (LabelFunc)Label, (EndFunc)EndEffect);
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
        CustomGameOptions.ConfuseImmunity) || (player.Is(SubFaction) && SubFaction != SubFaction.None && CustomGameOptions.ConfuseImmunity);

    public string Label() => ConfusedPlayer || HoldsDrive ? "CONFUSE" : "SET TARGET";

    public bool EndEffect() => (ConfusedPlayer && ConfusedPlayer.HasDied()) || (!HoldsDrive && Dead);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (ConfusedPlayer && !HoldsDrive && !ConfuseButton.EffectActive)
                ConfusedPlayer = null;

            LogMessage("Removed a target");
        }
    }

    public override void ReadRPC(MessageReader reader)
    {
        if (!HoldsDrive)
            ConfusedPlayer = reader.ReadPlayer();
    }
}