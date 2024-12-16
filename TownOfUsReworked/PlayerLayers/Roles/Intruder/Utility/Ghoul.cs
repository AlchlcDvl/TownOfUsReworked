namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Ghoul : Intruder
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number GhoulMarkCd { get; set; } = new(25);

    public CustomButton MarkButton { get; set; }
    public bool Caught { get; set; }
    public bool Faded { get; set; }
    public PlayerControl MarkedPlayer { get; set; }

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Ghoul: FactionColor;
    public override string Name => "Ghoul";
    public override LayerEnum Type => LayerEnum.Ghoul;
    public override Func<string> StartText => () => "BOO!";
    public override Func<string> Description => () => "- You can mark a player for death every round\n- Marked players will be announced to all players and will die at the end of the next" +
        " meeting if you are not clicked";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.IntruderUtil;
        MarkedPlayer = null;
        MarkButton ??= new(this, new SpriteName("GhoulMark"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClickPlayer)Mark, new Cooldown(GhoulMarkCd), "MARK", new PostDeath(true),
            (PlayerBodyExclusion)Exception1, (UsableFunc)Usable);
    }

    public void Fade()
    {
        if (Disconnected)
            return;

        Faded = true;
        var color = new UColor(1f, 1f, 1f, 0f);

        var maxDistance = Ship().MaxLightRadius * TownOfUsReworked.NormalOptions.CrewLightMod;
        var distance = (CustomPlayer.Local.GetTruePosition() - Player.GetTruePosition()).magnitude;

        var distPercent = distance / maxDistance;
        distPercent = Mathf.Max(0, distPercent - 1);

        var velocity = Player.GetComponent<Rigidbody2D>().velocity.magnitude;
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

    public void Mark(PlayerControl target)
    {
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, MarkedPlayer);
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

    public bool Usable() => !Caught && !MarkedPlayer;

    public bool Exception1(PlayerControl player) => player == MarkedPlayer || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None);

    public override void ReadRPC(MessageReader reader) => MarkedPlayer = reader.ReadPlayer();

    public override void UpdatePlayer()
    {
        if (!Caught)
            Fade();
        else if (Faded)
            UnFade();
    }
}