namespace TownOfUsReworked.Extensions;

public static class LayerExtentions
{
    public static string RoleColorString => $"<color=#{CustomColorManager.Role.ToHtmlStringRGBA()}>";
    public static string AlignmentColorString => $"<color=#{CustomColorManager.Alignment.ToHtmlStringRGBA()}>";
    public static string ObjectivesColorString => $"<color=#{CustomColorManager.Objectives.ToHtmlStringRGBA()}>";
    public static string AttributesColorString => $"<color=#{CustomColorManager.Attributes.ToHtmlStringRGBA()}>";
    public static string AbilitiesColorString => $"<color=#{CustomColorManager.Abilities.ToHtmlStringRGBA()}>";
    public static string ObjectifierColorString => $"<color=#{CustomColorManager.Objectifier.ToHtmlStringRGBA()}>";
    public static string ModifierColorString => $"<color=#{CustomColorManager.Modifier.ToHtmlStringRGBA()}>";
    public static string AbilityColorString => $"<color=#{CustomColorManager.Ability.ToHtmlStringRGBA()}>";
    public static string SubFactionColorString => $"<color=#{CustomColorManager.SubFaction.ToHtmlStringRGBA()}>";
    public static string AttackColorString => $"<color=#{CustomColorManager.Attack.ToHtmlStringRGBA()}>";
    public static string DefenseColorString => $"<color=#{CustomColorManager.Defense.ToHtmlStringRGBA()}>";

    public static bool Is(this PlayerControl player, LayerEnum type) => player.GetLayers().Any(x => x.Type == type);

    public static bool Is(this Role role, LayerEnum roleType) => role?.Type == roleType;

    public static bool Is(this Objectifier obj, LayerEnum objectifiertype) => obj?.Type == objectifiertype;

    public static bool Is(this PlayerControl player, Role role) => player.GetRole().Player == role.Player;

    public static bool Is(this PlayerControl player, SubFaction subFaction) => player.GetRole()?.SubFaction == subFaction;

    public static bool Is(this PlayerControl player, Faction faction) => player.GetFaction() == faction;

    public static bool IsBase(this PlayerControl player, Faction faction) => player.GetRole()?.BaseFaction == faction;

    public static bool IsBase(this PlayerVoteArea player, Faction faction) => PlayerByVoteArea(player).IsBase(faction);

    public static bool Is(this PlayerControl player, Alignment alignment) => player.GetRole()?.Alignment == alignment;

    public static bool Is(this PlayerVoteArea player, LayerEnum roleType) => PlayerByVoteArea(player).Is(roleType);

    public static bool Is(this PlayerVoteArea player, SubFaction subFaction) => PlayerByVoteArea(player).Is(subFaction);

    public static bool Is(this PlayerVoteArea player, Faction faction) => PlayerByVoteArea(player).Is(faction);

    public static bool IsAssassin(this PlayerControl player) => player.GetAbility() is NeutralAssassin or IntruderAssassin or SyndicateAssassin or IntruderAssassin;

    public static Faction GetFaction(this PlayerControl player)
    {
        if (player == null)
            return Faction.None;

        var role = player.GetRole();

        if (!role)
            return player.Data.IsImpostor() ? Faction.Intruder : Faction.Crew;

        return role.Faction;
    }

    public static SubFaction GetSubFaction(this PlayerControl player)
    {
        if (player == null)
            return SubFaction.None;

        var role = player.GetRole();

        if (!role)
            return SubFaction.None;

        return role.SubFaction;
    }

    public static Alignment GetAlignment(this PlayerControl player)
    {
        if (player == null)
            return Alignment.None;

        var role = player.GetRole();

        if (!role)
            return Alignment.None;

        return role.Alignment;
    }

    public static Alignment GetAlignment(this PlayerVoteArea player) => PlayerByVoteArea(player).GetAlignment();

    public static Faction GetFaction(this PlayerVoteArea player) => PlayerByVoteArea(player).GetFaction();

    public static SubFaction GetSubFaction(this PlayerVoteArea player) => PlayerByVoteArea(player).GetSubFaction();

    public static bool IsRecruit(this PlayerControl player) => player.GetRole().IsRecruit;

    public static bool IsBitten(this PlayerControl player) => player.GetRole().IsBitten;

    public static bool IsPersuaded(this PlayerControl player) => player.GetRole().IsPersuaded;

    public static bool IsResurrected(this PlayerControl player) => player.GetRole().IsResurrected;

    public static bool Diseased(this PlayerControl player) => player.GetRole().Diseased;

    public static bool IsCrewDefect(this PlayerControl player) => player.GetRole().IsCrewDefect;

    public static bool IsIntDefect(this PlayerControl player) => player.GetRole().IsIntDefect;

    public static bool IsSynDefect(this PlayerControl player) => player.GetRole().IsSynDefect;

    public static bool IsNeutDefect(this PlayerControl player) => player.GetRole().IsNeutDefect;

    public static bool IsDefect(this PlayerControl player) => player.IsCrewDefect() || player.IsIntDefect() || player.IsSynDefect() || player.IsNeutDefect();

    public static bool IsRecruit(this PlayerVoteArea player) => PlayerByVoteArea(player).IsRecruit();

    public static bool IsBitten(this PlayerVoteArea player) => PlayerByVoteArea(player).IsBitten();

    public static bool IsPersuaded(this PlayerVoteArea player) => PlayerByVoteArea(player).IsPersuaded();

    public static bool IsResurrected(this PlayerVoteArea player) => PlayerByVoteArea(player).IsResurrected();

    public static bool NotOnTheSameSide(this PlayerControl player) => !player.GetRole().Faithful;

    public static bool CanSabotage(this PlayerControl player)
    {
        if (IsHnS)
            return false;

        var result = (player.Is(Faction.Intruder) || (player.Is(Faction.Syndicate) && CustomGameOptions.AltImps)) && !Meeting && CustomGameOptions.IntrudersCanSabotage;

        if (!player.Data.IsDead)
            return result;
        else
            return result && CustomGameOptions.GhostsCanSabotage && !Role.GetRoles(player.GetFaction()).All(x => x.Dead);
    }

    public static bool HasAliveLover(this PlayerControl player) => PlayerLayer.GetLayers<Lovers>().Any(x => x.Player == player && x.LoversAlive);

    public static bool CanDoTasks(this PlayerControl player)
    {
        if (player == null)
            return false;

        if (!player.GetRole())
            return !player.Data.IsImpostor();

        var crewflag = player.Is(Faction.Crew);
        var neutralflag = player.Is(Faction.Neutral);
        var intruderflag = player.Is(Faction.Intruder);
        var syndicateflag = player.Is(Faction.Syndicate);

        var phantomflag = player.Is(LayerEnum.Phantom);

        var sideflag = player.NotOnTheSameSide();
        var taskmasterflag = player.Is(LayerEnum.Taskmaster);
        var defectflag = player.IsCrewDefect();

        var gmflag = player.Is(LayerEnum.Runner) || player.Is(LayerEnum.Hunted);

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

        if (player.TryGetLayer<Executioner>(LayerEnum.Executioner, out var exe))
            return exe.TargetPlayer;
        else if (player.TryGetLayer<GuardianAngel>(LayerEnum.GuardianAngel, out var ga))
            return ga.TargetPlayer;
        else if (player.TryGetLayer<Guesser>(LayerEnum.Guesser, out var guesser))
            return guesser.TargetPlayer;
        else if (player.TryGetLayer<BountyHunter>(LayerEnum.BountyHunter, out var bh))
            return bh.TargetPlayer;

        return null;
    }

