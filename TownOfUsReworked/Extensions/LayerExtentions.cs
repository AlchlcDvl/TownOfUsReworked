namespace TownOfUsReworked.Extensions;

public static class LayerExtentions
{
    public static readonly string RoleColorString = $"<#{CustomColorManager.Role.ToHtmlStringRGBA()}>";
    public static readonly string AlignmentColorString = $"<#{CustomColorManager.Alignment.ToHtmlStringRGBA()}>";
    public static readonly string ObjectivesColorString = $"<#{CustomColorManager.Objectives.ToHtmlStringRGBA()}>";
    public static readonly string AttributesColorString = $"<#{CustomColorManager.Attributes.ToHtmlStringRGBA()}>";
    public static readonly string AbilitiesColorString = $"<#{CustomColorManager.Abilities.ToHtmlStringRGBA()}>";
    public static readonly string DispositionColorString = $"<#{CustomColorManager.Disposition.ToHtmlStringRGBA()}>";
    public static readonly string ModifierColorString = $"<#{CustomColorManager.Modifier.ToHtmlStringRGBA()}>";
    public static readonly string AbilityColorString = $"<#{CustomColorManager.Ability.ToHtmlStringRGBA()}>";
    public static readonly string SubFactionColorString = $"<#{CustomColorManager.SubFaction.ToHtmlStringRGBA()}>";
    public static readonly string AttackColorString = $"<#{CustomColorManager.Attack.ToHtmlStringRGBA()}>";
    public static readonly string DefenseColorString = $"<#{CustomColorManager.Defense.ToHtmlStringRGBA()}>";

    public static bool Is<T>(this PlayerControl player) where T : PlayerLayer => player.TryGetLayer<T>(out _);

    public static bool IIs<T>(this PlayerControl player) where T : IPlayerLayer => player.TryGetILayer<T>(out _);

    public static bool Is(this PlayerControl player, LayerEnum type) => player.GetLayers().Any(x => x.Type == type);

    public static bool Is(this Disposition disp, LayerEnum dispositionType) => disp?.Type == dispositionType;

    public static bool Is(this PlayerControl player, Role role) => player.GetRole().Player == role.Player;

    public static bool Is(this PlayerControl player, SubFaction subFaction) => player.GetRole()?.SubFaction == subFaction;

    public static bool Is(this PlayerControl player, Faction faction) => player.GetFaction() == faction;

    public static bool Is(this PlayerControl player, params Faction[] factions) => factions.Contains(player.GetFaction());

    public static bool Is(this PlayerControl player, Alignment alignment) => player.GetRole()?.Alignment == alignment;

    public static bool Is(this PlayerControl player, Faction faction, Alignment alignment)
    {
        var role = player.GetRole();
        return role?.Faction == faction && role?.Alignment == alignment;
    }

    public static Faction GetFaction(this PlayerControl player)
    {
        if (!player)
            return Faction.None;

        var role = player.GetRole();

        if (!role)
            return player.Data.IsImpostor() ? Faction.Intruder : Faction.Crew;

        return role.Faction;
    }

    public static SubFaction GetSubFaction(this PlayerControl player)
    {
        if (!player)
            return SubFaction.None;

        var role = player.GetRole();

        if (!role)
            return SubFaction.None;

        return role.SubFaction;
    }

    public static Alignment GetAlignment(this PlayerControl player)
    {
        if (!player)
            return Alignment.None;

        var role = player.GetRole();

        if (!role)
            return Alignment.None;

        return role.Alignment;
    }

    public static bool IsConverted(this Role role) => role.SubFaction != SubFaction.None && role is not Neophyte;

    public static bool Diseased(this PlayerControl player) => player.GetRole().Diseased;

    public static bool IsCrewDefect(this PlayerControl player) => player.GetRole().IsCrewDefect;

    public static bool IsIntDefect(this PlayerControl player) => player.GetRole().IsIntDefect;

    public static bool IsSynDefect(this PlayerControl player) => player.GetRole().IsSynDefect;

    public static bool IsNeutDefect(this PlayerControl player) => player.GetRole().IsNeutDefect;

    public static bool IsDefect(this PlayerControl player) => player.IsCrewDefect() || player.IsIntDefect() || player.IsSynDefect() || player.IsNeutDefect();

    public static bool NotOnTheSameSide(this PlayerControl player) => !player.GetRole().Faithful;

    public static bool CanSabotage(this PlayerControl player)
    {
        if (IsHnS() || Meeting() || IsCustomHnS() || IsTaskRace() || !IntruderSettings.IntrudersCanSabotage)
            return false;

        var result = player.Is(Faction.Intruder, Faction.Illuminati, Faction.Pandorica) || (player.Is(Faction.Syndicate) && SyndicateSettings.AltImps);

        if (!player.Data.IsDead)
            return result;
        else
            return result && IntruderSettings.GhostsCanSabotage && !Role.GetRoles(player.GetFaction()).All(x => x.Dead);
    }

    public static bool HasAliveLover(this PlayerControl player) => player.TryGetLayer<Lovers>(out var lovers) && lovers.LoversAlive;

    public static bool CanDoTasks(this PlayerControl player)
    {
        if (!player)
            return false;

        if (!player.GetRole())
            return !player.Data.IsImpostor();

        var crewflag = player.Is(Faction.Crew);
        var neutralflag = player.Is(Faction.Neutral);
        var factionflag = player.Is(Faction.Intruder, Faction.Syndicate, Faction.Pandorica, Faction.Illuminati);

        var phantomflag = player.Is<Phantom>();

        var sideflag = player.NotOnTheSameSide();
        var taskmasterflag = player.Is<Taskmaster>();
        var defectflag = player.IsCrewDefect();

        var gmflag = player.Is<Runner>() || player.Is<Hunted>();

        var flag1 = crewflag && !sideflag;
        var flag2 = neutralflag && (taskmasterflag || phantomflag);
        var flag3 = factionflag && (taskmasterflag || defectflag);
        return flag1 || flag2 || flag3 || gmflag;
    }

