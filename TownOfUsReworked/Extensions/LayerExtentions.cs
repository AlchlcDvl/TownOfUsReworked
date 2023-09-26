namespace TownOfUsReworked.Extensions;

public static class LayerExtentions
{
    public static string RoleColorString => $"<color=#{Colors.Role.ToHtmlStringRGBA()}>";
    public static string AlignmentColorString => $"<color=#{Colors.Alignment.ToHtmlStringRGBA()}>";
    public static string ObjectivesColorString => $"<color=#{Colors.Objectives.ToHtmlStringRGBA()}>";
    public static string AttributesColorString => $"<color=#{Colors.Attributes.ToHtmlStringRGBA()}>";
    public static string AbilitiesColorString => $"<color=#{Colors.Abilities.ToHtmlStringRGBA()}>";
    public static string ObjectifierColorString => $"<color=#{Colors.Objectifier.ToHtmlStringRGBA()}>";
    public static string ModifierColorString => $"<color=#{Colors.Modifier.ToHtmlStringRGBA()}>";
    public static string AbilityColorString => $"<color=#{Colors.Ability.ToHtmlStringRGBA()}>";
    public static string SubFactionColorString => $"<color=#{Colors.SubFaction.ToHtmlStringRGBA()}>";

    public static bool Is(this PlayerControl player, LayerEnum type, PlayerLayerEnum layer) => PlayerLayer.GetLayers(player).Any(x => x.Type == type && x.LayerType == layer);

    public static bool Is(this PlayerControl player, LayerEnum type) => PlayerLayer.GetLayers(player).Any(x => x.Type == type);

    public static bool Is(this Role role, LayerEnum roleType) => role?.Type == roleType;

    public static bool Is(this Objectifier obj, LayerEnum objectifiertype) => obj?.Type == objectifiertype;

    public static bool Is(this PlayerControl player, Role role) => Role.GetRole(player).Player == role.Player;

    public static bool Is(this PlayerControl player, SubFaction subFaction) => Role.GetRole(player)?.SubFaction == subFaction;

    public static bool Is(this PlayerControl player, Faction faction) => player.GetFaction() == faction;

    public static bool Is(this PlayerControl player, Alignment alignment) => Role.GetRole(player)?.Alignment == alignment;

    public static bool Is(this PlayerVoteArea player, LayerEnum roleType) => PlayerByVoteArea(player).Is(roleType);

    public static bool Is(this PlayerVoteArea player, Role role) => PlayerByVoteArea(player).Is(role);

    public static bool Is(this PlayerVoteArea player, SubFaction subFaction) => PlayerByVoteArea(player).Is(subFaction);

    public static bool Is(this PlayerVoteArea player, Faction faction) => PlayerByVoteArea(player).Is(faction);

    public static bool Is(this PlayerVoteArea player, Alignment alignment) => PlayerByVoteArea(player).Is(alignment);

    public static bool IsAssassin(this PlayerControl player) => player.Is(LayerEnum.CrewAssassin) || player.Is(LayerEnum.NeutralAssassin) || player.Is(LayerEnum.IntruderAssassin) ||
        player.Is(LayerEnum.SyndicateAssassin);

    public static Faction GetFaction(this PlayerControl player)
    {
        if (player == null)
            return Faction.None;

        var role = Role.GetRole(player);

        if (role == null)
            return player.Data.IsImpostor() ? Faction.Intruder : Faction.Crew;

        return role.Faction;
    }

    public static SubFaction GetSubFaction(this PlayerControl player)
    {
        if (player == null)
            return SubFaction.None;

        var role = Role.GetRole(player);

        if (role == null)
            return SubFaction.None;

        return role.SubFaction;
    }

    public static LayerEnum GetRole(this PlayerControl player)
    {
        if (player == null)
            return LayerEnum.None;

        var role = Role.GetRole(player);

        if (!role)
            return LayerEnum.None;

        return role.Type;
    }

    public static LayerEnum GetRole(this PlayerVoteArea player) => PlayerByVoteArea(player).GetRole();

    public static LayerEnum GetAbility(this PlayerControl player)
    {
        if (player == null)
            return LayerEnum.None;

        var ability = Ability.GetAbility(player);

        if (!ability)
            return LayerEnum.None;

        return ability.Type;
    }

    public static Alignment GetAlignment(this PlayerControl player)
    {
        if (player == null)
            return Alignment.None;

        var role = Role.GetRole(player);

        if (role == null)
            return Alignment.None;

        return role.Alignment;
    }

    public static Alignment GetAlignment(this PlayerVoteArea player) => PlayerByVoteArea(player).GetAlignment();

    public static Faction GetFaction(this PlayerVoteArea player) => PlayerByVoteArea(player).GetFaction();

    public static SubFaction GetSubFaction(this PlayerVoteArea player) => PlayerByVoteArea(player).GetSubFaction();

    public static bool IsRecruit(this PlayerControl player) => Role.GetRole(player).IsRecruit;

    public static bool IsBitten(this PlayerControl player) => Role.GetRole(player).IsBitten;

    public static bool IsPersuaded(this PlayerControl player) => Role.GetRole(player).IsPersuaded;

    public static bool IsResurrected(this PlayerControl player) => Role.GetRole(player).IsResurrected;

    public static bool Diseased(this PlayerControl player) => Role.GetRole(player).Diseased;

    public static bool IsCrewDefect(this PlayerControl player) => Role.GetRole(player).IsCrewDefect;

    public static bool IsIntDefect(this PlayerControl player) => Role.GetRole(player).IsIntDefect;

    public static bool IsSynDefect(this PlayerControl player) => Role.GetRole(player).IsSynDefect;

