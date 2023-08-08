namespace TownOfUsReworked.Custom;

[HarmonyPatch]
public static class ButtonUtils
{
    private static bool Use;

    public static void DisableButtons(this PlayerControl player)
    {
        foreach (var button in CustomButton.AllButtons.Where(x => x.Owner.Player == player))
            button.Disable();

        HUD.KillButton.gameObject.SetActive(false);
        HUD.SabotageButton.gameObject.SetActive(false);
        HUD.ReportButton.gameObject.SetActive(false);
        HUD.ImpostorVentButton.gameObject.SetActive(false);
        Use = HUD.UseButton.isActiveAndEnabled;

        if (Use)
            HUD.UseButton.gameObject.SetActive(false);
        else
            HUD.PetButton.gameObject.SetActive(false);
    }

    public static void EnableButtons(this PlayerControl player)
    {
        foreach (var button in CustomButton.AllButtons.Where(x => x.Owner.Player == player))
            button.Enable();

        HUD.KillButton.gameObject.SetActive(false);
        HUD.SabotageButton.gameObject.SetActive(player.CanSabotage());
        HUD.ReportButton.gameObject.SetActive(!player.Is(LayerEnum.Coward));
        HUD.ImpostorVentButton.gameObject.SetActive(player.CanVent());

        if (Use)
            HUD.UseButton.gameObject.SetActive(true);
        else
            HUD.PetButton.gameObject.SetActive(true);
    }

    public static void DisableAllButtons()
    {
        CustomButton.AllButtons.ForEach(x => x.Disable());
        HUD.KillButton.gameObject.SetActive(false);
        HUD.SabotageButton.gameObject.SetActive(false);
        HUD.ReportButton.gameObject.SetActive(false);
        HUD.ImpostorVentButton.gameObject.SetActive(false);
        Use = HUD.UseButton.isActiveAndEnabled;

        if (Use)
            HUD.UseButton.gameObject.SetActive(false);
        else
            HUD.PetButton.gameObject.SetActive(false);
    }

    public static void DestroyButtons(this PlayerControl player) => CustomButton.AllButtons.Where(x => x.Owner.Player == player).ToList().ForEach(x => x.Destroy());

    public static bool CannotUse(this PlayerControl player) => player.onLadder || player.IsBlocked() || (player.inVent && !CustomGameOptions.VentTargetting) || player.inMovingPlat;

    public static float GetModifiedCooldown(this PlayerControl player, float cooldown, float difference = 0f, float factor = 1f)
    {
        var result = (cooldown * factor * player.GetMultiplier()) + difference + player.GetDifference();

        if (result <= 0f)
            result = 0.1f;

        return result;
    }

    public static float GetUnderdogChange(this PlayerControl player)
    {
        if (!player.Is(LayerEnum.Underdog))
            return 0f;

        var last = Last(player);
        var lowerKC = -CustomGameOptions.UnderdogKillBonus;
        var upperKC = CustomGameOptions.UnderdogKillBonus;

        if (CustomGameOptions.UnderdogIncreasedKC && !last)
            return upperKC;
        else if (last)
            return lowerKC;
        else
            return 0f;
    }

    public static float GetDifference(this PlayerControl player)
    {
        var result = 0f;
        result += player.GetUnderdogChange();
        return result;
    }

    public static float GetMultiplier(this PlayerControl player)
    {
        var num = 1f;

        if (player.Is(LayerEnum.PromotedGodfather))
            num *= CustomGameOptions.MafiosoAbilityCooldownDecrease;
        if (player.Is(LayerEnum.PromotedRebel))
            num *= CustomGameOptions.SidekickAbilityCooldownDecrease;

        if (player.Diseased())
            num *= CustomGameOptions.DiseasedMultiplier;

        return num;
    }