    public static bool IsMoving(this PlayerControl player) => Moving.Contains(player.PlayerId);

    public static bool IsGATarget(this PlayerControl player) => PlayerLayer.GetLayers<GuardianAngel>(true).Any(x => x.TargetPlayer == player);

    public static bool IsExeTarget(this PlayerControl player) => PlayerLayer.GetLayers<Executioner>(true).Any(x => x.TargetPlayer == player);

    public static bool IsBHTarget(this PlayerControl player) => PlayerLayer.GetLayers<BountyHunter>(true).Any(x => x.TargetPlayer == player);

    public static bool IsGuessTarget(this PlayerControl player) => PlayerLayer.GetLayers<Guesser>(true).Any(x => x.TargetPlayer == player);

    public static Jackal GetJackal(this PlayerControl player) => PlayerLayer.GetLayers<Jackal>().Find(role => role.Members.Contains(player.PlayerId));

    public static Necromancer GetNecromancer(this PlayerControl player) => PlayerLayer.GetLayers<Necromancer>().Find(role => role.Members.Contains(player.PlayerId));

    public static Dracula GetDracula(this PlayerControl player) => PlayerLayer.GetLayers<Dracula>().Find(role => role.Members.Contains(player.PlayerId));

    public static Whisperer GetWhisperer(this PlayerControl player) => PlayerLayer.GetLayers<Whisperer>().Find(role => role.Members.Contains(player.PlayerId));

    public static Neophyte GetNeophyte(this PlayerControl player) => PlayerLayer.GetLayers<Neophyte>().Find(role => role.Members.Contains(player.PlayerId));

    public static bool IsShielded(this PlayerControl player) => PlayerLayer.GetILayers<IShielder>().Any(role => player == role.ShieldedPlayer);

    public static bool IsTrapped(this PlayerControl player) => PlayerLayer.GetILayers<ITrapper>().Any(role => role.Trapped.Contains(player.PlayerId));

    public static bool IsKnighted(this PlayerControl player) => PlayerLayer.GetLayers<Monarch>().Any(role => role.Knighted.Contains(player.PlayerId));

    public static bool IsSpellbound(this PlayerControl player) => PlayerLayer.GetILayers<IHexer>().Any(role => role.Spelled.Contains(player.PlayerId));

    public static bool IsArsoDoused(this PlayerControl player) => PlayerLayer.GetLayers<Arsonist>().Any(role => role.Doused.Contains(player.PlayerId));

    public static bool IsCryoDoused(this PlayerControl player) => PlayerLayer.GetLayers<Cryomaniac>().Any(role => role.Doused.Contains(player.PlayerId));

    public static bool TryIgnitingFrozen(this PlayerControl player)
    {
        var result = false;

        foreach (var cryo in PlayerLayer.GetLayers<Cryomaniac>())
        {
            if (!cryo.Doused.Contains(player.PlayerId))
                continue;

            result = true;
            cryo.Doused.Remove(player.PlayerId);
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, cryo, DouseActionsRPC.UnDouse, player.PlayerId);
        }