    public static bool IsNeutDefect(this PlayerControl player) => Role.GetRole(player).IsNeutDefect;

    public static bool IsDefect(this PlayerControl player) => player.IsCrewDefect() || player.IsIntDefect() || player.IsSynDefect() || player.IsNeutDefect();

    public static bool IsRecruit(this PlayerVoteArea player) => PlayerByVoteArea(player).IsRecruit();

    public static bool IsBitten(this PlayerVoteArea player) => PlayerByVoteArea(player).IsBitten();

    public static bool IsPersuaded(this PlayerVoteArea player) => PlayerByVoteArea(player).IsPersuaded();

    public static bool IsResurrected(this PlayerVoteArea player) => PlayerByVoteArea(player).IsResurrected();

    public static bool NotOnTheSameSide(this PlayerControl player) => !Role.GetRole(player).Faithful;

    public static bool CanSabotage(this PlayerControl player)
    {
        var result = (player.Is(Faction.Intruder) || (player.Is(Faction.Syndicate) && CustomGameOptions.AltImps)) && !Meeting && CustomGameOptions.IntrudersCanSabotage &&
            !player.IsFlashed();

        if (!player.Data.IsDead)
            return result;
        else
            return result && CustomGameOptions.GhostsCanSabotage && !Role.GetRoles(player.GetFaction()).All(x => x.IsDead);
    }

    public static bool HasAliveLover(this PlayerControl player) => Objectifier.GetObjectifiers<Lovers>(LayerEnum.Lovers).Any(x => x.Player == player && x.LoversAlive);

    public static bool CanDoTasks(this PlayerControl player)
    {
        if (player == null)
            return false;

        if (!Role.GetRole(player))
            return !player.Data.IsImpostor();

        var crewflag = player.Is(Faction.Crew);
        var neutralflag = player.Is(Faction.Neutral);
        var intruderflag = player.Is(Faction.Intruder);
        var syndicateflag = player.Is(Faction.Syndicate);

        var phantomflag = player.Is(LayerEnum.Phantom);

        var sideflag = player.NotOnTheSameSide();
        var taskmasterflag = player.Is(LayerEnum.Taskmaster);
        var defectflag = player.IsCrewDefect();

        var flag1 = crewflag && !sideflag;
        var flag2 = neutralflag && (taskmasterflag || phantomflag);
        var flag3 = intruderflag && (taskmasterflag || defectflag);
        var flag4 = syndicateflag && (taskmasterflag || defectflag);
        return flag1 || flag2 || flag3 || flag4;
    }

    public static bool IsMoving(this PlayerControl player) => Role.GetRoles<Transporter>(LayerEnum.Transporter).Any(x => (x.TransportPlayer1 == player || x.TransportPlayer2 == player) &&
        x.Transporting) || Role.GetRoles<Retributionist>(LayerEnum.Retributionist).Any(x => (x.TransportPlayer1 == player || x.TransportPlayer2 == player) && x.Transporting) ||
        Role.GetRoles<Warper>(LayerEnum.Warper).Any(x => x.WarpPlayer1 == player && x.Warping) || Role.GetRoles<PromotedRebel>(LayerEnum.PromotedRebel).Any(x => x.WarpPlayer1 == player
        && x.Warping);

    public static bool IsGATarget(this PlayerControl player) => Role.GetRoles<GuardianAngel>(LayerEnum.GuardianAngel).Any(x => x.TargetPlayer == player);

    public static bool IsExeTarget(this PlayerControl player) => Role.GetRoles<Executioner>(LayerEnum.Executioner).Any(x => x.TargetPlayer == player);

    public static bool IsBHTarget(this PlayerControl player) => Role.GetRoles<BountyHunter>(LayerEnum.BountyHunter).Any(x => x.TargetPlayer == player);

    public static bool IsGuessTarget(this PlayerControl player) => Role.GetRoles<Guesser>(LayerEnum.Guesser).Any(x => x.TargetPlayer == player);

    public static PlayerControl GetTarget(this PlayerControl player)
    {
        var role = Role.GetRole(player);

        if (!role.HasTarget)
            return null;

        if (player.Is(LayerEnum.Executioner))
            return ((Executioner)role).TargetPlayer;
        else if (player.Is(LayerEnum.GuardianAngel))
            return ((GuardianAngel)role).TargetPlayer;
        else if (player.Is(LayerEnum.Guesser))
            return ((Guesser)role).TargetPlayer;
        else if (player.Is(LayerEnum.BountyHunter))
            return ((BountyHunter)role).TargetPlayer;

        return null;
    }

    public static Role GetLeader(this PlayerControl player)
    {
        if (!player.Is(LayerEnum.Mafioso) && !player.Is(LayerEnum.Sidekick))
            return null;

        var role = Role.GetRole(player);

        if (role == null)
            return null;

        if (player.Is(LayerEnum.Mafioso))
            return ((Mafioso)role).Godfather;
        else if (player.Is(LayerEnum.Sidekick))
            return ((Sidekick)role).Rebel;

        return null;
    }

    public static Role GetActorList(this PlayerControl player)
    {
        if (!player.Is(LayerEnum.Actor))
            return null;

        var role = Role.GetRole(player);

        if (role == null)
            return null;

        if (player.Is(LayerEnum.Actor))
            return ((Actor)role).TargetRole;

        return null;
    }

    public static bool IsGATarget(this PlayerVoteArea player) => PlayerByVoteArea(player).IsGATarget();

    public static bool IsExeTarget(this PlayerVoteArea player) => PlayerByVoteArea(player).IsExeTarget();

    public static bool IsBHTarget(this PlayerVoteArea player) => PlayerByVoteArea(player).IsBHTarget();

