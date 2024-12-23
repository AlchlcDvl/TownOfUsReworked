namespace TownOfUsReworked.Extensions;

public static class LayerExtentions
{
    public static string RoleColorString => $"<#{CustomColorManager.Role.ToHtmlStringRGBA()}>";
    public static string AlignmentColorString => $"<#{CustomColorManager.Alignment.ToHtmlStringRGBA()}>";
    public static string ObjectivesColorString => $"<#{CustomColorManager.Objectives.ToHtmlStringRGBA()}>";
    public static string AttributesColorString => $"<#{CustomColorManager.Attributes.ToHtmlStringRGBA()}>";
    public static string AbilitiesColorString => $"<#{CustomColorManager.Abilities.ToHtmlStringRGBA()}>";
    public static string DispositionColorString => $"<#{CustomColorManager.Disposition.ToHtmlStringRGBA()}>";
    public static string ModifierColorString => $"<#{CustomColorManager.Modifier.ToHtmlStringRGBA()}>";
    public static string AbilityColorString => $"<#{CustomColorManager.Ability.ToHtmlStringRGBA()}>";
    public static string SubFactionColorString => $"<#{CustomColorManager.SubFaction.ToHtmlStringRGBA()}>";
    public static string AttackColorString => $"<#{CustomColorManager.Attack.ToHtmlStringRGBA()}>";
    public static string DefenseColorString => $"<#{CustomColorManager.Defense.ToHtmlStringRGBA()}>";

    public static bool Is<T>(this PlayerControl player) where T : PlayerLayer => player.TryGetLayer<T>(out _);

    public static bool IIs<T>(this PlayerControl player) where T : IPlayerLayer => player.TryGetILayer<T>(out _);

    public static bool Is(this PlayerControl player, LayerEnum type) => player.GetLayers().Any(x => x.Type == type);

    public static bool Is(this Role role, LayerEnum roleType) => role?.Type == roleType;

    public static bool Is(this Disposition disp, LayerEnum dispositionType) => disp?.Type == dispositionType;

    public static bool Is(this PlayerControl player, Role role) => player.GetRole().Player == role.Player;

    public static bool Is(this PlayerControl player, SubFaction subFaction) => player.GetRole()?.SubFaction == subFaction;

    public static bool Is(this PlayerControl player, Faction faction) => player.GetFaction() == faction;

    public static bool IsBase(this PlayerControl player, Faction faction) => player.GetRole()?.BaseFaction == faction;

    public static bool Is(this PlayerControl player, Alignment alignment) => player.GetRole()?.Alignment == alignment;

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

    public static bool IsRecruit(this PlayerControl player) => player.GetRole().IsRecruit;

    public static bool IsBitten(this PlayerControl player) => player.GetRole().IsBitten;

    public static bool IsPersuaded(this PlayerControl player) => player.GetRole().IsPersuaded;

    public static bool IsResurrected(this PlayerControl player) => player.GetRole().IsResurrected;

    public static bool IsConverted(this Role role) => (role.IsRecruit || role.IsBitten || role.IsPersuaded || role.IsResurrected) && role is not Neophyte;

    public static bool Diseased(this PlayerControl player) => player.GetRole().Diseased;

    public static bool IsCrewDefect(this PlayerControl player) => player.GetRole().IsCrewDefect;

    public static bool IsIntDefect(this PlayerControl player) => player.GetRole().IsIntDefect;

    public static bool IsSynDefect(this PlayerControl player) => player.GetRole().IsSynDefect;

    public static bool IsNeutDefect(this PlayerControl player) => player.GetRole().IsNeutDefect;

    public static bool IsDefect(this PlayerControl player) => player.IsCrewDefect() || player.IsIntDefect() || player.IsSynDefect() || player.IsNeutDefect();

    public static bool NotOnTheSameSide(this PlayerControl player) => !player.GetRole().Faithful;

    public static bool CanSabotage(this PlayerControl player)
    {
        if (IsHnS())
            return false;

        var result = (player.Is(Faction.Intruder) || (player.Is(Faction.Syndicate) && SyndicateSettings.AltImps)) && !Meeting() && IntruderSettings.IntrudersCanSabotage;

        if (!player.Data.IsDead)
            return result;
        else
            return result && IntruderSettings.GhostsCanSabotage && !Role.GetRoles(player.GetFaction()).All(x => x.Dead);
    }

    public static bool HasAliveLover(this PlayerControl player) => PlayerLayer.GetLayers<Lovers>().Any(x => x.Player == player && x.LoversAlive);

    public static bool CanDoTasks(this PlayerControl player)
    {
        if (!player)
            return false;

        if (!player.GetRole())
            return !player.Data.IsImpostor();

        var crewflag = player.Is(Faction.Crew);
        var neutralflag = player.Is(Faction.Neutral);
        var intruderflag = player.Is(Faction.Intruder);
        var syndicateflag = player.Is(Faction.Syndicate);

        var phantomflag = player.Is<Phantom>();

        var sideflag = player.NotOnTheSameSide();
        var taskmasterflag = player.Is<Taskmaster>();
        var defectflag = player.IsCrewDefect();

        var gmflag = player.Is<Runner>() || player.Is<Hunted>();

        var flag1 = crewflag && !sideflag;
        var flag2 = neutralflag && (taskmasterflag || phantomflag);
        var flag3 = intruderflag && (taskmasterflag || defectflag);
        var flag4 = syndicateflag && (taskmasterflag || defectflag);
        return flag1 || flag2 || flag3 || flag4 || gmflag;
    }

