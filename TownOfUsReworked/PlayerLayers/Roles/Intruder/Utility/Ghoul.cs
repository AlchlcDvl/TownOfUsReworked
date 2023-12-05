namespace TownOfUsReworked.PlayerLayers.Roles;

public class Ghoul : Intruder
{
    public CustomButton MarkButton { get; set; }
    public bool Caught { get; set; }
    public bool Faded { get; set; }
    public PlayerControl MarkedPlayer { get; set; }

    public override Color Color => ClientGameOptions.CustomIntColors ? Colors.Ghoul : Colors.Intruder;
    public override string Name => "Ghoul";
    public override LayerEnum Type => LayerEnum.Ghoul;
    public override Func<string> StartText => () => "BOO!";
    public override Func<string> Description => () => "- You can mark a player for death every round\n- Marked players will be announced to all players and will die at the end of the "
        + "next meeting if you are not clicked";

    public Ghoul(PlayerControl player) : base(player)
    {
        Alignment = Alignment.IntruderUtil;
        MarkedPlayer = null;
        MarkButton = new(this, "GhoulMark", AbilityTypes.Target, "ActionSecondary", Mark, CustomGameOptions.GhoulMarkCd, Exception1, true);
    }

    public void Fade()
    {
        if (Disconnected)
            return;

        Faded = true;
        Player.Visible = true;
        var color = new Color(1f, 1f, 1f, 0f);

        var maxDistance = Ship.MaxLightRadius * TownOfUsReworked.NormalOptions.CrewLightMod;
        var distance = (CustomPlayer.Local.GetTruePosition() - Player.GetTruePosition()).magnitude;

        var distPercent = distance / maxDistance;
        distPercent = Mathf.Max(0, distPercent - 1);

        var velocity = Player.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
        color.a = 0.07f + (velocity / Player.MyPhysics.TrueSpeed * 0.13f);
        color.a = Mathf.Lerp(color.a, 0, distPercent);

        if (Player.GetCustomOutfitType() != CustomPlayerOutfitType.PlayerNameOnly)
            Player.SetOutfit(CustomPlayerOutfitType.PlayerNameOnly, BlankOutfit(Player));

        Player.MyRend().color = color;
        Player.NameText().color = new(0f, 0f, 0f, 0f);
        Player.cosmetics.colorBlindText.color = new(0f, 0f, 0f, 0f);

        if (Local)
            Camouflage();
    }

    public void Mark()
    {
        MarkedPlayer = MarkButton.TargetPlayer;
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, MarkedPlayer);
        MarkButton.StartCooldown();
    }

    public void UnFade()
    {
        DefaultOutfit(Player);
        Player.MyRend().color = UColor.white;
        Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
        Faded = false;
        Player.MyPhysics.ResetMoveState();

        if (Local)
            DefaultOutfitAll();
    }

    public bool Usable() => !Caught;

    public bool Exception1(PlayerControl player) => player == MarkedPlayer || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        KillButton.Disable();
        MarkButton.Update2("MARK", MarkedPlayer != null);
    }

    public override void ReadRPC(MessageReader reader) => MarkedPlayer = reader.ReadPlayer();
}