    public static bool IsGuessTarget(this PlayerVoteArea player) => PlayerByVoteArea(player).IsGuessTarget();

    public static bool IsTarget(this PlayerControl player) => player.IsBHTarget() || player.IsGuessTarget() || player.IsGATarget() || player.IsExeTarget();

    public static bool IsTarget(this PlayerVoteArea player) => player.IsBHTarget() || player.IsGuessTarget() || player.IsGATarget() || player.IsExeTarget();

    public static bool CanDoTasks(this PlayerVoteArea player) => PlayerByVoteArea(player).CanDoTasks();

    public static Jackal GetJackal(this PlayerControl player) => Role.GetRoles<Jackal>(LayerEnum.Jackal).Find(role => role.Recruited.Contains(player.PlayerId));

    public static Necromancer GetNecromancer(this PlayerControl player) => Role.GetRoles<Necromancer>(LayerEnum.Necromancer).Find(role => role.Resurrected.Contains(player.PlayerId));

    public static Dracula GetDracula(this PlayerControl player) => Role.GetRoles<Dracula>(LayerEnum.Dracula).Find(role => role.Converted.Contains(player.PlayerId));

    public static Whisperer GetWhisperer(this PlayerControl player) => Role.GetRoles<Whisperer>(LayerEnum.Whisperer).Find(role => role.Persuaded.Contains(player.PlayerId));

    public static Jackal GetJackal(this PlayerVoteArea player) => PlayerByVoteArea(player).GetJackal();

    public static Necromancer GetNecromancer(this PlayerVoteArea player) => PlayerByVoteArea(player).GetNecromancer();

    public static Dracula GetDracula(this PlayerVoteArea player) => PlayerByVoteArea(player).GetDracula();

    public static Whisperer GetWhisperer(this PlayerVoteArea player) => PlayerByVoteArea(player).GetWhisperer();

    public static bool IsShielded(this PlayerControl player) => Role.GetRoles<Medic>(LayerEnum.Medic).Any(role => player == role.ShieldedPlayer);

    public static bool IsKnighted(this PlayerControl player) => Role.GetRoles<Monarch>(LayerEnum.Monarch).Any(role => role.Knighted.Contains(player.PlayerId));

    public static bool IsSpelled(this PlayerControl player) => Role.GetRoles<Spellslinger>(LayerEnum.Spellslinger).Any(role => role.Spelled.Contains(player.PlayerId));

    public static bool IsArsoDoused(this PlayerControl player) => Role.GetRoles<Arsonist>(LayerEnum.Arsonist).Any(role => role.Doused.Contains(player.PlayerId));

    public static bool IsCryoDoused(this PlayerControl player) => Role.GetRoles<Cryomaniac>(LayerEnum.Cryomaniac).Any(role => role.Doused.Contains(player.PlayerId));

    public static bool IsProtectedMonarch(this PlayerControl player) => Role.GetRoles<Monarch>(LayerEnum.Monarch).Any(role => role.Protected && role.Player == player);

    public static bool IsFaithful(this PlayerControl player) => Role.GetRole(player).Faithful;

    public static bool IsBlackmailed(this PlayerControl player)
    {
        var bmFlag = Role.GetRoles<Blackmailer>(LayerEnum.Blackmailer).Any(role => role.BlackmailedPlayer == player);
        var gfFlag = Role.GetRoles<PromotedGodfather>(LayerEnum.PromotedGodfather).Any(role => role.BlackmailedPlayer == player);
        return bmFlag || gfFlag;
    }

    public static bool IsSilenced(this PlayerControl player)
    {
        var silFlag = Role.GetRoles<Silencer>(LayerEnum.Silencer).Any(role => role.SilencedPlayer == player);
        var rebFlag = Role.GetRoles<PromotedRebel>(LayerEnum.PromotedRebel).Any(role => role.SilencedPlayer == player);
        return silFlag || rebFlag;
    }

    public static Silencer GetSilencer(this PlayerControl player) => Role.GetRoles<Silencer>(LayerEnum.Silencer).Find(x => x.SilencedPlayer == player);

    public static PromotedRebel GetRebSilencer(this PlayerControl player) => Role.GetRoles<PromotedRebel>(LayerEnum.PromotedRebel).Find(x => x.SilencedPlayer == player && x.IsSil);

    public static bool IsRetShielded(this PlayerControl player) => Role.GetRoles<Retributionist>(LayerEnum.Retributionist).Any(x => player == x.ShieldedPlayer && x.IsMedic);

    public static bool IsShielded(this PlayerVoteArea player) => PlayerByVoteArea(player).IsShielded();

    public static bool IsKnighted(this PlayerVoteArea player) => PlayerByVoteArea(player).IsKnighted();

    public static bool IsSpelled(this PlayerVoteArea player) => PlayerByVoteArea(player).IsSpelled();

    public static bool IsArsoDoused(this PlayerVoteArea player) => PlayerByVoteArea(player).IsArsoDoused();

    public static bool IsCryoDoused(this PlayerVoteArea player) => PlayerByVoteArea(player).IsCryoDoused();

    public static bool IsProtectedMonarch(this PlayerVoteArea player) => PlayerByVoteArea(player).IsProtectedMonarch();

    public static bool IsRetShielded(this PlayerVoteArea player) => PlayerByVoteArea(player).IsRetShielded();

    public static Medic GetMedic(this PlayerControl player) => Role.GetRoles<Medic>(LayerEnum.Medic).Find(role => role.ShieldedPlayer == player);