    public static bool IsMoving(this PlayerControl player) => PlayerLayer.GetLayers<Transporter>().Any(x => (x.TransportPlayer1 == player || x.TransportPlayer2 == player) &&
        x.Transporting) || PlayerLayer.GetLayers<Retributionist>().Any(x => (x.TransportPlayer1 == player || x.TransportPlayer2 == player) && x.Transporting) ||
        PlayerLayer.GetLayers<Warper>().Any(x => x.WarpPlayer1 == player && x.Warping) || PlayerLayer.GetLayers<PromotedRebel>().Any(x => x.WarpPlayer1 == player && x.Warping);

    public static bool IsGATarget(this PlayerControl player) => PlayerLayer.GetLayers<GuardianAngel>(true).Any(x => x.TargetPlayer == player);

    public static bool IsExeTarget(this PlayerControl player) => PlayerLayer.GetLayers<Executioner>(true).Any(x => x.TargetPlayer == player);

    public static bool IsBHTarget(this PlayerControl player) => PlayerLayer.GetLayers<BountyHunter>(true).Any(x => x.TargetPlayer == player);

    public static bool IsGuessTarget(this PlayerControl player) => PlayerLayer.GetLayers<Guesser>(true).Any(x => x.TargetPlayer == player);

    public static PlayerControl GetTarget(this PlayerControl player)
    {
        if (!player.GetRole().HasTarget)
            return null;

        if (player.TryGetLayer<Executioner>(out var exe))
            return exe.TargetPlayer;
        else if (player.TryGetLayer<GuardianAngel>(out var ga))
            return ga.TargetPlayer;
        else if (player.TryGetLayer<Guesser>(out var guesser))
            return guesser.TargetPlayer;
        else if (player.TryGetLayer<BountyHunter>(out var bh))
            return bh.TargetPlayer;

        return null;
    }

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

    public static bool IsProtectedMonarch(this PlayerControl player) => PlayerLayer.GetLayers<Monarch>().Any(role => role.Protected && role.Player == player);

    public static bool IsFaithful(this PlayerControl player) => player.GetRole().Faithful;

    public static bool IsBlackmailed(this PlayerControl player) => PlayerLayer.GetILayers<IBlackmailer>().Any(role => role.BlackmailedPlayer == player);

    public static bool IsSilenced(this PlayerControl player) => PlayerLayer.GetILayers<ISilencer>().Any(role => role.SilencedPlayer == player);

    public static bool SilenceActive(this PlayerControl player) => !player.IsSilenced() && PlayerLayer.GetILayers<ISyndicate>().Any(role => role.HoldsDrive);

    public static bool IsOnAlert(this PlayerControl player) => PlayerLayer.GetILayers<IAlerter>().Any(role => role.Player == player && role.AlertButton?.EffectActive == true);

    public static bool IsVesting(this PlayerControl player) => PlayerLayer.GetLayers<Survivor>().Any(role => role.VestButton.EffectActive && player == role.Player);

    public static bool IsMarked(this PlayerControl player) => PlayerLayer.GetLayers<Ghoul>().Any(role => player == role.MarkedPlayer);

    public static bool IsAmbushed(this PlayerControl player)
    {
        var ambFlag = PlayerLayer.GetLayers<Ambusher>().Any(role => role.AmbushButton.EffectActive && player == role.AmbushedPlayer);
        var gfFlag = PlayerLayer.GetLayers<PromotedGodfather>().Any(role => player == role.AmbushedPlayer && role.AmbushButton.EffectActive);
        return ambFlag || gfFlag;
    }

    public static bool IsCrusaded(this PlayerControl player)
    {
        var crusFlag = PlayerLayer.GetLayers<Crusader>().Any(role => role.CrusadeButton.EffectActive && player == role.CrusadedPlayer);
        var rebFlag = PlayerLayer.GetLayers<PromotedRebel>().Any(role => player == role.CrusadedPlayer && role.CrusadeButton.EffectActive);
        return crusFlag || rebFlag;
    }

    public static bool CrusadeActive(this PlayerControl player)
    {
        var crusFlag = PlayerLayer.GetLayers<Crusader>().Any(role => role.CrusadeButton.EffectActive && player == role.CrusadedPlayer && role.HoldsDrive);
        var rebFlag = PlayerLayer.GetLayers<PromotedRebel>().Any(role => player == role.CrusadedPlayer && role.CrusadeButton.EffectActive && role.HoldsDrive);
        return crusFlag || rebFlag;
    }

    public static bool IsProtected(this PlayerControl player) => PlayerLayer.GetLayers<GuardianAngel>().Any(role => (role.ProtectButton.EffectActive || role.GraveProtectButton?.EffectActive
        == true) && player == role.TargetPlayer);

    public static bool IsInfected(this PlayerControl player) => PlayerLayer.GetLayers<Plaguebearer>().Any(role => role.Infected.Contains(player.PlayerId) || player == role.Player);

    public static bool IsFramed(this PlayerControl player) => PlayerLayer.GetLayers<Framer>().Any(role => role.Framed.Contains(player.PlayerId));

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

    public static bool IsFlashed(this PlayerControl player) => !player.HasDied() && (PlayerLayer.GetLayers<Grenadier>().Any(x => x.FlashedPlayers.Contains(player.PlayerId)) ||
        PlayerLayer.GetLayers<PromotedGodfather>().Any(x => x.FlashedPlayers.Contains(player.PlayerId)));

