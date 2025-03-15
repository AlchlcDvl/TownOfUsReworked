namespace TownOfUsReworked.Extensions;

public static class LayerExtensions
{
    private static readonly string RoleColorString = $"<#{CustomColorManager.Role.ToHtmlStringRGBA()}>";
    private static readonly string AlignmentColorString = $"<#{CustomColorManager.Alignment.ToHtmlStringRGBA()}>";
    private static readonly string ObjectivesColorString = $"<#{CustomColorManager.Objectives.ToHtmlStringRGBA()}>";
    private static readonly string AttributesColorString = $"<#{CustomColorManager.Attributes.ToHtmlStringRGBA()}>";
    private static readonly string AbilitiesColorString = $"<#{CustomColorManager.Abilities.ToHtmlStringRGBA()}>";
    private static readonly string DispositionColorString = $"<#{CustomColorManager.Disposition.ToHtmlStringRGBA()}>";
    private static readonly string ModifierColorString = $"<#{CustomColorManager.Modifier.ToHtmlStringRGBA()}>";
    private static readonly string AbilityColorString = $"<#{CustomColorManager.Ability.ToHtmlStringRGBA()}>";
    private static readonly string SubFactionColorString = $"<#{CustomColorManager.SubFaction.ToHtmlStringRGBA()}>";
    private static readonly string AttackColorString = $"<#{CustomColorManager.Attack.ToHtmlStringRGBA()}>";
    private static readonly string DefenseColorString = $"<#{CustomColorManager.Defense.ToHtmlStringRGBA()}>";

    public static bool Is<T>(this PlayerControl player)
        where T : IPlayerLayer => player.TryGetLayer<T>(out _);

    public static bool Is(this PlayerControl player, LayerEnum type) => player.GetLayers().Any(x => x.Type == type);

    // public static bool Is(this Disposition disp, LayerEnum dispositionType) => disp?.Type == dispositionType;

    // public static bool Is(this PlayerControl player, Role role) => player.GetRole().Player == role.Player;

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

        if (role != null)
            return role.Faction;

        if (!player.IsImpostor())
            return Faction.Crew;

        if (GameModifiers.IlluminatiUnleashed)
            return Faction.Illuminati;

        if (GameModifiers.PandoricaOpens)
            return Faction.Pandorica;

