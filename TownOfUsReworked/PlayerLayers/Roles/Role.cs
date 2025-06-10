namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Role : PlayerLayer
{
    protected override UColor MainColor => CustomColorManager.Role;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Role;
    public override LayerEnum Type => LayerEnum.NoneRole;
    protected override UColor LayerColor => FactionColor;
    protected override bool UseMainColor => false;

    public abstract Faction BaseFaction { get; }

    public virtual Func<string> StartText => () => "Woah The Game Started";
    public virtual bool RoleBlockImmune => false;
    public virtual bool AffectedByLights => true;
    public virtual bool CanSwitchVents => true;

    public Alignment Alignment { get; protected set; }

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

        if (Handler.CurrentDisposition is Lovers)
            team.Add(Player.GetOtherLover());
        else if (Handler.CurrentDisposition is Rivals)
            team.Add(Player.GetOtherRival());
        else if (Handler.CurrentDisposition is Mafia)
            team.AddRange(AllPlayers().Where(x => x != Player && x.Is<Mafia>()));

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

    public override void OnMeetingEnd(MeetingHud __instance) => GetLayers<Werewolf>().Do(x => x.Rounds++);

    public override void UpdateMap(MapBehaviour __instance)
    {
        __instance.ColorControl.baseColor = Color;
        __instance.ColorControl.SetColor(Color);
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        GetLayers<Arsonist>().Do(x => x.Doused.Clear());

        if (Requesting && BountyTimer > 2)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.PlaceHit, Player, Player);
            Requestor.GetLayer<BountyHunter>().TentativeTarget = Player;
            Requesting = false;
            Requestor = null;
        }

        foreach (var bh in GetLayers<BountyHunter>())
        {
            if (bh.TargetPlayer || !bh.TentativeTarget || bh.Assigned)
                continue;

            bh.TargetPlayer = bh.TentativeTarget;
            bh.Assigned = true;

            // Ensures only the Bounty Hunter sees this
            if (bh.Local)
                Run("<#B51E39FF>〖 Bounty Hunt 〗</color>", "Your bounty has been received! Prepare to hunt.");
        }

        foreach (var dict in GetLayers<Dictator>())
        {
            dict.ToBeEjected = null;
            dict.Tribunal = false;
        }

        foreach (var cryo in GetLayers<Cryomaniac>())
        {
            cryo.FreezeUsed = false;
            cryo.Doused.Clear();
        }
    }

    private void PlaceHit(PlayerControl target)
    {
        target = Requestor.IsLinkedTo(target) ? Player : target;
        Requestor.GetLayer<BountyHunter>().TentativeTarget = target;
        Requesting = false;
        Requestor = null;
        CallRpc(CustomRPC.Action, ActionsRPC.PlaceHit, Player, target);
    }

    public static void PublicReveal(PlayerControl player)
    {
        if (!player.Is<Sovereign>(out var revealer))
            return;

        Flash(revealer.Color);
        BreakShield(player, true);
        GetLayers<ITrapper>().Do(x => x.Trapped.Remove(player.PlayerId));
        revealer.Revealed = true;
        revealer.OnReveal();
    }

    public static void BreakShield(PlayerControl player, bool flag)
    {
        foreach (var role2 in GetLayers<IShielder>())
        {
            if (role2.ShieldedPlayer != player)
                continue;

            if ((role2.Local && Medic.WhoGetsNotification == ShieldOptions.Medic) || Medic.WhoGetsNotification == ShieldOptions.Everyone || (player.AmOwner && Medic.WhoGetsNotification ==
                ShieldOptions.Shielded))
            {
                var roleEffectAnimation = UObject.Instantiate(GetRoleAnim("ProtectAnim"), player.gameObject.transform);
                roleEffectAnimation.SetMaskLayerBasedOnWhoShouldSee(true);
                roleEffectAnimation.Play(player, null, player.cosmetics.FlipX, RoleEffectAnimation.SoundType.Global);
                Flash(role2.Color);
            }

            if (!flag)
                continue;

            role2.ShieldedPlayer = null;
            role2.ShieldBroken = true;

            if (TownOfUsReworked.MciActive)
                Message(player.name + " Is Now Ex-Shielded");
        }
    }

    public static void BastionBomb(Vent vent, bool flag)
    {
        foreach (var role2 in GetLayers<IVentBomber>())
        {
            if (role2.BombedIDs.Contains(vent.Id) && role2.Local)
                Flash(role2.Color);

            if (flag)
                role2.BombedIDs.Remove(vent.Id);
        }
    }

    private void BombKill(PlayerControl target)
    {
        var success = Interact(Player, target, true) != CooldownType.Fail;
        GetLayers<Enforcer>().Where(x => x.BombedPlayer == Player).Do(x => x.BombSuccessful = success);
        CallRpc(CustomRPC.Action, ActionsRPC.ForceKill, Player, success);
    }

    public static IEnumerable<Role> GetRoles(Faction faction) => GetLayers<Role>().Where(x => x.Faction == faction && !x.Deinitialised);

    public static IEnumerable<Role> GetRoles(Alignment ra) => GetLayers<Role>().Where(x => x.Alignment == ra && !x.Deinitialised);
}