    public static bool SyndicateSided(this PlayerControl player) => player.IsSynTraitor() || player.IsSynFanatic() || player.IsSynAlly() || (player.Is(Faction.Syndicate) &&
        player.Is<Betrayer>()) || player.IsSynDefect() || (player.Is(Faction.Syndicate) && !player.IsBase(Faction.Syndicate));

    public static bool IntruderSided(this PlayerControl player) => player.IsIntTraitor() || player.IsIntAlly() || player.IsIntFanatic() || (player.Is(Faction.Intruder) &&
        player.Is<Betrayer>()) || player.IsIntDefect() || (player.Is(Faction.Intruder) && !player.IsBase(Faction.Intruder));

    public static bool CrewSided(this PlayerControl player) => player.IsCrewAlly() || player.IsCrewDefect() || (player.Is(Faction.Crew) && !player.IsBase(Faction.Crew));

    public static bool Last(PlayerControl player) => (LastImp() && player.Is(Faction.Intruder)) || (LastSyn() && player.Is(Faction.Syndicate));

    public static bool CanKill(this PlayerControl player)
    {
        var role = player.GetRole();
        return role.BaseFaction is Faction.Intruder or Faction.Syndicate || role.Alignment is Alignment.NeutralKill or Alignment.NeutralHarb or Alignment.NeutralApoc or Alignment.CrewKill ||
            player.GetDisposition() is Corrupted or Fanatic or Traitor;
    }

    public static bool IsPostmortal(this PlayerControl player) => player.GetRole() is Revealer or Phantom or Ghoul or Banshee && player.HasDied();

    public static bool Caught(this PlayerControl player)
    {
        if (!player.IsPostmortal())
            return true;

        if (player.TryGetLayer<Phantom>(out var phan))
            return phan.Caught;
        else if (player.TryGetLayer<Revealer>(out var rev))
            return rev.Caught;
        else if (player.TryGetLayer<Ghoul>(out var ghoul))
            return ghoul.Caught;
        else if (player.TryGetLayer<Banshee>(out var ban))
            return ban.Caught;

        return true;
    }

    public static bool IsLinkedTo(this PlayerControl player, PlayerControl refplayer) => player.IsOtherRival(refplayer) || player.IsOtherLover(refplayer) || player.IsOtherLink(refplayer)
        || (player.Is<Mafia>() && refplayer.Is<Mafia>());

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

        if (PlayerLayer.GetLayers<Timekeeper>().Any(x => x.TimeButton.EffectActive))
        {
            if (!player.Is(Faction.Syndicate) || (player.Is(Faction.Syndicate) && PlayerLayer.GetLayers<Timekeeper>().Any(x => x.TimeButton.EffectActive) && !Timekeeper.TimeRewindImmunity))
                result = 0f;
        }

        if (PlayerLayer.GetLayers<PromotedRebel>().Any(x => x.TimeButton.EffectActive))
        {
            if (!player.Is(Faction.Syndicate) || (player.Is(Faction.Syndicate) && PlayerLayer.GetLayers<PromotedRebel>().Any(x => x.TimeButton.EffectActive && x.IsTK) &&
                !Timekeeper.TimeRewindImmunity))
            {
                result = 0f;
            }
        }

        if (Ship() && Ship().Systems.TryGetValue(SystemTypes.LifeSupp, out var life))
        {
            var lifeSuppSystemType = life.Cast<LifeSuppSystemType>();

            if (lifeSuppSystemType.IsActive && BetterSabotages.OxySlow && !player.Data.IsDead)
                result *= Math.Clamp(lifeSuppSystemType.Countdown / lifeSuppSystemType.LifeSuppDuration, 0.25f, 1f);
        }

        if (player.TryGetLayer<Trapper>(out var trap))
            result *= trap.Building ? 0f : 1f;

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
        if (Ship()?.Systems?.TryGetValue(SystemTypes.MushroomMixupSabotage, out var sab) == true)
        {
            var mixup = sab.TryCast<MushroomMixupSabotageSystem>();

            if (mixup && mixup.IsActive)
                return 1f;
        }

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
        else if (playerRole.IsRecruit && playerRole.Alignment != Alignment.NeutralNeo)
            mainflag = Jackal.RecruitVent;
        else if (playerRole.IsResurrected && playerRole.Alignment != Alignment.NeutralNeo)
            mainflag = Necromancer.ResurrectVent;
        else if (playerRole.IsPersuaded && playerRole.Alignment != Alignment.NeutralNeo)
            mainflag = Whisperer.PersuadedVent;
        else if (playerRole.IsBitten && playerRole.Alignment != Alignment.NeutralNeo)
            mainflag = Dracula.UndeadVent;
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
                    mainflag = Werewolf.WerewolfVent == 0 || (ww.CanMaul && (int)Werewolf.WerewolfVent == 1) || (!ww.CanMaul && (int)Werewolf.WerewolfVent == 3);
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
        var playerInfo = player?.Data;
        var disp = player.GetDisposition();

        if (!player || !playerInfo)
            return false;
        else if (playerInfo.IsDead || Meeting() || Lobby())
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

    public static bool IsBlocked(this PlayerControl player) => player.GetLayers().Any(x => x.IsBlocked);