    public static void ResetCustomTimers(bool start = false, bool meeting = false)
    {
        var local = CustomPlayer.Local;
        var role = Role.LocalRole;
        RoundOne = start;

        if (role.Requesting && !start)
            role.BountyTimer++;

        if (!start && Role.SyndicateHasChaosDrive)
            RoleGen.AssignChaosDrive();

        if (local.Is(LayerEnum.Chameleon))
        {
            var role2 = (Chameleon)role;

            if (start)
                role2.LastSwooped = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SwoopCooldown);
            else if (meeting)
                role2.LastSwooped = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.SwoopCooldown);
            else
                role2.LastSwooped = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Detective))
        {
            var role2 = (Detective)role;

            if (start)
                role2.LastExamined = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ExamineCd);
            else if (meeting)
                role2.LastExamined = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ExamineCd);
            else
                role2.LastExamined = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Escort))
        {
            var role2 = (Escort)role;
            role2.BlockTarget = null;

            if (start)
                role2.LastBlocked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.EscRoleblockCooldown);
            else if (meeting)
                role2.LastBlocked = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.EscRoleblockCooldown);
            else
                role2.LastBlocked = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Inspector))
        {
            var role2 = (Inspector)role;

            if (start)
                role2.LastInspected = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InspectCooldown);
            else if (meeting)
                role2.LastInspected = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.InspectCooldown);
            else
                role2.LastInspected = DateTime.UtcNow;

            if (local.Data.IsDead && DeadSeeEverything)
                role2.Inspected.Clear();
        }
        else if (local.Is(LayerEnum.Coroner))
        {
            var role2 = (Coroner)role;

            if (start)
            {
                role2.LastCompared = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CompareCooldown);
                role2.LastAutopsied = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.AutopsyCooldown);
            }
            else if (meeting)
            {
                role2.LastCompared = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.CompareCooldown);
                role2.LastAutopsied = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.AutopsyCooldown);
            }
            else
            {
                role2.LastCompared = DateTime.UtcNow;
                role2.LastAutopsied = DateTime.UtcNow;
            }
        }
        else if (local.Is(LayerEnum.Medium))
        {
            var role2 = (Medium)role;
            role2.OnLobby();

            if (start)
                role2.LastMediated = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MediateCooldown);
            else if (meeting)
                role2.LastMediated = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.MediateCooldown);
            else
                role2.LastMediated = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Operative))
        {
            var role2 = (Operative)role;
            role2.BuggedPlayers.Clear();

            if (start)
                role2.LastBugged = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BugCooldown);
            else if (meeting)
                role2.LastBugged = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.BugCooldown);
            else
                role2.LastBugged = DateTime.UtcNow;

            if (CustomGameOptions.BugsRemoveOnNewRound)
                Bug.Clear(role2.Bugs);
        }
        else if (local.Is(LayerEnum.Sheriff))
        {
            var role2 = (Sheriff)role;

            if (start)
                role2.LastInterrogated = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InterrogateCd);
            else if (meeting)
                role2.LastInterrogated = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.InterrogateCd);
            else
                role2.LastInterrogated = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Shifter))
        {
            var role2 = (Shifter)role;

            if (start)
                role2.LastShifted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ShifterCd);
            else if (meeting)
                role2.LastShifted = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ShifterCd);
            else
                role2.LastShifted = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Tracker))
        {
            var role2 = (Tracker)role;

            if (start)
                role2.LastTracked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TrackCd);
            else if (meeting)
                role2.LastTracked = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.TrackCd);
            else
                role2.LastTracked = DateTime.UtcNow;

            if (CustomGameOptions.ResetOnNewRound)
            {
                role2.UsesLeft = CustomGameOptions.MaxTracks + (role.TasksDone ? 1 : 0);
                role2.OnLobby();
            }
        }
        else if (local.Is(LayerEnum.Transporter))
        {
            var role2 = (Transporter)role;
            role2.TransportPlayer1 = null;
            role2.TransportPlayer2 = null;

            if (start)
                role2.LastTransported = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TransportCooldown);
            else if (meeting)
                role2.LastTransported = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.TransportCooldown);
            else
                role2.LastTransported = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Altruist))
        {
            var role2 = (Altruist)role;

            if (start)
                role2.LastRevived = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ReviveCooldown);
            else if (meeting)
                role2.LastRevived = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ReviveCooldown);
            else
                role2.LastRevived = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.VampireHunter))
        {
            var role2 = (VampireHunter)role;

            if (start)
                role2.LastStaked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.StakeCooldown);
            else if (meeting)
                role2.LastStaked = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.StakeCooldown);
            else
                role2.LastStaked = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Veteran))
        {
            var role2 = (Veteran)role;

            if (start)
                role2.LastAlerted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.AlertCd);
            else if (meeting)
                role2.LastAlerted = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.AlertCd);
            else
                role2.LastAlerted = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Mystic))
        {
            var role2 = (Mystic)role;

            if (start)
                role2.LastRevealed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.RevealCooldown);
            else if (meeting)
                role2.LastRevealed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.RevealCooldown);
            else
                role2.LastRevealed = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Seer))
        {
            var role2 = (Seer)role;

            if (start)
                role2.LastSeered = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SeerCooldown);
            else if (meeting)
                role2.LastSeered = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.SeerCooldown);
            else
                role2.LastSeered = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Vigilante))
        {
            var role2 = (Vigilante)role;
            role2.RoundOne = start && CustomGameOptions.RoundOneNoShot;

            if (start)
                role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.VigiKillCd);
            else if (meeting)
                role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.VigiKillCd);
            else
                role2.LastKilled = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Mayor))
        {
            var role2 = (Mayor)role;
            role2.RoundOne = start && CustomGameOptions.RoundOneNoReveal;
        }
        else if (local.Is(LayerEnum.Monarch))
        {
            var role2 = (Monarch)role;
            role2.RoundOne = start && CustomGameOptions.RoundOneNoKnighting;

            if (start)
                role2.LastKnighted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.KnightingCooldown);
            else if (meeting)
                role2.LastKnighted = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.KnightingCooldown);
            else
                role2.LastKnighted = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Retributionist))
        {
            var role2 = (Retributionist)role;
            role2.BuggedPlayers.Clear();
            role2.BlockTarget = null;
            role2.TransportPlayer1 = null;
            role2.TransportPlayer2 = null;

            if (role2.RevivedRole)
            {
                role2.LastSwooped = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.SwoopCooldown);
                role2.LastExamined = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ExamineCd);
                role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.VigiKillCd);
                role2.LastStaked = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.StakeCooldown);
                role2.LastAutopsied = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.AutopsyCooldown);
                role2.LastCompared = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.CompareCooldown);
                role2.LastAlerted = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.AlertCd);
                role2.LastTracked = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.TrackCd);
                role2.LastInterrogated = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.InterrogateCd);
                role2.LastMediated = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.MediateCooldown);
                role2.LastBugged = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.BugCooldown);
                role2.LastInspected = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.InspectCooldown);
                role2.LastBlocked = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.EscRoleblockCooldown);
                role2.LastTransported = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.TransportCooldown);
            }
            else
            {
                role2.LastSwooped = DateTime.UtcNow;
                role2.LastExamined = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
                role2.LastStaked = DateTime.UtcNow;
                role2.LastAutopsied = DateTime.UtcNow;
                role2.LastCompared = DateTime.UtcNow;
                role2.LastAlerted = DateTime.UtcNow;
                role2.LastTracked = DateTime.UtcNow;
                role2.LastInterrogated = DateTime.UtcNow;
                role2.LastMediated = DateTime.UtcNow;
                role2.LastBugged = DateTime.UtcNow;
                role2.LastInspected = DateTime.UtcNow;
                role2.LastBlocked = DateTime.UtcNow;
                role2.LastTransported = DateTime.UtcNow;
            }

            if (local.Data.IsDead && DeadSeeEverything)
                role2.Inspected.Clear();

            if (CustomGameOptions.BugsRemoveOnNewRound && meeting)
                Bug.Clear(role2.Bugs);

            if (CustomGameOptions.ResetOnNewRound)
            {
                role2.TrackerArrows.Values.ToList().DestroyAll();
                role2.TrackerArrows.Clear();
                role2.UsesLeft++;
            }

            if (role2.IsMed)
                role2.OnLobby();
        }
        else if (local.Is(LayerEnum.Blackmailer))
        {
            var role2 = (Blackmailer)role;
            role2.BlackmailedPlayer = null;

            if (start)
                role2.LastBlackmailed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BlackmailCd);
            else if (meeting)
                role2.LastBlackmailed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.BlackmailCd);
            else
                role2.LastBlackmailed = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Camouflager))
        {
            var role2 = (Camouflager)role;

            if (start)
                role2.LastCamouflaged = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CamouflagerCd);
            else if (meeting)
                role2.LastCamouflaged = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.CamouflagerCd);
            else
                role2.LastCamouflaged = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Enforcer))
        {
            var role2 = (Enforcer)role;
            role2.BombedPlayer = null;

            if (start)
                role2.LastBombed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.EnforceCooldown);
            else if (meeting)
                role2.LastBombed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.EnforceCooldown);
            else
                role2.LastBombed = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Consigliere))
        {
            var role2 = (Consigliere)role;

            if (start)
                role2.LastInvestigated = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConsigCd);
            else if (meeting)
                role2.LastInvestigated = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ConsigCd);
            else
                role2.LastInvestigated = DateTime.UtcNow;

            if (local.Data.IsDead && DeadSeeEverything)
                role2.Investigated.Clear();
        }
        else if (local.Is(LayerEnum.Consort))
        {
            var role2 = (Consort)role;
            role2.BlockTarget = null;

            if (start)
                role2.LastBlocked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConsRoleblockCooldown);
            else if (meeting)
                role2.LastBlocked = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ConsRoleblockCooldown);
            else
                role2.LastBlocked = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Disguiser))
        {
            var role2 = (Disguiser)role;
            role2.MeasuredPlayer = null;
            role2.DisguisedPlayer = null;
            role2.CopiedPlayer = null;

            if (start)
            {
                role2.LastDisguised = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DisguiseCooldown);
                role2.LastMeasured = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MeasureCooldown);
            }
            else if (meeting)
            {
                role2.LastDisguised = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.DisguiseCooldown);
                role2.LastMeasured = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.MeasureCooldown);
            }
            else
            {
                role2.LastDisguised = DateTime.UtcNow;
                role2.LastMeasured = DateTime.UtcNow;
            }
        }
        else if (local.Is(LayerEnum.PromotedGodfather))
        {
            var role2 = (PromotedGodfather)role;
            role2.BlackmailedPlayer = null;
            role2.BlockTarget = null;
            role2.MeasuredPlayer = null;
            role2.DisguisedPlayer = null;
            role2.CopiedPlayer = null;
            role2.SampledPlayer = null;
            role2.MorphedPlayer = null;
            role2.AmbushedPlayer = null;
            role2.TeleportPoint = Vector3.zero;

            if (!(role2.FormerRole == null || role2.IsImp || start))
            {
                role2.LastBlackmailed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.BlackmailCd);
                role2.LastCamouflaged = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.CamouflagerCd);
                role2.LastInvestigated = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ConsigCd);
                role2.LastDisguised = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.DisguiseCooldown);
                role2.LastMeasured = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.MeasureCooldown);
                role2.LastFlashed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.GrenadeCd);
                role2.LastMined = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.MineCd);
                role2.LastCleaned = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.JanitorCleanCd);
                role2.LastDragged = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.DragCd);
                role2.LastMorphed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.MorphlingCd);
                role2.LastSampled = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.SampleCooldown);
                role2.LastTeleported = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.TeleportCd);
                role2.LastInvis = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.InvisCd);
                role2.LastAmbushed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.AmbushCooldown);
                role2.LastBlocked = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ConsRoleblockCooldown);
            }
            else
            {
                role2.LastBlackmailed = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
                role2.LastCamouflaged = DateTime.UtcNow;
                role2.LastInvestigated = DateTime.UtcNow;
                role2.LastDisguised = DateTime.UtcNow;
                role2.LastMeasured = DateTime.UtcNow;
                role2.LastFlashed = DateTime.UtcNow;
                role2.LastMined = DateTime.UtcNow;
                role2.LastCleaned = DateTime.UtcNow;
                role2.LastDragged = DateTime.UtcNow;
                role2.LastMorphed = DateTime.UtcNow;
                role2.LastSampled = DateTime.UtcNow;
                role2.LastTeleported = DateTime.UtcNow;
                role2.LastInvis = DateTime.UtcNow;
                role2.LastAmbushed = DateTime.UtcNow;
                role2.LastBlocked = DateTime.UtcNow;
            }

            if (local.Data.IsDead && DeadSeeEverything)
                role2.Investigated.Clear();
        }
        else if (local.Is(LayerEnum.Grenadier))
        {
            var role2 = (Grenadier)role;

            if (start)
                role2.LastFlashed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.GrenadeCd);
            else if (meeting)
                role2.LastFlashed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.GrenadeCd);
            else
                role2.LastFlashed = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Janitor))
        {
            var role2 = (Janitor)role;
            role2.CurrentlyDragging = null;

            if (start)
            {
                role2.LastCleaned = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.JanitorCleanCd);
                role2.LastDragged = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DragCd);
            }
            else if (meeting)
            {
                role2.LastCleaned = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.JanitorCleanCd);
                role2.LastDragged = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.DragCd);
            }
            else
            {
                role2.LastCleaned = DateTime.UtcNow;
                role2.LastDragged = DateTime.UtcNow;
            }
        }
        else if (local.Is(LayerEnum.Miner))
        {
            var role2 = (Miner)role;

            if (start)
                role2.LastMined = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MineCd);
            else if (meeting)
                role2.LastMined = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.MineCd);
            else
                role2.LastMined = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Morphling))
        {
            var role2 = (Morphling)role;
            role2.SampledPlayer = null;
            role2.MorphedPlayer = null;

            if (start)
            {
                role2.LastMorphed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MorphlingCd);
                role2.LastSampled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SampleCooldown);
            }
            else if (meeting)
            {
                role2.LastMorphed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.MorphlingCd);
                role2.LastSampled = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.SampleCooldown);
            }
            else
            {
                role2.LastMorphed = DateTime.UtcNow;
                role2.LastSampled = DateTime.UtcNow;
            }
        }
        else if (local.Is(LayerEnum.Teleporter))
        {
            var role2 = (Teleporter)role;
            role2.TeleportPoint = Vector3.zero;

            if (start)
            {
                role2.LastTeleported = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TeleportCd);
                role2.LastMarked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MarkCooldown);
            }
            else if (meeting)
            {
                role2.LastMarked = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.MarkCooldown);
                role2.LastTeleported = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.TeleportCd);
            }
            else
            {
                role2.LastTeleported = DateTime.UtcNow;
                role2.LastMarked = DateTime.UtcNow;
            }
        }
        else if (local.Is(LayerEnum.Wraith))
        {
            var role2 = (Wraith)role;

            if (start)
                role2.LastInvis = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InvisCd);
            else if (meeting)
                role2.LastInvis = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.InvisCd);
            else
                role2.LastInvis = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Ambusher))
        {
            var role2 = (Ambusher)role;
            role2.AmbushedPlayer = null;

            if (start)
                role2.LastAmbushed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.AmbushCooldown);
            else if (meeting)
                role2.LastAmbushed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.AmbushCooldown);
            else
                role2.LastAmbushed = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Ghoul))
        {
            var role2 = (Ghoul)role;

            if (!role2.Caught)
            {
                if (meeting)
                    role2.LastMarked = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.GhoulMarkCd);
                else
                    role2.LastMarked = DateTime.UtcNow;
            }
        }
        else if (local.Is(LayerEnum.Concealer))
        {
            var role2 = (Concealer)role;
            role2.ConcealedPlayer = null;

            if (start)
                role2.LastConcealed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConcealCooldown);
            else if (meeting)
                role2.LastConcealed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ConcealCooldown);
            else
                role2.LastConcealed = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Silencer))
        {
            var role2 = (Silencer)role;
            role2.SilencedPlayer = null;

            if (start)
                role2.LastSilenced = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SilenceCooldown);
            else if (meeting)
                role2.LastSilenced = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.SilenceCooldown);
            else
                role2.LastSilenced = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Spellslinger))
        {
            var role2 = (Spellslinger)role;

            if (start)
                role2.LastSpelled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SpellCooldown);
            else if (meeting)
                role2.LastSpelled = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.SpellCooldown);
            else
                role2.LastSpelled = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.TimeKeeper))
        {
            var role2 = (TimeKeeper)role;

            if (start)
                role2.LastTimed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TimeControlCooldown);
            else if (meeting)
                role2.LastTimed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.TimeControlCooldown);
            else
                role2.LastTimed = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Stalker))
        {
            var role2 = (Stalker)role;

            if (start)
                role2.LastStalked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.StalkCd);
            else if (meeting)
                role2.LastStalked = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.StalkCd);
            else
                role2.LastStalked = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Bomber))
        {
            var role2 = (Bomber)role;

            if (start)
            {
                role2.LastDetonated = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DetonateCooldown);
                role2.LastPlaced = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BombCooldown);
            }
            else if (meeting)
            {
                role2.LastDetonated = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.DetonateCooldown);
                role2.LastPlaced = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.BombCooldown);
            }
            else
            {
                role2.LastDetonated = DateTime.UtcNow;
                role2.LastPlaced = DateTime.UtcNow;
            }

            if (CustomGameOptions.BombsRemoveOnNewRound && meeting)
                Bomb.Clear(role2.Bombs);
        }
        else if (local.Is(LayerEnum.Framer))
        {
            var role2 = (Framer)role;

            if (start)
                role2.LastFramed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.FrameCooldown);
            else if (meeting)
                role2.LastFramed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.FrameCooldown);
            else
                role2.LastFramed = DateTime.UtcNow;

            if (local.Data.IsDead || local.Data.Disconnected)
                role2.Framed.Clear();
        }
        else if (local.Is(LayerEnum.Crusader))
        {
            var role2 = (Crusader)role;
            role2.CrusadedPlayer = null;

            if (start)
                role2.LastCrusaded = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CrusadeCooldown);
            else if (meeting)
                role2.LastCrusaded = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.CrusadeCooldown);
            else
                role2.LastCrusaded = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Poisoner))
        {
            var role2 = (Poisoner)role;
            role2.PoisonedPlayer = null;

            if (start)
                role2.LastPoisoned = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.PoisonCd);
            else if (meeting)
                role2.LastPoisoned = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.PoisonCd);
            else
                role2.LastPoisoned = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.PromotedRebel))
        {
            var role2 = (PromotedRebel)role;
            role2.ShapeshiftPlayer1 = null;
            role2.ShapeshiftPlayer2 = null;
            role2.PoisonedPlayer = null;
            role2.ConcealedPlayer = null;
            role2.WarpPlayer1 = null;
            role2.WarpPlayer2 = null;
            role2.Positive = null;
            role2.Negative = null;
            role2.SilencedPlayer = null;
            role2.CrusadedPlayer = null;

            if (!(role2.FormerRole == null || role2.IsAnarch))
            {
                if (meeting)
                {
                    role2.LastConcealed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ConcealCooldown);
                    role2.LastFramed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.FrameCooldown);
                    role2.LastPoisoned = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.PoisonCd);
                    role2.LastShapeshifted = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ShapeshiftCooldown);
                    role2.LastWarped = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.WarpCooldown);
                    role2.LastPlaced = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.BombCooldown);
                    role2.LastDetonated = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.DetonateCooldown);
                    role2.LastCrusaded = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.CrusadeCooldown);
                    role2.LastSilenced = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.SilenceCooldown);
                    role2.LastTimed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.TimeControlCooldown);
                    role2.LastCharged = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ChargeCooldown);
                    role2.LastNegative = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.CollideCooldown);
                    role2.LastPositive = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.CollideCooldown);
                }
                else
                {
                    role2.LastConcealed = DateTime.UtcNow;
                    role2.LastFramed = DateTime.UtcNow;
                    role2.LastPoisoned = DateTime.UtcNow;
                    role2.LastShapeshifted = DateTime.UtcNow;
                    role2.LastWarped = DateTime.UtcNow;
                    role2.LastPlaced = DateTime.UtcNow;
                    role2.LastDetonated = DateTime.UtcNow;
                    role2.LastCrusaded = DateTime.UtcNow;
                    role2.LastSilenced = DateTime.UtcNow;
                    role2.LastTimed = DateTime.UtcNow;
                    role2.LastCharged = DateTime.UtcNow;
                    role2.LastNegative = DateTime.UtcNow;
                    role2.LastPositive = DateTime.UtcNow;
                }
            }

            if (CustomGameOptions.BombsRemoveOnNewRound && meeting)
                Bomb.Clear(role2.Bombs);
        }
        else if (local.Is(LayerEnum.Shapeshifter))
        {
            var role2 = (Shapeshifter)role;
            role2.ShapeshiftPlayer1 = null;
            role2.ShapeshiftPlayer2 = null;

            if (start)
                role2.LastShapeshifted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ShapeshiftCooldown);
            else if (meeting)
                role2.LastShapeshifted = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ShapeshiftCooldown);
            else
                role2.LastShapeshifted = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Warper))
        {
            var role2 = (Warper)role;
            role2.WarpPlayer1 = null;
            role2.WarpPlayer2 = null;

            if (start)
                role2.LastWarped = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.WarpCooldown);
            else if (meeting)
                role2.LastWarped = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.WarpCooldown);
            else
                role2.LastWarped = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Collider))
        {
            var role2 = (PlayerLayers.Roles.Collider)role;
            role2.Positive = null;
            role2.Negative = null;

            if (start)
            {
                role2.LastCharged = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ChargeCooldown);
                role2.LastNegative = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CollideCooldown);
                role2.LastPositive = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CollideCooldown);
            }
            else if (meeting)
            {
                role2.LastCharged = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ChargeCooldown);
                role2.LastNegative = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.CollideCooldown);
                role2.LastPositive = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.CollideCooldown);
            }
            else
            {
                role2.LastCharged = DateTime.UtcNow;
                role2.LastNegative = DateTime.UtcNow;
                role2.LastPositive = DateTime.UtcNow;
            }
        }
        else if (local.Is(LayerEnum.Banshee))
        {
            var role2 = (Banshee)role;

            if (!role2.Caught)
            {
                if (meeting)
                    role2.LastScreamed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ScreamCooldown);
                else
                    role2.LastScreamed = DateTime.UtcNow;
            }
        }
        else if (local.Is(LayerEnum.Arsonist))
        {
            var role2 = (Arsonist)role;

            if (start)
            {
                role2.LastDoused = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DouseCd);
                role2.LastIgnited = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IgniteCd);
            }
            else if (meeting)
            {
                role2.LastDoused = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.DouseCd);
                role2.LastIgnited = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.IgniteCd);
            }
            else
            {
                role2.LastDoused = DateTime.UtcNow;
                role2.LastIgnited = DateTime.UtcNow;
            }
        }
        else if (local.Is(LayerEnum.Cannibal))
        {
            var role2 = (Cannibal)role;

            if (start)
                role2.LastEaten = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CannibalCd);
            else if (meeting)
                role2.LastEaten = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.CannibalCd);
            else
                role2.LastEaten = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Cryomaniac))
        {
            var role2 = (Cryomaniac)role;

            if (start)
                role2.LastDoused = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CryoDouseCooldown);
            else if (meeting)
                role2.LastDoused = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.CryoDouseCooldown);
            else
                role2.LastDoused = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Dracula))
        {
            var role2 = (Dracula)role;

            if (start)
                role2.LastBitten = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BiteCd);
            else if (meeting)
                role2.LastBitten = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.BiteCd);
            else
                role2.LastBitten = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Glitch))
        {
            var role2 = (Glitch)role;
            role2.MimicTarget = null;
            role2.HackTarget = null;

            if (start)
            {
                role2.LastMimic = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MimicCooldown);
                role2.LastHacked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.HackCooldown);
                role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.GlitchKillCooldown);
            }
            else if (meeting)
            {
                role2.LastMimic = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.MimicCooldown);
                role2.LastHacked = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.HackCooldown);
                role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.GlitchKillCooldown);
            }
            else
            {
                role2.LastMimic = DateTime.UtcNow;
                role2.LastHacked = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }
        }
        else if (local.Is(LayerEnum.GuardianAngel))
        {
            var role2 = (GuardianAngel)role;

            if (meeting && role2.TargetPlayer == null)
                role2.Rounds++;

            if (start)
                role2.LastProtected = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ProtectCd);
            else if (meeting)
                role2.LastProtected = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ProtectCd);
            else
                role2.LastProtected = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Actor))
        {
            var role2 = (Actor)role;

            if (meeting && role2.TargetRole == null)
                role2.Rounds++;
        }
        else if (local.Is(LayerEnum.Jackal))
        {
            var role2 = (Jackal)role;

            if (start)
                role2.LastRecruited = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.RecruitCooldown);
            else if (meeting)
                role2.LastRecruited = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.RecruitCooldown);
            else
                role2.LastRecruited = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Necromancer))
        {
            var role2 = (Necromancer)role;

            if (start)
            {
                role2.LastResurrected = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ResurrectCooldown);
                role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.NecroKillCooldown);
            }
            else if (meeting)
            {
                role2.LastResurrected = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ResurrectCooldown);
                role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.NecroKillCooldown);
            }
            else
            {
                role2.LastResurrected = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }
        }
        else if (local.Is(LayerEnum.Jester))
        {
            var role2 = (Jester)role;

            if (meeting)
                role2.LastHaunted = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.HauntCooldown);
            else
                role2.LastHaunted = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Juggernaut))
        {
            var role2 = (Juggernaut)role;

            if (start)
                role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.JuggKillCooldown);
            else if (meeting)
                role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.JuggKillCooldown);
            else
                role2.LastKilled = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Murderer))
        {
            var role2 = (Murderer)role;

            if (start)
                role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MurdKCD);
            else if (meeting)
                role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.MurdKCD);
            else
                role2.LastKilled = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Pestilence))
        {
            var role2 = (Pestilence)role;

            if (start)
                role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.PestKillCd);
            else if (meeting)
                role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.PestKillCd);
            else
                role2.LastKilled = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Plaguebearer))
        {
            var role2 = (Plaguebearer)role;

            if (start)
                role2.LastInfected = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InfectCd);
            else if (meeting)
                role2.LastInfected = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.InfectCd);
            else
                role2.LastInfected = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.SerialKiller))
        {
            var role2 = (SerialKiller)role;

            if (start)
            {
                role2.LastLusted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BloodlustCd);
                role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.LustKillCd);
            }
            else if (meeting)
            {
                role2.LastLusted = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.BloodlustCd);
                role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.LustKillCd);
            }
            else
            {
                role2.LastLusted = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }
        }
        else if (local.Is(LayerEnum.Survivor))
        {
            var role2 = (Survivor)role;

            if (start)
                role2.LastVested = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.VestCd);
            else if (meeting)
                role2.LastVested = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.VestCd);
            else
                role2.LastVested = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Thief))
        {
            var role2 = (Thief)role;

            if (start)
                role2.LastStolen = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ThiefKillCooldown);
            else if (meeting)
                role2.LastStolen = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ThiefKillCooldown);
            else
                role2.LastStolen = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Troll))
        {
            var role2 = (Troll)role;

            if (start)
                role2.LastInteracted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InteractCooldown);
            else if (meeting)
                role2.LastInteracted = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.InteractCooldown);
            else
                role2.LastInteracted = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Werewolf))
        {
            var role2 = (Werewolf)role;

            if (start)
                role2.LastMauled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MaulCooldown);
            else if (meeting)
                role2.LastMauled = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.MaulCooldown);
            else
                role2.LastMauled = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Whisperer))
        {
            var role2 = (Whisperer)role;

            if (start)
                role2.LastWhispered = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.WhisperCooldown);
            else if (meeting)
                role2.LastWhispered = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.WhisperCooldown);
            else
                role2.LastWhispered = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.BountyHunter))
        {
            var role2 = (BountyHunter)role;

            if (start)
                role2.LastChecked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BountyHunterCooldown);
            else if (meeting)
                role2.LastChecked = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.BountyHunterCooldown);
            else
                role2.LastChecked = DateTime.UtcNow;
        }
        else if (local.Is(LayerEnum.Executioner))
        {
            var role2 = (Executioner)role;

            if (meeting)
                role2.LastDoomed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.DoomCooldown);
            else
                role2.LastDoomed = DateTime.UtcNow;

            if (meeting && role2.TargetPlayer == null)
                role2.Rounds++;
        }
        else if (local.Is(LayerEnum.Guesser))
        {
            var role2 = (Guesser)role;

            if (meeting && role2.TargetPlayer == null)
                role2.Rounds++;
        }

        if (role.BaseFaction == Faction.Intruder)
        {
            var role2 = (Intruder)role;

            if (start)
                role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            else if (meeting)
                role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.IntKillCooldown);
            else
                role2.LastKilled = DateTime.UtcNow;
        }
        else if (role.BaseFaction == Faction.Syndicate)
        {
            var role2 = (Syndicate)role;

            if (start)
            {
                role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - (role.Type is LayerEnum.Anarchist or LayerEnum.Sidekick or LayerEnum.Rebel &&
                    !role2.HoldsDrive ? CustomGameOptions.AnarchKillCooldown : CustomGameOptions.ChaosDriveKillCooldown));
            }
            else if (meeting)
            {
                role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - (role.Type is LayerEnum.Anarchist or LayerEnum.Sidekick or LayerEnum.Rebel &&
                    !role2.HoldsDrive ? CustomGameOptions.AnarchKillCooldown : CustomGameOptions.ChaosDriveKillCooldown));
            }
            else
                role2.LastKilled = DateTime.UtcNow;
        }

        var obj = Objectifier.GetObjectifier(local);

        if (local.Is(LayerEnum.Corrupted))
        {
            var obj2 = (Corrupted)obj;

            if (start)
                obj2.LastCorrupted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CorruptedKillCooldown);
            else if (meeting)
                obj2.LastCorrupted = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.CorruptedKillCooldown);
            else
                obj2.LastCorrupted = DateTime.UtcNow;
        }

        var ab = Ability.GetAbility(local);

        if (local.Is(LayerEnum.ButtonBarry))
        {
            var ab2 = (ButtonBarry)ab;

            if (start)
                ab2.LastButtoned = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ButtonCooldown);
            else
                ab2.LastButtoned = DateTime.UtcNow;
        }
    }

    public static float Timer(PlayerControl player, DateTime lastUsed, float cooldown, float difference = 0, float factor = 1f, bool isDead = false)
    {
        var timespan = DateTime.UtcNow - lastUsed;
        var num = (isDead ? cooldown : player.GetModifiedCooldown(cooldown, difference, factor)) * 1000f;
        var time = num - (float)timespan.TotalMilliseconds;
        var flag2 = time < 0f;
        return (flag2 ? 0f : time) / 1000f;
    }

    public static float Timer(PlayerControl player, DateTime lastUsed, float cooldown, bool isDead) => Timer(player, lastUsed, cooldown, 0, 1, isDead);
}