    public static bool IsGATarget(this PlayerVoteArea player) => PlayerByVoteArea(player).IsGATarget();

    public static bool IsExeTarget(this PlayerVoteArea player) => PlayerByVoteArea(player).IsExeTarget();

    public static bool IsBHTarget(this PlayerVoteArea player) => PlayerByVoteArea(player).IsBHTarget();

    public static bool IsGuessTarget(this PlayerVoteArea player) => PlayerByVoteArea(player).IsGuessTarget();

    public static bool CanDoTasks(this PlayerVoteArea player) => PlayerByVoteArea(player).CanDoTasks();

    public static Jackal GetJackal(this PlayerControl player) => PlayerLayer.GetLayers<Jackal>().Find(role => role.Recruited.Contains(player.PlayerId));

    public static Necromancer GetNecromancer(this PlayerControl player) => PlayerLayer.GetLayers<Necromancer>().Find(role => role.Resurrected.Contains(player.PlayerId));

    public static Dracula GetDracula(this PlayerControl player) => PlayerLayer.GetLayers<Dracula>().Find(role => role.Converted.Contains(player.PlayerId));

    public static Whisperer GetWhisperer(this PlayerControl player) => PlayerLayer.GetLayers<Whisperer>().Find(role => role.Persuaded.Contains(player.PlayerId));

    public static Jackal GetJackal(this PlayerVoteArea player) => PlayerByVoteArea(player).GetJackal();

    public static Necromancer GetNecromancer(this PlayerVoteArea player) => PlayerByVoteArea(player).GetNecromancer();

    public static Dracula GetDracula(this PlayerVoteArea player) => PlayerByVoteArea(player).GetDracula();

    public static Whisperer GetWhisperer(this PlayerVoteArea player) => PlayerByVoteArea(player).GetWhisperer();

    public static bool IsShielded(this PlayerControl player)
    {
        var medicFlag = PlayerLayer.GetLayers<Medic>().Any(role => player == role.ShieldedPlayer);
        var retFlag = PlayerLayer.GetLayers<Retributionist>().Any(role => player == role.ShieldedPlayer);
        return medicFlag || retFlag;
    }

    public static bool IsTrapped(this PlayerControl player)
    {
        var trapFlag = PlayerLayer.GetLayers<Trapper>().Any(role => role.Trapped.Contains(player.PlayerId));
        var retFlag = PlayerLayer.GetLayers<Retributionist>().Any(role => role.Trapped.Contains(player.PlayerId));
        return trapFlag || retFlag;
    }

    public static bool IsKnighted(this PlayerControl player) => PlayerLayer.GetLayers<Monarch>().Any(role => role.Knighted.Contains(player.PlayerId));

    public static bool IsSpellbound(this PlayerControl player) => PlayerLayer.GetLayers<Spellslinger>().Any(role => role.Spelled.Contains(player.PlayerId));

    public static bool IsArsoDoused(this PlayerControl player) => PlayerLayer.GetLayers<Arsonist>().Any(role => role.Doused.Contains(player.PlayerId));

    public static bool IsCryoDoused(this PlayerControl player) => PlayerLayer.GetLayers<Cryomaniac>().Any(role => role.Doused.Contains(player.PlayerId));

    public static bool IsProtectedMonarch(this PlayerControl player) => PlayerLayer.GetLayers<Monarch>().Any(role => role.Protected && role.Player == player);

    public static bool IsFaithful(this PlayerControl player) => player.GetRole().Faithful;

    public static bool IsBlackmailed(this PlayerControl player)
    {
        var bmFlag = PlayerLayer.GetLayers<Blackmailer>().Any(role => role.BlackmailedPlayer == player);
        var gfFlag = PlayerLayer.GetLayers<PromotedGodfather>().Any(role => role.BlackmailedPlayer == player);
        return bmFlag || gfFlag;
    }

    public static bool IsSilenced(this PlayerControl player)
    {
        var silFlag = PlayerLayer.GetLayers<Silencer>().Any(role => role.SilencedPlayer == player);
        var rebFlag = PlayerLayer.GetLayers<PromotedRebel>().Any(role => role.SilencedPlayer == player);
        return silFlag || rebFlag;
    }

    public static bool SilenceActive(this PlayerControl player)
    {
        var silenced = !player.IsSilenced();
        var silFlag = PlayerLayer.GetLayers<Silencer>().Any(role => role.HoldsDrive);
        var rebFlag = PlayerLayer.GetLayers<PromotedRebel>().Any(role => role.HoldsDrive && role.IsSil);
        return silenced && (silFlag || rebFlag);
    }

    public static bool IsShielded(this PlayerVoteArea player) => PlayerByVoteArea(player).IsShielded();

    public static bool IsTrapped(this PlayerVoteArea player) => PlayerByVoteArea(player).IsTrapped();

    public static bool IsKnighted(this PlayerVoteArea player) => PlayerByVoteArea(player).IsKnighted();

    public static bool IsSpellbound(this PlayerVoteArea player) => PlayerByVoteArea(player).IsSpellbound();

    public static bool IsArsoDoused(this PlayerVoteArea player) => PlayerByVoteArea(player).IsArsoDoused();

    public static bool IsCryoDoused(this PlayerVoteArea player) => PlayerByVoteArea(player).IsCryoDoused();

    public static bool IsOnAlert(this PlayerControl player)
    {
        var vetFlag = PlayerLayer.GetLayers<Veteran>().Any(role => role.AlertButton.EffectActive && player == role.Player);
        var retFlag = PlayerLayer.GetLayers<Retributionist>().Any(role => role.AlertButton.EffectActive && role.IsVet && player == role.Player);
        return vetFlag || retFlag;
    }

    public static bool IsVesting(this PlayerControl player) => PlayerLayer.GetLayers<Survivor>().Any(role => role.VestButton.EffectActive && player == role.Player);

    public static bool NotTransformed(this PlayerControl player) => PlayerLayer.GetLayers<Werewolf>().Any(role => !role.CanMaul && player == role.Player);

    public static bool IsMarked(this PlayerControl player) => PlayerLayer.GetLayers<Ghoul>().Any(role => player == role.MarkedPlayer);

    public static bool IsAmbushed(this PlayerControl player)
    {
        var ambFlag = PlayerLayer.GetLayers<Ambusher>().Any(role => role.AmbushButton.EffectActive && player == role.AmbushedPlayer);
        var gfFlag = PlayerLayer.GetLayers<PromotedGodfather>().Any(role => role.AmbushButton.EffectActive && player == role.AmbushedPlayer && role.IsAmb);
        return ambFlag || gfFlag;
    }

    public static bool IsCrusaded(this PlayerControl player)
    {
        var crusFlag = PlayerLayer.GetLayers<Crusader>().Any(role => role.CrusadeButton.EffectActive && player == role.CrusadedPlayer);
        var rebFlag = PlayerLayer.GetLayers<PromotedRebel>().Any(role => role.CrusadeButton.EffectActive && player == role.CrusadedPlayer && role.IsCrus);
        return crusFlag || rebFlag;
    }

