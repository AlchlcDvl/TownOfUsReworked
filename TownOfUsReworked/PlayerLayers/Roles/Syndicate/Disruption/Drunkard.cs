namespace TownOfUsReworked.PlayerLayers.Roles;

public class Drunkard : Syndicate
{
    public CustomButton ConfuseButton { get; set; }
    public float Modifier => ConfuseButton.EffectActive ? -1 : 1;
    public PlayerControl ConfusedPlayer { get; set; }
    public CustomMenu ConfuseMenu { get; set; }

    public override UColor Color => ClientGameOptions.CustomSynColors ? CustomColorManager.Drunkard : CustomColorManager.Syndicate;
    public override string Name => "Drunkard";
    public override LayerEnum Type => LayerEnum.Drunkard;
    public override Func<string> StartText => () => "<i>Burp</i>";
    public override Func<string> Description => () => $"- You can confuse {(HoldsDrive ? "everyone" : "a player")}\n- Confused players will have their controls reverse\n{CommonAbilities}";

    public Drunkard(PlayerControl player) : base(player)
    {
        Alignment = Alignment.SyndicateDisrup;
        ConfuseMenu = new(Player, Click, Exception1);
        ConfusedPlayer = null;
        ConfuseButton = new(this, "Confuse", AbilityTypes.Targetless, "Secondary", HitConfuse, CustomGameOptions.ConfuseCd, CustomGameOptions.ConfuseDur,
            (CustomButton.EffectStartVoid)StartConfusion, UnConfuse);
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
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, ConfuseButton);
            ConfuseButton.Begin();
        }
        else if (ConfusedPlayer == null)
            ConfuseMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, ConfuseButton, ConfusedPlayer);
            ConfuseButton.Begin();
        }
    }

    public bool Exception1(PlayerControl player) => player == ConfusedPlayer || player == Player || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate &&
        !CustomGameOptions.ConfuseImmunity) || (player.Is(SubFaction) && SubFaction != SubFaction.None && !CustomGameOptions.ConfuseImmunity);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        ConfuseButton.Update2(ConfusedPlayer == null && !HoldsDrive ? "SET TARGET" : "CONFUSE");

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (ConfusedPlayer != null && !HoldsDrive && !ConfuseButton.EffectActive)
                ConfusedPlayer = null;

            LogMessage("Removed a target");
        }
    }

    public override void TryEndEffect() => ConfuseButton.Update3(!HoldsDrive && ConfusedPlayer != null && ConfusedPlayer.HasDied());

    public override void ReadRPC(MessageReader reader)
    {
        if (!HoldsDrive)
            ConfusedPlayer = reader.ReadPlayer();
    }
}