    public static bool SeemsEvil(this PlayerControl player)
    {
        var role = player.GetRole();
        var disp = player.GetDisposition();
        var intruderFlag = role.Faction is Faction.Intruder && disp is not (Traitor or Fanatic) && role is PromotedGodfather;
        var syndicateFlag = role.Faction is Faction.Syndicate && disp is not (Traitor or Fanatic) && role is PromotedRebel && Syndicate.DriveHolder != player;
        var traitorFlag = player.IsTurnedTraitor() && Traitor.TraitorColourSwap;
        var fanaticFlag = player.IsTurnedFanatic() && Fanatic.FanaticColourSwap;
        var nkFlag = role.Alignment is Alignment.NeutralKill && !Sheriff.NeutKillingRed;
        var neFlag = role.Alignment is Alignment.NeutralEvil && !Sheriff.NeutEvilRed;
        var framedFlag = player.IsFramed();
        return intruderFlag || syndicateFlag || traitorFlag || nkFlag || neFlag || framedFlag || fanaticFlag;
    }

    public static bool IsBlockImmune(PlayerControl player) => player.GetRole().RoleBlockImmune;

    public static PlayerControl GetOtherLover(this PlayerControl player) => player.TryGetLayer<Lovers>(out var lovers) ? lovers.OtherLover : null;

    public static PlayerControl GetOtherRival(this PlayerControl player) => player.TryGetLayer<Rivals>(out var rivals) ? rivals.OtherRival : null;

    public static PlayerControl GetOtherLink(this PlayerControl player) => player.TryGetLayer<Linked>(out var linked) ? linked.OtherLink : null;