    public static bool CrusadeActive(this PlayerControl player)
    {
        var crusFlag = PlayerLayer.GetLayers<Crusader>().Any(role => role.CrusadeButton.EffectActive && player == role.CrusadedPlayer && role.HoldsDrive);
        var rebFlag = PlayerLayer.GetLayers<PromotedRebel>().Any(role => role.CrusadeButton.EffectActive && player == role.CrusadedPlayer && role.IsCrus && role.HoldsDrive);
        return crusFlag || rebFlag;
    }

    public static bool IsProtected(this PlayerControl player) => PlayerLayer.GetLayers<GuardianAngel>().Any(role => (role.ProtectButton.EffectActive || role.GraveProtectButton.EffectActive)
        && player == role.TargetPlayer);

    public static bool IsInfected(this PlayerControl player) => PlayerLayer.GetLayers<Plaguebearer>().Any(role => role.Infected.Contains(player.PlayerId) || player == role.Player);

    public static bool IsFramed(this PlayerControl player) => PlayerLayer.GetLayers<Framer>().Any(role => role.Framed.Contains(player.PlayerId));

    public static bool IsInfected(this PlayerVoteArea player) => PlayerByVoteArea(player).IsInfected();

    public static bool IsFramed(this PlayerVoteArea player) => PlayerByVoteArea(player).IsFramed();

    public static bool IsMarked(this PlayerVoteArea player) => PlayerByVoteArea(player).IsMarked();

    public static bool IsWinningRival(this PlayerControl player) => PlayerLayer.GetLayers<Rivals>().Any(x => x.Player == player && x.IsWinningRival);

    public static bool IsTurnedTraitor(this PlayerControl player) => player.IsIntTraitor() || player.IsSynTraitor();

    public static bool IsTurnedFanatic(this PlayerControl player) => player.IsIntFanatic() || player.IsSynFanatic();

    public static bool IsTurnedTraitor(this PlayerVoteArea player) => PlayerByVoteArea(player).IsTurnedTraitor();

    public static bool IsTurnedFanatic(this PlayerVoteArea player) => PlayerByVoteArea(player).IsTurnedFanatic();

    public static bool IsUnturnedFanatic(this PlayerControl player) => !player.IsIntFanatic() && !player.IsSynFanatic();

    public static bool IsIntFanatic(this PlayerControl player) => player.GetRole().IsIntFanatic;

    public static bool IsSynFanatic(this PlayerControl player) => player.GetRole().IsSynFanatic;

    public static bool IsIntTraitor(this PlayerControl player) => player.GetRole().IsIntTraitor;

    public static bool IsSynTraitor(this PlayerControl player) => player.GetRole().IsSynTraitor;

    public static bool IsCrewAlly(this PlayerControl player) => player.GetRole().IsCrewAlly;

    public static bool IsSynAlly(this PlayerControl player) => player.GetRole().IsSynAlly;

    public static bool IsIntAlly(this PlayerControl player) => player.GetRole().IsIntAlly;

    public static bool IsIntFanatic(this PlayerVoteArea player) => PlayerByVoteArea(player).IsIntFanatic();

    public static bool IsSynFanatic(this PlayerVoteArea player) => PlayerByVoteArea(player).IsSynFanatic();

    public static bool IsIntTraitor(this PlayerVoteArea player) => PlayerByVoteArea(player).IsIntTraitor();

    public static bool IsSynTraitor(this PlayerVoteArea player) => PlayerByVoteArea(player).IsSynTraitor();

    public static bool IsCrewAlly(this PlayerVoteArea player) => PlayerByVoteArea(player).IsCrewAlly();

    public static bool IsSynAlly(this PlayerVoteArea player) => PlayerByVoteArea(player).IsSynAlly();

    public static bool IsIntAlly(this PlayerVoteArea player) => PlayerByVoteArea(player).IsIntAlly();

    public static bool IsOtherRival(this PlayerControl player, PlayerControl refPlayer) => player.GetOtherRival() == refPlayer;

    public static bool IsOtherLover(this PlayerControl player, PlayerControl refPlayer) => player.GetOtherLover() == refPlayer;

    public static bool IsOtherLink(this PlayerControl player, PlayerControl refPlayer) => player.GetOtherLink() == refPlayer;

    public static bool IsFlashed(this PlayerControl player) => !player.HasDied() && (PlayerLayer.GetLayers<Grenadier>().Any(x => x.FlashedPlayers.Contains(player.PlayerId)) ||
        PlayerLayer.GetLayers<PromotedGodfather>().Any(x => x.IsGren && x.FlashedPlayers.Contains(player.PlayerId)));

    public static bool SyndicateSided(this PlayerControl player) => player.IsSynTraitor() || player.IsSynFanatic() || player.IsSynAlly() || (player.Is(Faction.Syndicate) &&
        player.Is(LayerEnum.Betrayer)) || player.IsSynDefect() || (player.Is(Faction.Syndicate) && !player.IsBase(Faction.Syndicate));

    public static bool IntruderSided(this PlayerControl player) => player.IsIntTraitor() || player.IsIntAlly() || player.IsIntFanatic() || (player.Is(Faction.Intruder) &&
        player.Is(LayerEnum.Betrayer)) || player.IsIntDefect() || (player.Is(Faction.Intruder) && !player.IsBase(Faction.Intruder));

    public static bool CrewSided(this PlayerControl player) => player.IsCrewAlly() || player.IsCrewDefect() || (player.Is(Faction.Crew) && !player.IsBase(Faction.Crew));

    public static bool SyndicateSided(this PlayerVoteArea player) => player.IsSynTraitor() || player.IsSynFanatic() || player.IsSynAlly();

    public static bool IntruderSided(this PlayerVoteArea player) => player.IsIntTraitor() || player.IsIntAlly() || player.IsIntFanatic();

    public static bool CrewSided(this PlayerVoteArea player) => player.IsCrewAlly();

    public static bool Last(PlayerControl player) => (LastImp && player.Is(Faction.Intruder)) || (LastSyn && player.Is(Faction.Syndicate));

    public static bool CanKill(this PlayerControl player) => player.Is(Faction.Intruder) || player.Is(Faction.Syndicate) || player.Is(Alignment.NeutralKill) ||
        player.Is(Alignment.NeutralHarb) || player.Is(Alignment.NeutralApoc) || player.Is(LayerEnum.Corrupted) || player.Is(LayerEnum.Fanatic) || player.Is(LayerEnum.Traitor) ||
        player.Is(Alignment.CrewKill);

    public static bool IsPostmortal(this PlayerControl player) => player.GetRole() is Revealer or Phantom or Ghoul or Banshee;

    public static bool Caught(this PlayerControl player)
    {
        if (!player.IsPostmortal())
            return true;

        if (player.TryGetLayer<Phantom>(LayerEnum.Phantom, out var phan))
            return phan.Caught;
        else if (player.TryGetLayer<Revealer>(LayerEnum.Revealer, out var rev))
            return rev.Caught;
        else if (player.TryGetLayer<Ghoul>(LayerEnum.Ghoul, out var ghoul))
            return ghoul.Caught;
        else if (player.TryGetLayer<Banshee>(LayerEnum.Banshee, out var ban))
            return ban.Caught;

        return true;
    }