    public static Retributionist GetRetMedic(this PlayerControl player) => Role.GetRoles<Retributionist>(LayerEnum.Retributionist).Find(role => player == role.ShieldedPlayer &&
        role.RevivedRole?.Type == LayerEnum.Medic);

    public static Crusader GetCrusader(this PlayerControl player) => Role.GetRoles<Crusader>(LayerEnum.Crusader).Find(role => player == role.CrusadedPlayer);

    public static PromotedRebel GetRebCrus(this PlayerControl player) => Role.GetRoles<PromotedRebel>(LayerEnum.PromotedRebel).Find(role => player == role.CrusadedPlayer);

    public static bool IsOnAlert(this PlayerControl player)
    {
        var vetFlag = Role.GetRoles<Veteran>(LayerEnum.Veteran).Any(role => role.AlertButton.EffectActive && player == role.Player);
        var retFlag = Role.GetRoles<Retributionist>(LayerEnum.Retributionist).Any(role => role.AlertButton.EffectActive && role.IsVet && player == role.Player);
        return vetFlag || retFlag;
    }

    public static bool IsVesting(this PlayerControl player) => Role.GetRoles<Survivor>(LayerEnum.Survivor).Any(role => role.VestButton.EffectActive && player == role.Player);

    public static bool IsMarked(this PlayerControl player) => Role.GetRoles<Ghoul>(LayerEnum.Ghoul).Any(role => player == role.MarkedPlayer);

    public static bool IsAmbushed(this PlayerControl player) => Role.GetRoles<Ambusher>(LayerEnum.Ambusher).Any(role => role.OnAmbush && player == role.AmbushedPlayer);

    public static bool IsGFAmbushed(this PlayerControl player) => Role.GetRoles<PromotedGodfather>(LayerEnum.PromotedGodfather).Any(role => role.AmbushButton.EffectActive && player ==
        role.AmbushedPlayer);

    public static bool IsCrusaded(this PlayerControl player) => Role.GetRoles<Crusader>(LayerEnum.Crusader).Any(role => role.CrusadeButton.EffectActive && player == role.CrusadedPlayer);

    public static bool IsRebCrusaded(this PlayerControl player) => Role.GetRoles<PromotedRebel>(LayerEnum.PromotedRebel).Any(role => role.CrusadeButton.EffectActive && player ==
        role.CrusadedPlayer);

    public static bool IsProtected(this PlayerControl player) => Role.GetRoles<GuardianAngel>(LayerEnum.GuardianAngel).Any(role => (role.ProtectButton.EffectActive ||
        role.GraveProtectButton.EffectActive) && player == role.TargetPlayer);

    public static bool IsInfected(this PlayerControl player) => Role.GetRoles<Plaguebearer>(LayerEnum.Plaguebearer).Any(role => role.Infected.Contains(player.PlayerId) || player ==
        role.Player);

    public static bool IsFramed(this PlayerControl player) => Role.GetRoles<Framer>(LayerEnum.Framer).Any(role => role.Framed.Contains(player.PlayerId));

    public static bool IsInfected(this PlayerVoteArea player) => PlayerByVoteArea(player).IsInfected();

    public static bool IsFramed(this PlayerVoteArea player) => PlayerByVoteArea(player).IsFramed();

    public static bool IsMarked(this PlayerVoteArea player) => PlayerByVoteArea(player).IsMarked();

    public static bool IsWinningRival(this PlayerControl player) => Objectifier.GetObjectifiers<Rivals>(LayerEnum.Rivals).Any(x => x.Player == player && x.RivalDead);

    public static bool IsTurnedTraitor(this PlayerControl player) => player.IsIntTraitor() || player.IsSynTraitor();

    public static bool IsTurnedFanatic(this PlayerControl player) => player.IsIntFanatic() || player.IsSynFanatic();

    public static bool IsTurnedTraitor(this PlayerVoteArea player) => PlayerByVoteArea(player).IsTurnedTraitor();

    public static bool IsTurnedFanatic(this PlayerVoteArea player) => PlayerByVoteArea(player).IsTurnedFanatic();

    public static bool IsUnturnedFanatic(this PlayerControl player) => !player.IsIntFanatic() && !player.IsSynFanatic();

    public static bool IsIntFanatic(this PlayerControl player) => Role.GetRole(player).IsIntFanatic;

    public static bool IsSynFanatic(this PlayerControl player) => Role.GetRole(player).IsSynFanatic;

    public static bool IsIntTraitor(this PlayerControl player) => Role.GetRole(player).IsIntTraitor;

    public static bool IsSynTraitor(this PlayerControl player) => Role.GetRole(player).IsSynTraitor;

    public static bool IsCrewAlly(this PlayerControl player) => Role.GetRole(player).IsCrewAlly;

    public static bool IsSynAlly(this PlayerControl player) => Role.GetRole(player).IsSynAlly;

    public static bool IsIntAlly(this PlayerControl player) => Role.GetRole(player).IsIntAlly;

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

    public static bool IsFlashed(this PlayerControl player) => !player.HasDied() && (Role.GetRoles<Grenadier>(LayerEnum.Grenadier).Any(x => x.FlashedPlayers.Contains(player)) ||
        Role.GetRoles<PromotedGodfather>(LayerEnum.PromotedGodfather).Any(x => x.IsGren && x.FlashedPlayers.Contains(player)));

    public static bool SyndicateSided(this PlayerControl player) => player.IsSynTraitor() || player.IsSynFanatic() || player.IsSynAlly() || (player.Is(Faction.Syndicate) &&
        player.Is(LayerEnum.Betrayer)) || player.IsSynDefect() || (player.Is(Faction.Syndicate) && Role.GetRole(player).BaseFaction != Faction.Syndicate);