        return Faction.Intruder;
    }

    public static SubFaction GetSubFaction(this PlayerControl player)
    {
        if (!player)
            return SubFaction.None;

        var role = player.GetRole();
        return role?.SubFaction ?? SubFaction.None;
    }

    public static Alignment GetAlignment(this PlayerControl player)
    {
        if (!player)
            return Alignment.None;

        var role = player.GetRole();
        return role?.Alignment ?? Alignment.None;
    }

    public static bool IsConverted(this Role role) => role.SubFaction != SubFaction.None && role is not Neophyte;

    public static bool Diseased(this PlayerControl player) => player.GetRole().Diseased;

    private static bool IsCrewDefect(this PlayerControl player) => player.GetRole().IsCrewDefect;

    // private static bool IsIntDefect(this PlayerControl player) => player.GetRole().IsIntDefect;

    // private static bool IsSynDefect(this PlayerControl player) => player.GetRole().IsSynDefect;

    // private static bool IsNeutDefect(this PlayerControl player) => player.GetRole().IsNeutDefect;

    // public static bool IsDefect(this PlayerControl player) => player.IsCrewDefect() || player.IsIntDefect() || player.IsSynDefect() || player.IsNeutDefect();

    public static bool NotOnTheSameSide(this PlayerControl player) => !player.GetRole().Faithful;

    public static bool CanSabotage(this PlayerControl player)
    {
        if (IsHnS() || Meeting() || IsCustomHnS() || IsTaskRace() || !IntruderSettings.IntrudersCanSabotage)
            return false;

        return (player.Is(Faction.Intruder, Faction.Illuminati, Faction.Pandorica) || (player.Is(Faction.Syndicate) && SyndicateSettings.AltImps)) && (!player.Data.IsDead ||
            (IntruderSettings.GhostsCanSabotage && !Role.GetRoles(player.GetFaction()).All(x => x.Dead)));
    }

    public static bool HasAliveLover(this PlayerControl player) => player.TryGetLayer<Lovers>(out var lovers) && lovers.LoversAlive;

    public static bool CanDoTasks(this PlayerControl player)
    {
        if (!player)
            return false;

        if (!player.GetRole())
            return !player.Data.IsImpostor();

        var crewFlag = player.Is(Faction.Crew);
        var neutralFlag = player.Is(Faction.Neutral);
        var factionFlag = player.Is(Faction.Intruder, Faction.Syndicate, Faction.Pandorica, Faction.Illuminati);

        var phantomFlag = player.Is<Phantom>();

        var sideFlag = player.NotOnTheSameSide();
        var taskmasterFlag = player.Is<Taskmaster>();
        var defectFlag = player.IsCrewDefect();

        var gmFlag = player.Is<Runner>() || player.Is<Hunted>();

        var flag1 = crewFlag && !sideFlag;
        var flag2 = neutralFlag && (taskmasterFlag || phantomFlag);
        var flag3 = factionFlag && (taskmasterFlag || defectFlag);
        return flag1 || flag2 || flag3 || gmFlag;
    }

    public static bool IsMoving(this PlayerControl player) => Moving.Contains(player.PlayerId);

    public static bool IsGaTarget(this PlayerControl player) => PlayerLayer.GetLayers<GuardianAngel>(true).Any(x => x.TargetPlayer == player);

    public static bool IsExeTarget(this PlayerControl player) => PlayerLayer.GetLayers<Executioner>(true).Any(x => x.TargetPlayer == player);

    public static bool IsBhTarget(this PlayerControl player) => PlayerLayer.GetLayers<BountyHunter>(true).Any(x => x.TargetPlayer == player);

    public static bool IsGuessTarget(this PlayerControl player) => PlayerLayer.GetLayers<Guesser>(true).Any(x => x.TargetPlayer == player);

    public static Jackal GetJackal(this PlayerControl player) => PlayerLayer.GetLayers<Jackal>().Find(role => role.Members.Contains(player.PlayerId));

    // public static Necromancer GetNecromancer(this PlayerControl player) => PlayerLayer.GetLayers<Necromancer>().Find(role => role.Members.Contains(player.PlayerId));

    // public static Dracula GetDracula(this PlayerControl player) => PlayerLayer.GetLayers<Dracula>().Find(role => role.Members.Contains(player.PlayerId));

    // public static Whisperer GetWhisperer(this PlayerControl player) => PlayerLayer.GetLayers<Whisperer>().Find(role => role.Members.Contains(player.PlayerId));

    public static Neophyte GetNeophyte(this PlayerControl player) => PlayerLayer.GetLayers<Neophyte>().Find(role => role.Members.Contains(player.PlayerId));

    public static bool IsShielded(this PlayerControl player) => PlayerLayer.GetLayers<IShielder>().Any(role => player == role.ShieldedPlayer);

    public static bool IsTrapped(this PlayerControl player) => PlayerLayer.GetLayers<ITrapper>().Any(role => role.Trapped.Contains(player.PlayerId));

    public static bool IsKnighted(this PlayerControl player) => PlayerLayer.GetLayers<Monarch>().Any(role => role.Knighted.Contains(player.PlayerId));

    public static bool IsSpellbound(this PlayerControl player) => PlayerLayer.GetLayers<IHexer>().Any(role => role.Spelled.Contains(player.PlayerId));

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

    // public static bool IsFaithful(this PlayerControl player) => player.GetRole()?.Faithful ?? false;

    public static bool IsBlackmailed(this PlayerControl player) => PlayerLayer.GetLayers<IBlackmailer>().Any(role => role.BlackmailedPlayer == player);

    public static bool IsSilenced(this PlayerControl player) => PlayerLayer.GetLayers<ISilencer>().Any(role => role.SilencedPlayer == player);

    public static bool SilenceActive(this PlayerControl player) => !player.IsSilenced() && PlayerLayer.GetLayers<ISilencer>().Any(role => role.HoldsDrive);

    public static bool IsOnAlert(this PlayerControl player) => PlayerLayer.GetLayers<IAlerter>().Any(role => role.Player == player && role.AlertButton?.EffectActive == true);

    public static bool IsVesting(this PlayerControl player) => PlayerLayer.GetLayers<Survivor>().Any(role => role.VestButton.EffectActive && player == role.Player);

    public static bool IsMarked(this PlayerControl player) => PlayerLayer.GetLayers<Ghoul>().Any(role => player == role.MarkedPlayer);

    public static bool IsCampaigned(this PlayerControl player) => PlayerLayer.GetLayers<Democrat>().Any(role => role.Campaigned.Contains(player.PlayerId));

    public static bool IsAmbushed(this PlayerControl player) => PlayerLayer.GetLayers<IAmbusher>().Any(role => player == role.AmbushedPlayer && role.AmbushButton.EffectActive);

    public static bool IsCrusaded(this PlayerControl player) => PlayerLayer.GetLayers<ICrusader>().Any(role => player == role.CrusadedPlayer && role.CrusadeButton.EffectActive);

    public static bool CrusadeActive(this PlayerControl player) => PlayerLayer.GetLayers<ICrusader>().Any(role => player == role.CrusadedPlayer && role.CrusadeButton.EffectActive &&
        role.HoldsDrive);

    public static bool IsProtected(this PlayerControl player) => PlayerLayer.GetLayers<GuardianAngel>().Any(role => role.Protecting && player == role.TargetPlayer);

    public static bool IsInfected(this PlayerControl player) => PlayerLayer.GetLayers<Plaguebearer>().Any(role => role.Infected.Contains(player.PlayerId) || player == role.Player);

    public static bool IsFramed(this PlayerControl player) => PlayerLayer.GetLayers<IFramer>().Any(role => role.Framed.Contains(player.PlayerId));

    public static bool IsWinningRival(this PlayerControl player) => PlayerLayer.GetLayers<Rivals>().Any(x => x.Player == player && x.IsWinningRival);

    public static bool IsTurnedTraitor(this PlayerControl player) => player.IsIntTraitor() || player.IsSynTraitor();

    public static bool IsTurnedFanatic(this PlayerControl player) => player.IsIntFanatic() || player.IsSynFanatic();

    public static bool IsUnturnedFanatic(this PlayerControl player) => player.GetDisposition() is Fanatic { Side: Faction.Crew };

    // private static bool IsIllFanatic(this PlayerControl player) => player.GetDisposition() is Fanatic { Side: Faction.Illuminati };

    // private static bool IsPandFanatic(this PlayerControl player) => player.GetDisposition() is Fanatic { Side: Faction.Pandorica };

    private static bool IsIntFanatic(this PlayerControl player) => player.GetDisposition() is Fanatic { Side: Faction.Intruder };

    private static bool IsSynFanatic(this PlayerControl player) => player.GetDisposition() is Fanatic { Side: Faction.Syndicate };

    // private static bool IsIllTraitor(this PlayerControl player) => player.GetDisposition() is Traitor { Side: Faction.Illuminati };

    // private static bool IsPandTraitor(this PlayerControl player) => player.GetDisposition() is Traitor { Side: Faction.Pandorica };

    private static bool IsIntTraitor(this PlayerControl player) => player.GetDisposition() is Traitor { Side: Faction.Intruder };

    private static bool IsSynTraitor(this PlayerControl player) => player.GetDisposition() is Traitor { Side: Faction.Intruder };

    // public static bool IsCrewAlly(this PlayerControl player) => player.GetDisposition() is Allied { Side: Faction.Crew };

    // public static bool IsSynAlly(this PlayerControl player) => player.GetDisposition() is Allied { Side: Faction.Syndicate };

    // public static bool IsIntAlly(this PlayerControl player) => player.GetDisposition() is Allied { Side: Faction.Intruder };

    public static bool IsOtherRival(this PlayerControl player, PlayerControl refPlayer) => player.GetOtherRival() == refPlayer;

    public static bool IsOtherLover(this PlayerControl player, PlayerControl refPlayer) => player.GetOtherLover() == refPlayer;

    public static bool IsOtherLink(this PlayerControl player, PlayerControl refPlayer) => player.GetOtherLink() == refPlayer;

    public static bool IsFlashed(this PlayerControl player) => !player.HasDied() && PlayerLayer.GetLayers<IFlasher>().Any(x => x.FlashedPlayers.Contains(player.PlayerId));

    public static bool SyndicateSided(this PlayerControl player) => player.Is(Faction.Syndicate, Faction.Illuminati, Faction.Pandorica) && !player.Is<Syndicate>();

    public static bool IntruderSided(this PlayerControl player) => player.Is(Faction.Intruder, Faction.Illuminati, Faction.Pandorica) && !player.Is<Intruder>();

    public static bool CrewSided(this PlayerControl player) => player.Is(Faction.Crew) && !player.Is<Crew>();

    public static bool Last(PlayerControl player) => Utils.GameStates.Last(player.GetFaction());

    public static bool CanKill(this PlayerControl player)
    {
        var role = player.GetRole();
        return role is Intruder or Syndicate || role?.Alignment is Alignment.Killing or Alignment.Harbinger or Alignment.Apocalypse || player.GetDisposition() is Corrupted or Fanatic or Traitor;
    }

    public static bool IsPostmortal(this PlayerControl player) => player.HasDied() && player.Is<IGhosty>();

    public static bool Caught(this PlayerControl player)
    {
        if (!player.IsPostmortal())
            return true;

        return !player.TryGetLayer<IGhosty>(out var iGhost) || iGhost.Caught;
    }

    public static bool IsLinkedTo(this PlayerControl player, PlayerControl refPlayer) => player.IsOtherRival(refPlayer) || player.IsOtherLover(refPlayer) || player.IsOtherLink(refPlayer) ||
        (player.Is<Mafia>() && refPlayer.Is<Mafia>());

    public static float GetBaseSpeed(this PlayerControl player) => player.HasDied() && (!player.IsPostmortal() || player.Caught()) ? GameSettings.GhostSpeed : GameSettings.PlayerSpeed;

    public static float GetModifiedSpeed(this PlayerControl player)
    {
        if (TransitioningSpeed.TryGetValue(player.PlayerId, out var speed))
            return speed;

        return player.IsMimicking(out var mimicked) ? mimicked.GetSpeed() : player.GetSpeed();
    }

    public static float GetSpeed(this PlayerControl player)
    {
        var result = 1f;

        if (player.HasDied() || Lobby() || (Hud.Instance.IsCamoed && BetterSabotages.CamoHideSpeed && !TransitioningSpeed.ContainsKey(player.PlayerId)))
            return result;

        if (HUD().IsIntroDisplayed)
            return 0f;

        if (player.TryGetLayer<Hunter>(out var hunt))
            return hunt.Starting ? 0f : GameModeSettings.HunterSpeedModifier;

        if (player.Is<Dwarf>())
            result *= Dwarf.DwarfSpeed;
        else if (player.Is<Giant>())
            result *= Giant.GiantSpeed;
        else if (player.TryGetLayer<Drunk>(out var drunk))
            result *= drunk.Modify;

        if (DragHandler.Dragging.ContainsKey(player.PlayerId))
            result *= Janitor.DragModifier;

        if (PlayerLayer.GetLayers<IDrunkard>().Any(x => x.ConfuseButton?.EffectActive == true && (x.HoldsDrive || (x.ConfusedPlayer == player && !x.HoldsDrive))))
            result *= -1;

        if (PlayerLayer.GetLayers<ITimeLord>().Any(x => x.TimeButton.EffectActive))
        {
            if (!player.Is(Faction.Syndicate) || (player.Is(Faction.Syndicate) && !Timekeeper.TimeFreezeImmunity))
                result = 0f;
        }

        if (player.TryGetLayer<ITrapper>(out var trap))
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

        return player.IsMimicking(out var mimicked) ? mimicked.GetSize() : player.GetSize();
    }

    public static float GetSize(this PlayerControl player)
    {
        if (Ship()?.Systems?.TryGetValue(SystemTypes.MushroomMixupSabotage, out var sab) == true && sab.TryCast<MushroomMixupSabotageSystem>(out var mixup) && mixup.IsActive)
            return 1f;

        if (Lobby() || (Hud.Instance.IsCamoed && BetterSabotages.CamoHideSize && !TransitioningSize.ContainsKey(player.PlayerId)))
            return 1f;

        if (player.Is<Dwarf>())
            return Dwarf.DwarfScale;

        if (player.Is<Giant>())
            return Giant.GiantScale;

        return 1f;
    }

    private static bool TryGetShaper(this PlayerControl player, out IShaper shaper) => PlayerLayer.GetLayers<IShaper>().TryFinding(x => player.IsAny(x.ShapeshiftPlayer1, x.ShapeshiftPlayer2),
        out shaper);

    public static bool IsMimicking(this PlayerControl player, out PlayerControl mimicked)
    {
        mimicked = player;

        if (player.HasDied())
            return false;

        if (CachedMorphs.TryGetValue(player.PlayerId, out var mimickedId))
            return mimicked = PlayerById(mimickedId);

        if (mimicked != player)
            return false;

        if (player.TryGetLayer<IMorpher>(out var morph) && morph.MorphedPlayer)
            mimicked = morph.MorphedPlayer;
        else if (player.TryGetShaper(out var ss))
            mimicked = ss.ShapeshiftPlayer1 == player ? ss.ShapeshiftPlayer2 : ss.ShapeshiftPlayer1;

        return mimicked && mimicked != player;
    }

    public static bool CanVent(this PlayerControl player)
    {
        var playerInfo = player?.Data;

        if (!player || !playerInfo)
            return false;

        if (IsHnS())
            return !playerInfo.IsImpostor();

        if (playerInfo.Disconnected || (int)GameModifiers.WhoCanVent is 3 || player.inMovingPlat || player.onLadder || Meeting())
            return false;

        if (player.inVent || GameModifiers.WhoCanVent == WhoCanVentOptions.Everyone)
            return true;

        if (playerInfo.IsDead)
            return player.IsPostmortal() && !player.Caught() && player.inVent;

        var playerRole = player.GetRole();
        var mainflag = false;

        if (!playerRole)
            mainflag = playerInfo.IsImpostor();
        else switch (playerRole)
        {
            case Hunter:
            {
                mainflag = GameModeSettings.HunterVent;
                break;
            }
            case Hunted or Runner:
                break;
            default:
            {
                if (player.Is<Mafia>())
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
                else switch (playerRole)
                {
                    case Syndicate syn:
                    {
                        mainflag = (syn.HoldsDrive && (int)SyndicateSettings.SyndicateVent is 1) || (int)SyndicateSettings.SyndicateVent is 0;
                        break;
                    }
                    case Intruder when IntruderSettings.IntrudersVent:
                    {
                        switch (playerRole)
                        {
                            case Janitor jani:
                            {
                                mainflag = (int)Janitor.JanitorVentOptions is 3 || (jani.CurrentlyDragging && (int)Janitor.JanitorVentOptions is 1) || (!jani.CurrentlyDragging &&
                                    (int)Janitor.JanitorVentOptions is 2);
                                break;
                            }
                            case PromotedGodfather { IsJani: true } gf:
                            {
                                mainflag = (int)Janitor.JanitorVentOptions is 3 || (gf.CurrentlyDragging && (int)Janitor.JanitorVentOptions is 1) || (!gf.CurrentlyDragging &&
                                    (int)Janitor.JanitorVentOptions is 2);
                                break;
                            }
                            case PromotedGodfather { IsMorph: true }:
                            {
                                mainflag = Morphling.MorphlingVent;
                                break;
                            }
                            case PromotedGodfather { IsWraith: true }:
                            {
                                mainflag = Wraith.WraithVent;
                                break;
                            }
                            case PromotedGodfather { IsGren: true }:
                            {
                                mainflag = Grenadier.GrenadierVent;
                                break;
                            }
                            case PromotedGodfather { IsTele: true }:
                            {
                                mainflag = Teleporter.TeleVent;
                                break;
                            }
                            case PromotedGodfather:
                            {
                                mainflag = true;
                                break;
                            }
                            default:
                            {
                                if ((playerRole is not Morphling || Morphling.MorphlingVent) && (playerRole is not Wraith || Wraith.WraithVent) && (playerRole is not Grenadier || Grenadier.GrenadierVent) &&
                                    (playerRole is not Teleporter || Teleporter.TeleVent))
                                {
                                    mainflag = true;
                                }

                                break;
                            }
                        }

                        break;
                    }
                    default:
                    {
                        if (player.Is(Faction.Crew) || playerRole is Crew)
                            mainflag = CrewSettings.CrewVent == CrewVenting.Always || ((CrewSettings.CrewVent == CrewVenting.OnTasksDone || player.Is<Tunneler>()) && playerRole.TasksDone);
                        else if ((playerRole.Faction is Faction.Neutral || playerRole is Neutral) && NeutralSettings.NeutralsVent)
                        {
                            mainflag = playerRole switch
                            {
                                SerialKiller sk => SerialKiller.SkVentOptions == 0 || (sk.BloodlustButton.EffectActive && (int)SerialKiller.SkVentOptions == 1) ||
                                    (!sk.BloodlustButton.EffectActive && (int)SerialKiller.SkVentOptions == 2),
                                Werewolf ww => Werewolf.WerewolfVent == 0 || (ww.CanMaul && (int)Werewolf.WerewolfVent == 1) || (!ww.CanMaul && (int)Werewolf.WerewolfVent == 2),
                                Murderer => Murderer.MurdVent,
                                Glitch => Glitch.GlitchVent,
                                Juggernaut => Juggernaut.JuggVent,
                                Pestilence => Pestilence.PestVent,
                                Jester => Jester.JesterVent,
                                Plaguebearer => Plaguebearer.PbVent,
                                Arsonist => Arsonist.ArsoVent,
                                Executioner => Executioner.ExeVent,
                                Cannibal => Cannibal.CannibalVent,
                                Dracula => Dracula.DracVent,
                                Survivor => Survivor.SurvVent,
                                Actor => Actor.ActorVent,
                                GuardianAngel => GuardianAngel.GaVent,
                                Amnesiac => Amnesiac.AmneVent,
                                Jackal => Jackal.JackalVent,
                                BountyHunter => BountyHunter.BhVent,
                                Betrayer => Betrayer.BetrayerVent,
                                Thief => Thief.ThiefVent,
                                Cryomaniac => Cryomaniac.CryoVent,
                                Necromancer => Necromancer.NecroVent,
                                Whisperer => Whisperer.WhispVent,
                                _ => false
                            };
                        }

                        break;
                    }
                }

                break;
            }
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

        if (playerInfo.IsDead)
            return true;

        return disp switch
        {
            Lovers => Lovers.LoversChat,
            Rivals => Rivals.RivalsChat,
            Linked => Linked.LinkedChat,
            _ => player.Is<Hunted>() && GameModeSettings.HuntedChat
        };
    }

    public static bool IsBlocked(this PlayerControl player) => PlayerLayer.GetLayers<IBlocker>().Any(x => x.BlockTarget == player) || PlayerLayer.GetLayers<Banshee>().Any(x =>
        x.Blocked.Contains(player.PlayerId));

    public static bool SeemsEvil(this PlayerControl player)
    {
        var role = player.GetRole();
        var intruderFlag = role.Faction is Faction.Intruder or Faction.Illuminati or Faction.Pandorica && role is PromotedGodfather;
        var syndicateFlag = role.Faction is Faction.Syndicate or Faction.Illuminati or Faction.Pandorica && role is PromotedRebel && Syndicate.DriveHolder != player;
        var traitorFlag = player.IsTurnedTraitor() && Traitor.TraitorColourSwap;
        var fanaticFlag = player.IsTurnedFanatic() && Fanatic.FanaticColourSwap;
        var nkFlag = role.Alignment is Alignment.Killing && !Sheriff.NeutKillingRed;
        var neFlag = role.Alignment is Alignment.Evil && !Sheriff.NeutEvilRed;
        var framedFlag = player.IsFramed();
        var compFlag = role.Faction == Faction.Compliance;
        return intruderFlag || syndicateFlag || traitorFlag || nkFlag || neFlag || framedFlag || fanaticFlag || compFlag;
    }

    // public static bool IsBlockImmune(PlayerControl player) => player.GetRole().RoleBlockImmune;

    public static PlayerControl GetOtherLover(this PlayerControl player) => player.TryGetLayer<Lovers>(out var lovers) ? lovers.OtherLover : null;

    public static PlayerControl GetOtherRival(this PlayerControl player) => player.TryGetLayer<Rivals>(out var rivals) ? rivals.OtherRival : null;

    private static PlayerControl GetOtherLink(this PlayerControl player) => player.TryGetLayer<Linked>(out var linked) ? linked.OtherLink : null;

    public static bool IsExcludedNeutral(PlayerControl player)
    {
        if (!player.TryGetLayer<Role>(out var role))
            return false;

        if (role is GuardianAngel ga)
            return ga.TargetAlive;

        return role is Evil { HasWon: true };
    }

    public static string RoleCardInfo(this PlayerControl player)
    {
        var info = player.GetLayers().ToList();

        if (info.Count != 4)
            return "";

        var role = (info[0] as Role)!;
        var modifier = (info[1] as Modifier)!;
        var ability = (info[2] as Ability)!;
        var disposition = (info[3] as Disposition)!;

        var objectives = $"{ObjectivesColorString}Goal:";
        var abilities = $"{AbilitiesColorString}Abilities:";
        var attributes = $"{AttributesColorString}Attributes:";
        var roleName = $"{RoleColorString}Role: <b>";
        var dispositionName = $"{DispositionColorString}Disposition: <b>";
        var abilityName = $"{AbilityColorString}Ability: <b>";
        var modifierName = $"{ModifierColorString}Modifier: <b>";
        var alignment = $"{AlignmentColorString}Alignment: <b>";
        var subfaction = $"{SubFactionColorString}Sub-Faction: <b>";
        var attdef = $"{AttackColorString}Attack</color>/{DefenseColorString}Defense</color>: <b>";

        if (role)
        {
            roleName += $"{role.ColorString}{role}</color>";
            objectives += $"\n{role.ColorString}{role.Objectives()}</color>";
            alignment += $"{role.FactionColorString}{role.Faction}({AlignmentColorString}{role.Alignment}</color>)</color>";
            subfaction += $"{role.SubFactionColorString}{role.SubFactionName} {role.SubFactionSymbol}</color>";
            attdef += $"{Join(",", info.Where(x => x.AttackVal > AttackEnum.None).Select(x => x.AttackVal))}</color>/{DefenseColorString}{Join(", ",
                info.Where(x => x.DefenseVal > DefenseEnum.None).Select(x => x.DefenseVal))}</color>";
        }
        else
        {
            roleName += "None";
            alignment += "None";
            subfaction += "None";
            attdef += $"None</color>/{DefenseColorString}None</color>";
        }

        roleName += "</b></color>";
        alignment += "</b></color>";
        subfaction += "</b></color>";
        attdef += "</b>";

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
        objectives += role.SubFaction switch
        {
            SubFaction.None => "",
            _ => $"\n{role.SubFactionColorString}- You are a member of the {role.SubFaction}. Help {player.GetNeophyte().PlayerName} in taking over the mission {role.SubFactionSymbol}</color>"
        };
        objectives += "</color>";
        var desc1 = role.Description();

        if (info[0] && desc1 is not ("" or "- None"))
            abilities += $"\n{role.ColorString}{desc1}</color>";

        if (info[0] && role.RoleBlockImmune)
            abilities += "\n- You are immune to roleblocks";

        var desc2 = ability.Description();

        if (info[2] && !ability.Hidden && ability.Type != LayerEnum.NoneAbility && desc2 is not ("" or "- None"))
            abilities += $"\n{ability.ColorString}{desc2}</color>";

        abilities = abilities == $"{AbilitiesColorString}Abilities:" ? "" : $"\n{abilities}</color>";
        var desc3 = modifier.Description();

        if (info[1] && !modifier.Hidden && modifier.Type != LayerEnum.NoneModifier && desc3 is not ("" or "- None"))
            attributes += $"\n{modifier.ColorString}{desc3}</color>";

        if (player.IsGuessTarget() && Guesser.GuessTargetKnows)
            attributes += "\n<#EEE5BEFF>- Someone wants to assassinate you π</color>";

        if (player.IsExeTarget() && Executioner.ExeTargetKnows)
            attributes += "\n<#CCCCCCFF>- Someone wants you ejected §</color>";

        if (player.IsGaTarget() && GuardianAngel.GaTargetKnows)
            attributes += "\n<#FFFFFFFF>- Someone wants to protect you ★</color>";

        if (player.IsBhTarget())
            attributes += "\n<#B51E39FF>- There is a bounty on your head Θ</color>";

        if (player.Is(Faction.Syndicate) && role is Syndicate { HoldsDrive: true })
            attributes += "\n<#008000FF>- You have the power of the Chaos Drive Δ</color>";

        if (!player.CanDoTasks())
            attributes += "\n<#ABCDEFFF>- Your tasks are fake</color>";

        if (player.Data.IsDead)
            attributes += "\n<#FF1919FF>- You are dead</color>";

        attributes = attributes == $"{AttributesColorString}Attributes:" ? "" : $"\n{attributes}</color>";
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
        var role = player.GetRole();
        var ability = player.GetAbility();
        (name, var result) = role switch
        {
            Mayor => ("Mayor", Mayor.MayorButton),
            Jester => ("Jester", Jester.JesterButton),
            Actor => ("Actor", Actor.ActorButton),
            Executioner => ("Executioner", Executioner.ExecutionerButton),
            Guesser => ("Guesser", Guesser.GuesserButton),
            Dictator => ("Dictator", Dictator.DictatorButton),
            Monarch => ("Monarch", Monarch.MonarchButton),
            Democrat => ("Democrat", Democrat.DemocratButton),
            _ when player.IsKnighted() => ("Knight", Monarch.KnightButton),
            _ when ability is Swapper => ("Swapper", Swapper.SwapperButton),
            _ when ability is Politician => ("Politician", Politician.PoliticianButton),
            _ when IsTaskRace() || IsCustomHnS() => ("GameMode", false),
            _ when player.Is<Shy>() => ("Shy", false),
            _ => ("", player.RemainingEmergencies > 0)
        };
        return result;
    }

    public static bool IsBombed(this Vent vent) => PlayerLayer.GetLayers<IVentBomber>().Any(x => x.BombedIDs.Contains(vent.Id));

    public static PlayerLayerEnum GetLayerType(this LayerEnum layer) => layer switch
    {
        < LayerEnum.NoneRole => PlayerLayerEnum.Role,
        < LayerEnum.NoneModifier => PlayerLayerEnum.Modifier,
        < LayerEnum.NoneDisposition => PlayerLayerEnum.Disposition,
        < LayerEnum.NoneAbility => PlayerLayerEnum.Ability,
        _ => PlayerLayerEnum.None
    };

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

    private static IEnumerable<PlayerLayer> GetLayersFromList(this PlayerControl player) => PlayerLayer.AllLayers.Where(x => x.Player == player).OrderBy(x => (int)x.LayerType);

    public static IEnumerable<PlayerLayer> GetLayers(this PlayerControl player)
    {
        if (player?.Data?.Role is LayerHandler handler)
            return handler.CustomLayers;

        return player.GetLayersFromList();
    }

    private static T GetLayerFromList<T>(this PlayerControl player) where T : IPlayerLayer => PlayerLayer.GetLayers<T>().Find(x => x.Player == player);

    public static T GetLayer<T>(this PlayerControl player) where T : IPlayerLayer
    {
        if (player?.Data?.Role is LayerHandler handler)
            return handler.GetLayer<T>();

        return player.GetLayerFromList<T>();
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

    public static bool TryGetLayer<T>(this PlayerControl player, out T layer) where T : IPlayerLayer => (layer = player.GetLayer<T>()) != null;

    public static void AssignChaosDrive()
    {
        if (SyndicateSettings.SyndicateCount == 0 || !AmongUsClient.Instance.AmHost)
            return;

        var all = PlayerLayer.GetLayers<Syndicate>().Where(x => x.Faction == Faction.Syndicate && x.Alive);

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

        Syndicate.DriveHolder = chosen?.Player;
        Syndicate.SyndicateHasChaosDrive = chosen;
        CallRpc(CustomRPC.Misc, MiscRPC.ChaosDrive, chosen?.Player?.PlayerId ?? 255);
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

        if (Lovers.ConvertLovers && converted.TryGetLayer<Lovers>(out var lovers) && lovers.OtherLover.Is(SubFaction.None))
            ConvertPlayer(lovers.OtherLover.PlayerId, convert, sub, false);
    }

    public static void RpcConvert(byte target, byte convert, SubFaction sub, bool condition = false)
    {
        ConvertPlayer(target, convert, sub, condition);
        CallRpc(CustomRPC.Action, ActionsRPC.Convert, convert, target, sub, condition);
    }
}