    public static bool NeutralHasUnfinishedBusiness(PlayerControl player)
    {
        if (player.TryGetLayer<GuardianAngel>(out var ga))
            return ga.TargetAlive;
        else if (player.TryGetLayer<Executioner>(out var exe))
            return exe.TargetVotedOut;
        else if (player.TryGetLayer<Jester>(out var jest))
            return jest.VotedOut;
        else if (player.TryGetLayer<Guesser>(out var guess))
            return guess.TargetGuessed;
        else if (player.TryGetLayer<BountyHunter>(out var bh))
            return bh.TargetKilled;
        else if (player.TryGetLayer<Actor>(out var act))
            return act.Guessed;
        else if (player.TryGetLayer<Troll>(out var troll))
            return troll.Killed;

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

        if (info[0])
        {
            roleName += $"{role.ColorString}{role}</color>";
            objectives += $"\n{role.ColorString}{role.Objectives()}</color>";
            alignment += $"{role.Alignment.AlignmentName(true)}";
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

        if (player.IsRecruit())
            objectives += $"\n<#{CustomColorManager.Cabal.ToHtmlStringRGBA()}>- You are a member of the Cabal. Help {neo.PlayerName} in taking over the mission $</color>";
        else if (player.IsResurrected())
            objectives += $"\n<#{CustomColorManager.Reanimated.ToHtmlStringRGBA()}>- You are a member of the Reanimated. Help {neo.PlayerName} in taking over the mission Σ</color>";
        else if (player.IsPersuaded())
            objectives += $"\n<#{CustomColorManager.Sect.ToHtmlStringRGBA()}>- You are a member of the Sect. Help {neo.PlayerName} in taking over the mission Λ</color>";
        else if (player.IsBitten())
        {
            objectives += $"\n<#{CustomColorManager.Undead.ToHtmlStringRGBA()}>- You are a member of the Undead. Help {neo.PlayerName} in taking over the mission γ</color>";
            abilities += $"\n{role.ColorString}- Attempting to interact with a <#C0C0C0FF>Vampire Hunter</color> will force them to kill you</color>";
        }

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

        if (player.Is(Faction.Syndicate) && role.BaseFaction == Faction.Syndicate && ((Syndicate)role).HoldsDrive)
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
        try
        {
            foreach (var task2 in player.myTasks)
            {
                var task3 = task2?.TryCast<ImportantTextTask>();

                if (task3.Text.Contains("Sabotage and kill everyone") || task3.Text.Contains("Fake Tasks") || task3.Text.Contains("tasks to win"))
                    player.myTasks.Remove(task3);
            }
        } catch {}
    }

    public static void RoleUpdate(this Role newRole, PlayerControl player, Role former = null, bool retainFaction = false)
    {
        former ??= player.GetRole();
        newRole.RoleUpdate(former, player, retainFaction);
    }

    public static void RoleUpdate(this Role newRole, Role former, PlayerControl player = null, bool retainFaction = false)
    {
        player ??= former.Player;
        AllButtons.Where(x => x.Owner == former || !x.Owner.Player).ForEach(x => x.Destroy());
        CustomArrow.AllArrows.Where(x => x.Owner == player).ForEach(x => x.Disable());
        former.End();
        newRole.Start(player);

        if (!retainFaction)
            newRole.Faction = former.Faction;

        newRole.Alignment = newRole.Alignment.GetNewAlignment(newRole.Faction);
        newRole.SubFaction = former.SubFaction;
        newRole.DeathReason = former.DeathReason;
        newRole.KilledBy = former.KilledBy;
        newRole.IsBlocked = former.IsBlocked;
        newRole.Diseased = former.Diseased;
        newRole.AllArrows = former.AllArrows;
        newRole.RoleHistory.Add(former);
        newRole.RoleHistory.AddRange(former.RoleHistory);
        former.RoleHistory.Clear();
        PlayerLayer.AllLayers.Remove(former);

        if (newRole.Local || former.Local)
        {
            ButtonUtils.Reset();
            Flash(newRole.Color);
        }

        if (newRole.Local)
            newRole.UpdateButtons();

        if (CustomPlayer.Local.Is<Seer>())
            Flash(CustomColorManager.Seer);

        if (player.AmOwner)
            Flash(newRole.Color);

        if (player.Data.Role is LayerHandler layerHandler)
            layerHandler.SetUpLayers(newRole);

        ButtonUtils.Reset(player: newRole.Player);
        newRole.Player.RegenTask();
    }

    public static string AlignmentName(this Alignment alignment, bool withColors = false) => alignment switch
    {
        Alignment.CrewSupport => withColors ? "<#8CFFFFFF>Crew (<#1D7CF2FF>Support</color>)</color>" : "Crew (Support)",
        Alignment.CrewInvest => withColors ? "<#8CFFFFFF>Crew (<#1D7CF2FF>Investigative</color>)</color>" : "Crew (Investigative)",
        Alignment.CrewProt => withColors ? "<#8CFFFFFF>Crew (<#1D7CF2FF>Protective</color>)</color>" : "Crew (Protective)",
        Alignment.CrewKill => withColors ? "<#8CFFFFFF>Crew (<#1D7CF2FF>Killing</color>)</color>" : "Crew (Killing)",
        Alignment.CrewUtil => withColors ? "<#8CFFFFFF>Crew (<#1D7CF2FF>Utility</color>)</color>" : "Crew (Utility)",
        Alignment.CrewSov => withColors ? "<#8CFFFFFF>Crew (<#1D7CF2FF>Sovereign</color>)</color>" : "Crew (Sovereign)",
        Alignment.CrewAudit => withColors ? "<#8CFFFFFF>Crew (<#1D7CF2FF>Auditor</color>)</color>" : "Crew (Auditor)",
        Alignment.CrewDecep => withColors ? "<#8CFFFFFF>Crew (<#1D7CF2FF>Deception</color>)</color>" : "Crew (Deception)",
        Alignment.CrewConceal => withColors ? "<#8CFFFFFF>Crew (<#1D7CF2FF>Concealing</color>)</color>" : "Crew (Concealing)",
        Alignment.CrewPower => withColors ? "<#8CFFFFFF>Crew (<#1D7CF2FF>Power</color>)</color>" : "Crew (Power)",
        Alignment.CrewDisrup => withColors ? "<#8CFFFFFF>Crew (<#1D7CF2FF>Disruption</color>)</color>" : "Crew (Disruption)",
        Alignment.CrewHead => withColors ? "<#8CFFFFFF>Crew (<#1D7CF2FF>Head</color>)</color>" : "Crew (Head)",
        Alignment.IntruderSupport => withColors ? "<#FF1919FF>Intruder (<#1D7CF2FF>Support</color>)</color>" : "Intruder (Support)",
        Alignment.IntruderConceal => withColors ? "<#FF1919FF>Intruder (<#1D7CF2FF>Concealing</color>)</color>" : "Intruder (Concealing)",
        Alignment.IntruderDecep => withColors ? "<#FF1919FF>Intruder (<#1D7CF2FF>Deception</color>)</color>" : "Intuder (Deception)",
        Alignment.IntruderKill => withColors ? "<#FF1919FF>Intruder (<#1D7CF2FF>Killing</color>)</color>" : "Intruder (Killing)",
        Alignment.IntruderUtil => withColors ? "<#FF1919FF>Intruder (<#1D7CF2FF>Utility</color>)</color>" : "Intruder (Utility)",
        Alignment.IntruderInvest => withColors ? "<#FF1919FF>Intruder (<#1D7CF2FF>Investigative</color>)</color>" : "Intruder (Investigative)",
        Alignment.IntruderAudit => withColors ? "<#FF1919FF>Intruder (<#1D7CF2FF>Auditor</color>)</color>" : "Intruder (Auditor)",
        Alignment.IntruderProt => withColors ? "<#FF1919FF>Intruder (<#1D7CF2FF>Protective</color>)</color>" : "Intruder (Protective)",
        Alignment.IntruderSov => withColors ? "<#FF1919FF>Intruder (<#1D7CF2FF>Sovereign</color>)</color>" : "Intruder (Sovereign)",
        Alignment.IntruderDisrup=> withColors ? "<#FF1919FF>Intruder (<#1D7CF2FF>Disruption</color>)</color>" : "Intruder (Disruption)",
        Alignment.IntruderPower => withColors ? "<#FF1919FF>Intruder (<#1D7CF2FF>Power</color>)</color>" : "Intruder (Power)",
        Alignment.IntruderHead => withColors ? "<#FF1919FF>Intruder (<#1D7CF2FF>Head</color>)</color>" : "Intruder (Head)",
        Alignment.NeutralKill => withColors ? "<#B3B3B3FF>Neutral (<#1D7CF2FF>Killing</color>)</color>" : "Neutral (Killing)",
        Alignment.NeutralNeo => withColors ? "<#B3B3B3FF>Neutral (<#1D7CF2FF>Neophyte</color>)</color>" : "Neutral (Neophyte)",
        Alignment.NeutralEvil => withColors ? "<#B3B3B3FF>Neutral (<#1D7CF2FF>Evil</color>)</color>" : "Neutral (Evil)",
        Alignment.NeutralBen => withColors ? "<#B3B3B3FF>Neutral (<#1D7CF2FF>Benign</color>)</color>" : "Neutral (Benign)",
        Alignment.NeutralPros => withColors ? "<#B3B3B3FF>Neutral (<#1D7CF2FF>Proselyte</color>)</color>" : "Neutral (Proselyte)",
        Alignment.NeutralSupport => withColors ? "<#B3B3B3FF>Neutral (<#1D7CF2FF>Support</color>)</color>" : "Neutral (Support)",
        Alignment.NeutralInvest => withColors ? "<#B3B3B3FF>Neutral (<#1D7CF2FF>Investigative</color>)</color>" : "Neutral (Investigative)",
        Alignment.NeutralProt => withColors ? "<#B3B3B3FF>Neutral (<#1D7CF2FF>Protective</color>)</color>" : "Neutral (Protective)",
        Alignment.NeutralUtil => withColors ? "<#B3B3B3FF>Neutral (<#1D7CF2FF>Utility</color>)</color>" : "Neutral (Utility)",
        Alignment.NeutralSov => withColors ? "<#B3B3B3FF>Neutral (<#1D7CF2FF>Sovereign</color>)</color>" : "Neutral (Sovereign)",
        Alignment.NeutralAudit => withColors ? "<#B3B3B3FF>Neutral (<#1D7CF2FF>Auditor</color>)</color>" : "Neutral (Auditor)",
        Alignment.NeutralConceal => withColors ? "<#B3B3B3FF>Neutral (<#1D7CF2FF>Concealing</color>)</color>" : "Neutral (Concealing)",
        Alignment.NeutralDecep => withColors ? "<#B3B3B3FF>Neutral (<#1D7CF2FF>Deception</color>)</color>" : "Neutral (Deception)",
        Alignment.NeutralPower => withColors ? "<#B3B3B3FF>Neutral (<#1D7CF2FF>Power</color>)</color>" : "Neutral (Power)",
        Alignment.NeutralDisrup => withColors ? "<#B3B3B3FF>Neutral (<#1D7CF2FF>Disruption</color>)</color>" : "Neutral (Disruption)",
        Alignment.NeutralApoc => withColors ? "<#B3B3B3FF>Neutral (<#1D7CF2FF>Apocalypse</color>)</color>" : "Neutral (Apocalypse)",
        Alignment.NeutralHarb => withColors ? "<#B3B3B3FF>Neutral (<#1D7CF2FF>Harbinger</color>)</color>" : "Neutral (Harbinger)",
        Alignment.NeutralHead => withColors ? "<#B3B3B3FF>Neutral (<#1D7CF2FF>Head</color>)</color>" : "Neutral (Head)",
        Alignment.SyndicateKill => withColors ? "<#008000FF>Syndicate (<#1D7CF2FF>Killing</color>)</color>" : "Syndicate (Killing)",
        Alignment.SyndicateSupport => withColors ? "<#008000FF>Syndicate (<#1D7CF2FF>Support</color>)</color>" : "Syndicate (Support)",
        Alignment.SyndicateDisrup => withColors ? "<#008000FF>Syndicate (<#1D7CF2FF>Disruption</color>)</color>" : "Syndicate (Disruption)",
        Alignment.SyndicatePower => withColors ? "<#008000FF>Syndicate (<#1D7CF2FF>Power</color>)</color>" : "Syndicate (Power)",
        Alignment.SyndicateUtil => withColors ? "<#008000FF>Syndicate (<#1D7CF2FF>Utility</color>)</color>" : "Syndicate (Utility)",
        Alignment.SyndicateInvest => withColors ? "<#008000FF>Syndicate (<#1D7CF2FF>Investigative</color>)</color>" : "Syndicate (Investigative)",
        Alignment.SyndicateAudit => withColors ? "<#008000FF>Syndicate (<#1D7CF2FF>Auditor</color>)</color>" : "Syndicate (Auditor)",
        Alignment.SyndicateSov => withColors ? "<#008000FF>Syndicate (<#1D7CF2FF>Sovereign</color>)</color>" : "Syndicate (Sovereign)",
        Alignment.SyndicateProt => withColors ? "<#008000FF>Syndicate (<#1D7CF2FF>Protective</color>)</color>" : "Syndicate (Protective)",
        Alignment.SyndicateConceal => withColors ? "<#008000FF>Syndicate (<#1D7CF2FF>Concealing</color>)</color>" : "Syndicate (Concealing)",
        Alignment.SyndicateDecep => withColors ? "<#008000FF>Syndicate (<#1D7CF2FF>Deception</color>)</color>" : "Syndicate (Deception)",
        Alignment.SyndicateHead => withColors ? "<#008000FF>Syndicate (<#1D7CF2FF>Head</color>)</color>" : "Syndicate (Head)",
        Alignment.GameModeHideAndSeek => withColors ? "<#A81538FF>Game Mode (<#7500AFFF>Hide And Seek</color>)</color>" : "Game Mode (Hide And Seek)",
        Alignment.GameModeTaskRace => withColors ? "<#A81538FF>Game Mode (<#1E49CFFF>Task Race</color>)</color>" : "Game Mode (Task Race)",
        _ => $"{alignment}"
    };

    public static string GameModeName(this GameMode mode, bool withColors = false) => mode switch
    {
        GameMode.TaskRace => withColors ? "<#1E49CFFF>Task Race</color>" : "Task Race",
        GameMode.HideAndSeek => withColors ? "<#7500AFFF>Hide And Seek</color>" : "Hide And Seek",
        GameMode.Classic => withColors ? "<#C02A2CFF>Classic</color>" : "Classic",
        GameMode.AllAny => withColors ? "<#CBD542FF>All Any</color>" : "All Any",
        GameMode.KillingOnly => withColors ? "<#06E00CFF>Killing Only</color>" : "Killing Only",
        GameMode.Custom => withColors ? "<#E6956AFF>Custom</color>" : "Custom",
        GameMode.Vanilla => "Vanilla",
        GameMode.RoleList => withColors ? "<#FA1C79FF>Role List</color>" : "Role List",
        _ => "Invalid"
    };

    public static Alignment GetNewAlignment(this Alignment alignment, Faction faction) => faction switch
    {
        Faction.Crew => alignment switch
        {
            Alignment.NeutralKill or Alignment.SyndicateKill or Alignment.IntruderKill => Alignment.CrewKill,
            Alignment.IntruderSupport or Alignment.SyndicateSupport => Alignment.CrewSupport,
            Alignment.IntruderConceal => Alignment.CrewConceal,
            Alignment.IntruderDecep => Alignment.CrewDecep,
            Alignment.IntruderUtil or Alignment.SyndicateUtil => Alignment.CrewUtil,
            Alignment.SyndicateDisrup => Alignment.CrewDisrup,
            Alignment.SyndicatePower => Alignment.CrewPower,
            Alignment.IntruderHead => Alignment.CrewHead,
            _ => alignment
        },
        Faction.Intruder => alignment switch
        {
            Alignment.CrewSupport or Alignment.SyndicateSupport => Alignment.IntruderSupport,
            Alignment.CrewInvest => Alignment.IntruderInvest,
            Alignment.CrewProt => Alignment.IntruderProt,
            Alignment.CrewKill or Alignment.SyndicateKill or Alignment.NeutralKill => Alignment.IntruderKill,
            Alignment.CrewUtil or Alignment.SyndicateUtil => Alignment.IntruderUtil,
            Alignment.CrewSov => Alignment.IntruderSov,
            Alignment.CrewAudit => Alignment.IntruderAudit,
            Alignment.SyndicateDisrup => Alignment.IntruderDisrup,
            Alignment.SyndicatePower => Alignment.IntruderPower,
            _ => alignment
        },
        Faction.Syndicate =>  alignment switch
        {
            Alignment.CrewSupport or Alignment.IntruderSupport => Alignment.SyndicateSupport,
            Alignment.CrewInvest => Alignment.SyndicateInvest,
            Alignment.CrewProt => Alignment.SyndicateProt,
            Alignment.CrewKill or Alignment.NeutralKill or Alignment.IntruderKill => Alignment.SyndicateKill,
            Alignment.CrewUtil or Alignment.IntruderUtil => Alignment.SyndicateUtil,
            Alignment.CrewSov => Alignment.SyndicateSov,
            Alignment.CrewAudit => Alignment.SyndicateAudit,
            Alignment.IntruderConceal => Alignment.SyndicateConceal,
            Alignment.IntruderDecep => Alignment.SyndicateDecep,
            Alignment.IntruderHead => Alignment.SyndicateHead,
            _ => alignment
        },
        Faction.Neutral => alignment switch
        {
            Alignment.CrewSupport or Alignment.IntruderSupport or Alignment.SyndicateSupport => Alignment.NeutralSupport,
            Alignment.CrewInvest => Alignment.NeutralInvest,
            Alignment.CrewProt => Alignment.NeutralProt,
            Alignment.CrewKill or Alignment.SyndicateUtil or Alignment.IntruderUtil => Alignment.NeutralKill,
            Alignment.CrewUtil or Alignment.SyndicateKill or Alignment.IntruderKill => Alignment.NeutralUtil,
            Alignment.CrewSov => Alignment.NeutralSov,
            Alignment.CrewAudit => Alignment.NeutralAudit,
            Alignment.IntruderConceal => Alignment.NeutralConceal,
            Alignment.IntruderDecep => Alignment.NeutralDecep,
            Alignment.SyndicateDisrup => Alignment.NeutralDisrup,
            Alignment.SyndicatePower => Alignment.NeutralDisrup,
            Alignment.IntruderHead => Alignment.NeutralHead,
            _ => alignment
        },
        _ => alignment
    };

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

    public static bool IsBombed(this Vent vent)
    {
        var bastflag = PlayerLayer.GetLayers<Bastion>().Any(x => x.BombedIDs.Contains(vent.Id));
        var retflag = PlayerLayer.GetLayers<Retributionist>().Any(x => x.BombedIDs.Contains(vent.Id) && x.IsBast);
        return bastflag || retflag;
    }

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
        var attack = 0;

        foreach (var layer in player.GetLayers())
        {
            if (attack < 2)
                attack += (int)layer.AttackVal;
        }

        if ((player.IsAmbushed() || player.IsCrusaded() || player.GetRole().Bombed) && attack < 1)
            attack = 1;

        if (target && target.Is(SubFaction.Undead) && player.Is<VampireHunter>())
            attack = 3;

        if (target && player.IsLinkedTo(target))
            attack = 0;

        attack = Mathf.Clamp(attack, 0, 3);
        return overrideAtt ?? (AttackEnum)attack;
    }