    public static bool IntruderSided(this PlayerControl player) => player.IsIntTraitor() || player.IsIntAlly() || player.IsIntFanatic() || (player.Is(Faction.Intruder) &&
        player.Is(LayerEnum.Betrayer)) || player.IsIntDefect() || (player.Is(Faction.Intruder) && Role.GetRole(player).BaseFaction != Faction.Intruder);

    public static bool CrewSided(this PlayerControl player) => player.IsCrewAlly() || player.IsCrewDefect() || (player.Is(Faction.Crew) && Role.GetRole(player).BaseFaction != Faction.Crew);

    public static bool SyndicateSided(this PlayerVoteArea player) => player.IsSynTraitor() || player.IsSynFanatic() || player.IsSynAlly();

    public static bool IntruderSided(this PlayerVoteArea player) => player.IsIntTraitor() || player.IsIntAlly() || player.IsIntFanatic();

    public static bool CrewSided(this PlayerVoteArea player) => player.IsCrewAlly();

    public static bool Last(PlayerControl player) => (LastImp && player.Is(Faction.Intruder)) || (LastSyn && player.Is(Faction.Syndicate));

    public static bool CanKill(this PlayerControl player) => player.Is(Faction.Intruder) || player.Is(Faction.Syndicate) || player.Is(Alignment.NeutralKill) ||
        player.Is(Alignment.NeutralHarb) || player.Is(Alignment.NeutralApoc) || player.Is(LayerEnum.Corrupted) || player.Is(LayerEnum.Fanatic) || player.Is(LayerEnum.Traitor) ||
        player.Is(Alignment.CrewKill);

    public static bool IsPostmortal(this PlayerControl player) => player.GetRole() is LayerEnum.Revealer or LayerEnum.Phantom or LayerEnum.Ghoul or LayerEnum.Banshee;

    public static bool Caught(this PlayerControl player)
    {
        var role = Role.GetRole(player);

        if (role == null || !player.IsPostmortal())
            return false;

        if (player.Is(LayerEnum.Phantom))
            return ((Phantom)role).Caught;
        else if (player.Is(LayerEnum.Revealer))
            return ((Revealer)role).Caught;
        else if (player.Is(LayerEnum.Ghoul))
            return ((Ghoul)role).Caught;
        else if (player.Is(LayerEnum.Banshee))
            return ((Banshee)role).Caught;

        return true;
    }

    public static bool IsLinkedTo(this PlayerControl player, PlayerControl refplayer) => player.IsOtherRival(refplayer) || player.IsOtherLover(refplayer) || player.IsOtherLink(refplayer)
        || (player.Is(LayerEnum.Mafia) && refplayer.Is(LayerEnum.Mafia));

    public static float GetModifiedSpeed(this PlayerControl player) => player.IsMimicking(out var mimicked) ? player.GetSpeed() : mimicked.GetSpeed();

    public static float GetSpeed(this PlayerControl player)
    {
        var result = 1f;

        if (LobbyBehaviour.Instance || (HudUpdate.IsCamoed && CustomGameOptions.CamoHideSpeed))
            return result;

        if (player.Is(LayerEnum.Dwarf))
            result *= CustomGameOptions.DwarfSpeed;
        else if (player.Is(LayerEnum.Giant))
            result *= CustomGameOptions.GiantSpeed;
        else if (player.Is(LayerEnum.Drunk))
            result *= Modifier.GetModifier<Drunk>(player).Modify;

        if (player.IsDragging())
            result *= CustomGameOptions.DragModifier;

        if (Role.GetRoles<Drunkard>(LayerEnum.Drunkard).Any(x => x.ConfuseButton.EffectActive && (x.HoldsDrive || (x.ConfusedPlayer == player && !x.HoldsDrive))) ||
            Role.GetRoles<PromotedRebel>(LayerEnum.PromotedRebel).Any(x => x.ConfuseButton.EffectActive && (x.HoldsDrive || (x.ConfusedPlayer == player && !x.HoldsDrive)) && x.IsDrunk))
        {
            result *= -1;
        }

        if (Role.GetRoles<Timekeeper>(LayerEnum.Timekeeper).Any(x => x.TimeButton.EffectActive))
        {
            if (!player.Is(Faction.Syndicate) || (player.Is(Faction.Syndicate) && ((Role.GetRoles<Timekeeper>(LayerEnum.Timekeeper).Any(x => x.TimeButton.EffectActive && x.HoldsDrive) &&
                !CustomGameOptions.TimeRewindImmunity) || (Role.GetRoles<Timekeeper>(LayerEnum.Timekeeper).Any(x => x.TimeButton.EffectActive && !x.HoldsDrive) &&
                !CustomGameOptions.TimeFreezeImmunity))))
            {
                result *= 0;
            }
        }

        if (Role.GetRoles<PromotedRebel>(LayerEnum.PromotedRebel).Any(x => x.TimeButton.EffectActive))
        {
            if (!player.Is(Faction.Syndicate) || (player.Is(Faction.Syndicate) && ((Role.GetRoles<PromotedRebel>(LayerEnum.PromotedRebel).Any(x => x.TimeButton.EffectActive &&
                x.HoldsDrive && x.IsTK) && !CustomGameOptions.TimeRewindImmunity) || (Role.GetRoles<PromotedRebel>(LayerEnum.PromotedRebel).Any(x => x.TimeButton.EffectActive &&
                !x.HoldsDrive && x.IsTK) && !CustomGameOptions.TimeFreezeImmunity))))
            {
                result *= 0;
            }
        }

        if (ShipStatus.Instance != null && ShipStatus.Instance.Systems.ContainsKey(SystemTypes.LifeSupp))
        {
            var lifeSuppSystemType = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

            if (lifeSuppSystemType.IsActive && CustomGameOptions.OxySlow && !player.Data.IsDead)
                result *= Math.Clamp(lifeSuppSystemType.Countdown / lifeSuppSystemType.LifeSuppDuration, 0.25f, 1f);
        }

        return result;
    }

