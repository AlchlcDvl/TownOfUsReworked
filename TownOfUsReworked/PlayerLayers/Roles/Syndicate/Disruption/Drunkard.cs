namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Drunkard)]
public sealed class Drunkard : Syndicate
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number ConfuseCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number ConfuseDur = 10;

    [ToggleOption]
    public static bool ConfuseImmunity = true;

    public CustomButton ConfuseButton { get; private set; }
    public PlayerControl ConfusedPlayer { get; private set; }
    private CustomPlayerMenu ConfuseMenu { get; set; }

    protected override UColor MainColor => CustomColorManager.Drunkard;
    public override LayerEnum Type => LayerEnum.Drunkard;
    public override Func<string> StartText => () => "<i>Burp</i>";
    public override Func<string> Description => () => $"- You can confuse {(HoldsDrive ? "everyone" : "a player")}\n- Confused players will have their controls reverse\n{CommonAbilities}";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Disruption;
        ConfuseMenu = new(Player, Click, Exception1);
        ConfusedPlayer = null;
        ConfuseButton ??= new(this, new SpriteName("Confuse"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitConfuse, new Cooldown(ConfuseCd), (LabelFunc)Label,
            new Duration(ConfuseDur), (EffectStartVoid)StartConfusion, (EffectEndVoid)UnConfuse, (EndFunc)EndEffect);
    }

    private void StartConfusion()
    {
        if (ConfusedPlayer.AmOwner || HoldsDrive)
            Flash(CustomColorManager.Drunkard);
    }

    private void UnConfuse() => ConfusedPlayer = null;

    private void Click(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            ConfusedPlayer = player;
        else
            ConfuseButton.StartCooldown(cooldown);
    }

    private void HitConfuse()
    {
        if (HoldsDrive || ConfusedPlayer)
        {
            using var writer = CreateWriter(CustomRPC.Action, ActionsRPC.ButtonAction, ConfusedPlayer);

            if (writer != null)
            {
                if (ConfusedPlayer)
                    writer.Write(ConfusedPlayer.PlayerId);

                writer.Send();
            }

            ConfuseButton.Begin();
        }
        else
            ConfuseMenu.Open();
    }

    private bool Exception1(PlayerControl player) => player == ConfusedPlayer || player == Player || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate &&
        ConfuseImmunity) || (player.Is(SubFaction) && SubFaction != SubFaction.None && ConfuseImmunity);

    private string Label() => ConfusedPlayer || HoldsDrive ? "CONFUSE" : "SET TARGET";

    private bool EndEffect() => (ConfusedPlayer && ConfusedPlayer.HasDied()) || (!HoldsDrive && Dead);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (HoldsDrive || !KeyboardJoystick.player.GetButtonDown("Delete"))
            return;

        if (ConfusedPlayer && !ConfuseButton.EffectActive)
            ConfusedPlayer = null;

        Message("Removed a target");
    }

    public override void ReadRPC(NetData reader)
    {
        if (!HoldsDrive)
            ConfusedPlayer = reader.ReadPlayer();
    }
}