    public static DefenseEnum GetDefenseValue(this PlayerControl player, PlayerControl source = null, DefenseEnum? overrideDef = null)
    {
        var defense = 0;
        player.GetLayers().ForEach(x => defense += (int)x.DefenseVal);

        if ((player.IsShielded() || player.IsAmbushed() || player.IsCrusaded() | player.IsProtected()) && defense < 2)
            defense = 2;

        if (source && source.Is<VampireHunter>() && player.Is(SubFaction.Undead))
            defense = 3;

        if (source && player.IsLinkedTo(source))
            defense = 3;

        if (PlayerLayer.GetLayers<Glitch>().Any(x => x.HackTarget == player && !x.Player.IsLinkedTo(player)))
            defense = 0;

        if (player.Data.PlayerName == CachedFirstDead)
            defense = 3;

        defense = Mathf.Clamp(defense, 0, 3);
        return overrideDef ?? (DefenseEnum)defense;
    }

    public static IEnumerable<PlayerLayer> GetLayersFromList(this PlayerControl player) => PlayerLayer.AllLayers.Where(x => x.Player == player).OrderBy(x => (int)x.LayerType);

    public static IEnumerable<PlayerLayer> GetLayers(this PlayerControl player)
    {
        if (player.Data.Role is LayerHandler handler)
            return handler.CustomLayers;

        return player.GetLayersFromList();
    }