    public static bool IsDragging(this PlayerControl player) => (player.Is(LayerEnum.Janitor) && Role.GetRole<Janitor>(player).CurrentlyDragging) || (player.Is(LayerEnum.PromotedGodfather)
        && Role.GetRole<PromotedGodfather>(player).CurrentlyDragging);

    public static float GetModifiedSize(this PlayerControl player) => player.IsMimicking(out var mimicked) ? mimicked.GetSize() : player.GetSize();

    public static float GetSize(this PlayerControl player)
    {
        if (LobbyBehaviour.Instance || (HudUpdate.IsCamoed && CustomGameOptions.CamoHideSize))
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
        var role = Role.GetRole(player);
        mimicked = player;

        if (!role || !CachedMorphs.ContainsKey(player.PlayerId))
            return false;

        mimicked = PlayerById(CachedMorphs[player.PlayerId]);
        return true;
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
        else if (playerInfo.IsDead && !player.IsPostmortal())
            return false;
        else if (playerInfo.IsDead && player.IsPostmortal())
            return !player.Caught();

        var playerRole = Role.GetRole(player);
        var mainflag = false;

        if (playerRole == null)
            mainflag = playerInfo.IsImpostor();
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
        else if (player.Is(Faction.Intruder))
        {
            var flag = (player.Is(LayerEnum.Morphling) && !CustomGameOptions.MorphlingVent) || (player.Is(LayerEnum.Wraith) && !CustomGameOptions.WraithVent) ||
                (player.Is(LayerEnum.Grenadier) && !CustomGameOptions.GrenadierVent) || (player.Is(LayerEnum.Teleporter) && !CustomGameOptions.TeleVent);

            mainflag = CustomGameOptions.IntrudersVent;

            if (mainflag)
            {
                if (player.Is(LayerEnum.Janitor))
                {
                    var janitor = (Janitor)playerRole;
                    mainflag = (int)CustomGameOptions.JanitorVentOptions is 3 || (janitor.CurrentlyDragging && (int)CustomGameOptions.JanitorVentOptions is 1) || (!janitor.CurrentlyDragging
                        && (int)CustomGameOptions.JanitorVentOptions is 2);
                }
                else if (player.Is(LayerEnum.PromotedGodfather))
                {
                    var gf = (PromotedGodfather)playerRole;

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
                CustomGameOptions.GAVent) || (player.Is(LayerEnum.Amnesiac) && CustomGameOptions.AmneVent) || (player.Is(LayerEnum.Werewolf) && CustomGameOptions.WerewolfVent) ||
                (player.Is(LayerEnum.Jackal) && CustomGameOptions.JackalVent) || (player.Is(LayerEnum.BountyHunter) && CustomGameOptions.BHVent)) && CustomGameOptions.NeutralsVent;

            if (player.Is(LayerEnum.SerialKiller))
            {
                var role2 = (SerialKiller)playerRole;
                mainflag = (int)CustomGameOptions.SKVentOptions is 0 || (role2.BloodlustButton.EffectActive && (int)CustomGameOptions.SKVentOptions is 1) ||
                    (!role2.BloodlustButton.EffectActive && (int)CustomGameOptions.SKVentOptions is 2);
            }
        }
        else if (player.Is(LayerEnum.Betrayer))
            mainflag = CustomGameOptions.BetrayerVent;
        else if (player.IsPostmortal() && player.inVent)
            mainflag = true;

        return mainflag;
    }

    public static bool CanChat(this PlayerControl player)
    {
        var playerInfo = player?.Data;

        if (player == null || playerInfo == null)
            return false;
        else if (playerInfo.IsDead || Meeting || LobbyBehaviour.Instance)
            return true;
        else if (player.Is(LayerEnum.Lovers))
            return CustomGameOptions.LoversChat;
        else if (player.Is(LayerEnum.Rivals))
            return CustomGameOptions.RivalsChat;
        else if (player.Is(LayerEnum.Linked))
            return CustomGameOptions.LinkedChat;
        else
            return false;
    }

    public static InspectorResults GetInspResults(this PlayerControl player)
    {
        if (player == null)
            return InspectorResults.None;

        var role = Role.GetRole(player);

        if (role == null)
            return InspectorResults.None;

        return role.InspectorResults;
    }

    public static InspectorResults GetInspResults(this PlayerVoteArea player) => PlayerByVoteArea(player).GetInspResults();

    public static bool IsBlocked(this PlayerControl player) => PlayerLayer.GetLayers(player).Any(x => x.IsBlocked);

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

    public static bool IsBlockImmune(PlayerControl player) => Role.GetRole(player).RoleBlockImmune;

    public static List<PlayerLayer> AllPlayerInfo(this PlayerControl player) => new()
    {
        Role.GetRole(player),
        Modifier.GetModifier(player),
        Ability.GetAbility(player),
        Objectifier.GetObjectifier(player)
    };

    public static List<PlayerLayer> AllPlayerInfo(this PlayerVoteArea player) => PlayerByVoteArea(player).AllPlayerInfo();

    public static PlayerControl GetOtherLover(this PlayerControl player)
    {
        if (!player.Is(LayerEnum.Lovers))
            return null;

        return Objectifier.GetObjectifier<Lovers>(player).OtherLover;
    }