        return result;
    }

    public static bool TryFreezingIgnited(this PlayerControl player)
    {
        var result = false;

        foreach (var arso in PlayerLayer.GetLayers<Arsonist>())
        {
            if (!arso.Doused.Contains(player.PlayerId))
                continue;

            result = true;
            arso.Doused.Remove(player.PlayerId);
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, arso, DouseActionsRPC.UnDouse, player.PlayerId);
        }

        return result;
    }

    public static bool IsFaithful(this PlayerControl player) => player.GetRole()?.Faithful ?? false;

    public static bool IsBlackmailed(this PlayerControl player) => PlayerLayer.GetILayers<IBlackmailer>().Any(role => role.BlackmailedPlayer == player);

    public static bool IsSilenced(this PlayerControl player) => PlayerLayer.GetILayers<ISilencer>().Any(role => role.SilencedPlayer == player);

    public static bool SilenceActive(this PlayerControl player) => !player.IsSilenced() && PlayerLayer.GetILayers<ISilencer>().Any(role => role.HoldsDrive);

    public static bool IsOnAlert(this PlayerControl player) => PlayerLayer.GetILayers<IAlerter>().Any(role => role.Player == player && role.AlertButton?.EffectActive == true);

    public static bool IsVesting(this PlayerControl player) => PlayerLayer.GetLayers<Survivor>().Any(role => role.VestButton.EffectActive && player == role.Player);

    public static bool IsMarked(this PlayerControl player) => PlayerLayer.GetLayers<Ghoul>().Any(role => player == role.MarkedPlayer);

    public static bool IsAmbushed(this PlayerControl player) => PlayerLayer.GetILayers<IAmbusher>().Any(role => player == role.AmbushedPlayer && role.AmbushButton.EffectActive);

    public static bool IsCrusaded(this PlayerControl player) => PlayerLayer.GetILayers<ICrusader>().Any(role => player == role.CrusadedPlayer && role.CrusadeButton.EffectActive);

    public static bool CrusadeActive(this PlayerControl player) => PlayerLayer.GetILayers<ICrusader>().Any(role => player == role.CrusadedPlayer && role.CrusadeButton.EffectActive &&
        role.HoldsDrive);

    public static bool IsProtected(this PlayerControl player) => PlayerLayer.GetLayers<GuardianAngel>().Any(role => role.Protecting && player == role.TargetPlayer);

    public static bool IsInfected(this PlayerControl player) => PlayerLayer.GetLayers<Plaguebearer>().Any(role => role.Infected.Contains(player.PlayerId) || player == role.Player);

    public static bool IsFramed(this PlayerControl player) => PlayerLayer.GetILayers<IFramer>().Any(role => role.Framed.Contains(player.PlayerId));

    public static bool IsWinningRival(this PlayerControl player) => PlayerLayer.GetLayers<Rivals>().Any(x => x.Player == player && x.IsWinningRival);

    public static bool IsTurnedTraitor(this PlayerControl player) => player.IsIntTraitor() || player.IsSynTraitor();

    public static bool IsTurnedFanatic(this PlayerControl player) => player.IsIntFanatic() || player.IsSynFanatic();

    public static bool IsUnturnedFanatic(this PlayerControl player) => player.GetDisposition() is Fanatic fanatic && fanatic.Side == Faction.Crew;

    public static bool IsIntFanatic(this PlayerControl player) => player.GetDisposition() is Fanatic fanatic && fanatic.Side == Faction.Intruder;

    public static bool IsSynFanatic(this PlayerControl player) => player.GetDisposition() is Fanatic fanatic && fanatic.Side == Faction.Syndicate;

    public static bool IsIntTraitor(this PlayerControl player) => player.GetDisposition() is Traitor traitor && traitor.Side == Faction.Intruder;

    public static bool IsSynTraitor(this PlayerControl player) => player.GetDisposition() is Traitor traitor && traitor.Side == Faction.Intruder;

    public static bool IsCrewAlly(this PlayerControl player) => player.GetDisposition() is Allied ally && ally.Side == Faction.Crew;

    public static bool IsSynAlly(this PlayerControl player) => player.GetDisposition() is Allied ally && ally.Side == Faction.Syndicate;

    public static bool IsIntAlly(this PlayerControl player) => player.GetDisposition() is Allied ally && ally.Side == Faction.Intruder;

    public static bool IsOtherRival(this PlayerControl player, PlayerControl refPlayer) => player.GetOtherRival() == refPlayer;

    public static bool IsOtherLover(this PlayerControl player, PlayerControl refPlayer) => player.GetOtherLover() == refPlayer;

    public static bool IsOtherLink(this PlayerControl player, PlayerControl refPlayer) => player.GetOtherLink() == refPlayer;

    public static bool IsFlashed(this PlayerControl player) => !player.HasDied() && PlayerLayer.GetILayers<IFlasher>().Any(x => x.FlashedPlayers.Contains(player.PlayerId));

    public static bool SyndicateSided(this PlayerControl player) => player.Is(Faction.Syndicate) && !player.Is<Syndicate>();

    public static bool IntruderSided(this PlayerControl player) => player.Is(Faction.Intruder) && !player.Is<Intruder>();

    public static bool CrewSided(this PlayerControl player) => player.Is(Faction.Crew) && !player.Is<Crew>();

    public static bool Last(PlayerControl player) => Classes.GameStates.Last(player.GetFaction());

    public static bool CanKill(this PlayerControl player)
    {
        var role = player.GetRole();
        return role is Intruder or Syndicate || role?.Alignment is Alignment.Killing or Alignment.Harbinger or Alignment.Apocalypse || player.GetDisposition() is Corrupted or Fanatic or Traitor;
    }

    public static bool IsPostmortal(this PlayerControl player) => player.HasDied() && player.IIs<IGhosty>();

    public static bool Caught(this PlayerControl player)
    {
        if (!player.IsPostmortal())
            return true;

        if (player.TryGetILayer<IGhosty>(out var iGhost))
            return iGhost.Caught;

        return true;
    }

    public static bool IsLinkedTo(this PlayerControl player, PlayerControl refplayer) => player.IsOtherRival(refplayer) || player.IsOtherLover(refplayer) || player.IsOtherLink(refplayer) ||
        (player.Is<Mafia>() && refplayer.Is<Mafia>());

    public static float GetBaseSpeed(this PlayerControl player) => player.HasDied() && (!player.IsPostmortal() || player.Caught()) ? GameSettings.GhostSpeed : GameSettings.PlayerSpeed;

    public static float GetModifiedSpeed(this PlayerControl player)
    {
        if (TransitioningSpeed.TryGetValue(player.PlayerId, out var speed))
            return speed;
        else
            return player.IsMimicking(out var mimicked) ? mimicked.GetSpeed() : player.GetSpeed();
    }

    public static float GetSpeed(this PlayerControl player)
    {
        var result = 1f;

        if (player.HasDied() || Lobby() || (HudHandler.Instance.IsCamoed && BetterSabotages.CamoHideSpeed && !TransitioningSpeed.ContainsKey(player.PlayerId)))
            return result;

        if (IntroCutscene.Instance)
            return 0f;

        if (player.TryGetLayer<Hunter>(out var hunt))
            return hunt.Starting ? 0f : GameModeSettings.HunterSpeedModifier;

        if (player.Is<Dwarf>())
            result *= Dwarf.DwarfSpeed;
        else if (player.Is<Giant>())
            result *= Giant.GiantSpeed;
        else if (player.TryGetLayer<Drunk>(out var drunk))
            result *= drunk.Modify;

        if (DragHandler.Instance.Dragging.ContainsKey(player.PlayerId))
            result *= Janitor.DragModifier;

        if (PlayerLayer.GetLayers<Drunkard>().Any(x => x.ConfuseButton.EffectActive && (x.HoldsDrive || (x.ConfusedPlayer == player && !x.HoldsDrive))) ||
            PlayerLayer.GetLayers<PromotedRebel>().Any(x => x.ConfuseButton.EffectActive && (x.HoldsDrive || (x.ConfusedPlayer == player && !x.HoldsDrive)) && x.IsDrunk))
        {
            result *= -1;
        }

        if (PlayerLayer.GetLayers<Timekeeper>().Any(x => x.TimeButton.EffectActive) || PlayerLayer.GetLayers<PromotedRebel>().Any(x => x.IsTK && x.TimeButton.EffectActive))
        {
            if (!player.Is(Faction.Syndicate) || (player.Is(Faction.Syndicate) && !Timekeeper.TimeRewindImmunity))
                result = 0f;
        }

        if (player.TryGetLayer<Trapper>(out var trap))
            result *= trap.Building ? 0f : 1f;

        if (Ship() && Ship().Systems.TryGetValue(SystemTypes.LifeSupp, out var life))
        {
            var lifeSuppSystemType = life.Cast<LifeSuppSystemType>();

            if (lifeSuppSystemType.IsActive && BetterSabotages.OxySlow && !player.Data.IsDead)
                result *= Mathf.Lerp(1f, 0.25f, lifeSuppSystemType.Countdown / lifeSuppSystemType.LifeSuppDuration);
        }

        return result;
    }

    public static float GetModifiedSize(this PlayerControl player)
    {
        if (TransitioningSize.TryGetValue(player.PlayerId, out var size))
            return size;
        else
            return player.IsMimicking(out var mimicked) ? mimicked.GetSize() : player.GetSize();
    }

    public static float GetSize(this PlayerControl player)
    {
        if (Ship()?.Systems?.TryGetValue(SystemTypes.MushroomMixupSabotage, out var sab) == true && sab.TryCast<MushroomMixupSabotageSystem>(out var mixup) && mixup.IsActive)
            return 1f;

        if (Lobby() || (HudHandler.Instance.IsCamoed && BetterSabotages.CamoHideSize && !TransitioningSize.ContainsKey(player.PlayerId)))
            return 1f;
        else if (player.Is<Dwarf>())
            return Dwarf.DwarfScale;
        else if (player.Is<Giant>())
            return Giant.GiantScale;
        else
            return 1f;
    }

    public static bool IsMimicking(this PlayerControl player, out PlayerControl mimicked)
    {
        mimicked = player;

        if (player.HasDied())
            return false;

        var flag = CachedMorphs.TryGetValue(player.PlayerId, out var mimickedId);

        if (flag)
            mimicked = PlayerById(mimickedId);

        if (!flag || mimicked == player)
        {
            foreach (var morph in PlayerLayer.GetLayers<Morphling>())
            {
                if (morph.Player == player && morph.MorphedPlayer && morph.MorphButton.EffectActive)
                {
                    mimicked = morph.MorphedPlayer;
                    return true;
                }
            }

            foreach (var gf in PlayerLayer.GetLayers<PromotedGodfather>())
            {
                if (gf.IsMorph && gf.Player == player && gf.MorphedPlayer && gf.MorphButton.EffectActive)
                {
                    mimicked = gf.MorphedPlayer;
                    return true;
                }
            }

            foreach (var ss in PlayerLayer.GetLayers<Shapeshifter>())
            {
                if ((ss.ShapeshiftPlayer1 == player || ss.ShapeshiftPlayer2 == player) && ss.ShapeshiftButton.EffectActive)
                {
                    mimicked = ss.ShapeshiftPlayer1 == player ? ss.ShapeshiftPlayer1 : ss.ShapeshiftPlayer2;
                    return true;
                }
            }

            foreach (var reb in PlayerLayer.GetLayers<PromotedRebel>())
            {
                if (reb.IsSS && (reb.ShapeshiftPlayer1 == player || reb.ShapeshiftPlayer2 == player) && reb.ShapeshiftButton.EffectActive)
                {
                    mimicked = reb.ShapeshiftPlayer1 == player ? reb.ShapeshiftPlayer1 : reb.ShapeshiftPlayer2;
                    return true;
                }
            }
        }

        return flag;
    }

    public static bool CanVent(this PlayerControl player)
    {
        var playerInfo = player?.Data;

        if (!player || !playerInfo)
            return false;
        else if (IsHnS())
            return !playerInfo.IsImpostor();
        else if (playerInfo.Disconnected || (int)GameModifiers.WhoCanVent is 3 || player.inMovingPlat || player.onLadder || Meeting())
            return false;
        else if (player.inVent || GameModifiers.WhoCanVent == WhoCanVentOptions.Everyone)
            return true;
        else if (playerInfo.IsDead)
            return player.IsPostmortal() && !player.Caught() && player.inVent;

        var playerRole = player.GetRole();
        var mainflag = false;

        if (!playerRole)
            mainflag = playerInfo.IsImpostor();
        else if (playerRole is Hunter)
            mainflag = GameModeSettings.HunterVent;
        else if (playerRole is Hunted or Runner)
            mainflag = false;
        else if (player.Is<Mafia>())
            mainflag = Mafia.MafVent;
        else if (player.Is<Corrupted>())
            mainflag = Corrupted.CorruptedVent;
        else if (playerRole.SubFaction != SubFaction.None && playerRole.Alignment != Alignment.Neophyte)
        {
            mainflag = playerRole.SubFaction switch
            {
                SubFaction.Undead => Dracula.UndeadVent,
                SubFaction.Cabal => Jackal.RecruitVent,
                SubFaction.Reanimated => Necromancer.ResurrectVent,
                SubFaction.Cult => Whisperer.PersuadedVent,
                _ => false
            };
        }
        else if (playerRole is Syndicate syn)
            mainflag = (syn.HoldsDrive && (int)SyndicateSettings.SyndicateVent is 1) || (int)SyndicateSettings.SyndicateVent is 0;
        else if (playerRole is Intruder)
        {
            if (IntruderSettings.IntrudersVent)
            {
                if (playerRole is Janitor jani)
                {
                    mainflag = (int)Janitor.JanitorVentOptions is 3 || (jani.CurrentlyDragging && (int)Janitor.JanitorVentOptions is 1) || (!jani.CurrentlyDragging &&
                        (int)Janitor.JanitorVentOptions is 2);
                }
                else if (playerRole is PromotedGodfather gf)
                {
                    if (gf.IsJani)
                    {
                        mainflag = (int)Janitor.JanitorVentOptions is 3 || (gf.CurrentlyDragging && (int)Janitor.JanitorVentOptions is 1) || (!gf.CurrentlyDragging &&
                            (int)Janitor.JanitorVentOptions is 2);
                    }
                    else if (gf.IsMorph)
                        mainflag = Morphling.MorphlingVent;
                    else if (gf.IsWraith)
                        mainflag = Wraith.WraithVent;
                    else if (gf.IsGren)
                        mainflag = Grenadier.GrenadierVent;
                    else if (gf.IsTele)
                        mainflag = Teleporter.TeleVent;
                    else
                        mainflag = true;
                }
                else if ((playerRole is Morphling && !Morphling.MorphlingVent) || (playerRole is Wraith && !Wraith.WraithVent) || (playerRole is Grenadier && !Grenadier.GrenadierVent) ||
                    (playerRole is Teleporter && !Teleporter.TeleVent))
                {
                    mainflag = false;
                }
                else
                    mainflag = true;
            }
            else
                mainflag = false;
        }
        else if (player.Is(Faction.Crew) || playerRole is Crew)
        {
            mainflag = playerRole is Engineer || CrewSettings.CrewVent == CrewVenting.Always || ((CrewSettings.CrewVent == CrewVenting.OnTasksDone || player.Is<Tunneler>()) &&
                playerRole.TasksDone);
        }
        else if (playerRole.Faction is Faction.Neutral || playerRole is Neutral)
        {
            if (NeutralSettings.NeutralsVent)
            {
                if (playerRole is SerialKiller sk)
                {
                    mainflag = SerialKiller.SKVentOptions == 0 || (sk.BloodlustButton.EffectActive && (int)SerialKiller.SKVentOptions == 1) || (!sk.BloodlustButton.EffectActive &&
                        (int)SerialKiller.SKVentOptions == 2);
                }
                else if (playerRole is Werewolf ww)
                    mainflag = Werewolf.WerewolfVent == 0 || (ww.CanMaul && (int)Werewolf.WerewolfVent == 1) || (!ww.CanMaul && (int)Werewolf.WerewolfVent == 2);
                else
                {
                    mainflag = (playerRole is Murderer && Murderer.MurdVent) || (playerRole is Glitch && Glitch.GlitchVent) || (playerRole is Juggernaut && Juggernaut.JuggVent) || (playerRole
                        is Pestilence && Pestilence.PestVent) || (playerRole is Jester && Jester.JesterVent) || (playerRole is Plaguebearer && Plaguebearer.PBVent) || (playerRole is Arsonist &&
                        Arsonist.ArsoVent) || (playerRole is Executioner && Executioner.ExeVent) || (playerRole is Cannibal && Cannibal.CannibalVent) || (playerRole is Dracula &&
                        Dracula.DracVent) || (playerRole is Survivor && Survivor.SurvVent) || (playerRole is Actor && Actor.ActorVent) || (playerRole is GuardianAngel &&
                        GuardianAngel.GAVent) || (playerRole is Amnesiac && Amnesiac.AmneVent) || (playerRole is Jackal && Jackal.JackalVent) || (playerRole is BountyHunter &&
                        BountyHunter.BHVent) || (playerRole is Betrayer && Betrayer.BetrayerVent);
                }
            }
            else
                mainflag = false;
        }

        return mainflag;
    }

    public static bool CanChat(this PlayerControl player)
    {
        if (Lobby() || Meeting())
            return true;

        var playerInfo = player?.Data;
        var disp = player.GetDisposition();

        if (!player || !playerInfo)
            return false;
        else if (playerInfo.IsDead)
            return true;
        else if (disp is Lovers)
            return Lovers.LoversChat;
        else if (disp is Rivals)
            return Rivals.RivalsChat;
        else if (disp is Linked)
            return Linked.LinkedChat;
        else if (player.Is<Hunted>())
            return GameModeSettings.HuntedChat;

        return false;
    }

    public static bool IsBlocked(this PlayerControl player) => PlayerLayer.GetILayers<IBlocker>().Any(x => x.BlockTarget == player) || PlayerLayer.GetLayers<Banshee>().Any(x =>
        x.Blocked.Contains(player.PlayerId));

    public static bool SeemsEvil(this PlayerControl player)
    {
        var role = player.GetRole();
        var disp = player.GetDisposition();
        var intruderFlag = role.Faction is Faction.Intruder && disp is not (Traitor or Fanatic) && role is PromotedGodfather;
        var syndicateFlag = role.Faction is Faction.Syndicate && disp is not (Traitor or Fanatic) && role is PromotedRebel && Syndicate.DriveHolder != player;
        var traitorFlag = player.IsTurnedTraitor() && Traitor.TraitorColourSwap;
        var fanaticFlag = player.IsTurnedFanatic() && Fanatic.FanaticColourSwap;
        var nkFlag = role.Alignment is Alignment.Killing && !Sheriff.NeutKillingRed;
        var neFlag = role.Alignment is Alignment.Evil && !Sheriff.NeutEvilRed;
        var framedFlag = player.IsFramed();
        return intruderFlag || syndicateFlag || traitorFlag || nkFlag || neFlag || framedFlag || fanaticFlag;
    }

    public static bool IsBlockImmune(PlayerControl player) => player.GetRole().RoleBlockImmune;

    public static PlayerControl GetOtherLover(this PlayerControl player) => player.TryGetLayer<Lovers>(out var lovers) ? lovers.OtherLover : null;

    public static PlayerControl GetOtherRival(this PlayerControl player) => player.TryGetLayer<Rivals>(out var rivals) ? rivals.OtherRival : null;

    public static PlayerControl GetOtherLink(this PlayerControl player) => player.TryGetLayer<Linked>(out var linked) ? linked.OtherLink : null;

    public static bool IsExcludedNeutral(PlayerControl player)
    {
        if (player.TryGetLayer<GuardianAngel>(out var ga))
            return ga.TargetAlive;
        else if (player.TryGetLayer<Evil>(out var exe))
            return exe.HasWon;

        return false;
    }

    public static string RoleCardInfo(this PlayerControl player)
    {
        var info = player.GetLayers().ToList();

        if (info.Count != 4)
            return "";

        var role = info[0] as Role;
        var modifier = info[1] as Modifier;
        var ability = info[2] as Ability;
        var disposition = info[3] as Disposition;

        var objectives = $"{ObjectivesColorString}Goal:";
        var abilities = $"{AbilitiesColorString}Abilities:";
        var attributes = $"{AttributesColorString}Attributes:";
        var roleName = $"{RoleColorString}Role: <b>";
        var dispositionName = $"{DispositionColorString}Disposition: <b>";
        var abilityName = $"{AbilityColorString}Ability: <b>";
        var modifierName = $"{ModifierColorString}Modifier: <b>";
        var alignment = $"{AlignmentColorString}Alignment: <b>";
        var subfaction = $"{SubFactionColorString}Sub-Faction: <b>";
        var attdef = $"{AttackColorString}Attack/{DefenseColorString}Defense</color>: <b>";

        if (role)
        {
            roleName += $"{role.ColorString}{role}</color>";
            objectives += $"\n{role.ColorString}{role.Objectives()}</color>";
            alignment += $"{role.FactionColorString}{role.Faction}({AlignmentColorString}{role.Alignment}</color>)</color>";
            subfaction += $"{role.SubFactionColorString}{role.SubFactionName} {role.SubFactionSymbol}</color>";
            attdef += $"{player.GetAttackValue()}/{DefenseColorString}{player.GetDefenseValue()}</color>";
        }
        else
        {
            roleName += "None";
            alignment += "None";
            subfaction += "None";
            attdef += $"None/{DefenseColorString}None</color>";
        }

        roleName += "</b></color>";
        alignment += "</b></color>";
        subfaction += "</b></color>";
        attdef += "</b></color>";

        if (info[3] && !disposition.Hidden)
        {
            objectives += $"\n{disposition.ColorString}{disposition.Description()}</color>";
            dispositionName += $"{disposition.ColorString}{disposition.Name} {disposition.Symbol}</color>";
        }
        else
            dispositionName += "None φ";

        dispositionName += "</b></color>";

        if (info[2] && !ability.Hidden && ability.Type != LayerEnum.NoneAbility)
            abilityName += $"{ability.ColorString}{ability.Name}</color>";
        else
            abilityName += "None";

        abilityName += "</b></color>";

        if (info[1] && !modifier.Hidden && modifier.Type != LayerEnum.NoneModifier)
            modifierName += $"{modifier.ColorString}{modifier.Name}</color>";
        else
            modifierName += "None";

        modifierName += "</b></color>";
        var neo = player.GetNeophyte();
        objectives += role.SubFaction switch
        {
            SubFaction.None => "",
            _ => $"\n{role.SubFactionColorString}- You are a member of the {role.SubFaction}. Help {neo.PlayerName} in taking over the mission {role.SubFactionSymbol}</color>"
        };
        objectives += "</color>";

        if (info[0] && role.Description() is not ("" or "- None"))
            abilities += $"\n{role.ColorString}{role.Description()}</color>";

        if (info[0] && role.RoleBlockImmune)
            abilities += "\n- You are immune to roleblocks";

        if (info[2] && !ability.Hidden && ability.Type != LayerEnum.NoneAbility && ability.Description() is not ("" or "- None"))
            abilities += $"\n{ability.ColorString}{ability.Description()}</color>";

        if (abilities == $"{AbilitiesColorString}Abilities:")
            abilities = "";
        else
            abilities = $"\n{abilities}</color>";

        if (info[1] && !modifier.Hidden && modifier.Type != LayerEnum.NoneModifier && modifier.Description() is not ("" or "- None"))
            attributes += $"\n{modifier.ColorString}{modifier.Description()}</color>";

        if (player.IsGuessTarget() && Guesser.GuessTargetKnows)
            attributes += "\n<#EEE5BEFF>- Someone wants to assassinate you π</color>";

        if (player.IsExeTarget() && Executioner.ExeTargetKnows)
            attributes += "\n<#CCCCCCFF>- Someone wants you ejected §</color>";

        if (player.IsGATarget() && GuardianAngel.GATargetKnows)
            attributes += "\n<#FFFFFFFF>- Someone wants to protect you ★</color>";

        if (player.IsBHTarget())
            attributes += "\n<#B51E39FF>- There is a bounty on your head Θ</color>";

        if (player.Is(Faction.Syndicate) && role is Syndicate syn && syn.HoldsDrive)
            attributes += "\n<#008000FF>- You have the power of the Chaos Drive Δ</color>";

        if (!player.CanDoTasks())
            attributes += "\n<#ABCDEFFF>- Your tasks are fake</color>";

        if (player.Data.IsDead)
            attributes += "\n<#FF1919FF>- You are dead</color>";

        if (attributes == $"{AttributesColorString}Attributes:")
            attributes = "";
        else
            attributes = $"\n{attributes}</color>";

        return $"{roleName}\n{attdef}\n{alignment}\n{subfaction}\n{dispositionName}\n{abilityName}\n{modifierName}\n{objectives}{abilities}{attributes}";
    }

    public static void RegenTask(this PlayerControl player)
    {
        foreach (var task in player.myTasks)
        {
            if (!task.TryCast<ImportantTextTask>(out var imp))
                continue;

            if (imp.Text.Contains("Sabotage and kill everyone") || imp.Text.Contains("Fake Tasks") || imp.Text.Contains("tasks to win"))
                player.myTasks.Remove(imp);
        }
    }

    public static void RoleUpdate(this Role newRole, Role former, PlayerControl player = null, bool retainFaction = false)
    {
        player ??= former.Player;
        CustomButton.AllButtons.Where(x => x.Owner == former || !x.Owner.Player).ForEach(x => x.Destroy());
        CustomArrow.AllArrows.Where(x => x.Owner == player).ForEach(x => x.Disable());
        var allArrows = former.AllArrows.Clone();
        var history = former.RoleHistory.Clone();
        former.End();
        newRole.Start(player);
        newRole.SubFaction = former.SubFaction;
        newRole.DeathReason = former.DeathReason;
        newRole.KilledBy = former.KilledBy;
        newRole.Diseased = former.Diseased;
        newRole.AllArrows.AddRange(allArrows);
        newRole.RoleHistory.AddRange(history);
        newRole.RoleHistory.Add(former.Type);

        if (!retainFaction)
            newRole.Faction = former.Faction;

        if (newRole.Local)
        {
            ButtonUtils.Reset();
            newRole.UpdateButtons();
            newRole.Player.RegenTask();
            Flash(newRole.Color);
        }

        if (CustomPlayer.Local.TryGetLayer<Seer>(out var seer))
            Flash(seer.Color);

        if (player.Data.Role is LayerHandler layerHandler)
            layerHandler.SetUpLayers();
    }

    public static bool CanButton(this PlayerControl player, out string name)
    {
        name = "Shy";
        var result = !player.Is<Shy>() && player.RemainingEmergencies > 0;
        var role = player.GetRole();
        var ability = player.GetAbility();

        if (role is Mayor)
        {
            name = "Mayor";
            result = Mayor.MayorButton;
        }
        else if (role is Jester)
        {
            name = "Jester";
            result = Jester.JesterButton;
        }
        else if (role is Actor)
        {
            name = "Actor";
            result = Actor.ActorButton;
        }
        else if (role is Executioner)
        {
            name = "Executioner";
            result = Executioner.ExecutionerButton;
        }
        else if (role is Guesser)
        {
            name = "Guesser";
            result = Guesser.GuesserButton;
        }
        else if (role is Dictator)
        {
            name = "Dictator";
            result = Dictator.DictatorButton;
        }
        else if (role is Monarch)
        {
            name = "Monarch";
            result = Monarch.MonarchButton;
        }
        else if (player.IsKnighted())
        {
            name = "Knight";
            result = Monarch.KnightButton;
        }
        else if (ability is Swapper)
        {
            name = "Swapper";
            result = Swapper.SwapperButton;
        }
        else if (ability is Politician)
        {
            name = "Politician";
            result = Politician.PoliticianButton;
        }
        else if (IsTaskRace() || IsCustomHnS())
        {
            name = "GameMode";
            result = false;
        }

        return result;
    }

    public static bool IsBombed(this Vent vent) => PlayerLayer.GetILayers<IVentBomber>().Any(x => x.BombedIDs.Contains(vent.Id));

    public static PlayerLayerEnum GetLayerType(this LayerEnum layer)
    {
        var id = (int)layer;

        if (id < 91)
            return PlayerLayerEnum.Role;
        else if (id < 105)
            return PlayerLayerEnum.Modifier;
        else if (id < 116)
            return PlayerLayerEnum.Disposition;
        else if (id < 133)
            return PlayerLayerEnum.Ability;
        else
            return PlayerLayerEnum.None;
    }

    public static AttackEnum GetAttackValue(this PlayerControl player, PlayerControl target = null, AttackEnum? overrideAtt = null)
    {
        if (overrideAtt.HasValue)
            return overrideAtt.Value;

        var attack = 0;

        foreach (var layer in player.GetLayers())
        {
            if (attack < 2)
                attack += (int)layer.AttackVal;
        }

        if ((player.IsAmbushed() || player.IsCrusaded() || player.GetRole().Bombed) && attack < 1)
            attack = 1;

        if (target && player.IsLinkedTo(target))
            attack = 0;

        attack = Mathf.Clamp(attack, 0, 3);
        return (AttackEnum)attack;
    }

    public static DefenseEnum GetDefenseValue(this PlayerControl player, PlayerControl source = null, DefenseEnum? overrideDef = null)
    {
        if (overrideDef.HasValue)
            return overrideDef.Value;

        var defense = 0;
        player.GetLayers().ForEach(x => defense += (int)x.DefenseVal);

        if ((player.IsShielded() || player.IsAmbushed() || player.IsCrusaded() | player.IsProtected()) && defense < 2)
            defense = 2;

        if (source && player.IsLinkedTo(source))
            defense = 3;

        if (PlayerLayer.GetLayers<Glitch>().Any(x => x.HackTarget == player && !x.Player.IsLinkedTo(player)))
            defense = 0;

        if (player.name == CachedFirstDead)
            defense = 3;

        defense = Mathf.Clamp(defense, 0, 3);
        return (DefenseEnum)defense;
    }

    public static IEnumerable<PlayerLayer> GetLayersFromList(this PlayerControl player) => PlayerLayer.AllLayers.Where(x => x.Player == player).OrderBy(x => (int)x.LayerType);

    public static IEnumerable<PlayerLayer> GetLayers(this PlayerControl player)
    {
        if (player?.Data?.Role is LayerHandler handler)
            return handler.CustomLayers;

        return player.GetLayersFromList();
    }

    public static T GetLayerFromList<T>(this PlayerControl player) where T : PlayerLayer => PlayerLayer.GetLayers<T>().Find(x => x.Player == player);

    public static T GetILayerFromList<T>(this PlayerControl player) where T : IPlayerLayer => PlayerLayer.GetILayers<T>().Find(x => x.Player == player);

    public static T GetLayer<T>(this PlayerControl player) where T : PlayerLayer
    {
        if (player?.Data?.Role is LayerHandler handler)
            return handler.GetLayer<T>();

        return player.GetLayerFromList<T>();
    }

    public static T GetILayer<T>(this PlayerControl player) where T : IPlayerLayer
    {
        if (player?.Data?.Role is LayerHandler handler)
            return handler.GetILayer<T>();

        return player.GetILayerFromList<T>();
    }

    public static Role GetRoleFromList(this PlayerControl player) => player.GetLayerFromList<Role>();

    public static Role GetRole(this PlayerControl player)
    {
        if (player?.Data?.Role is LayerHandler handler)
            return handler.CustomRole;

        return player.GetRoleFromList();
    }

    public static Disposition GetDispositionFromList(this PlayerControl player) => player.GetLayerFromList<Disposition>();

    public static Disposition GetDisposition(this PlayerControl player)
    {
        if (player?.Data?.Role is LayerHandler handler)
            return handler.CustomDisposition;

        return player.GetDispositionFromList();
    }

    public static Modifier GetModifierFromList(this PlayerControl player) => player.GetLayerFromList<Modifier>();

    public static Modifier GetModifier(this PlayerControl player)
    {
        if (player.Data?.Role is LayerHandler handler)
            return handler.CustomModifier;

        return player.GetModifierFromList();
    }

    public static Ability GetAbilityFromList(this PlayerControl player) => player.GetLayerFromList<Ability>();

    public static Ability GetAbility(this PlayerControl player)
    {
        if (player?.Data?.Role is LayerHandler handler)
            return handler.CustomAbility;

        return player.GetAbilityFromList();
    }

    public static bool TryGetLayer<T>(this PlayerControl player, out T layer) where T : PlayerLayer => layer = player.GetLayer<T>();

    public static bool TryGetILayer<T>(this PlayerControl player, out T iLayer) where T : IPlayerLayer => (iLayer = player.GetILayer<T>()) != null;

    public static void AssignChaosDrive()
    {
        if (SyndicateSettings.SyndicateCount == 0 || !AmongUsClient.Instance.AmHost)
            return;

        var all = AllPlayers().Where(x => !x.HasDied()).Select(x => x.GetRole()).Where(x => x.Faction == Faction.Syndicate && x is Syndicate);

        if (!all.Any())
            return;

        Role chosen = null;

        if (!Syndicate.DriveHolder || Syndicate.DriveHolder.HasDied())
        {
            if (!all.TryFinding(x => x is PromotedRebel, out chosen))
                chosen = all.Find(x => x.Alignment == Alignment.Disruption);

            if (!chosen)
                chosen = all.Find(x => x.Alignment == Alignment.Support);

            if (!chosen)
                chosen = all.Find(x => x.Alignment == Alignment.Power);

            if (!chosen)
                chosen = all.Find(x => x.Alignment == Alignment.Killing);

            if (!chosen)
                chosen = all.Find(x => x is Anarchist or Rebel or Sidekick);
        }

        Syndicate.DriveHolder = chosen.Player;
        Syndicate.SyndicateHasChaosDrive = chosen;
        CallRpc(CustomRPC.Misc, MiscRPC.ChaosDrive, chosen.Player?.PlayerId ?? 255);
    }

    public static void ConvertPlayer(byte target, byte convert, SubFaction sub, bool skip)
    {
        var converted = PlayerById(target);
        var converter = PlayerById(convert);
        var converts = converted.Is(SubFaction.None) || (converted.Is(sub) && !converted.Is(Alignment.Neophyte));

        if (skip || RoleGenManager.Convertible <= 0 || RoleGenManager.Pure == converted || !converts)
        {
            if (AmongUsClient.Instance.AmHost)
                Interact(converter, converted, true, true);

            return;
        }

        var role1 = converted.GetRole();
        var role2 = converter.GetRole();

        if (role2 is Neophyte neophyte)
        {
            neophyte.Members.Add(target);

            if (converted.Is(SubFaction.None) && neophyte is Jackal jackal)
            {
                if (!jackal.Recruit1)
                    jackal.Recruit1 = converted;
                else if (!jackal.Recruit2)
                    jackal.Recruit2 = converted;
                else if (!jackal.Recruit3)
                    jackal.Recruit3 = converted;
            }

            if (role1 is Neophyte neophyte1 && role1.Type == role2.Type)
            {
                neophyte1.Members.AddRange(neophyte.Members);
                neophyte.Members.AddRange(neophyte1.Members);

                if (role1 is Whisperer whisperer1 && role2 is Whisperer whisperer2)
                {
                    whisperer1.Members.ForEach(x => whisperer2.PlayerConversion.Remove(x));
                    whisperer2.Members.ForEach(x => whisperer1.PlayerConversion.Remove(x));
                }
            }
        }

        role1.SubFaction = sub;
        role1.Faction = Faction.Neutral;
        RoleGenManager.Convertible--;

        if (converted.AmOwner)
            Flash(role1.SubFactionColor);
        else if (CustomPlayer.Local.Is<Mystic>())
            Flash(CustomColorManager.Mystic);
    }

    public static void RpcConvert(byte target, byte convert, SubFaction sub, bool condition = false)
    {
        ConvertPlayer(target, convert, sub, condition);
        CallRpc(CustomRPC.Action, ActionsRPC.Convert, convert, target, sub, condition);
    }
}