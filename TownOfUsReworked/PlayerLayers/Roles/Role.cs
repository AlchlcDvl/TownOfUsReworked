namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Role : PlayerLayer
{
    protected override UColor MainColor => CustomColorManager.Role;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Role;
    public override LayerEnum Type => LayerEnum.NoneRole;
    protected override UColor LayerColor => FactionColor;
    protected override bool UseMainColor => false;

    public abstract Faction BaseFaction { get; }
    public abstract Alignment Alignment { get; }

    public virtual string StartText => "Woah The Game Started";
    public virtual bool RoleBlockImmune => false;
    public virtual bool AffectedByLights => true;
    public virtual bool CanSwitchVents => true;

    public Faction Faction => Handler.CurrentFaction;
    public UColor FactionColor { get; set; }
    public string FactionColorString => $"<#{FactionColor.ToHtmlStringRGBA()}>";
    public virtual string FactionName => $"{Faction}";

    public Func<string> Objectives { get; set; } = () => "- None";

    public bool Bombed { get; set; }
    private CustomButton BombKillButton { get; set; }

    public bool Requesting { get; set; }
    public PlayerControl Requestor { get; set; }
    private CustomButton PlaceHitButton { get; set; }
    private int BountyTimer { get; set; }

    public override void Init()
    {
        // if (MapPatches.CurrentMap == 4 && CustomGameOptions.CallPlatformButton)
        // {
        //     CallButton ??= new(this, "CALL PLATFORM", "CallPlatform", AbilityTypes.Targetless, KeybindType.Quarternary, (OnClickTargetless)UsePlatform, (UsableFunc)CallUsable,
        //         (ConditionFunc)CallCondition);
        // }

        if (GameModeSettings.GameMode is Mode.HideAndSeek or Mode.TaskRace)
            return;

        if (RoleGenManager.GetSpawnItem(LayerEnum.Enforcer).IsActive())
            BombKillButton ??= new(this, "KILL", new SpriteName("BombKill"), AbilityTypes.Player, KeybindType.Quarternary, (OnClickPlayer)BombKill, (UsableFunc)BombUsable);

        if (RoleGenManager.GetSpawnItem(LayerEnum.BountyHunter).IsActive() && BountyHunter.BountyHunterCanPickTargets)
            PlaceHitButton ??= new(this, "PLACE HIT", new SpriteName("PlaceHit"), AbilityTypes.Player, KeybindType.Quarternary, (OnClickPlayer)PlaceHit, (UsableFunc)RequestUsable);
    }

    public void UpdateButtons()
    {
        try
        {
            var hud = HUD();

            hud.SabotageButton.graphic.sprite = GetSprite($"{Faction}Sabotage");
            hud.SabotageButton.graphic.SetCooldownNormalizedUvs();

            hud.ImpostorVentButton.graphic.sprite = GetSprite($"{Faction}Vent");
            hud.ImpostorVentButton.graphic.SetCooldownNormalizedUvs();

            hud.ReportButton.buttonLabelText.SetOutlineColor(FactionColor);
            hud.UseButton.buttonLabelText.SetOutlineColor(FactionColor);
            hud.PetButton.buttonLabelText.SetOutlineColor(FactionColor);
            hud.ImpostorVentButton.buttonLabelText.SetOutlineColor(FactionColor);
            hud.SabotageButton.buttonLabelText.SetOutlineColor(FactionColor);

            Player.GetButtons().Do(x => x.UpdateSprite());
        } catch {}
    }

    public virtual List<PlayerControl> Team()
    {
        var team = new List<PlayerControl>() { Player };

        switch (Handler.CurrentDisposition)
        {
            case Paired pair:
            {
                team.Add(pair.Other);
                break;
            }
            case Mafia:
            {
                team.AddRange(AllPlayers().Where(x => x != Player && x.Is<Mafia>()));
                break;
            }
        }

        if (Faction == Faction.Cabal && Alignment != Alignment.Neophyte)
        {
            var jackal = Player.GetJackal();
            team.Add(jackal.Player);
            team.AddRange(jackal.GetOtherRecruits(Player));
        }

        return team;
    }

    public override void OnIntroEnd() => UpdateButtons();

    private bool BombUsable() => Bombed;

    private bool RequestUsable() => Requesting;

    public virtual void Reset(bool meeting, bool start)
    {
        if (Requesting && !start)
            BountyTimer++;
    }

    public void RoleUpdate(Role former, PlayerControl player = null, bool inherit = false)
    {
        player ??= former.Player;
        CustomButton.AllButtons.Where(x => x.Owner == former || !x.Owner.Player).Do(x => x.Destroy());
        CustomArrow.AllArrows.Where(x => x.Owner == player).Do(x => x.Disable());
        former.End();
        Start(player);

        if (Local)
        {
            ButtonUtils.Reset();
            Player.RegenTask();
            Flash(Color);
            UpdateButtons();
        }

        if (LocalPlayer.Is<Seer>(out var seer))
            Flash(seer.Color);

        if (LayerHandler.Handlers.TryGetValue(player.PlayerId, out var handler))
            handler.SetUpLayers(inherit);
    }

    public override void UpdateMap(MapBehaviour __instance) => __instance.ColorControl.SetColor(Color);

    public override void BeforeMeeting()
    {
        if (!Requesting || BountyTimer <= 2)
            return;

        CallRpc(CustomRPC.Action, ActionsRPC.PlaceHit, Player, Player);
        Requestor.GetLayer<BountyHunter>().TentativeTarget = Player;
        Requesting = false;
        Requestor = null;
    }

    private void PlaceHit(PlayerControl target)
    {
        target = Requestor.IsLinkedTo(target) ? Player : target;
        Requestor.GetLayer<BountyHunter>().TentativeTarget = target;
        Requesting = false;
        Requestor = null;
        CallRpc(CustomRPC.Action, ActionsRPC.PlaceHit, Player, target);
    }

    private void BombKill(PlayerControl target)
    {
        var success = Interact(Player, target, true) != CooldownType.Fail;
        GetLayers<Enforcer>().Where(x => x.BombedPlayer == Player).Do(x => x.BombSuccessful = success);
        CallRpc(CustomRPC.Action, ActionsRPC.ForceKill, Player, success);
    }

    public static IEnumerable<Role> GetRoles(Faction faction) => GetLayers<Role>().Where(x => x.Faction == faction && !x.Deinitialised);
}