    public static PlayerControl GetOtherRival(this PlayerControl player)
    {
        if (!player.Is(LayerEnum.Rivals))
            return null;

        return Objectifier.GetObjectifier<Rivals>(player).OtherRival;
    }

    public static PlayerControl GetOtherLink(this PlayerControl player)
    {
        if (!player.Is(LayerEnum.Linked))
            return null;

        return Objectifier.GetObjectifier<Linked>(player).OtherLink;
    }

    public static bool NeutralHasUnfinishedBusiness(PlayerControl player)
    {
        if (player.Is(LayerEnum.GuardianAngel))
        {
            var ga = Role.GetRole<GuardianAngel>(player);
            return ga.TargetAlive;
        }
        else if (player.Is(LayerEnum.Executioner))
        {
            var exe = Role.GetRole<Executioner>(player);
            return exe.TargetVotedOut;
        }
        else if (player.Is(LayerEnum.Jester))
        {
            var jest = Role.GetRole<Jester>(player);
            return jest.VotedOut;
        }
        else if (player.Is(LayerEnum.Guesser))
        {
            var guess = Role.GetRole<Guesser>(player);
            return guess.TargetGuessed;
        }
        else if (player.Is(LayerEnum.BountyHunter))
        {
            var bh = Role.GetRole<BountyHunter>(player);
            return bh.TargetKilled;
        }
        else if (player.Is(LayerEnum.Actor))
        {
            var act = Role.GetRole<Actor>(player);
            return act.Guessed;
        }
        else if (player.Is(LayerEnum.Troll))
        {
            var troll = Role.GetRole<Troll>(player);
            return troll.Killed;
        }

        return false;
    }

    public static string RoleCardInfo(this PlayerControl player)
    {
        var info = player.AllPlayerInfo();

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

        if (info[0])
        {
            roleName += $"{role.ColorString}{role}</color>";
            objectives += $"\n{role.ColorString}{role.Objectives()}</color>";
            alignment += $"{role.Alignment.AlignmentName(true)}";
            subfaction += $"{role.SubFactionColorString}{role.SubFactionName} {role.SubFactionSymbol}</color>";
        }
        else
        {
            roleName += "None";
            alignment += "None";
            subfaction += "None";
        }

        roleName += "</b></color>";
        alignment += "</b></color>";
        subfaction += "</b></color>";

        if (info[3] && !objectifier.Hidden)
        {
            objectives += $"\n{objectifier.ColorString}{objectifier.Description()}</color>";
            objectifierName += $"{objectifier.ColorString}{objectifier.Name} {objectifier.Symbol}</color>";
        }
        else
            objectifierName += "None φ";

        objectifierName += "</b></color>";

        if (info[2] && !ability.Hidden && ability.Type != LayerEnum.None)
            abilityName += $"{ability.ColorString}{ability.Name}</color>";
        else
            abilityName += "None";

        abilityName += "</b></color>";

        if (info[1] && !modifier.Hidden && modifier.Type != LayerEnum.None)
            modifierName += $"{modifier.ColorString}{modifier.Name}</color>";
        else
            modifierName += "None";

        modifierName += "</b></color>";

        if (player.IsRecruit())
        {
            var jackal = player.GetJackal();
            objectives += $"\n<color=#{Colors.Cabal.ToHtmlStringRGBA()}>- You are a member of the Cabal. Help {jackal.PlayerName} in taking over the mission $</color>";
        }
        else if (player.IsResurrected())
        {
            var necromancer = player.GetNecromancer();
            objectives += $"\n<color=#{Colors.Reanimated.ToHtmlStringRGBA()}>- You are a member of the Reanimated. Help {necromancer.PlayerName} in taking over the mission Σ</color>";
        }
        else if (player.IsPersuaded())
        {
            var whisperer = player.GetWhisperer();
            objectives += $"\n<color=#{Colors.Sect.ToHtmlStringRGBA()}>- You are a member of the Sect. Help {whisperer.PlayerName} in taking over the mission Λ</color>";
        }
        else if (player.IsBitten())
        {
            var dracula = player.GetDracula();
            objectives += $"\n<color=#{Colors.Undead.ToHtmlStringRGBA()}>- You are a member of the Undead. Help {dracula.PlayerName} in taking over the mission γ</color>";
            abilities += $"\n{role.ColorString}- Attempting to interact with a <color=#C0C0C0FF>Vampire Hunter</color> will force them to kill you</color>";
        }

        objectives += "</color>";

        if (info[0] && role.Description() is not ("" or "- None"))
            abilities += $"\n{role.ColorString}{role.Description()}</color>";

        if (info[2] && !ability.Hidden && ability.Type != LayerEnum.None && ability.Description() is not ("" or "- None"))
            abilities += $"\n{ability.ColorString}{ability.Description()}</color>";

        if (abilities == $"{AbilitiesColorString}Abilities:")
            abilities = "";
        else
            abilities = $"\n{abilities}</color>";

        if (info[1] && !modifier.Hidden && modifier.Type != LayerEnum.None && modifier.Description() is not ("" or "- None"))
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
            attributes += "\n<color=#FF0000FF>- You are dead</color>";

        if (attributes == $"{AttributesColorString}Attributes:")
            attributes = "";
        else
            attributes = $"\n{attributes}</color>";

        return $"{roleName}\n{alignment}\n{subfaction}\n{objectifierName}\n{abilityName}\n{modifierName}\n{objectives}{abilities}{attributes}";
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
        CustomButton.AllButtons.Where(x => x.Owner == former || x.Owner.Player == null).ForEach(x => x.Destroy());
        CustomArrow.AllArrows.Where(x => x.Owner == former.Player).ForEach(x => x.Disable());
        former.OnLobby();
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
        newRole.IsBitten = former.IsBitten;
        newRole.IsRecruit = former.IsRecruit;
        newRole.IsResurrected = former.IsResurrected;
        newRole.IsPersuaded = former.IsPersuaded;
        newRole.IsIntFanatic = former.IsIntFanatic;
        newRole.IsIntTraitor = former.IsIntTraitor;
        newRole.IsSynFanatic = former.IsSynFanatic;
        newRole.IsSynTraitor = former.IsSynTraitor;
        newRole.IsIntAlly = former.IsIntAlly;
        newRole.IsSynAlly = former.IsSynAlly;
        newRole.IsCrewAlly = former.IsCrewAlly;
        newRole.IsBlocked = former.IsBlocked;
        newRole.Diseased = former.Diseased;
        newRole.IsIntDefect = former.IsIntDefect;
        newRole.IsSynDefect = former.IsSynDefect;
        newRole.IsCrewDefect = former.IsCrewDefect;
        newRole.IsNeutDefect = former.IsNeutDefect;
        newRole.AllArrows = former.AllArrows;
        newRole.SubFactionSymbol = former.SubFactionSymbol;
        newRole.RoleHistory.Add(former);
        newRole.RoleHistory.AddRange(former.RoleHistory);
        Role.AllRoles.Remove(former);
        PlayerLayer.AllLayers.Remove(former);

        if (newRole.Local || former.Local)
        {
            ButtonUtils.ResetCustomTimers();
            Flash(newRole.Color);
        }

        if (CustomPlayer.Local.Is(LayerEnum.Seer))
            Flash(Colors.Seer);
    }

