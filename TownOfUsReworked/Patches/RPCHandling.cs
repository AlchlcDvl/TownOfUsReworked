namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    public static class RPCHandling
    {
        public static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
        {
            switch ((CustomRPC)callId)
            {
                case CustomRPC.SetLayer:
                    RoleGen.SetLayer(reader.ReadInt32(), Utils.PlayerById(reader.ReadByte()), (PlayerLayerEnum)reader.ReadByte());
                    break;

                case CustomRPC.NullAbility:
                    _ = new Abilityless(Utils.PlayerById(reader.ReadByte()));
                    break;

                case CustomRPC.NullModifier:
                    _ = new Modifierless(Utils.PlayerById(reader.ReadByte()));
                    break;

                case CustomRPC.NullObjectifier:
                    _ = new Objectifierless(Utils.PlayerById(reader.ReadByte()));
                    break;

                case CustomRPC.NullRole:
                    _ = new Roleless(Utils.PlayerById(reader.ReadByte()));
                    break;

                case CustomRPC.SetRevealer:
                    SetPostmortals.WillBeRevealer = Utils.PlayerById(reader.ReadByte());
                    break;

                case CustomRPC.SetPhantom:
                    SetPostmortals.WillBePhantom = Utils.PlayerById(reader.ReadByte());
                    break;

                case CustomRPC.SetBanshee:
                    SetPostmortals.WillBeBanshee = Utils.PlayerById(reader.ReadByte());
                    break;

                case CustomRPC.SetGhoul:
                    SetPostmortals.WillBeGhoul = Utils.PlayerById(reader.ReadByte());
                    break;

                case CustomRPC.Whisper:
                    if (!HudManager.Instance.Chat)
                        break;

                    var whisperer = Utils.PlayerById(reader.ReadByte());
                    var whispered = Utils.PlayerById(reader.ReadByte());
                    var message = reader.ReadString();

                    if (whispered == CustomPlayer.Local)
                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{whisperer.name} whispers to you:{message}");
                    else if ((CustomPlayer.Local.Is(RoleEnum.Blackmailer) && CustomGameOptions.WhispersNotPrivate) || ConstantVariables.DeadSeeEverything ||
                        (CustomPlayer.Local.Is(RoleEnum.Silencer) && CustomGameOptions.WhispersNotPrivateSilencer))
                    {
                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{whisperer.name} is whispering to {whispered.name}: {message}");
                    }
                    else if (CustomGameOptions.WhispersAnnouncement)
                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{whisperer.name} is whispering to {whispered.name}.");

                    break;

                case CustomRPC.CatchPhantom:
                    var phan = Utils.PlayerById(reader.ReadByte());
                    Role.GetRole<Phantom>(phan).Caught = true;
                    phan.Exiled();
                    break;

                case CustomRPC.CatchBanshee:
                    var ban = Utils.PlayerById(reader.ReadByte());
                    Role.GetRole<Banshee>(ban).Caught = true;
                    ban.Exiled();
                    break;

                case CustomRPC.CatchGhoul:
                    var gho = Utils.PlayerById(reader.ReadByte());
                    Role.GetRole<Ghoul>(gho).Caught = true;
                    gho.Exiled();
                    break;

                case CustomRPC.Start:
                    RoleGen.ResetEverything();
                    break;

                case CustomRPC.AttemptSound:
                    Role.BreakShield(reader.ReadByte(), CustomGameOptions.ShieldBreaks);
                    break;

                case CustomRPC.CatchRevealer:
                    var rev = Utils.PlayerById(reader.ReadByte());
                    var revRole = Role.GetRole<Revealer>(rev);
                    revRole.Caught = true;
                    revRole.CompletedTasks = false;
                    rev.Exiled();
                    break;

                case CustomRPC.AddVoteBank:
                    Ability.GetAbility<Politician>(Utils.PlayerById(reader.ReadByte())).VoteBank += reader.ReadInt32();
                    break;

                case CustomRPC.MeetingStart:
                    foreach (var body in Utils.AllBodies)
                        body.gameObject.Destroy();

                    foreach (var player10 in CustomPlayer.AllPlayers)
                        player10.MyPhysics.ResetAnimState();

                    break;

                case CustomRPC.DoorSyncToilet:
                    UObject.FindObjectsOfType<PlainDoor>().FirstOrDefault(door => door.Id == reader.ReadInt32())?.SetDoorway(true);
                    break;

                case CustomRPC.SyncPlateform:
                    CallPlateform.SyncPlateform(reader.ReadBoolean());
                    break;

                case CustomRPC.CheckMurder:
                    Utils.PlayerById(reader.ReadByte()).CheckMurder(Utils.PlayerById(reader.ReadByte()));
                    break;

                case CustomRPC.SetColor:
                    Utils.PlayerById(reader.ReadByte()).SetColor(reader.ReadByte());
                    break;

                case CustomRPC.VersionHandshake:
                    Utils.VersionHandshake(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), new(reader.Length - reader.Position >= 17 ? reader.ReadBytes(16) :
                        new byte[16]), reader.ReadPackedInt32());
                    break;

                case CustomRPC.SubmergedFixOxygen:
                    ModCompatibility.RepairOxygen();
                    break;

                case CustomRPC.FixLights:
                    var lights = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                    lights.ActualSwitches = lights.ExpectedSwitches;
                    break;

                case CustomRPC.RemoveMeetings:
                    Utils.PlayerById(reader.ReadByte()).RemainingEmergencies = 0;
                    break;

                case CustomRPC.SetReports:
                    Utils.PlayerById(reader.ReadByte()).MaxReportDistance = CustomGameOptions.ReportDistance;
                    break;

                case CustomRPC.SetSpawnAirship:
                    SpawnInMinigamePatch.SpawnPoints.Clear();
                    SpawnInMinigamePatch.SpawnPoints.AddRange(reader.ReadBytesAndSize().ToList());
                    break;

                case CustomRPC.ChaosDrive:
                    Role.DriveHolder = Utils.PlayerById(reader.ReadByte());
                    Role.SyndicateHasChaosDrive = true;
                    break;

                case CustomRPC.SetPos:
                    var setplayer = Utils.PlayerById(reader.ReadByte());
                    var pos = reader.ReadVector2();
                    pos.y += 0.3636f;
                    setplayer.transform.position = new(pos.x, pos.y, setplayer.transform.position.z);
                    break;

                case CustomRPC.SyncCustomSettings:
                    RPC.ReceiveRPC(reader);
                    break;

                case CustomRPC.Notify:
                    ChatCommands.Notify(reader.ReadByte());
                    break;

                case CustomRPC.SetSettings:
                    var map = reader.ReadByte();
                    TownOfUsReworked.VanillaOptions.MapId = map == 255 ? (byte)0 : map;
                    TownOfUsReworked.VanillaOptions.RoleOptions.SetRoleRate(RoleTypes.Scientist, 0, 0);
                    TownOfUsReworked.VanillaOptions.RoleOptions.SetRoleRate(RoleTypes.Engineer, 0, 0);
                    TownOfUsReworked.VanillaOptions.RoleOptions.SetRoleRate(RoleTypes.GuardianAngel, 0, 0);
                    TownOfUsReworked.VanillaOptions.RoleOptions.SetRoleRate(RoleTypes.Shapeshifter, 0, 0);
                    TownOfUsReworked.VanillaOptions.CrewLightMod = CustomGameOptions.CrewVision;
                    TownOfUsReworked.VanillaOptions.ImpostorLightMod = CustomGameOptions.IntruderVision;
                    TownOfUsReworked.VanillaOptions.AnonymousVotes = CustomGameOptions.AnonymousVoting;
                    TownOfUsReworked.VanillaOptions.VisualTasks = CustomGameOptions.VisualTasks;
                    TownOfUsReworked.VanillaOptions.PlayerSpeedMod = CustomGameOptions.PlayerSpeed;
                    TownOfUsReworked.VanillaOptions.NumImpostors = CustomGameOptions.IntruderCount;
                    TownOfUsReworked.VanillaOptions.TaskBarMode = (AmongUs.GameOptions.TaskBarMode)CustomGameOptions.TaskBarMode;
                    TownOfUsReworked.VanillaOptions.ConfirmImpostor = CustomGameOptions.ConfirmEjects;
                    TownOfUsReworked.VanillaOptions.VotingTime = CustomGameOptions.VotingTime;
                    TownOfUsReworked.VanillaOptions.DiscussionTime = CustomGameOptions.DiscussionTime;
                    TownOfUsReworked.VanillaOptions.KillDistance = CustomGameOptions.InteractionDistance;
                    TownOfUsReworked.VanillaOptions.EmergencyCooldown = CustomGameOptions.EmergencyButtonCooldown;
                    TownOfUsReworked.VanillaOptions.NumEmergencyMeetings = CustomGameOptions.EmergencyButtonCount;
                    TownOfUsReworked.VanillaOptions.KillCooldown = CustomGameOptions.IntKillCooldown;
                    TownOfUsReworked.VanillaOptions.GhostsDoTasks = CustomGameOptions.GhostTasksCountToWin;
                    TownOfUsReworked.VanillaOptions.MaxPlayers = CustomGameOptions.LobbySize;
                    GameOptionsManager.Instance.currentNormalGameOptions = TownOfUsReworked.VanillaOptions;

                    foreach (var player in CustomPlayer.AllPlayers)
                        player.MaxReportDistance = CustomGameOptions.ReportDistance;

                    break;

                case CustomRPC.Change:
                    var id5 = reader.ReadByte();

                    switch ((TurnRPC)id5)
                    {
                        case TurnRPC.TurnFanatic:
                            Objectifier.GetObjectifier<Fanatic>(Utils.PlayerById(reader.ReadByte())).TurnFanatic((Faction)reader.ReadByte());
                            break;

                        case TurnRPC.TurnTraitorBetrayer:
                            Objectifier.GetObjectifier<Traitor>(Utils.PlayerById(reader.ReadByte())).TurnBetrayer();
                            break;

                        case TurnRPC.TurnFanaticBetrayer:
                            Objectifier.GetObjectifier<Fanatic>(Utils.PlayerById(reader.ReadByte())).TurnBetrayer();
                            break;

                        case TurnRPC.TurnSides:
                            Objectifier.GetObjectifier<Defector>(Utils.PlayerById(reader.ReadByte())).TurnSides();
                            break;

                        case TurnRPC.TurnVigilante:
                            Role.GetRole<VampireHunter>(Utils.PlayerById(reader.ReadByte())).TurnVigilante();
                            break;

                        case TurnRPC.TurnPestilence:
                            Role.GetRole<Plaguebearer>(Utils.PlayerById(reader.ReadByte())).TurnPestilence();
                            break;

                        case TurnRPC.TurnTroll:
                            Role.GetRole<BountyHunter>(Utils.PlayerById(reader.ReadByte())).TurnTroll();
                            break;

                        case TurnRPC.TurnSurv:
                            Role.GetRole<GuardianAngel>(Utils.PlayerById(reader.ReadByte())).TurnSurv();
                            break;

                        case TurnRPC.TurnGodfather:
                            Role.GetRole<Mafioso>(Utils.PlayerById(reader.ReadByte())).TurnGodfather();
                            break;

                        case TurnRPC.TurnJest:
                            Role.GetRole<Executioner>(Utils.PlayerById(reader.ReadByte())).TurnJest();
                            break;

                        case TurnRPC.TurnTraitor:
                            Objectifier.GetObjectifier<Traitor>(Utils.PlayerById(reader.ReadByte()));
                            break;

                        case TurnRPC.TurnRebel:
                            Role.GetRole<Sidekick>(Utils.PlayerById(reader.ReadByte())).TurnRebel();
                            break;

                        case TurnRPC.TurnThief:
                            Role.GetRole<Amnesiac>(Utils.PlayerById(reader.ReadByte())).TurnThief();
                            break;

                        case TurnRPC.TurnSheriff:
                            Role.GetRole<Seer>(Utils.PlayerById(reader.ReadByte())).TurnSheriff();
                            break;

                        case TurnRPC.TurnAct:
                            Role.GetRole<Guesser>(Utils.PlayerById(reader.ReadByte())).TurnAct();
                            break;

                        case TurnRPC.TurnSeer:
                            Role.GetRole<Mystic>(Utils.PlayerById(reader.ReadByte())).TurnSeer();
                            break;
                    }

                    break;

                case CustomRPC.Target:
                    var id2 = reader.ReadByte();

                    switch ((TargetRPC)id2)
                    {
                        case TargetRPC.SetExeTarget:
                            Role.GetRole<Executioner>(Utils.PlayerById(reader.ReadByte())).TargetPlayer = Utils.PlayerById(reader.ReadByte());
                            break;

                        case TargetRPC.SetGuessTarget:
                            Role.GetRole<Guesser>(Utils.PlayerById(reader.ReadByte())).TargetPlayer = Utils.PlayerById(reader.ReadByte());
                            break;

                        case TargetRPC.SetGATarget:
                            Role.GetRole<GuardianAngel>(Utils.PlayerById(reader.ReadByte())).TargetPlayer = Utils.PlayerById(reader.ReadByte());
                            break;

                        case TargetRPC.SetBHTarget:
                            Role.GetRole<BountyHunter>(Utils.PlayerById(reader.ReadByte())).TargetPlayer = Utils.PlayerById(reader.ReadByte());
                            break;

                        case TargetRPC.SetActPretendList:
                            Role.GetRole<Actor>(Utils.PlayerById(reader.ReadByte())).PretendRoles = (InspectorResults)reader.ReadByte();
                            break;

                        case TargetRPC.SetGoodRecruit:
                            var jackal = Utils.PlayerById(reader.ReadByte());
                            var goodRecruit = Utils.PlayerById(reader.ReadByte());
                            var jackalRole = Role.GetRole<Jackal>(jackal);
                            jackalRole.GoodRecruit = goodRecruit;
                            jackalRole.Recruited.Add(goodRecruit.PlayerId);
                            Role.GetRole(goodRecruit).SubFaction = SubFaction.Cabal;
                            Role.GetRole(goodRecruit).SubFactionColor = Colors.Cabal;
                            Role.GetRole(goodRecruit).IsRecruit = true;
                            break;

                        case TargetRPC.SetEvilRecruit:
                            var jackal2 = Utils.PlayerById(reader.ReadByte());
                            var evilRecruit = Utils.PlayerById(reader.ReadByte());
                            var jackalRole2 = Role.GetRole<Jackal>(jackal2);
                            jackalRole2.EvilRecruit = evilRecruit;
                            jackalRole2.Recruited.Add(evilRecruit.PlayerId);
                            Role.GetRole(evilRecruit).SubFactionColor = Colors.Cabal;
                            Role.GetRole(evilRecruit).SubFaction = SubFaction.Cabal;
                            Role.GetRole(evilRecruit).IsRecruit = true;
                            break;

                        case TargetRPC.SetAlliedFaction:
                            var player6 = Utils.PlayerById(reader.ReadByte());
                            var alliedRole = Role.GetRole(player6);
                            var ally = Objectifier.GetObjectifier<Allied>(player6);
                            var faction = (Faction)reader.ReadByte();
                            alliedRole.Faction = faction;

                            if (faction == Faction.Crew)
                            {
                                alliedRole.FactionColor = Colors.Crew;
                                alliedRole.IsCrewAlly = true;
                                alliedRole.RoleAlignment = RoleAlignment.CrewKill;
                                ally.Color = Colors.Crew;
                            }
                            else if (faction == Faction.Intruder)
                            {
                                alliedRole.FactionColor = Colors.Intruder;
                                alliedRole.IsIntAlly = true;
                                alliedRole.RoleAlignment = RoleAlignment.IntruderKill;
                                ally.Color = Colors.Intruder;
                            }
                            else if (faction == Faction.Syndicate)
                            {
                                alliedRole.FactionColor = Colors.Syndicate;
                                alliedRole.IsSynAlly = true;
                                alliedRole.RoleAlignment = RoleAlignment.SyndicateKill;
                                ally.Color = Colors.Syndicate;
                            }

                            ally.Side = faction;
                            alliedRole.RoleAlignment = alliedRole.RoleAlignment.GetNewAlignment(faction);
                            break;

                        case TargetRPC.SetCouple:
                            var lover1 = Utils.PlayerById(reader.ReadByte());
                            var lover2 = Utils.PlayerById(reader.ReadByte());
                            Objectifier.GetObjectifier<Lovers>(lover1).OtherLover = lover2;
                            Objectifier.GetObjectifier<Lovers>(lover2).OtherLover = lover1;
                            break;

                        case TargetRPC.SetDuo:
                            var rival1 = Utils.PlayerById(reader.ReadByte());
                            var rival2 = Utils.PlayerById(reader.ReadByte());
                            Objectifier.GetObjectifier<Rivals>(rival1).OtherRival = rival2;
                            Objectifier.GetObjectifier<Rivals>(rival2).OtherRival = rival1;
                            break;
                    }

                    break;

                case CustomRPC.Action:
                    var id6 = reader.ReadByte();

                    switch ((ActionsRPC)id6)
                    {
                        case ActionsRPC.RetributionistAction:
                            var retId = reader.ReadByte();

                            switch ((RetributionistActionsRPC)retId)
                            {
                                case RetributionistActionsRPC.RetributionistRevive:
                                    var retRole2 = Role.GetRole<Retributionist>(Utils.PlayerById(reader.ReadByte()));
                                    retRole2.Revived = Utils.PlayerById(reader.ReadByte());
                                    retRole2.RevivedRole = retRole2.Revived == null ? null : (retRole2.Revived.Is(RoleEnum.Revealer) ? Role.GetRole<Revealer>(retRole2.Revived).FormerRole :
                                        Role.GetRole(retRole2.Revived));
                                    break;

                                case RetributionistActionsRPC.Transport:
                                    var retRole3 = Role.GetRole<Retributionist>(Utils.PlayerById(reader.ReadByte()));
                                    retRole3.TransportPlayer1 = Utils.PlayerById(reader.ReadByte());
                                    retRole3.TransportPlayer2 = Utils.PlayerById(reader.ReadByte());
                                    Coroutines.Start(retRole3.TransportPlayers());
                                    break;

                                case RetributionistActionsRPC.Protect:
                                    Role.GetRole<Retributionist>(Utils.PlayerById(reader.ReadByte())).ShieldedPlayer = Utils.PlayerById(reader.ReadByte());
                                    break;

                                case RetributionistActionsRPC.Alert:
                                    var retRole7 = Role.GetRole<Retributionist>(Utils.PlayerById(reader.ReadByte()));
                                    retRole7.TimeRemaining = CustomGameOptions.AlertDuration;
                                    retRole7.Alert();
                                    break;

                                case RetributionistActionsRPC.AltruistRevive:
                                    var retRole8 = Role.GetRole<Retributionist>(Utils.PlayerById(reader.ReadByte()));
                                    retRole8.RevivingBody = Utils.BodyById(reader.ReadByte());
                                    retRole8.TimeRemaining = CustomGameOptions.AltReviveDuration;
                                    retRole8.Revive();
                                    break;

                                case RetributionistActionsRPC.Swoop:
                                    var retRole9 = Role.GetRole<Retributionist>(Utils.PlayerById(reader.ReadByte()));
                                    retRole9.TimeRemaining = CustomGameOptions.SwoopDuration;
                                    retRole9.Invis();
                                    break;

                                case RetributionistActionsRPC.Mediate:
                                    var retId2 = reader.ReadByte();
                                    var ret = Role.GetRole<Medium>(Utils.PlayerById(retId2));
                                    var playerid2 = reader.ReadByte();
                                    ret.MediatedPlayers.Add(playerid2);

                                    if (CustomPlayer.Local.PlayerId == playerid2 || (CustomPlayer.LocalCustom.IsDead && CustomGameOptions.ShowMediumToDead ==
                                        ShowMediumToDead.AllDead))
                                    {
                                        Role.LocalRole.DeadArrows.Add(retId2, new(PlayerControl.LocalPlayer, Colors.Retributionist));
                                    }

                                    break;

                                case RetributionistActionsRPC.EscRoleblock:
                                    var retRole1 = Role.GetRole<Retributionist>(Utils.PlayerById(reader.ReadByte()));
                                    var blocked4 = Utils.PlayerById(reader.ReadByte());
                                    retRole1.BlockTarget = Utils.PlayerById(reader.ReadByte());
                                    retRole1.TimeRemaining = CustomGameOptions.EscRoleblockDuration;
                                    retRole1.Block();
                                    break;
                            }

                            break;

                        case ActionsRPC.GodfatherAction:
                            var gfId = reader.ReadByte();

                            switch ((GodfatherActionsRPC)gfId)
                            {
                                case GodfatherActionsRPC.Morph:
                                    var gfRole3 = Role.GetRole<PromotedGodfather>(Utils.PlayerById(reader.ReadByte()));
                                    gfRole3.TimeRemaining = CustomGameOptions.MorphlingDuration;
                                    gfRole3.MorphedPlayer = Utils.PlayerById(reader.ReadByte());
                                    gfRole3.Morph();
                                    break;

                                case GodfatherActionsRPC.Disguise:
                                    var gfRole4 = Role.GetRole<PromotedGodfather>(Utils.PlayerById(reader.ReadByte()));
                                    gfRole4.TimeRemaining2 = CustomGameOptions.TimeToDisguise;
                                    gfRole4.TimeRemaining = CustomGameOptions.DisguiseDuration;
                                    gfRole4.DisguisedPlayer = Utils.PlayerById(reader.ReadByte());
                                    gfRole4.DisguisePlayer = Utils.PlayerById(reader.ReadByte());
                                    gfRole4.DisgDelay();
                                    break;

                                case GodfatherActionsRPC.ConsRoleblock:
                                    var gfRole5 = Role.GetRole<PromotedGodfather>(Utils.PlayerById(reader.ReadByte()));
                                    gfRole5.BlockTarget = Utils.PlayerById(reader.ReadByte());
                                    gfRole5.TimeRemaining = CustomGameOptions.ConsRoleblockDuration;
                                    gfRole5.Block();
                                    break;

                                case GodfatherActionsRPC.Blackmail:
                                    Role.GetRole<PromotedGodfather>(Utils.PlayerById(reader.ReadByte())).BlackmailedPlayer = Utils.PlayerById(reader.ReadByte());
                                    break;

                                case GodfatherActionsRPC.Invis:
                                    var gfRole8 = Role.GetRole<PromotedGodfather>(Utils.PlayerById(reader.ReadByte()));
                                    gfRole8.TimeRemaining = CustomGameOptions.InvisDuration;
                                    gfRole8.Invis();
                                    break;

                                case GodfatherActionsRPC.Drag:
                                    var gf = Utils.PlayerById(reader.ReadByte());
                                    var gfRole7 = Role.GetRole<PromotedGodfather>(gf);
                                    var dragged2 = Utils.BodyById(reader.ReadByte());
                                    gfRole7.CurrentlyDragging = dragged2;
                                    var drag3 = dragged2.gameObject.AddComponent<DragBehaviour>();
                                    drag3.Source = gf;
                                    drag3.Body = dragged2.gameObject.AddComponent<Rigidbody2D>();
                                    drag3.Collider = dragged2.gameObject.GetComponent<Collider2D>();
                                    drag3.Dragged = dragged2;
                                    break;

                                case GodfatherActionsRPC.Camouflage:
                                    var gfRole11 = Role.GetRole<PromotedGodfather>(Utils.PlayerById(reader.ReadByte()));
                                    gfRole11.TimeRemaining = CustomGameOptions.CamouflagerDuration;
                                    gfRole11.Camouflage();
                                    break;

                                case GodfatherActionsRPC.FlashGrenade:
                                    var gfRole12 = Role.GetRole<PromotedGodfather>(Utils.PlayerById(reader.ReadByte()));
                                    gfRole12.TimeRemaining = CustomGameOptions.GrenadeDuration;
                                    gfRole12.Flash();
                                    break;

                                case GodfatherActionsRPC.SetBomb:
                                    var gfRole13 = Role.GetRole<PromotedGodfather>(Utils.PlayerById(reader.ReadByte()));
                                    gfRole13.TimeRemaining = CustomGameOptions.EnforceDuration;
                                    gfRole13.TimeRemaining2 = CustomGameOptions.EnforceDelay;
                                    gfRole13.BombedPlayer = Utils.PlayerById(reader.ReadByte());
                                    gfRole13.BombDelay();
                                    break;

                                case GodfatherActionsRPC.Ambush:
                                    var gfRole14 = Role.GetRole<PromotedGodfather>(Utils.PlayerById(reader.ReadByte()));
                                    gfRole14.TimeRemaining = CustomGameOptions.AmbushDuration;
                                    gfRole14.AmbushedPlayer = Utils.PlayerById(reader.ReadByte());
                                    gfRole14.Ambush();
                                    break;
                            }

                            break;

                        case ActionsRPC.RebelAction:
                            var rebId = reader.ReadByte();

                            switch ((RebelActionsRPC)rebId)
                            {
                                case RebelActionsRPC.Poison:
                                    var rebRole1 = Role.GetRole<PromotedRebel>(Utils.PlayerById(reader.ReadByte()));
                                    rebRole1.PoisonedPlayer = Utils.PlayerById(reader.ReadByte());
                                    rebRole1.TimeRemaining = CustomGameOptions.PoisonDuration;
                                    rebRole1.Poison();
                                    break;

                                case RebelActionsRPC.Conceal:
                                    var rebRole2 = Role.GetRole<PromotedRebel>(Utils.PlayerById(reader.ReadByte()));

                                    if (!rebRole2.HoldsDrive)
                                        rebRole2.ConcealedPlayer = Utils.PlayerById(reader.ReadByte());

                                    rebRole2.TimeRemaining = CustomGameOptions.ConcealDuration;
                                    rebRole2.Conceal();
                                    break;

                                case RebelActionsRPC.Shapeshift:
                                    var rebRole3 = Role.GetRole<PromotedRebel>(Utils.PlayerById(reader.ReadByte()));

                                    if (!rebRole3.HoldsDrive)
                                    {
                                        rebRole3.ShapeshiftPlayer1 = Utils.PlayerById(reader.ReadByte());
                                        rebRole3.ShapeshiftPlayer2 = Utils.PlayerById(reader.ReadByte());
                                    }

                                    rebRole3.TimeRemaining = CustomGameOptions.ShapeshiftDuration;
                                    rebRole3.Shapeshift();
                                    break;

                                case RebelActionsRPC.Warp:
                                    var rebRole4 = Role.GetRole<Warper>(Utils.PlayerById(reader.ReadByte()));
                                    rebRole4.WarpPlayer1 = Utils.PlayerById(reader.ReadByte());
                                    rebRole4.WarpPlayer2 = Utils.PlayerById(reader.ReadByte());
                                    Coroutines.Start(rebRole4.WarpPlayers());
                                    break;

                                case RebelActionsRPC.Frame:
                                    Role.GetRole<PromotedRebel>(Utils.PlayerById(reader.ReadByte())).Framed.Add(reader.ReadByte());
                                    break;

                                case RebelActionsRPC.Crusade:
                                    var rebRole7 = Role.GetRole<PromotedRebel>(Utils.PlayerById(reader.ReadByte()));
                                    rebRole7.CrusadedPlayer = Utils.PlayerById(reader.ReadByte());
                                    rebRole7.TimeRemaining = CustomGameOptions.CrusadeDuration;
                                    rebRole7.Crusade();
                                    break;

                                case RebelActionsRPC.Spell:
                                    Role.GetRole<PromotedRebel>(Utils.PlayerById(reader.ReadByte())).Spelled.Add(reader.ReadByte());
                                    break;

                                case RebelActionsRPC.Confuse:
                                    var rebRole6 = Role.GetRole<PromotedRebel>(Utils.PlayerById(reader.ReadByte()));

                                    if (!rebRole6.HoldsDrive)
                                        rebRole6.ConfusedPlayer = Utils.PlayerById(reader.ReadByte());

                                    rebRole6.TimeRemaining = CustomGameOptions.ConfuseDuration;
                                    rebRole6.Confuse();
                                    break;

                                case RebelActionsRPC.TimeControl:
                                    var rebRole5 = Role.GetRole<PromotedRebel>(Utils.PlayerById(reader.ReadByte()));
                                    rebRole5.TimeRemaining = CustomGameOptions.TimeControlDuration;
                                    rebRole5.Control();
                                    break;

                                case RebelActionsRPC.Silence:
                                    Role.GetRole<PromotedRebel>(Utils.PlayerById(reader.ReadByte())).SilencedPlayer = Utils.PlayerById(reader.ReadByte());
                                    break;
                            }

                            break;

                        case ActionsRPC.FreezeDouse:
                            Role.GetRole<Cryomaniac>(Utils.PlayerById(reader.ReadByte())).Doused.Add(reader.ReadByte());
                            break;

                        case ActionsRPC.FadeBody:
                            Coroutines.Start(Utils.FadeBody(Utils.BodyById(reader.ReadByte())));
                            break;

                        case ActionsRPC.SetExtraVotes:
                            var politian = Utils.PlayerById(reader.ReadByte());
                            var polRole = Ability.GetAbility<Politician>(politian);
                            polRole.ExtraVotes = reader.ReadBytesAndSize().ToList();
                            polRole.VoteBank -= polRole.ExtraVotes.Count;
                            break;

                        case ActionsRPC.SetSwaps:
                            var swapper = Ability.GetAbility<Swapper>(Utils.PlayerById(reader.ReadByte()));
                            swapper.Swap1 = Utils.VoteAreaById(reader.ReadByte());
                            swapper.Swap2 = Utils.VoteAreaById(reader.ReadByte());
                            break;

                        case ActionsRPC.Remember:
                            Amnesiac.Remember(Role.GetRole<Amnesiac>(Utils.PlayerById(reader.ReadByte())), Utils.PlayerById(reader.ReadByte()));
                            break;

                        case ActionsRPC.Steal:
                            Thief.Steal(Role.GetRole<Thief>(Utils.PlayerById(reader.ReadByte())), Utils.PlayerById(reader.ReadByte()));
                            break;

                        case ActionsRPC.Shift:
                            Shifter.Shift(Role.GetRole<Shifter>(Utils.PlayerById(reader.ReadByte())), Utils.PlayerById(reader.ReadByte()));
                            break;

                        case ActionsRPC.Convert:
                            RoleGen.Convert(reader.ReadByte(), reader.ReadByte(), (SubFaction)reader.ReadByte(), reader.ReadBoolean());
                            break;

                        case ActionsRPC.Teleport:
                            Utils.Teleport(Utils.PlayerById(reader.ReadByte()), reader.ReadVector2());
                            break;

                        case ActionsRPC.Sidekick:
                            Rebel.Sidekick(Role.GetRole<Rebel>(Utils.PlayerById(reader.ReadByte())), Utils.PlayerById(reader.ReadByte()));
                            break;

                        case ActionsRPC.Frame:
                             Role.GetRole<Framer>(Utils.PlayerById(reader.ReadByte())).Framed.Add(reader.ReadByte());
                            break;

                        case ActionsRPC.Declare:
                            Godfather.Declare(Role.GetRole<Godfather>(Utils.PlayerById(reader.ReadByte())), Utils.PlayerById(reader.ReadByte()));
                            break;

                        case ActionsRPC.Protect:
                            Role.GetRole<Medic>(Utils.PlayerById(reader.ReadByte())).ShieldedPlayer = Utils.PlayerById(reader.ReadByte());
                            break;

                        case ActionsRPC.BypassKill:
                            Utils.MurderPlayer(Utils.PlayerById(reader.ReadByte()), Utils.PlayerById(reader.ReadByte()), (DeathReasonEnum)reader.ReadByte(), reader.ReadBoolean());
                            break;

                        case ActionsRPC.AssassinKill:
                            Ability.GetAbility<Assassin>(Utils.PlayerById(reader.ReadByte())).MurderPlayer(Utils.PlayerById(reader.ReadByte()), reader.ReadString());
                            break;

                        case ActionsRPC.GuesserKill:
                            Role.GetRole<Guesser>(Utils.PlayerById(reader.ReadByte())).MurderPlayer(Utils.PlayerById(reader.ReadByte()), reader.ReadString());
                            break;

                        case ActionsRPC.ThiefKill:
                            Role.GetRole<Thief>(Utils.PlayerById(reader.ReadByte())).MurderPlayer(Utils.PlayerById(reader.ReadByte()), reader.ReadString());
                            break;

                        case ActionsRPC.ForceKill:
                            var victim = Utils.PlayerById(reader.ReadByte());
                            victim.GetEnforcer().BombSuccessful = reader.ReadBoolean();
                            Role.GetRole(victim).Bombed = false;
                            break;

                        case ActionsRPC.SetBomb:
                            var enfRole = Role.GetRole<Enforcer>(Utils.PlayerById(reader.ReadByte()));
                            enfRole.TimeRemaining = CustomGameOptions.EnforceDuration;
                            enfRole.TimeRemaining2 = CustomGameOptions.EnforceDelay;
                            enfRole.BombedPlayer = Utils.PlayerById(reader.ReadByte());
                            enfRole.Delay();
                            break;

                        case ActionsRPC.Morph:
                            var morphRole = Role.GetRole<Morphling>(Utils.PlayerById(reader.ReadByte()));
                            morphRole.TimeRemaining = CustomGameOptions.MorphlingDuration;
                            morphRole.MorphedPlayer = Utils.PlayerById(reader.ReadByte());
                            morphRole.Morph();
                            break;

                        case ActionsRPC.Scream:
                            var bansheeRole2 = Role.GetRole<Banshee>(Utils.PlayerById(reader.ReadByte()));
                            bansheeRole2.TimeRemaining = CustomGameOptions.ScreamDuration;

                            foreach (var player8 in CustomPlayer.AllPlayers)
                            {
                                if (!player8.Data.IsDead && !player8.Data.Disconnected && !player8.Is(Faction.Syndicate))
                                    bansheeRole2.Blocked.Add(player8.PlayerId);
                            }

                            bansheeRole2.Scream();
                            break;

                        case ActionsRPC.Mark:
                            Role.GetRole<Ghoul>(Utils.PlayerById(reader.ReadByte())).MarkedPlayer = Utils.PlayerById(reader.ReadByte());
                            break;

                        case ActionsRPC.Disguise:
                            var disguiseRole = Role.GetRole<Disguiser>(Utils.PlayerById(reader.ReadByte()));
                            disguiseRole.TimeRemaining = CustomGameOptions.DisguiseDuration;
                            disguiseRole.TimeRemaining2 = CustomGameOptions.TimeToDisguise;
                            disguiseRole.DisguisePlayer = Utils.PlayerById(reader.ReadByte());
                            disguiseRole.DisguisedPlayer = Utils.PlayerById(reader.ReadByte());
                            disguiseRole.Delay();
                            break;

                        case ActionsRPC.Poison:
                            var poisonerRole = Role.GetRole<Poisoner>(Utils.PlayerById(reader.ReadByte()));
                            poisonerRole.TimeRemaining = CustomGameOptions.PoisonDuration;
                            poisonerRole.PoisonedPlayer = Utils.PlayerById(reader.ReadByte());
                            poisonerRole.Poison();
                            break;

                        case ActionsRPC.Blackmail:
                            Role.GetRole<Blackmailer>(Utils.PlayerById(reader.ReadByte())).BlackmailedPlayer = Utils.PlayerById(reader.ReadByte());
                            break;

                        case ActionsRPC.Silence:
                            Role.GetRole<Silencer>(Utils.PlayerById(reader.ReadByte())).SilencedPlayer = Utils.PlayerById(reader.ReadByte());
                            break;

                        case ActionsRPC.Mine:
                            Utils.SpawnVent(reader.ReadInt32(), Role.GetRole(Utils.PlayerById(reader.ReadByte())), reader.ReadVector2(), reader.ReadSingle());
                            break;

                        case ActionsRPC.Invis:
                            var wraithRole = Role.GetRole<Wraith>(Utils.PlayerById(reader.ReadByte()));
                            wraithRole.TimeRemaining = CustomGameOptions.InvisDuration;
                            wraithRole.Invis();
                            break;

                        case ActionsRPC.Alert:
                            var veteranRole = Role.GetRole<Veteran>(Utils.PlayerById(reader.ReadByte()));
                            veteranRole.TimeRemaining = CustomGameOptions.AlertDuration;
                            veteranRole.Alert();
                            break;

                        case ActionsRPC.Vest:
                            var survRole = Role.GetRole<Survivor>(Utils.PlayerById(reader.ReadByte()));
                            survRole.TimeRemaining = CustomGameOptions.VestDuration;
                            survRole.Vest();
                            break;

                        case ActionsRPC.Ambush:
                            var ambRole = Role.GetRole<Ambusher>(Utils.PlayerById(reader.ReadByte()));
                            ambRole.TimeRemaining = CustomGameOptions.AmbushDuration;
                            ambRole.AmbushedPlayer = Utils.PlayerById(reader.ReadByte());
                            ambRole.Ambush();
                            break;

                        case ActionsRPC.Crusade:
                            var crusRole = Role.GetRole<Crusader>(Utils.PlayerById(reader.ReadByte()));
                            crusRole.TimeRemaining = CustomGameOptions.CrusadeDuration;
                            crusRole.CrusadedPlayer = Utils.PlayerById(reader.ReadByte());
                            crusRole.Crusade();
                            break;

                        case ActionsRPC.GAProtect:
                            var ga2Role = Role.GetRole<GuardianAngel>(Utils.PlayerById(reader.ReadByte()));
                            ga2Role.TimeRemaining = CustomGameOptions.ProtectDuration;
                            ga2Role.Protect();
                            break;

                        case ActionsRPC.Transport:
                            var transRole = Role.GetRole<Transporter>(Utils.PlayerById(reader.ReadByte()));
                            transRole.TransportPlayer1 = Utils.PlayerById(reader.ReadByte());
                            transRole.TransportPlayer2 = Utils.PlayerById(reader.ReadByte());
                            Coroutines.Start(transRole.TransportPlayers());
                            break;

                        case ActionsRPC.SetUninteractable:
                            if (CustomPlayer.Local.Is(RoleEnum.Transporter))
                                Role.GetRole<Transporter>(CustomPlayer.Local).UntransportablePlayers.Add(reader.ReadByte(), DateTime.UtcNow);
                            else if (CustomPlayer.Local.Is(RoleEnum.Warper))
                                Role.GetRole<Warper>(CustomPlayer.Local).UnwarpablePlayers.Add(reader.ReadByte(), DateTime.UtcNow);
                            else if (CustomPlayer.Local.Is(RoleEnum.Retributionist))
                                Role.GetRole<Retributionist>(CustomPlayer.Local).UntransportablePlayers.Add(reader.ReadByte(), DateTime.UtcNow);

                            break;

                        case ActionsRPC.Warp:
                            var warpRole = Role.GetRole<Warper>(Utils.PlayerById(reader.ReadByte()));
                            warpRole.WarpPlayer1 = Utils.PlayerById(reader.ReadByte());
                            warpRole.WarpPlayer2 = Utils.PlayerById(reader.ReadByte());
                            Coroutines.Start(warpRole.WarpPlayers());
                            break;

                        case ActionsRPC.Mediate:
                            var medid = reader.ReadByte();
                            var med = Role.GetRole<Medium>(Utils.PlayerById(medid));
                            var playerid = reader.ReadByte();
                            med.MediatedPlayers.Add(playerid);

                            if (CustomPlayer.Local.PlayerId == playerid || (CustomPlayer.LocalCustom.IsDead && CustomGameOptions.ShowMediumToDead == ShowMediumToDead.AllDead))
                                Role.LocalRole.DeadArrows.Add(medid, new(PlayerControl.LocalPlayer, Colors.Medium));

                            break;

                        case ActionsRPC.FlashGrenade:
                            var grenadierRole = Role.GetRole<Grenadier>(Utils.PlayerById(reader.ReadByte()));
                            grenadierRole.TimeRemaining = CustomGameOptions.GrenadeDuration;
                            grenadierRole.Flash();
                            break;

                        case ActionsRPC.Douse:
                            Role.GetRole<Arsonist>(Utils.PlayerById(reader.ReadByte())).Doused.Add(reader.ReadByte());
                            break;

                        case ActionsRPC.Infect:
                            Role.GetRole<Plaguebearer>(Utils.PlayerById(reader.ReadByte())).Infected.Add(reader.ReadByte());
                            break;

                        case ActionsRPC.AltruistRevive:
                            var altruistRole = Role.GetRole<Altruist>(Utils.PlayerById(reader.ReadByte()));
                            altruistRole.RevivingBody = Utils.BodyById(reader.ReadByte());
                            altruistRole.TimeRemaining = CustomGameOptions.AltReviveDuration;
                            altruistRole.Revive();
                            break;

                        case ActionsRPC.NecromancerResurrect:
                            var necroRole = Role.GetRole<Necromancer>(Utils.PlayerById(reader.ReadByte()));
                            necroRole.ResurrectingBody = Utils.BodyById(reader.ReadByte());
                            necroRole.TimeRemaining = CustomGameOptions.NecroResurrectDuration;
                            necroRole.Resurrect();
                            break;

                        case ActionsRPC.WarpAll:
                            var teleports = reader.ReadByte();
                            var coordinates = new Dictionary<byte, Vector2>();

                            for (var i = 0; i < teleports; i++)
                                coordinates.Add(reader.ReadByte(), reader.ReadVector2());

                            Utils.WarpPlayersToCoordinates(coordinates);
                            break;

                        case ActionsRPC.Swoop:
                            var chameleonRole = Role.GetRole<Chameleon>(Utils.PlayerById(reader.ReadByte()));
                            chameleonRole.TimeRemaining = CustomGameOptions.SwoopDuration;
                            chameleonRole.Invis();
                            break;

                        case ActionsRPC.BarryButton:
                            var buttonBarry = Utils.PlayerById(reader.ReadByte());

                            if (AmongUsClient.Instance.AmHost)
                            {
                                MeetingRoomManager.Instance.reporter = buttonBarry;
                                MeetingRoomManager.Instance.target = null;
                                AmongUsClient.Instance.DisconnectHandlers.AddUnique(MeetingRoomManager.Instance.Cast<IDisconnectHandler>());
                                HudManager.Instance.OpenMeetingRoom(buttonBarry);
                                buttonBarry.RpcStartMeeting(null);
                            }

                            break;

                        case ActionsRPC.BaitReport:
                            Utils.PlayerById(reader.ReadByte()).ReportDeadBody(Utils.PlayerById(reader.ReadByte()).Data);
                            break;

                        case ActionsRPC.Drag:
                            var jani = Utils.PlayerById(reader.ReadByte());
                            var janiRole = Role.GetRole<Janitor>(jani);
                            var dragged = Utils.BodyById(reader.ReadByte());
                            janiRole.CurrentlyDragging = dragged;
                            var drag = dragged.gameObject.AddComponent<DragBehaviour>();
                            drag.Source = jani;
                            drag.Body = dragged.gameObject.AddComponent<Rigidbody2D>();
                            drag.Collider = dragged.gameObject.GetComponent<Collider2D>();
                            drag.Dragged = dragged;
                            break;

                        case ActionsRPC.Drop:
                            Utils.BodyById(reader.ReadByte()).gameObject.GetComponent<DragBehaviour>().Destroy();
                            break;

                        case ActionsRPC.Camouflage:
                            var camouflagerRole = Role.GetRole<Camouflager>(Utils.PlayerById(reader.ReadByte()));
                            camouflagerRole.TimeRemaining = CustomGameOptions.CamouflagerDuration;
                            camouflagerRole.Camouflage();
                            break;

                        case ActionsRPC.EscRoleblock:
                            var escortRole = Role.GetRole<Escort>(Utils.PlayerById(reader.ReadByte()));
                            escortRole.BlockTarget = Utils.PlayerById(reader.ReadByte());
                            escortRole.TimeRemaining = CustomGameOptions.EscRoleblockDuration;
                            escortRole.Block();
                            break;

                        case ActionsRPC.GlitchRoleblock:
                            var glitchRole = Role.GetRole<Glitch>(Utils.PlayerById(reader.ReadByte()));
                            glitchRole.HackTarget = Utils.PlayerById(reader.ReadByte());
                            glitchRole.TimeRemaining = CustomGameOptions.HackDuration;
                            glitchRole.Hack();
                            break;

                        case ActionsRPC.ConsRoleblock:
                            var consortRole = Role.GetRole<Consort>(Utils.PlayerById(reader.ReadByte()));
                            consortRole.BlockTarget = Utils.PlayerById(reader.ReadByte());
                            consortRole.TimeRemaining = CustomGameOptions.ConsRoleblockDuration;
                            consortRole.Block();
                            break;

                        case ActionsRPC.Mimic:
                            var glitchRole4 = Role.GetRole<Glitch>(Utils.PlayerById(reader.ReadByte()));
                            glitchRole4.MimicTarget = Utils.PlayerById(reader.ReadByte());
                            glitchRole4.TimeRemaining2 = CustomGameOptions.MimicDuration;
                            glitchRole4.Mimic();
                            break;

                        case ActionsRPC.Conceal:
                            var concealerRole = Role.GetRole<Concealer>(Utils.PlayerById(reader.ReadByte()));

                            if (!concealerRole.HoldsDrive)
                                concealerRole.ConcealedPlayer = Utils.PlayerById(reader.ReadByte());

                            concealerRole.TimeRemaining = CustomGameOptions.ConcealDuration;
                            concealerRole.Conceal();
                            break;

                        case ActionsRPC.Shapeshift:
                            var ssRole = Role.GetRole<Shapeshifter>(Utils.PlayerById(reader.ReadByte()));

                            if (!ssRole.HoldsDrive)
                            {
                                ssRole.ShapeshiftPlayer1 = Utils.PlayerById(reader.ReadByte());
                                ssRole.ShapeshiftPlayer2 = Utils.PlayerById(reader.ReadByte());
                            }

                            ssRole.TimeRemaining = CustomGameOptions.ShapeshiftDuration;
                            ssRole.Shapeshift();
                            break;

                        case ActionsRPC.Burn:
                            var arsoRole = Role.GetRole<Arsonist>(Utils.PlayerById(reader.ReadByte()));

                            foreach (var body in Utils.AllBodies)
                            {
                                if (arsoRole.Doused.Contains(body.ParentId))
                                {
                                    Coroutines.Start(Utils.FadeBody(body));
                                    _ = new Ash(body.TruePosition);
                                }
                            }

                            break;

                        case ActionsRPC.MayorReveal:
                            var mayor = Utils.PlayerById(reader.ReadByte());
                            var mayorRole = Role.GetRole<Mayor>(mayor);
                            mayorRole.Revealed = true;
                            Utils.Flash(Colors.Mayor);
                            Role.BreakShield(mayor.PlayerId, true);
                            break;

                        case ActionsRPC.DictatorReveal:
                            var dictator = Utils.PlayerById(reader.ReadByte());
                            var dictatorRole = Role.GetRole<Dictator>(dictator);
                            dictatorRole.Revealed = true;
                            Utils.Flash(Colors.Dictator);
                            Role.BreakShield(dictator.PlayerId, true);
                            break;

                        case ActionsRPC.SetExiles:
                            var dictRole = Role.GetRole<Dictator>(Utils.PlayerById(reader.ReadByte()));
                            dictRole.ToDie = reader.ReadBoolean();
                            dictRole.ToBeEjected = reader.ReadBytesAndSize().ToList();
                            break;

                        case ActionsRPC.Spell:
                            Role.GetRole<Spellslinger>(Utils.PlayerById(reader.ReadByte())).Spelled.Add(reader.ReadByte());
                            break;

                        case ActionsRPC.Knight:
                            Role.GetRole<Monarch>(Utils.PlayerById(reader.ReadByte())).ToBeKnighted.Add(reader.ReadByte());
                            break;

                        case ActionsRPC.Confuse:
                            var drunkRole = Role.GetRole<Drunkard>(Utils.PlayerById(reader.ReadByte()));

                            if (!drunkRole.HoldsDrive)
                                drunkRole.ConfusedPlayer = Utils.PlayerById(reader.ReadByte());

                            drunkRole.TimeRemaining = CustomGameOptions.ConfuseDuration;
                            drunkRole.Confuse();
                            break;

                        case ActionsRPC.TimeControl:
                            var tkRole = Role.GetRole<TimeKeeper>(Utils.PlayerById(reader.ReadByte()));
                            tkRole.TimeRemaining = CustomGameOptions.TimeControlDuration;
                            tkRole.Control();
                            break;
                    }

                    break;

                case CustomRPC.WinLose:
                    var id7 = reader.ReadByte();

                    switch ((WinLoseRPC)id7)
                    {
                        case WinLoseRPC.CrewWin:
                            Role.CrewWin = true;
                            break;

                        case WinLoseRPC.IntruderWin:
                            Role.IntruderWin = true;
                            break;

                        case WinLoseRPC.SyndicateWin:
                            Role.SyndicateWin = true;
                            break;

                        case WinLoseRPC.UndeadWin:
                            Role.UndeadWin = true;
                            break;

                        case WinLoseRPC.ReanimatedWin:
                            Role.ReanimatedWin = true;
                            break;

                        case WinLoseRPC.SectWin:
                            Role.SectWin = true;
                            break;

                        case WinLoseRPC.CabalWin:
                            Role.CabalWin = true;
                            break;

                        case WinLoseRPC.NobodyWins:
                            PlayerLayer.NobodyWins = true;
                            break;

                        case WinLoseRPC.AllNeutralsWin:
                            Role.AllNeutralsWin = true;
                            break;

                        case WinLoseRPC.AllNKsWin:
                            Role.NKWins = true;
                            break;

                        case WinLoseRPC.SameNKWins:
                        case WinLoseRPC.SoloNKWins:
                            var nkRole = Role.GetRole(Utils.PlayerById(reader.ReadByte()));

                            switch (nkRole.RoleType)
                            {
                                case RoleEnum.Glitch:
                                    Role.GlitchWins = true;
                                    break;

                                case RoleEnum.Arsonist:
                                    Role.ArsonistWins = true;
                                    break;

                                case RoleEnum.Cryomaniac:
                                    Role.CryomaniacWins = true;
                                    break;

                                case RoleEnum.Juggernaut:
                                    Role.JuggernautWins = true;
                                    break;

                                case RoleEnum.Murderer:
                                    Role.MurdererWins = true;
                                    break;

                                case RoleEnum.Werewolf:
                                    Role.WerewolfWins = true;
                                    break;

                                case RoleEnum.SerialKiller:
                                    Role.SerialKillerWins = true;
                                    break;
                            }

                            if ((WinLoseRPC)id7 == WinLoseRPC.SameNKWins)
                            {
                                foreach (var role in Role.GetRoles(nkRole.RoleType))
                                {
                                    if (!role.Disconnected && role.Faithful)
                                        role.Winner = true;
                                }
                            }

                            nkRole.Winner = true;
                            break;

                        case WinLoseRPC.InfectorsWin:
                            Role.InfectorsWin = true;
                            break;

                        case WinLoseRPC.JesterWin:
                            Role.GetRole<Jester>(Utils.PlayerById(reader.ReadByte())).VotedOut = true;
                            break;

                        case WinLoseRPC.CannibalWin:
                            Role.GetRole<Cannibal>(Utils.PlayerById(reader.ReadByte())).Eaten = true;
                            break;

                        case WinLoseRPC.ExecutionerWin:
                            Role.GetRole<Executioner>(Utils.PlayerById(reader.ReadByte())).TargetVotedOut = true;
                            break;

                        case WinLoseRPC.BountyHunterWin:
                            Role.GetRole<BountyHunter>(Utils.PlayerById(reader.ReadByte())).TargetKilled = true;
                            break;

                        case WinLoseRPC.TrollWin:
                            Role.GetRole<Troll>(Utils.PlayerById(reader.ReadByte())).Killed = true;
                            break;

                        case WinLoseRPC.ActorWin:
                            Role.GetRole<Actor>(Utils.PlayerById(reader.ReadByte())).Guessed = true;
                            break;

                        case WinLoseRPC.GuesserWin:
                            Role.GetRole<Guesser>(Utils.PlayerById(reader.ReadByte())).TargetGuessed = true;
                            break;

                        case WinLoseRPC.CorruptedWin:
                            Objectifier.CorruptedWins = true;

                            if (CustomGameOptions.AllCorruptedWin)
                            {
                                foreach (var corr in Objectifier.GetObjectifiers(ObjectifierEnum.Corrupted))
                                    corr.Winner = true;
                            }

                            Objectifier.GetObjectifier(Utils.PlayerById(reader.ReadByte())).Winner = true;
                            break;

                        case WinLoseRPC.LoveWin:
                            Objectifier.LoveWins = true;
                            var lover = Objectifier.GetObjectifier<Lovers>(Utils.PlayerById(reader.ReadByte()));
                            lover.Winner = true;
                            Objectifier.GetObjectifier(lover.OtherLover).Winner = true;
                            break;

                        case WinLoseRPC.OverlordWin:
                            Objectifier.OverlordWins = true;

                            foreach (var ov in Objectifier.GetObjectifiers(ObjectifierEnum.Overlord))
                                ov.Winner = true;

                            break;

                        case WinLoseRPC.MafiaWins:
                            Objectifier.MafiaWins = true;
                            break;

                        case WinLoseRPC.TaskmasterWin:
                            Objectifier.TaskmasterWins = true;
                            Objectifier.GetObjectifier(Utils.PlayerById(reader.ReadByte())).Winner = true;
                            break;

                        case WinLoseRPC.RivalWin:
                            Objectifier.RivalWins = true;
                            Objectifier.GetObjectifier<Rivals>(Utils.PlayerById(reader.ReadByte())).Winner = true;
                            break;

                        case WinLoseRPC.PhantomWin:
                            Role.PhantomWins = true;
                            var phantom3 = Role.GetRole<Phantom>(Utils.PlayerById(reader.ReadByte()));
                            phantom3.CompletedTasks = true;
                            phantom3.Winner = true;
                            break;
                    }

                    break;
            }
        }
    }
}