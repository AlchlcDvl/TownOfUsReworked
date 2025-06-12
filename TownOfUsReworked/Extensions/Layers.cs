namespace TownOfUsReworked.Extensions;

// TODO: Improve and optimise as much as you can before the next update
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
    private static readonly string AttackColorString = $"<#{CustomColorManager.Attack.ToHtmlStringRGBA()}>";
    private static readonly string DefenseColorString = $"<#{CustomColorManager.Defense.ToHtmlStringRGBA()}>";

    public static bool Is<T>(this PlayerControl player, out T layer) where T : IPlayerLayer => (layer = player.GetLayer<T>()) is not null;

    public static bool Is<T>(this PlayerControl player) where T : IPlayerLayer => player.Is<T>(out _);

    public static bool Is(this PlayerControl player, LayerEnum type) => player.GetLayers().Any(x => x.Type == type);

    public static bool Is(this PlayerControl player, Faction faction)
    {
        var handler = player.GetRole().Handler;
        var part = BadGuysSettings.IlluminatiUnleashed switch
        {
            true => faction switch
            {
                Faction.Intruder => BadGuysSettings.IlluminatiMembers == IlluminatiType.Intruders,
                Faction.Syndicate => BadGuysSettings.IlluminatiMembers == IlluminatiType.Syndicate,
                Faction.Apocalypse => BadGuysSettings.IlluminatiMembers == IlluminatiType.Apocalypse,
                _ => false
            },
            false => faction switch
            {
                Faction.Intruder => BadGuysSettings.PandoricaOpens && BadGuysSettings.PandoricaMembers == PandoricaType.Intruders,
                Faction.Syndicate => BadGuysSettings.PandoricaOpens && BadGuysSettings.PandoricaMembers == PandoricaType.Syndicate,
                Faction.Apocalypse => BadGuysSettings.PandoricaOpens && BadGuysSettings.PandoricaMembers == PandoricaType.Apocalypse,
                _ => false
            }
        };
        return handler.CurrentFaction == faction || handler.FakeFactions.Contains(faction) || part;
    }

    // private static bool Is(this PlayerControl player, params Faction[] factions) => factions.ContainsAny(player.GetFactions());

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

        if (LayerHandler.Handlers.TryGetValue(player.PlayerId, out var handler))
            return handler.CurrentFaction;

        return player.IsImpostor() ? Faction.Intruder : Faction.Crew;
    }

    // public static Faction[] GetFactions(this PlayerControl player)
    // {
    //     if (!player)
    //         return [Faction.None];

    //     if (LayerHandler.Handlers.TryGetValue(player.PlayerId, out var handler))
    //         return [handler.CurrentFaction, .. handler.FakeFactions];

    //     return [player.IsImpostor() ? Faction.Intruder : Faction.Crew];
    // }

    public static Alignment GetAlignment(this PlayerControl player)
    {
        if (!player)
            return Alignment.None;

        var role = player.GetRole();
        return role?.Alignment ?? Alignment.None;
    }

    public static bool CanSabotage(this PlayerControl player)
    {
        if (IsHnS() || Meeting() || IsCustomHnS() || IsTaskRace() || !BadGuysSettings.MainBadGuysCanSabotage)
            return false;

        return player.GetFaction() == BadGuysSettings.MainBadGuys && (!player.Data.IsDead || (BadGuysSettings.GhostsCanSabotage && !Role.GetRoles(player.GetFaction()).All(x => x.Dead)));
    }

    public static bool CanDoTasks(this PlayerControl player)
    {
        if (!player)
            return false;

        if (TaskOptions.AllCanDoTasks)
            return true;

        if (!player.Is<Role>(out var role))
            return !player.Data.IsImpostor();

        var crewFlag = role.Faction == Faction.Crew;
        var outcastFlag = role.Faction == Faction.Outcast;
        var factionFlag = role.Faction.IsAny(Faction.Intruder, Faction.Syndicate, Faction.Pandorica, Faction.Illuminati, Faction.Apocalypse, Faction.Compliance);

        var phantomFlag = role is Phantom;

        var taskmasterFlag = player.Is<Taskmaster>();

        var gmFlag = role is Runner or Hunted;

        var flag1 = outcastFlag && (taskmasterFlag || phantomFlag);
        var flag2 = factionFlag && taskmasterFlag;
        return crewFlag || flag1 || flag2 || gmFlag;
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

    public static bool IsSpellbound(this PlayerControl player) => PlayerLayer.GetLayers<Spellslinger>().Any(role => role.Spelled.Contains(player.PlayerId));

    public static bool IsArsoDoused(this PlayerControl player) => PlayerLayer.GetLayers<Arsonist>().Any(role => role.Doused.Contains(player.PlayerId));

    public static bool IsCryoDoused(this PlayerControl player) => PlayerLayer.GetLayers<Cryomaniac>().Any(role => role.Doused.Contains(player.PlayerId));

    public static bool TryReversingDouses<T>(this PlayerControl player) where T : IDouser
    {
        var result = false;

        foreach (var douser in PlayerLayer.GetLayers<T>())
        {
            if (!douser.Doused.Contains(player.PlayerId))
                continue;

            result = true;
            douser.Doused.Remove(player.PlayerId);
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, douser, DouseActionsRPC.UnDouse, player.PlayerId);
        }

        return result;
    }

    public static bool IsBlackmailed(this PlayerControl player) => PlayerLayer.GetLayers<Blackmailer>().Any(role => role.Target == player);

    public static bool IsSilenced(this PlayerControl player) => PlayerLayer.GetLayers<Silencer>().Any(role => role.Target == player);

    public static bool SilenceActive(this PlayerControl player) => !player.IsSilenced() && PlayerLayer.GetLayers<Silencer>().Any(role => role.HoldsDrive);

    public static bool IsOnAlert(this PlayerControl player) => player.Is<IAlerter>(out var alerter) && alerter.AlertButton?.EffectActive == true;

    public static bool IsVesting(this PlayerControl player) => player.Is<Survivor>(out var surv) && surv.VestButton.EffectActive;

    public static bool IsMarked(this PlayerControl player) => PlayerLayer.GetLayers<Ghoul>().Any(role => player == role.MarkedPlayer);

    public static bool IsCampaigned(this PlayerControl player) => PlayerLayer.GetLayers<Democrat>().Any(role => role.Campaigned.Contains(player.PlayerId));

    public static bool IsAmbushed(this PlayerControl player) => PlayerLayer.GetLayers<Ambusher>().Any(role => player == role.AmbushedPlayer && role.AmbushButton.EffectActive);

    public static bool IsAmbushed(this PlayerControl player, out Ambusher amb) => PlayerLayer.GetLayers<Ambusher>().TryFinding(role => player == role.AmbushedPlayer &&
        role.AmbushButton.EffectActive, out amb);

    public static bool IsCrusaded(this PlayerControl player) => PlayerLayer.GetLayers<Crusader>().Any(role => player == role.CrusadedPlayer && role.CrusadeButton.EffectActive);

    public static bool IsCrusaded(this PlayerControl player, out Crusader crus) => PlayerLayer.GetLayers<Crusader>().TryFinding(role => player == role.CrusadedPlayer &&
        role.CrusadeButton.EffectActive, out crus);

    public static bool CrusadeActive(this PlayerControl player, out Crusader crus) => PlayerLayer.GetLayers<Crusader>().TryFinding(role => player == role.CrusadedPlayer &&
        role is { CrusadeButton.EffectActive: true, HoldsDrive: true }, out crus);

    public static bool IsProtected(this PlayerControl player) => PlayerLayer.GetLayers<GuardianAngel>().Any(role => role.Protecting && player == role.TargetPlayer);

    public static bool IsInfected(this PlayerControl player) => PlayerLayer.GetLayers<Plaguebearer>().Any(role => role.Infected.Contains(player.PlayerId) || player == role.Player);

    public static bool IsFramed(this PlayerControl player) => PlayerLayer.GetLayers<Framer>().Any(role => role.Framed.Contains(player.PlayerId));

    public static bool IsOtherRival(this PlayerControl player, PlayerControl refPlayer) => player.GetOtherRival() == refPlayer;

    public static bool IsOtherLover(this PlayerControl player, PlayerControl refPlayer) => player.GetOtherLover() == refPlayer;

    public static bool IsOtherLink(this PlayerControl player, PlayerControl refPlayer) => player.GetOtherLink() == refPlayer;

    public static bool IsFlashed(this PlayerControl player) => !player.HasDied() && PlayerLayer.GetLayers<Grenadier>().Any(x => x.FlashedPlayers.Contains(player.PlayerId));

    public static bool Last(PlayerControl player) => GameStateUtils.Last(player.GetFaction());

    public static bool CanKill(this PlayerControl player)
    {
        var role = player.GetRole();
        return role is Intruder or Syndicate or Apocalypse || role?.Alignment is Alignment.Killing or Alignment.Neophyte or Alignment.Proselyte || player.GetDisposition() is Corrupted or
            FactionChanger;
    }

    public static bool IsLinkedTo(this PlayerControl player, PlayerControl refPlayer) => (player.Is<Paired>(out var other) && other.Other == refPlayer) || (player.Is<Mafia>() &&
        refPlayer.Is<Mafia>());

    public static float GetFinalSpeed(this PlayerControl player) => player.GetBaseSpeed() * AppearanceHandler.Handlers[player.PlayerId].GetTrueSpeed();

    private static float GetBaseSpeed(this PlayerControl player) => player.HasDied() && (!player.Is<IGhosty>(out var ghost) || ghost.Caught) ? GameOptions.GhostSpeed : GameOptions.PlayerSpeed;

    public static float GetSpeed(this PlayerControl player) => player.GetModifier() switch
    {
        Dwarf => Dwarf.DwarfSpeed,
        Giant => Giant.GiantSpeed,
        _ => 1f
    };

    public static float GetSize(this PlayerControl player) => player.GetModifier() switch
    {
        Dwarf => Dwarf.DwarfScale,
        Giant => Giant.GiantScale,
        _ => 1f
    };

    public static bool CanVent(this PlayerControl player)
    {
        var playerInfo = player?.Data;

        if (!player || !playerInfo || playerInfo.Disconnected || player.inMovingPlat || player.onLadder || Meeting())
            return false;

        if (IsHnS())
            return !playerInfo.IsImpostor();

        if ((int)VentingOptions.WhoCanVent is 3)
            return false;

        if (player.inVent)
            return true;

        if (AllPlayers().Count() == 2 && VentingOptions.FinalTwoDisableVenting)
            return false;

        if (VentingOptions.WhoCanVent == WhoCanVentOptions.Everyone)
            return true;

        if (playerInfo.IsDead)
            return player.Is<IGhosty>(out var ghost) && !ghost.Caught;

        if (!player.Is<Role>(out var playerRole))
            return playerInfo.IsImpostor();

        var miscFactionFlag = false;

        if (playerRole.Alignment != Alignment.Neophyte)
        {
            miscFactionFlag = playerRole.Faction switch
            {
                Faction.Undead => Dracula.UndeadVent,
                Faction.Cabal => Jackal.RecruitVent,
                Faction.Reanimated => Necromancer.ResurrectVent,
                Faction.Cult => Whisperer.PersuadedVent,
                Faction.Followers => Zealot.FollowersVent,
                _ => false
            };
        }

        return player.GetLayers().Any(x => x.CanVent) || miscFactionFlag;
    }

    public static bool CanChat(this PlayerControl player)
    {
        if (Lobby() || Meeting())
            return true;

        var playerInfo = player?.Data;

        if (!player || !playerInfo)
            return false;

        if (playerInfo.IsDead)
            return true;

        return player.GetDisposition() switch
        {
            Lovers => Lovers.LoversChat,
            Rivals => Rivals.RivalsChat,
            Linked => Linked.LinkedChat,
            Mafia => Mafia.MafiaChat,
            _ => player.Is<Hunted>() && Hunted.HuntedChat
        };
    }

    public static bool IsBlocked(this PlayerControl player) => PlayerLayer.GetLayers<IBlocker>().Any(x => x.BlockTarget == player) || PlayerLayer.GetLayers<Banshee>().Any(x =>
        x.Blocked.Contains(player.PlayerId)) || HiddenBlock;

    public static bool SeemsEvil(this PlayerControl player)
    {
        var role = player.GetRole();
        var intruderFlag = role.Faction is Faction.Intruder or Faction.Illuminati or Faction.Pandorica && role is Intruder { IsPromoted: false };
        var syndicateFlag = role.Faction is Faction.Syndicate or Faction.Illuminati or Faction.Pandorica && role is Syndicate { IsPromoted: false, HoldsDrive: false };
        var apocalypseFlag = role.Faction is Faction.Apocalypse or Faction.Illuminati or Faction.Pandorica && role is { Alignment: Alignment.Harbinger };
        var changerFlag = player.GetDisposition() is FactionChanger { SheriffSwap: true, Turned: true };
        var nkFlag = role.Alignment is Alignment.Killing && Sheriff.NeutKillingRed;
        var neFlag = role.Alignment is Alignment.Evil && Sheriff.NeutEvilRed;
        var nnFlag = role.Alignment is Alignment.Neophyte && Sheriff.NeutNeophyteRed;
        var framedFlag = player.IsFramed();
        var compFlag = role.Faction == Faction.Compliance;
        return intruderFlag || syndicateFlag || apocalypseFlag || changerFlag || nkFlag || neFlag || nnFlag || framedFlag || compFlag;
    }

    public static PlayerControl GetOtherLover(this PlayerControl player) => player.Is<Lovers>(out var lovers) ? lovers.Other : null;

    public static PlayerControl GetOtherRival(this PlayerControl player) => player.Is<Rivals>(out var rivals) ? rivals.Other : null;

    private static PlayerControl GetOtherLink(this PlayerControl player) => player.Is<Linked>(out var linked) ? linked.Other : null;

    public static bool IsExcludedOutcast(PlayerControl player)
    {
        if (!player.Is<Role>(out var role))
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
        var attdef = $"{AttackColorString}Attack</color>/{DefenseColorString}Defense</color>: <b>";

        if (role)
        {
            roleName += $"{role.ColorString}{role}</color>";
            objectives += $"\n{role.ColorString}{role.Objectives()}</color>";
            alignment += $"{role.FactionColorString}{role.Faction}({AlignmentColorString}{role.Alignment}</color>)</color>";
            attdef += $"{info.Max(x => x.AttackVal)}</color>/{DefenseColorString}{info.Max(x => x.DefenseVal)}</color>";
        }
        else
        {
            roleName += "None";
            alignment += "None";
            attdef += $"None</color>/{DefenseColorString}None</color>";
        }

        roleName += "</b></color>";
        alignment += "</b></color>";
        attdef += "</b>";

        if (info[3] && !disposition.Hidden && disposition.Type != LayerEnum.NoneDisposition)
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

        if (player.IsBhTarget() && BountyHunter.BhTargetKnows)
            attributes += "\n<#B51E39FF>- There is a bounty on your head Θ</color>";

        if (role is Syndicate { HoldsDrive: true, Faction: Faction.Syndicate or Faction.Pandorica or Faction.Illuminati })
            attributes += "\n<#008000FF>- You have the power of the Chaos Drive Δ</color>";

        if (!player.CanDoTasks())
            attributes += "\n<#ABCDEFFF>- Your tasks are fake</color>";

        if (player.Data.IsDead)
            attributes += "\n<#FF1919FF>- You are dead</color>";

        attributes = attributes == $"{AttributesColorString}Attributes:" ? "" : $"\n{attributes}</color>";
        return $"{roleName}\n{attdef}\n{alignment}\n{dispositionName}\n{abilityName}\n{modifierName}\n{objectives}{abilities}{attributes}";
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
        LayerEnum.None => PlayerLayerEnum.None,
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
        player.GetLayers().Do(x => defense += (int)x.DefenseVal);

        if ((player.IsShielded() || player.IsAmbushed() || player.IsCrusaded() || player.IsProtected()) && defense < 2)
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

    public static IEnumerable<PlayerLayer> GetLayers(this PlayerControl player) => LayerHandler.Handlers.TryGetValue(player.PlayerId, out var handler)
        ? handler.CurrentLayers : player.GetLayersFromList();

    private static T GetLayerFromList<T>(this PlayerControl player) where T : IPlayerLayer => PlayerLayer.GetLayers<T>().Find(x => x.Player == player);

    public static T GetLayer<T>(this PlayerControl player) where T : IPlayerLayer => LayerHandler.Handlers.TryGetValue(player.PlayerId, out var handler)
        ? handler.GetLayer<T>() : player.GetLayerFromList<T>();

    public static Role GetRoleFromList(this PlayerControl player) => player.GetLayerFromList<Role>();

    public static Role GetRole(this PlayerControl player) => LayerHandler.Handlers.TryGetValue(player.PlayerId, out var handler)
        ? handler.CurrentRole : player.GetRoleFromList();

    public static Disposition GetDispositionFromList(this PlayerControl player) => player.GetLayerFromList<Disposition>();

    public static Disposition GetDisposition(this PlayerControl player) => LayerHandler.Handlers.TryGetValue(player.PlayerId, out var handler)
        ? handler.CurrentDisposition : player.GetDispositionFromList();

    public static Modifier GetModifierFromList(this PlayerControl player) => player.GetLayerFromList<Modifier>();

    public static Modifier GetModifier(this PlayerControl player) => LayerHandler.Handlers.TryGetValue(player.PlayerId, out var handler)
        ? handler.CurrentModifier : player.GetModifierFromList();

    public static Ability GetAbilityFromList(this PlayerControl player) => player.GetLayerFromList<Ability>();

    public static Ability GetAbility(this PlayerControl player) => LayerHandler.Handlers.TryGetValue(player.PlayerId, out var handler)
        ? handler.CurrentAbility : player.GetAbilityFromList();

    public static void AssignChaosDrive()
    {
        if (SyndicateSettings.SyndicateCount == 0 || !AmongUsClient.Instance.AmHost)
            return;

        var all = PlayerLayer.GetLayers<Syndicate>().Where(x => x is { Faction: Faction.Syndicate or Faction.Pandorica or Faction.Illuminati, Alive: true });

        if (!all.Any())
            return;

        Syndicate chosen = null;

        if (Syndicate.DriveHolder.HasDied())
        {
            if (!all.TryFinding(x => x.IsPromoted, out chosen))
                chosen = all.Find(x => x is { Alignment: Alignment.Disruption, IsUnderling: false });

            if (!chosen)
                chosen = all.Find(x => x is { Alignment: Alignment.Support, IsUnderling: false });

            if (!chosen)
                chosen = all.Find(x => x is { Alignment: Alignment.Head, IsUnderling: false });

            if (!chosen)
                chosen = all.Find(x => x is { Alignment: Alignment.Killing, IsUnderling: false });

            if (!chosen)
                chosen = all.Find(x => x is Anarchist or Rebel || x.IsUnderling);
        }

        Syndicate.DriveHolder = chosen?.Player;
        Syndicate.SyndicateHasChaosDrive = chosen;
        CallRpc(CustomRPC.Misc, MiscRPC.ChaosDrive, chosen?.PlayerId ?? 255);
    }

    public static void ConvertPlayer(byte target, byte convert, bool skip)
    {
        var converted = PlayerById(target);
        var converter = PlayerById(convert);
        var role1 = converted.GetRole();
        var role2 = converter.GetRole();
        var sub = role2.Faction;
        var convertible = role1.Faction.IsConvertible();
        var converts = convertible || role1.Faction == sub;
        var comp = BadGuysSettings.OrderOfCompliance && BadGuysSettings.ComplianceMembers == ComplianceType.Neophytes;

        if (skip || RoleGenManager.Convertible <= 0 || RoleGenManager.Pure == converted || !converts || (comp && convertible))
        {
            if (AmongUsClient.Instance.AmHost)
                Interact(converter, converted, true, true);

            return;
        }

        if (role2 is Neophyte neophyte)
        {
            neophyte.Members.Add(target);

            if (convertible && neophyte is Jackal jackal)
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
                    whisperer1.Members.Do(x => whisperer2.PlayerConversion.Remove(x));
                    whisperer2.Members.Do(x => whisperer1.PlayerConversion.Remove(x));
                }
            }
        }

        role1.Handler.CurrentFaction = comp ? Faction.Compliance : sub;
        RoleGenManager.Convertible--;

        if (converted.AmOwner)
            Flash(role1.FactionColor);
        else if (LocalPlayer.Is<Mystic>())
            Flash(CustomColorManager.Mystic);

        if (Lovers.ConvertLovers && converted.Is<Lovers>(out var lovers) && lovers.Other.GetFaction().IsConvertible())
            ConvertPlayer(lovers.Other.PlayerId, convert, false);
    }

    public static void RpcConvert(byte target, byte convert, bool condition = false)
    {
        ConvertPlayer(target, convert, condition);
        CallRpc(CustomRPC.Action, ActionsRPC.Convert, convert, target, condition);
    }

    public static bool IsConvertible(this Faction originalFaction) => originalFaction is < Faction.Outcast or Faction.Pandorica or (> Faction.GameMode and < Faction.Mafia);

    public static bool IsFactionedEvil(this Faction faction, bool isKnown = false) => faction is (> Faction.Crew and < Faction.GameMode and not Faction.Outcast) or > Faction.Mafia || (faction is
        > Faction.GameMode and < Faction.Cabal && !OutcastKillingSettings.WinSolo && (!isKnown || OutcastKillingSettings.KnowEachOther));

    public static bool IsOk(this Faction faction) => faction is > Faction.GameMode and < Faction.Cabal;

    public static bool IsBuddyWith(this PlayerControl player, PlayerControl refPlayer, Faction faction) => (refPlayer.Is(faction) && faction.IsFactionedEvil(true)) || player.IsLinkedTo(refPlayer);
}