    public static void SetImpostor(this GameData.PlayerInfo player, bool impostor)
    {
        if (player == null)
            return;

        player.Role.TeamType = impostor ? RoleTeamTypes.Impostor : RoleTeamTypes.Crewmate;
        var imp = player.IsDead ? RoleTypes.ImpostorGhost : RoleTypes.Impostor;
        var crew = player.IsDead ? RoleTypes.CrewmateGhost : RoleTypes.Crewmate;
        RoleManager.Instance.SetRole(player.Object, impostor ? imp : crew);
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
        Alignment.IntruderSupport => withColors ? "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Support</color>)</color>" : "Intruder (Support)",
        Alignment.IntruderConceal => withColors ? "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Concealing</color>)</color>" : "Intruder (Concealing)",
        Alignment.IntruderDecep => withColors ? "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Deception</color>)</color>" : "Intuder (Deception)",
        Alignment.IntruderKill => withColors ? "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Killing</color>)</color>" : "Intruder (Killing)",
        Alignment.IntruderUtil => withColors ? "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Utility</color>)</color>" : "Intruder (Utility)",
        Alignment.IntruderInvest => withColors ? "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Investigative</color>)</color>" : "Intruder (Investigative)",
        Alignment.IntruderAudit => withColors ? "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Auditor</color>)</color>" : "Intruder (Auditor)",
        Alignment.IntruderProt => withColors ? "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Protective</color>)</color>" : "Intruder (Protective)",
        Alignment.IntruderSov => withColors ? "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Sovereign</color>)</color>" : "Intruder (Sovereign)",
        Alignment.IntruderDisrup=> withColors ? "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Disruption</color>)</color>" : "Intruder (Disruption)",
        Alignment.IntruderPower => withColors ? "<color=#FF0000FF>Intruder (<color=#1D7CF2FF>Power</color>)</color>" : "Intruder (Power)",
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
        Alignment.NeutralDisrup => withColors ? "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Apocalypse</color>)</color>" : "Neutral (Disruption)",
        Alignment.NeutralApoc => withColors ? "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Disruption</color>)</color>" : "Neutral (Apocalypse)",
        Alignment.NeutralHarb => withColors ? "<color=#B3B3B3FF>Neutral (<color=#1D7CF2FF>Harbinger</color>)</color>" : "Neutral (Harbinger)",
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
        Alignment.None => "Invalid",
        _ => "Invalid",
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
                _ => alignment
            };
        }

        return alignment;
    }

    public static bool CanButton(this PlayerControl player, out string name)
    {
        name = "Shy";

        if (player.Is(LayerEnum.Mayor))
        {
            name = "Mayor";
            return CustomGameOptions.MayorButton;
        }
        else if (player.Is(LayerEnum.Jester))
        {
            name = "Jester";
            return CustomGameOptions.JesterButton;
        }
        else if (player.Is(LayerEnum.Swapper))
        {
            name = "Swapper";
            return CustomGameOptions.SwapperButton;
        }
        else if (player.Is(LayerEnum.Actor))
        {
            name = "Actor";
            return CustomGameOptions.ActorButton;
        }
        else if (player.Is(LayerEnum.Executioner))
        {
            name = "Executioner";
            return CustomGameOptions.ExecutionerButton;
        }
        else if (player.Is(LayerEnum.Guesser))
        {
            name = "Guesser";
            return CustomGameOptions.GuesserButton;
        }
        else if (player.Is(LayerEnum.Politician))
        {
            name = "Politician";
            return CustomGameOptions.PoliticianButton;
        }
        else if (player.Is(LayerEnum.Dictator))
        {
            name = "Dictator";
            return CustomGameOptions.DictatorButton;
        }
        else if (player.Is(LayerEnum.Monarch))
        {
            name = "Monarch";
            return CustomGameOptions.MonarchButton;
        }
        else if (player.IsKnighted())
        {
            name = "Knight";
            return CustomGameOptions.KnightButton;
        }

        return !player.Is(LayerEnum.Shy) && player.RemainingEmergencies > 0;
    }
}