    public static T GetLayerFromList<T>(this PlayerControl player) where T : PlayerLayer => PlayerLayer.GetLayers<T>().Find(x => x.Player == player);

    public static T GetILayerFromList<T>(this PlayerControl player) where T : IPlayerLayer => PlayerLayer.GetILayers<T>().Find(x => x.Player == player);

    public static T GetLayer<T>(this PlayerControl player) where T : PlayerLayer
    {
        if (!player || !player.Data)
            return null;

        if (player.Data.Role is LayerHandler handler)
            return handler.GetLayer<T>();

        return player.GetLayerFromList<T>();
    }

    public static T GetILayer<T>(this PlayerControl player) where T : IPlayerLayer
    {
        if (!player || !player.Data)
            return default;

        if (player.Data.Role is LayerHandler handler)
            return handler.GetILayer<T>();

        return player.GetILayerFromList<T>();
    }

    public static Role GetRoleFromList(this PlayerControl player) => player.GetLayerFromList<Role>();

    public static Role GetRole(this PlayerControl player)
    {
        if (player.Data.Role is LayerHandler handler)
            return handler.CustomRole;

        return player.GetRoleFromList();
    }

    public static Disposition GetDispositionFromList(this PlayerControl player) => player.GetLayerFromList<Disposition>();