    public static bool IsLinkedTo(this PlayerControl player, PlayerControl refplayer) => player.IsOtherRival(refplayer) || player.IsOtherLover(refplayer) || player.IsOtherLink(refplayer)
        || (player.Is(LayerEnum.Mafia) && refplayer.Is(LayerEnum.Mafia));

    public static float GetBaseSpeed(this PlayerControl player)
    {
        if (player.HasDied() && (!player.IsPostmortal() || player.Caught()))
            return CustomGameOptions.GhostSpeed;
        else
            return CustomGameOptions.PlayerSpeed;
    }

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

        if (player.HasDied() || Lobby || (HudHandler.Instance.IsCamoed && CustomGameOptions.CamoHideSpeed && !TransitioningSpeed.ContainsKey(player.PlayerId)))
            return result;

        if (IntroCutscene.Instance)
            return 0f;

        if (player.TryGetLayer<Hunter>(LayerEnum.Hunter, out var hunt))
            return hunt.Starting ? 0f : CustomGameOptions.HunterSpeedModifier;

        if (player.Is(LayerEnum.Dwarf))
            result *= CustomGameOptions.DwarfSpeed;
        else if (player.Is(LayerEnum.Giant))
            result *= CustomGameOptions.GiantSpeed;
        else if (player.TryGetLayer<Drunk>(LayerEnum.Drunk, out var drunk))
            result *= drunk.Modify;

        if (DragHandler.Instance.Dragging.Keys.Any(x => x == player.PlayerId))
            result *= CustomGameOptions.DragModifier;

        if (PlayerLayer.GetLayers<Drunkard>().Any(x => x.ConfuseButton.EffectActive && (x.HoldsDrive || (x.ConfusedPlayer == player && !x.HoldsDrive))) ||
            PlayerLayer.GetLayers<PromotedRebel>().Any(x => x.ConfuseButton.EffectActive && (x.HoldsDrive || (x.ConfusedPlayer == player && !x.HoldsDrive)) && x.IsDrunk))
        {
            result *= -1;
        }

        if (PlayerLayer.GetLayers<Timekeeper>().Any(x => x.TimeButton.EffectActive))
        {
            if (!player.Is(Faction.Syndicate) || (player.Is(Faction.Syndicate) && ((PlayerLayer.GetLayers<Timekeeper>().Any(x => x.TimeButton.EffectActive && x.HoldsDrive) &&
                !CustomGameOptions.TimeRewindImmunity) || (PlayerLayer.GetLayers<Timekeeper>().Any(x => x.TimeButton.EffectActive && !x.HoldsDrive) &&
                !CustomGameOptions.TimeFreezeImmunity))))
            {
                result = 0f;
            }
        }

        if (PlayerLayer.GetLayers<PromotedRebel>().Any(x => x.TimeButton.EffectActive))
        {
            if (!player.Is(Faction.Syndicate) || (player.Is(Faction.Syndicate) && ((PlayerLayer.GetLayers<PromotedRebel>().Any(x => x.TimeButton.EffectActive && x.HoldsDrive && x.IsTK) &&
                !CustomGameOptions.TimeRewindImmunity) || (PlayerLayer.GetLayers<PromotedRebel>().Any(x => x.TimeButton.EffectActive && !x.HoldsDrive && x.IsTK) &&
                !CustomGameOptions.TimeFreezeImmunity))))
            {
                result = 0f;
            }
        }

        if (Ship != null && Ship.Systems.TryGetValue(SystemTypes.LifeSupp, out var life))
        {
            var lifeSuppSystemType = life.Cast<LifeSuppSystemType>();

            if (lifeSuppSystemType.IsActive && CustomGameOptions.OxySlow && !player.Data.IsDead)
                result *= Math.Clamp(lifeSuppSystemType.Countdown / lifeSuppSystemType.LifeSuppDuration, 0.25f, 1f);
        }

        if (player.TryGetLayer<Trapper>(LayerEnum.Trapper, out var trap))
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
        if (Ship?.Systems?.TryGetValue(SystemTypes.MushroomMixupSabotage, out var sab) == true)
        {
            var mixup = sab.TryCast<MushroomMixupSabotageSystem>();

            if (mixup != null && mixup.IsActive)
                return 1f;
        }

