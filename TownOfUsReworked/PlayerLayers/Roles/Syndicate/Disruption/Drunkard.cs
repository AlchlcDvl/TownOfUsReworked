namespace TownOfUsReworked.PlayerLayers.Roles;

public class Drunkard : Syndicate
{
    public CustomButton ConfuseButton { get; set; }
    public bool Enabled { get; set; }
    public DateTime LastConfused { get; set; }
    public float TimeRemaining { get; set; }
    public bool Confused => TimeRemaining > 0f;
    public float Modifier => Confused ? -1 : 1;
    public PlayerControl ConfusedPlayer { get; set; }
    public CustomMenu ConfuseMenu { get; set; }

    public override Color32 Color => ClientGameOptions.CustomSynColors ? Colors.Drunkard : Colors.Syndicate;
    public override string Name => "Drunkard";
    public override LayerEnum Type => LayerEnum.Drunkard;
    public override Func<string> StartText => () => "<i>Burp</i>";
    public override Func<string> Description => () => $"- You can confuse {(HoldsDrive ? "everyone" : "a player")}\n- Confused players will have their controls reverse\n" +
        CommonAbilities;
    public override InspectorResults InspectorResults => InspectorResults.HindersOthers;
    public float Timer => ButtonUtils.Timer(Player, LastConfused, CustomGameOptions.ConfuseCd);

    public Drunkard(PlayerControl player) : base(player)
    {
        RoleAlignment = RoleAlignment.SyndicateDisrup;
        ConfuseMenu = new(Player, Click, Exception1);
        ConfusedPlayer = null;
        ConfuseButton = new(this, "Confuse", AbilityTypes.Effect, "Secondary", HitConfuse);
    }

    public void Confuse()
    {
        if (!Enabled && (CustomPlayer.Local == ConfusedPlayer || HoldsDrive))
            Flash(Color);

        Enabled = true;
        TimeRemaining -= Time.deltaTime;

        if (Meeting || (ConfusedPlayer == null && !HoldsDrive))
            TimeRemaining = 0f;
    }

    public void UnConfuse()
    {
        Enabled = false;
        LastConfused = DateTime.UtcNow;
        ConfusedPlayer = null;
    }

    public void Click(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact[3])
            ConfusedPlayer = player;
        else if (interact[0])
            LastConfused = DateTime.UtcNow;
        else if (interact[1])
            LastConfused.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public void HitConfuse()
    {
        if (Timer != 0f || Confused)
            return;

        if (HoldsDrive)
        {
            TimeRemaining = CustomGameOptions.ConfuseDur;
            Confuse();
            CallRpc(CustomRPC.Action, ActionsRPC.Confuse, this);
        }
        else if (ConfusedPlayer == null)
            ConfuseMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.Confuse, this, ConfusedPlayer);
            TimeRemaining = CustomGameOptions.ConfuseDur;
            Confuse();
        }
    }

    public bool Exception1(PlayerControl player) => player == ConfusedPlayer || player == Player || (player.Is(Faction) && !CustomGameOptions.ConfuseImmunity);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        var flag = ConfusedPlayer == null && !HoldsDrive;
        ConfuseButton.Update(flag ? "SET TARGET" : "CONFUSE", Timer, CustomGameOptions.ConfuseCd, Confused, TimeRemaining, CustomGameOptions.ConfuseDur);

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (ConfusedPlayer != null && !HoldsDrive && !Confused)
                ConfusedPlayer = null;

            LogInfo("Removed a target");
        }
    }
}