    public static Disposition GetDisposition(this PlayerControl player)
    {
        if (!player || !player.Data)
            return null;

        if (player.Data.Role is LayerHandler handler)
            return handler.CustomDisposition;

        return player.GetDispositionFromList();
    }

    public static Modifier GetModifierFromList(this PlayerControl player) => player.GetLayerFromList<Modifier>();

    public static Modifier GetModifier(this PlayerControl player)
    {
        if (!player || !player.Data)
            return null;

        if (player.Data.Role is LayerHandler handler)
            return handler.CustomModifier;

        return player.GetModifierFromList();
    }

    public static Ability GetAbilityFromList(this PlayerControl player) => player.GetLayerFromList<Ability>();

    public static Ability GetAbility(this PlayerControl player)
    {
        if (!player || !player.Data)
            return null;

        if (player.Data.Role is LayerHandler handler)
            return handler.CustomAbility;

        return player.GetAbilityFromList();
    }

    public static bool TryGetLayer<T>(this PlayerControl player, out T layer) where T : PlayerLayer => layer = player.GetLayer<T>();

    public static bool TryGetILayer<T>(this PlayerControl player, out T iLayer) where T : IPlayerLayer => (iLayer = player.GetILayer<T>()) != null;

    public static void AssignChaosDrive()
    {
        var all = AllPlayers().Where(x => !x.HasDied()).Select(x => x.GetRole()).Where(x => x.Faction == Faction.Syndicate && x.BaseFaction == Faction.Syndicate);

        if (SyndicateSettings.SyndicateCount == 0 || !AmongUsClient.Instance.AmHost || !all.Any())
            return;

        Role chosen = null;

        if (!Syndicate.DriveHolder || Syndicate.DriveHolder.HasDied())
        {
            if (!all.TryFinding(x => x is PromotedRebel, out chosen))
                chosen = all.Find(x => x.Alignment == Alignment.SyndicateDisrup);

            if (!chosen)
                chosen = all.Find(x => x.Alignment == Alignment.SyndicateSupport);

            if (!chosen)
                chosen = all.Find(x => x.Alignment == Alignment.SyndicatePower);

            if (!chosen)
                chosen = all.Find(x => x.Alignment == Alignment.SyndicateKill);

            if (!chosen)
                chosen = all.Find(x => x is Anarchist or Rebel or Sidekick);
        }

        if (chosen)
        {
            Syndicate.DriveHolder = chosen.Player;
            CallRpc(CustomRPC.Misc, MiscRPC.ChaosDrive, chosen);
        }

        Syndicate.SyndicateHasChaosDrive = chosen;
    }

    public static void Convert(byte target, byte convert, SubFaction sub, bool condition)
    {
        var converted = PlayerById(target);
        var converter = PlayerById(convert);
        var converts = converted.Is(SubFaction.None) || (converted.Is(sub) && !converted.Is(Alignment.NeutralNeo));

        if (condition || RoleGenManager.Convertible <= 0 || RoleGenManager.Pure == converted || !converts)
        {
            if (AmongUsClient.Instance.AmHost)
                Interact(converter, converted, true, true);

            return;
        }

        var role1 = converted.GetRole();
        var role2 = converter.GetRole();

        if (role2 is Neophyte neophyte)
        {
            if (converts)
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
            }
            else if (role1 is Neophyte neophyte1 && role1.Type == role2.Type)
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
        Convert(target, convert, sub, condition);
        CallRpc(CustomRPC.Action, ActionsRPC.Convert, convert, target, sub, condition);
    }
}