        if (Lobby || (HudHandler.Instance.IsCamoed && CustomGameOptions.CamoHideSize && !TransitioningSize.ContainsKey(player.PlayerId)))
            return 1f;
        else if (player.Is(LayerEnum.Dwarf))
            return CustomGameOptions.DwarfScale;
        else if (player.Is(LayerEnum.Giant))
            return CustomGameOptions.GiantScale;
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
                if (morph.Player == player && morph.MorphedPlayer != null && morph.MorphButton.EffectActive)
                {
                    mimicked = morph.MorphedPlayer;
                    return true;
                }
            }

            foreach (var gf in PlayerLayer.GetLayers<PromotedGodfather>())
            {
                if (gf.IsMorph && gf.Player == player && gf.MorphedPlayer != null && gf.MorphButton.EffectActive)
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

        if (player == null || playerInfo == null)
            return false;
        else if (IsHnS)
            return !playerInfo.IsImpostor();
        else if (playerInfo.Disconnected || (int)CustomGameOptions.WhoCanVent is 3 || player.inMovingPlat || player.onLadder || Meeting)
            return false;
        else if (player.inVent || CustomGameOptions.WhoCanVent == WhoCanVentOptions.Everyone)
            return true;
        else if (playerInfo.IsDead)
            return !player.Caught();

        var playerRole = player.GetRole();
        var mainflag = false;

        if (!playerRole)
            mainflag = playerInfo.IsImpostor();
        else if (player.Is(LayerEnum.Hunter))
            mainflag = CustomGameOptions.HunterVent;
        else if (player.Is(LayerEnum.Hunted) || player.Is(LayerEnum.Runner))
            mainflag = false;
        else if (player.Is(LayerEnum.Mafia))
            mainflag = CustomGameOptions.MafVent;
        else if (player.Is(LayerEnum.Corrupted))
            mainflag = CustomGameOptions.CorruptedVent;
        else if (player.IsRecruit())
            mainflag = CustomGameOptions.RecruitVent;
        else if (player.IsResurrected())
            mainflag = CustomGameOptions.ResurrectVent;
        else if (player.IsPersuaded())
            mainflag = CustomGameOptions.PersuadedVent;
        else if (player.IsBitten())
            mainflag = CustomGameOptions.UndeadVent;
        else if (player.Is(Faction.Syndicate) && playerRole.BaseFaction == Faction.Syndicate)
            mainflag = (((Syndicate)playerRole).HoldsDrive && (int)CustomGameOptions.SyndicateVent is 1) || (int)CustomGameOptions.SyndicateVent is 0;
        else if (player.Is(Faction.Intruder) && playerRole.BaseFaction == Faction.Intruder)
        {
            var flag = (player.Is(LayerEnum.Morphling) && !CustomGameOptions.MorphlingVent) || (player.Is(LayerEnum.Wraith) && !CustomGameOptions.WraithVent) ||
                (player.Is(LayerEnum.Grenadier) && !CustomGameOptions.GrenadierVent) || (player.Is(LayerEnum.Teleporter) && !CustomGameOptions.TeleVent);

            mainflag = CustomGameOptions.IntrudersVent;

            if (mainflag)
            {
                if (player.TryGetLayer<Janitor>(LayerEnum.Janitor, out var jani))
                {
                    mainflag = (int)CustomGameOptions.JanitorVentOptions is 3 || (jani.CurrentlyDragging && (int)CustomGameOptions.JanitorVentOptions is 1) || (!jani.CurrentlyDragging
                        && (int)CustomGameOptions.JanitorVentOptions is 2);
                }
                else if (player.TryGetLayer<PromotedGodfather>(LayerEnum.PromotedGodfather, out var gf))
                {
                    if (gf.IsJani)
                    {
                        mainflag = (int)CustomGameOptions.JanitorVentOptions is 3 || (gf.CurrentlyDragging && (int)CustomGameOptions.JanitorVentOptions is 1) || (!gf.CurrentlyDragging
                            && (int)CustomGameOptions.JanitorVentOptions is 2);
                    }
                    else if (gf.IsMorph)
                        mainflag = CustomGameOptions.MorphlingVent;
                    else if (gf.IsWraith)
                        mainflag = CustomGameOptions.WraithVent;
                    else if (gf.IsGren)
                        mainflag = CustomGameOptions.GrenadierVent;
                    else if (gf.IsTele)
                        mainflag = CustomGameOptions.TeleVent;
                }
                else if (flag)
                    mainflag = false;
            }
        }
        else if (player.Is(Faction.Crew) || playerRole.BaseFaction == Faction.Crew)
        {
            if (player.Is(LayerEnum.Tunneler))
                mainflag =  playerRole.TasksDone;
            else
                mainflag = player.Is(LayerEnum.Engineer) || CustomGameOptions.CrewVent;
        }
        else if (player.Is(Faction.Neutral) || playerRole.BaseFaction == Faction.Neutral)
        {
            mainflag = ((player.Is(LayerEnum.Murderer) && CustomGameOptions.MurdVent) || (player.Is(LayerEnum.Glitch) && CustomGameOptions.GlitchVent) || (player.Is(LayerEnum.Juggernaut)
                && CustomGameOptions.JuggVent) || (player.Is(LayerEnum.Pestilence) && CustomGameOptions.PestVent) || (player.Is(LayerEnum.Jester) && CustomGameOptions.JesterVent) ||
                (player.Is(LayerEnum.Plaguebearer) && CustomGameOptions.PBVent) || (player.Is(LayerEnum.Arsonist) && CustomGameOptions.ArsoVent) || (player.Is(LayerEnum.Executioner) &&
                CustomGameOptions.ExeVent) || (player.Is(LayerEnum.Cannibal) && CustomGameOptions.CannibalVent) || (player.Is(LayerEnum.Dracula) && CustomGameOptions.DracVent) ||
                (player.Is(LayerEnum.Survivor) && CustomGameOptions.SurvVent) || (player.Is(LayerEnum.Actor) && CustomGameOptions.ActorVent) || (player.Is(LayerEnum.GuardianAngel) &&
                CustomGameOptions.GAVent) || (player.Is(LayerEnum.Amnesiac) && CustomGameOptions.AmneVent) || (player.Is(LayerEnum.Jackal) && CustomGameOptions.JackalVent) ||
                (player.Is(LayerEnum.BountyHunter) && CustomGameOptions.BHVent) || (player.Is(LayerEnum.Betrayer) && CustomGameOptions.BetrayerVent)) && CustomGameOptions.NeutralsVent;

            if (player.TryGetLayer<SerialKiller>(LayerEnum.SerialKiller, out var sk))
            {
                mainflag = CustomGameOptions.NeutralsVent && (CustomGameOptions.SKVentOptions == 0 || (sk.BloodlustButton.EffectActive && (int)CustomGameOptions.SKVentOptions == 1)
                    || (!sk.BloodlustButton.EffectActive && (int)CustomGameOptions.SKVentOptions == 2));
            }
            else if (player.TryGetLayer<Werewolf>(LayerEnum.Werewolf, out var ww))
            {
                mainflag = CustomGameOptions.NeutralsVent && (CustomGameOptions.WerewolfVent == 0 || (ww.CanMaul && (int)CustomGameOptions.WerewolfVent == 1) || (!ww.CanMaul &&
                    (int)CustomGameOptions.WerewolfVent == 3));
            }
        }
        else if (player.IsPostmortal() && player.inVent)
            mainflag = true;

        return mainflag;
    }

    public static bool CanChat(this PlayerControl player)
    {
        var playerInfo = player?.Data;

        if (player == null || playerInfo == null)
            return false;
        else if (playerInfo.IsDead || Meeting || Lobby)
            return true;
        else if (player.Is(LayerEnum.Lovers))
            return CustomGameOptions.LoversChat;
        else if (player.Is(LayerEnum.Rivals))
            return CustomGameOptions.RivalsChat;
        else if (player.Is(LayerEnum.Linked))
            return CustomGameOptions.LinkedChat;
        else if (player.Is(LayerEnum.Hunted))
            return CustomGameOptions.HuntedChat;
        else
            return false;
    }

    public static bool IsBlocked(this PlayerControl player) => player.GetLayers().Any(x => x.IsBlocked);

    public static bool SeemsEvil(this PlayerControl player)
    {
        var intruderFlag = player.Is(Faction.Intruder) && !player.Is(LayerEnum.Traitor) && !player.Is(LayerEnum.Fanatic) && !player.Is(LayerEnum.PromotedGodfather);
        var syndicateFlag = player.Is(Faction.Syndicate) && !player.Is(LayerEnum.Traitor) && !player.Is(LayerEnum.Fanatic) && !player.Is(LayerEnum.PromotedRebel);
        var traitorFlag = player.IsTurnedTraitor() && CustomGameOptions.TraitorColourSwap;
        var fanaticFlag = player.IsTurnedFanatic() && CustomGameOptions.FanaticColourSwap;
        var nkFlag = player.Is(Alignment.NeutralKill) && !CustomGameOptions.NeutKillingRed;
        var neFlag = player.Is(Alignment.NeutralEvil) && !CustomGameOptions.NeutEvilRed;
        var framedFlag = player.IsFramed();
        return intruderFlag || syndicateFlag || traitorFlag || nkFlag || neFlag || framedFlag || fanaticFlag;
    }

    public static bool SeemsGood(this PlayerControl player) => !SeemsEvil(player) || Role.DriveHolder == player;

    public static bool SeemsEvil(this PlayerVoteArea player) => PlayerByVoteArea(player).SeemsEvil();

    public static bool SeemsGood(this PlayerVoteArea player) => PlayerByVoteArea(player).SeemsGood();

    public static bool IsBlockImmune(PlayerControl player) => player.GetRole().RoleBlockImmune;

    public static PlayerControl GetOtherLover(this PlayerControl player) => player.Is(LayerEnum.Lovers) ? player.GetObjectifier<Lovers>().OtherLover : null;

    public static PlayerControl GetOtherRival(this PlayerControl player) => player.Is(LayerEnum.Rivals) ? player.GetObjectifier<Rivals>().OtherRival : null;

    public static PlayerControl GetOtherLink(this PlayerControl player) => player.Is(LayerEnum.Linked) ? player.GetObjectifier<Linked>().OtherLink : null;

    public static bool NeutralHasUnfinishedBusiness(PlayerControl player)
    {
        if (player.TryGetLayer<GuardianAngel>(LayerEnum.GuardianAngel, out var ga))
            return ga.TargetAlive;
        else if (player.TryGetLayer<Executioner>(LayerEnum.Executioner, out var exe))
            return exe.TargetVotedOut;
        else if (player.TryGetLayer<Jester>(LayerEnum.Jester, out var jest))
            return jest.VotedOut;
        else if (player.TryGetLayer<Guesser>(LayerEnum.Guesser, out var guess))
            return guess.TargetGuessed;
        else if (player.TryGetLayer<BountyHunter>(LayerEnum.BountyHunter, out var bh))
            return bh.TargetKilled;
        else if (player.TryGetLayer<Actor>(LayerEnum.Actor, out var act))
            return act.Guessed;
        else if (player.TryGetLayer<Troll>(LayerEnum.Troll, out var troll))
            return troll.Killed;

        return false;
    }

    public static string RoleCardInfo(this PlayerControl player)
    {
        var info = player.GetLayers();

        if (info.Count != 4)
            return "";

        var role = info[0] as Role;
        var modifier = info[1] as Modifier;
        var ability = info[2] as Ability;
        var objectifier = info[3] as Objectifier;

        var objectives = $"{ObjectivesColorString}Goal:";
        var abilities = $"{AbilitiesColorString}Abilities:";
        var attributes = $"{AttributesColorString}Attributes:";
        var roleName = $"{RoleColorString}Role: <b>";
        var objectifierName = $"{ObjectifierColorString}Objectifier: <b>";
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

        if (info[3] && !objectifier.Hidden)
        {
            objectives += $"\n{objectifier.ColorString}{objectifier.Description()}</color>";
            objectifierName += $"{objectifier.ColorString}{objectifier.Name} {objectifier.Symbol}</color>";
        }
        else
            objectifierName += "None φ";

        objectifierName += "</b></color>";

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

        if (player.IsRecruit())
        {
            var jackal = player.GetJackal();
            objectives += $"\n<color=#{CustomColorManager.Cabal.ToHtmlStringRGBA()}>- You are a member of the Cabal. Help {jackal.PlayerName} in taking over the mission $</color>";
        }
        else if (player.IsResurrected())
        {
            var necromancer = player.GetNecromancer();
            objectives += $"\n<color=#{CustomColorManager.Reanimated.ToHtmlStringRGBA()}>- You are a member of the Reanimated. Help {necromancer.PlayerName} in taking over the mission Σ</color>";
        }
        else if (player.IsPersuaded())
        {
            var whisperer = player.GetWhisperer();
            objectives += $"\n<color=#{CustomColorManager.Sect.ToHtmlStringRGBA()}>- You are a member of the Sect. Help {whisperer.PlayerName} in taking over the mission Λ</color>";
        }
        else if (player.IsBitten())
        {
            var dracula = player.GetDracula();
            objectives += $"\n<color=#{CustomColorManager.Undead.ToHtmlStringRGBA()}>- You are a member of the Undead. Help {dracula.PlayerName} in taking over the mission γ</color>";
            abilities += $"\n{role.ColorString}- Attempting to interact with a <color=#C0C0C0FF>Vampire Hunter</color> will force them to kill you</color>";
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

        if (player.IsGuessTarget() && CustomGameOptions.GuesserTargetKnows)
            attributes += "\n<color=#EEE5BEFF>- Someone wants to guess you π</color>";

        if (player.IsExeTarget() && CustomGameOptions.ExeTargetKnows)
            attributes += "\n<color=#CCCCCCFF>- Someone wants you ejected §</color>";

        if (player.IsGATarget() && CustomGameOptions.GATargetKnows)
            attributes += "\n<color=#FFFFFFFF>- Someone wants to protect you ★</color>";

        if (player.IsBHTarget())
            attributes += "\n<color=#B51E39FF>- There is a bounty on your head Θ</color>";

        if (player.Is(Faction.Syndicate) && role.BaseFaction == Faction.Syndicate && ((Syndicate)role).HoldsDrive)
            attributes += "\n<color=#008000FF>- You have the power of the Chaos Drive Δ</color>";

        if (!player.CanDoTasks())
            attributes += "\n<color=#ABCDEFFF>- Your tasks are fake</color>";

        if (player.Data.IsDead)
            attributes += "\n<color=#FF1919FF>- You are dead</color>";

        if (attributes == $"{AttributesColorString}Attributes:")
            attributes = "";
        else
            attributes = $"\n{attributes}</color>";

        return $"{roleName}\n{attdef}\n{alignment}\n{subfaction}\n{objectifierName}\n{abilityName}\n{modifierName}\n{objectives}{abilities}{attributes}";
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

    public static void RoleUpdate(this Role newRole, Role former, bool retainFaction = false)
    {
        AllButtons.Where(x => x.Owner == former || x.Owner.Player == null).ForEach(x => x.Destroy());
        CustomArrow.AllArrows.Where(x => x.Owner == former.Player).ForEach(x => x.Disable());
        former.OnLobby();
        former.ExitingLayer();
        former.Ignore = true;
        former.Player = null;

        if (!retainFaction)
        {
            newRole.Faction = former.Faction;
            newRole.FactionColor = former.FactionColor;
        }

        newRole.Alignment = newRole.Alignment.GetNewAlignment(newRole.Faction);
        newRole.SubFaction = former.SubFaction;
        newRole.SubFactionColor = former.SubFactionColor;
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

        if (CustomPlayer.Local.Is(LayerEnum.Seer))
            Flash(CustomColorManager.Seer);

        ButtonUtils.Reset(CooldownType.Reset, newRole.Player);
    }

    public static void SetImpostor(this PlayerControl player, bool impostor)
    {
        if (!player)
            return;

        var imp = player.HasDied() ? RoleTypes.ImpostorGhost : RoleTypes.Impostor;
        var crew = player.HasDied() ? RoleTypes.CrewmateGhost : RoleTypes.Crewmate;
        RoleManager.Instance.SetRole(player, impostor ? imp : crew);
    }

    public static string AlignmentName(this Alignment alignment, bool withColors = false) => alignment switch
    {
        Alignment.CrewSupport => withColors ? "<color=#8CFFFFFF>Crew (<color=#1D7CF2FF>Support</color>)</color>" : "Crew (Support)",
        Alignment.CrewInvest => withColors ? "<color=#8CFFFFFF>Crew (<color=#1D7CF2FF>Investigative</color>)</color>" : "Crew (Investigative)",
        Alignment.CrewProt => withColors ? "<color=#8CFFFFFF>Crew (<color=#1D7CF2FF>Protective</color>)</color>" : "Crew (Protective)",
        Alignment.CrewKill => withColors ? "<color=#8CFFFFFF>Crew (<color=#1D7CF2FF>Killing</color>)</color>" : "Crew (Killing)",
        Alignment.CrewUtil => withColors ? "<color=#8CFFFFFF>Crew (<color=#1D7CF2FF>Utility</color>)</color>" : "Crew (Utility)",
        Alignment.CrewSov => withColors ? "<color=#8CFFFFFF>Crew (<color=#1D7CF2FF>Sovereign</color>)</color>" : "Crew (Sovereign)",
        Alignment.CrewAudit => withColors ? "<color=#8CFFFFFF>Crew (<color=#1D7CF2FF>Auditor</color>)</color>" : "Crew (Auditor)",
        Alignment.CrewDecep => withColors ? "<color=#8CFFFFFF>Crew (<color=#1D7CF2FF>Deception</color>)</color>" : "Crew (Deception)",
        Alignment.CrewConceal => withColors ? "<color=#8CFFFFFF>Crew (<color=#1D7CF2FF>Concealing</color>)</color>" : "Crew (Concealing)",
        Alignment.CrewPower => withColors ? "<color=#8CFFFFFF>Crew (<color=#1D7CF2FF>Power</color>)</color>" : "Crew (Power)",
        Alignment.CrewDisrup => withColors ? "<color=#8CFFFFFF>Crew (<color=#1D7CF2FF>Disruption</color>)</color>" : "Crew (Disruption)",
        Alignment.CrewHead => withColors ? "<color=#8CFFFFFF>Crew (<color=#1D7CF2FF>Head</color>)</color>" : "Crew (Head)",
        Alignment.IntruderSupport => withColors ? "<color=#FF1919FF>Intruder (<color=#1D7CF2FF>Support</color>)</color>" : "Intruder (Support)",
        Alignment.IntruderConceal => withColors ? "<color=#FF1919FF>Intruder (<color=#1D7CF2FF>Concealing</color>)</color>" : "Intruder (Concealing)",
        Alignment.IntruderDecep => withColors ? "<color=#FF1919FF>Intruder (<color=#1D7CF2FF>Deception</color>)</color>" : "Intuder (Deception)",
        Alignment.IntruderKill => withColors ? "<color=#FF1919FF>Intruder (<color=#1D7CF2FF>Killing</color>)</color>" : "Intruder (Killing)",
        Alignment.IntruderUtil => withColors ? "<color=#FF1919FF>Intruder (<color=#1D7CF2FF>Utility</color>)</color>" : "Intruder (Utility)",
        Alignment.IntruderInvest => withColors ? "<color=#FF1919FF>Intruder (<color=#1D7CF2FF>Investigative</color>)</color>" : "Intruder (Investigative)",
        Alignment.IntruderAudit => withColors ? "<color=#FF1919FF>Intruder (<color=#1D7CF2FF>Auditor</color>)</color>" : "Intruder (Auditor)",
        Alignment.IntruderProt => withColors ? "<color=#FF1919FF>Intruder (<color=#1D7CF2FF>Protective</color>)</color>" : "Intruder (Protective)",
        Alignment.IntruderSov => withColors ? "<color=#FF1919FF>Intruder (<color=#1D7CF2FF>Sovereign</color>)</color>" : "Intruder (Sovereign)",
        Alignment.IntruderDisrup=> withColors ? "<color=#FF1919FF>Intruder (<color=#1D7CF2FF>Disruption</color>)</color>" : "Intruder (Disruption)",
        Alignment.IntruderPower => withColors ? "<color=#FF1919FF>Intruder (<color=#1D7CF2FF>Power</color>)</color>" : "Intruder (Power)",
        Alignment.IntruderHead => withColors ? "<color=#FF1919FF>Intruder (<color=#1D7CF2FF>Head</color>)</color>" : "Intruder (Head)",
        Alignment.NeutralKill => withColors ? "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Killing</color>)</color>" : "Neutral (Killing)",
        Alignment.NeutralNeo => withColors ? "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Neophyte</color>)</color>" : "Neutral (Neophyte)",
        Alignment.NeutralEvil => withColors ? "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Evil</color>)</color>" : "Neutral (Evil)",
        Alignment.NeutralBen => withColors ? "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Benign</color>)</color>" : "Neutral (Benign)",
        Alignment.NeutralPros => withColors ? "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Proselyte</color>)</color>" : "Neutral (Proselyte)",
        Alignment.NeutralSupport => withColors ? "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Support</color>)</color>" : "Neutral (Support)",
        Alignment.NeutralInvest => withColors ? "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Investigative</color>)</color>" : "Neutral (Investigative)",
        Alignment.NeutralProt => withColors ? "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Protective</color>)</color>" : "Neutral (Protective)",
        Alignment.NeutralUtil => withColors ? "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Utility</color>)</color>" : "Neutral (Utility)",
        Alignment.NeutralSov => withColors ? "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Sovereign</color>)</color>" : "Neutral (Sovereign)",
        Alignment.NeutralAudit => withColors ? "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Auditor</color>)</color>" : "Neutral (Auditor)",
        Alignment.NeutralConceal => withColors ? "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Concealing</color>)</color>" : "Neutral (Concealing)",
        Alignment.NeutralDecep => withColors ? "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Deception</color>)</color>" : "Neutral (Deception)",
        Alignment.NeutralPower => withColors ? "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Power</color>)</color>" : "Neutral (Power)",
        Alignment.NeutralDisrup => withColors ? "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Disruption</color>)</color>" : "Neutral (Disruption)",
        Alignment.NeutralApoc => withColors ? "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Apocalypse</color>)</color>" : "Neutral (Apocalypse)",
        Alignment.NeutralHarb => withColors ? "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Harbinger</color>)</color>" : "Neutral (Harbinger)",
        Alignment.NeutralHead => withColors ? "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Head</color>)</color>" : "Neutral (Head)",
        Alignment.SyndicateKill => withColors ? "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Killing</color>)</color>" : "Syndicate (Killing)",
        Alignment.SyndicateSupport => withColors ? "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Support</color>)</color>" : "Syndicate (Support)",
        Alignment.SyndicateDisrup => withColors ? "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Disruption</color>)</color>" : "Syndicate (Disruption)",
        Alignment.SyndicatePower => withColors ? "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Power</color>)</color>" : "Syndicate (Power)",
        Alignment.SyndicateUtil => withColors ? "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Utility</color>)</color>" : "Syndicate (Utility)",
        Alignment.SyndicateInvest => withColors ? "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Investigative</color>)</color>" : "Syndicate (Investigative)",
        Alignment.SyndicateAudit => withColors ? "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Auditor</color>)</color>" : "Syndicate (Auditor)",
        Alignment.SyndicateSov => withColors ? "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Sovereign</color>)</color>" : "Syndicate (Sovereign)",
        Alignment.SyndicateProt => withColors ? "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Protective</color>)</color>" : "Syndicate (Protective)",
        Alignment.SyndicateConceal => withColors ? "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Concealing</color>)</color>" : "Syndicate (Concealing)",
        Alignment.SyndicateDecep => withColors ? "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Deception</color>)</color>" : "Syndicate (Deception)",
        Alignment.SyndicateHead => withColors ? "<color=#008000FF>Syndicate (<color=#1D7CF2FF>Head</color>)</color>" : "Syndicate (Head)",
        Alignment.GameModeHideAndSeek => withColors ? "<color=#A81538FF>Game Mode (<color=#7500AFFF>Hide And Seek</color>)</color>" : "Game Mode (Hide And Seek)",
        Alignment.GameModeTaskRace => withColors ? "<color=#A81538FF>Game Mode (<color=#1E49CFFF>Task Race</color>)</color>" : "Game Mode (Task Race)",
        _ => alignment.ToString()
    };

    public static string GameModeName(this GameMode mode, bool withColors = false) => mode switch
    {
        GameMode.TaskRace => withColors ? "<color=#1E49CFFF>Task Race</color>" : "Task Race",
        GameMode.HideAndSeek => withColors ? "<color=#7500AFFF>Hide And Seek</color>" : "Hide And Seek",
        GameMode.Classic => withColors ? "<color=#C02A2CFF>Classic</color>" : "Classic",
        GameMode.AllAny => withColors ? "<color=#CBD542FF>All Any</color>" : "All Any",
        GameMode.KillingOnly => withColors ? "<color=#06E00CFF>Killing Only</color>" : "Killing Only",
        GameMode.Custom => withColors ? "<color=#E6956AFF>Custom</color>" : "Custom",
        GameMode.Vanilla => "Vanilla",
        GameMode.RoleList => withColors ? "<color=#FA1C79FF>Role List</color>" : "Role List",
        _ => "Invalid"
    };

    public static Alignment GetNewAlignment(this Alignment alignment, Faction faction)
    {
        if (faction == Faction.Crew)
        {
            return alignment switch
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
            };
        }
        else if (faction == Faction.Intruder)
        {
            return alignment switch
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
            };
        }
        else if (faction == Faction.Syndicate)
        {
            return alignment switch
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
            };
        }
        else if (faction == Faction.Neutral)
        {
            return alignment switch
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
            };
        }

        return alignment;
    }

    public static bool CanButton(this PlayerControl player, out string name)
    {
        name = "Shy";
        var result = !player.Is(LayerEnum.Shy) && player.RemainingEmergencies > 0;

        if (player.Is(LayerEnum.Mayor))
        {
            name = "Mayor";
            result = CustomGameOptions.MayorButton;
        }
        else if (player.Is(LayerEnum.Jester))
        {
            name = "Jester";
            result = CustomGameOptions.JesterButton;
        }
        else if (player.Is(LayerEnum.Swapper))
        {
            name = "Swapper";
            result = CustomGameOptions.SwapperButton;
        }
        else if (player.Is(LayerEnum.Actor))
        {
            name = "Actor";
            result = CustomGameOptions.ActorButton;
        }
        else if (player.Is(LayerEnum.Executioner))
        {
            name = "Executioner";
            result = CustomGameOptions.ExecutionerButton;
        }
        else if (player.Is(LayerEnum.Guesser))
        {
            name = "Guesser";
            result = CustomGameOptions.GuesserButton;
        }
        else if (player.Is(LayerEnum.Politician))
        {
            name = "Politician";
            result = CustomGameOptions.PoliticianButton;
        }
        else if (player.Is(LayerEnum.Dictator))
        {
            name = "Dictator";
            result = CustomGameOptions.DictatorButton;
        }
        else if (player.Is(LayerEnum.Monarch))
        {
            name = "Monarch";
            result = CustomGameOptions.MonarchButton;
        }
        else if (player.IsKnighted())
        {
            name = "Knight";
            result = CustomGameOptions.KnightButton;
        }
        else if (IsTaskRace || IsCustomHnS)
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

    /*public static PlayerLayerEnum GetLayerType(this LayerEnum layer)
    {
        var id = (int)layer;

        if (id < 91)
            return PlayerLayerEnum.Role;
        else if (id < 105)
            return PlayerLayerEnum.Modifier;
        else if (id < 116)
            return PlayerLayerEnum.Objectifier;
        else if (id < 133)
            return PlayerLayerEnum.Ability;
        else
            return PlayerLayerEnum.None;
    }*/

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

        if (target && target.Is(SubFaction.Undead) && player.Is(LayerEnum.VampireHunter))
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

        if (source && source.Is(LayerEnum.VampireHunter) && player.Is(SubFaction.Undead))
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

    public static List<PlayerLayer> GetLayers(this PlayerControl player) => [..PlayerLayer.AllLayers.Where(x => x.Player == player).OrderBy(x => (int)x.LayerType)];

    public static List<PlayerLayer> GetLayers(this PlayerVoteArea player) => PlayerByVoteArea(player).GetLayers();

    public static PlayerLayer GetLayer(this PlayerControl player, PlayerLayerEnum layerType) => PlayerLayer.AllLayers.Find(x => x.Player == player && x.LayerType == layerType);

    public static T GetLayer<T>(this PlayerControl player, PlayerLayerEnum layerType) where T : PlayerLayer => player.GetLayer(layerType) as T;

    public static T GetLayer<T>(this PlayerControl player) where T : PlayerLayer => player.GetLayers().Find(x => x.GetType() == typeof(T)) as T;

    public static Role GetRole(this PlayerControl player) => player.GetLayer<Role>(PlayerLayerEnum.Role);

    public static Role GetRoleOrBlank(this PlayerControl player) => player.GetRole() ?? new Roleless().Start<Role>(player);

    public static T GetRole<T>(this PlayerControl player) where T : Role => player.GetRole() as T;

    public static Role GetRole(this PlayerVoteArea area) => PlayerByVoteArea(area).GetRole();

    public static Objectifier GetObjectifier(this PlayerControl player) => player.GetLayer<Objectifier>(PlayerLayerEnum.Objectifier);

    public static Objectifier GetObjectifierOrBlank(this PlayerControl player) => player.GetObjectifier() ?? new Objectifierless().Start<Objectifier>(player);

    public static T GetObjectifier<T>(this PlayerControl player) where T : Objectifier => player.GetObjectifier() as T;

    public static Objectifier GetObjectifier(this PlayerVoteArea area) => PlayerByVoteArea(area).GetObjectifier();

    public static Modifier GetModifier(this PlayerControl player) => player.GetLayer<Modifier>(PlayerLayerEnum.Modifier);

    public static Modifier GetModifierOrBlank(this PlayerControl player) => player.GetModifier() ?? new Modifierless().Start<Modifier>(player);

    public static T GetModifier<T>(this PlayerControl player) where T : Modifier => player.GetModifier() as T;

    public static Modifier GetModifier(this PlayerVoteArea area) => PlayerByVoteArea(area).GetModifier();

    public static Ability GetAbility(this PlayerControl player) => player.GetLayer<Ability>(PlayerLayerEnum.Ability);

    public static Ability GetAbilityOrBlank(this PlayerControl player) => player.GetAbility() ?? new Abilityless().Start<Ability>(player);

    public static T GetAbility<T>(this PlayerControl player) where T : Ability => player.GetAbility() as T;

    public static Ability GetAbility(this PlayerVoteArea area) => PlayerByVoteArea(area).GetAbility();

    public static bool TryGetLayer<T>(this PlayerControl player, LayerEnum type, out T layer) where T : PlayerLayer
    {
        layer = null;

        if (player.Is(type))
        {
            layer = player.GetLayers().Find(x => x.Type == type) as T;
            return layer != null;
        }
        else
            return false;
    }
}