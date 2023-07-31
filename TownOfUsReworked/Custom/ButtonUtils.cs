namespace TownOfUsReworked.Custom
{
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
            HUD.ReportButton.gameObject.SetActive(!player.Is(ModifierEnum.Coward));
            HUD.ImpostorVentButton.gameObject.SetActive(player.CanVent());

            if (Use)
                HUD.UseButton.gameObject.SetActive(true);
            else
                HUD.PetButton.gameObject.SetActive(true);
        }

        public static void DestroyButtons(this PlayerControl player)
        {
            foreach (var button in CustomButton.AllButtons.Where(x => x.Owner.Player == player))
                button.Destroy();
        }

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
            if (!player.Is(AbilityEnum.Underdog))
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

            if (player.Is(RoleEnum.PromotedGodfather))
                num *= CustomGameOptions.MafiosoAbilityCooldownDecrease;
            if (player.Is(RoleEnum.PromotedRebel))
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

            if (role.Requesting)
                role.BountyTimer++;

            if (!start && Role.SyndicateHasChaosDrive)
                RoleGen.AssignChaosDrive();

            if (local.Is(RoleEnum.Chameleon))
            {
                var role2 = (Chameleon)role;

                if (start)
                    role2.LastSwooped = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SwoopCooldown);
                else if (meeting)
                    role2.LastSwooped = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.SwoopCooldown);
                else
                    role2.LastSwooped = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Detective))
            {
                var role2 = (Detective)role;

                if (start)
                    role2.LastExamined = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ExamineCd);
                else if (meeting)
                    role2.LastExamined = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ExamineCd);
                else
                    role2.LastExamined = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Escort))
            {
                var role2 = (Escort)role;
                role2.BlockTarget = null;

                if (start)
                    role2.LastBlock = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.EscRoleblockCooldown);
                else if (meeting)
                    role2.LastBlock = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.EscRoleblockCooldown);
                else
                    role2.LastBlock = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Inspector))
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
            else if (local.Is(RoleEnum.Coroner))
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
            else if (local.Is(RoleEnum.Medium))
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
            else if (local.Is(RoleEnum.Operative))
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
            else if (local.Is(RoleEnum.Sheriff))
            {
                var role2 = (Sheriff)role;

                if (start)
                    role2.LastInterrogated = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InterrogateCd);
                else if (meeting)
                    role2.LastInterrogated = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.InterrogateCd);
                else
                    role2.LastInterrogated = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Shifter))
            {
                var role2 = (Shifter)role;

                if (start)
                    role2.LastShifted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ShifterCd);
                else if (meeting)
                    role2.LastShifted = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ShifterCd);
                else
                    role2.LastShifted = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Tracker))
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
            else if (local.Is(RoleEnum.Transporter))
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
            else if (local.Is(RoleEnum.Altruist))
            {
                var role2 = (Altruist)role;

                if (start)
                    role2.LastRevived = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ReviveCooldown);
                else if (meeting)
                    role2.LastRevived = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ReviveCooldown);
                else
                    role2.LastRevived = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.VampireHunter))
            {
                var role2 = (VampireHunter)role;

                if (start)
                    role2.LastStaked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.StakeCooldown);
                else if (meeting)
                    role2.LastStaked = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.StakeCooldown);
                else
                    role2.LastStaked = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Veteran))
            {
                var role2 = (Veteran)role;

                if (start)
                    role2.LastAlerted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.AlertCd);
                else if (meeting)
                    role2.LastAlerted = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.AlertCd);
                else
                    role2.LastAlerted = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Mystic))
            {
                var role2 = (Mystic)role;

                if (start)
                    role2.LastRevealed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.RevealCooldown);
                else if (meeting)
                    role2.LastRevealed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.RevealCooldown);
                else
                    role2.LastRevealed = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Seer))
            {
                var role2 = (Seer)role;

                if (start)
                    role2.LastSeered = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SeerCooldown);
                else if (meeting)
                    role2.LastSeered = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.SeerCooldown);
                else
                    role2.LastSeered = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Vigilante))
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
            else if (local.Is(RoleEnum.Mayor))
            {
                var role2 = (Mayor)role;
                role2.RoundOne = start && CustomGameOptions.RoundOneNoReveal;
            }
            else if (local.Is(RoleEnum.Monarch))
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
            else if (local.Is(RoleEnum.Retributionist))
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
                    role2.LastBlock = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.EscRoleblockCooldown);
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
                    role2.LastBlock = DateTime.UtcNow;
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
            else if (local.Is(RoleEnum.Blackmailer))
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
            else if (local.Is(RoleEnum.Camouflager))
            {
                var role2 = (Camouflager)role;

                if (start)
                    role2.LastCamouflaged = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CamouflagerCd);
                else if (meeting)
                    role2.LastCamouflaged = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.CamouflagerCd);
                else
                    role2.LastCamouflaged = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Enforcer))
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
            else if (local.Is(RoleEnum.Consigliere))
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
            else if (local.Is(RoleEnum.Consort))
            {
                var role2 = (Consort)role;
                role2.BlockTarget = null;

                if (start)
                    role2.LastBlock = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConsRoleblockCooldown);
                else if (meeting)
                    role2.LastBlock = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ConsRoleblockCooldown);
                else
                    role2.LastBlock = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Disguiser))
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
            else if (local.Is(RoleEnum.PromotedGodfather))
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
                    role2.LastTeleport = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.TeleportCd);
                    role2.LastInvis = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.InvisCd);
                    role2.LastAmbushed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.AmbushCooldown);
                    role2.LastBlock = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ConsRoleblockCooldown);
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
                    role2.LastTeleport = DateTime.UtcNow;
                    role2.LastInvis = DateTime.UtcNow;
                    role2.LastAmbushed = DateTime.UtcNow;
                    role2.LastBlock = DateTime.UtcNow;
                }

                if (local.Data.IsDead && DeadSeeEverything)
                    role2.Investigated.Clear();
            }
            else if (local.Is(RoleEnum.Godfather))
            {
                var role2 = (Godfather)role;

                if (!role2.HasDeclared)
                {
                    if (start)
                        role2.LastDeclared = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - 10f);
                    else if (meeting)
                        role2.LastDeclared = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - 10f);
                    else
                        role2.LastDeclared = DateTime.UtcNow;
                }
            }
            else if (local.Is(RoleEnum.Grenadier))
            {
                var role2 = (Grenadier)role;

                if (start)
                    role2.LastFlashed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.GrenadeCd);
                else if (meeting)
                    role2.LastFlashed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.GrenadeCd);
                else
                    role2.LastFlashed = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Janitor))
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
            else if (local.Is(RoleEnum.Miner))
            {
                var role2 = (Miner)role;

                if (start)
                    role2.LastMined = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MineCd);
                else if (meeting)
                    role2.LastMined = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.MineCd);
                else
                    role2.LastMined = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Morphling))
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
            else if (local.Is(RoleEnum.Teleporter))
            {
                var role2 = (Teleporter)role;
                role2.TeleportPoint = Vector3.zero;

                if (start)
                {
                    role2.LastTeleport = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TeleportCd);
                    role2.LastMarked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MarkCooldown);
                }
                else if (meeting)
                {
                    role2.LastMarked = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.MarkCooldown);
                    role2.LastTeleport = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.TeleportCd);
                }
                else
                {
                    role2.LastTeleport = DateTime.UtcNow;
                    role2.LastMarked = DateTime.UtcNow;
                }
            }
            else if (local.Is(RoleEnum.Wraith))
            {
                var role2 = (Wraith)role;

                if (start)
                    role2.LastInvis = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InvisCd);
                else if (meeting)
                    role2.LastInvis = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.InvisCd);
                else
                    role2.LastInvis = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Ambusher))
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
            else if (local.Is(RoleEnum.Ghoul))
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
            else if (local.Is(RoleEnum.Concealer))
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
            else if (local.Is(RoleEnum.Silencer))
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
            else if (local.Is(RoleEnum.Spellslinger))
            {
                var role2 = (Spellslinger)role;

                if (start)
                    role2.LastSpelled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SpellCooldown);
                else if (meeting)
                    role2.LastSpelled = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.SpellCooldown);
                else
                    role2.LastSpelled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.TimeKeeper))
            {
                var role2 = (TimeKeeper)role;

                if (start)
                    role2.LastTimed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TimeControlCooldown);
                else if (meeting)
                    role2.LastTimed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.TimeControlCooldown);
                else
                    role2.LastTimed = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Stalker))
            {
                var role2 = (Stalker)role;

                if (start)
                    role2.LastStalked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.StalkCd);
                else if (meeting)
                    role2.LastStalked = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.StalkCd);
                else
                    role2.LastStalked = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Bomber))
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
            else if (local.Is(RoleEnum.Framer))
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
            else if (local.Is(RoleEnum.Crusader))
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
            else if (local.Is(RoleEnum.Poisoner))
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
            else if (local.Is(RoleEnum.Rebel))
            {
                var role2 = (Rebel)role;

                if (!role2.HasDeclared)
                {
                    if (start)
                        role2.LastDeclared = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - 10f);
                    else if (start)
                        role2.LastDeclared = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - 10f);
                    else
                        role2.LastDeclared = DateTime.UtcNow;
                }
            }
            else if (local.Is(RoleEnum.PromotedRebel))
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
            else if (local.Is(RoleEnum.Shapeshifter))
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
            else if (local.Is(RoleEnum.Warper))
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
            else if (local.Is(RoleEnum.Collider))
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
            else if (local.Is(RoleEnum.Banshee))
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
            else if (local.Is(RoleEnum.Arsonist))
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
            else if (local.Is(RoleEnum.Cannibal))
            {
                var role2 = (Cannibal)role;

                if (start)
                    role2.LastEaten = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CannibalCd);
                else if (meeting)
                    role2.LastEaten = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.CannibalCd);
                else
                    role2.LastEaten = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Cryomaniac))
            {
                var role2 = (Cryomaniac)role;

                if (start)
                    role2.LastDoused = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CryoDouseCooldown);
                else if (meeting)
                    role2.LastDoused = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.CryoDouseCooldown);
                else
                    role2.LastDoused = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Dracula))
            {
                var role2 = (Dracula)role;

                if (start)
                    role2.LastBitten = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BiteCd);
                else if (meeting)
                    role2.LastBitten = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.BiteCd);
                else
                    role2.LastBitten = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Glitch))
            {
                var role2 = (Glitch)role;
                role2.MimicTarget = null;
                role2.HackTarget = null;

                if (start)
                {
                    role2.LastMimic = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MimicCooldown);
                    role2.LastHack = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.HackCooldown);
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.GlitchKillCooldown);
                }
                else if (meeting)
                {
                    role2.LastMimic = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.MimicCooldown);
                    role2.LastHack = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.HackCooldown);
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.GlitchKillCooldown);
                }
                else
                {
                    role2.LastMimic = DateTime.UtcNow;
                    role2.LastHack = DateTime.UtcNow;
                    role2.LastKilled = DateTime.UtcNow;
                }
            }
            else if (local.Is(RoleEnum.GuardianAngel))
            {
                var role2 = (GuardianAngel)role;
                role2.Rounds++;

                if (start)
                    role2.LastProtected = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ProtectCd);
                else if (meeting)
                    role2.LastProtected = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ProtectCd);
                else
                    role2.LastProtected = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Actor))
            {
                var role2 = (Actor)role;
                role2.Rounds++;
            }
            else if (local.Is(RoleEnum.Jackal))
            {
                var role2 = (Jackal)role;

                if (start)
                    role2.LastRecruited = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.RecruitCooldown);
                else if (meeting)
                    role2.LastRecruited = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.RecruitCooldown);
                else
                    role2.LastRecruited = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Necromancer))
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
            else if (local.Is(RoleEnum.Jester))
            {
                var role2 = (Jester)role;

                if (meeting)
                    role2.LastHaunted = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.HauntCooldown);
                else
                    role2.LastHaunted = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Juggernaut))
            {
                var role2 = (Juggernaut)role;

                if (start)
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.JuggKillCooldown);
                else if (meeting)
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.JuggKillCooldown);
                else
                    role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Murderer))
            {
                var role2 = (Murderer)role;

                if (start)
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MurdKCD);
                else if (meeting)
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.MurdKCD);
                else
                    role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Pestilence))
            {
                var role2 = (Pestilence)role;

                if (start)
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.PestKillCd);
                else if (meeting)
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.PestKillCd);
                else
                    role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Plaguebearer))
            {
                var role2 = (Plaguebearer)role;

                if (start)
                    role2.LastInfected = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InfectCd);
                else if (meeting)
                    role2.LastInfected = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.InfectCd);
                else
                    role2.LastInfected = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.SerialKiller))
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
            else if (local.Is(RoleEnum.Survivor))
            {
                var role2 = (Survivor)role;

                if (start)
                    role2.LastVested = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.VestCd);
                else if (meeting)
                    role2.LastVested = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.VestCd);
                else
                    role2.LastVested = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Thief))
            {
                var role2 = (Thief)role;

                if (start)
                    role2.LastStolen = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ThiefKillCooldown);
                else if (meeting)
                    role2.LastStolen = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.ThiefKillCooldown);
                else
                    role2.LastStolen = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Troll))
            {
                var role2 = (Troll)role;

                if (start)
                    role2.LastInteracted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InteractCooldown);
                else if (meeting)
                    role2.LastInteracted = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.InteractCooldown);
                else
                    role2.LastInteracted = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Werewolf))
            {
                var role2 = (Werewolf)role;

                if (start)
                    role2.LastMauled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MaulCooldown);
                else if (meeting)
                    role2.LastMauled = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.MaulCooldown);
                else
                    role2.LastMauled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Whisperer))
            {
                var role2 = (Whisperer)role;

                if (start)
                    role2.LastWhispered = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.WhisperCooldown);
                else if (meeting)
                    role2.LastWhispered = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.WhisperCooldown);
                else
                    role2.LastWhispered = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.BountyHunter))
            {
                var role2 = (BountyHunter)role;

                if (start)
                    role2.LastChecked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BountyHunterCooldown);
                else if (meeting)
                    role2.LastChecked = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.BountyHunterCooldown);
                else
                    role2.LastChecked = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Executioner))
            {
                var role2 = (Executioner)role;

                if (meeting)
                    role2.LastDoomed = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - CustomGameOptions.DoomCooldown);
                else
                    role2.LastDoomed = DateTime.UtcNow;

                role2.Rounds++;
            }
            else if (local.Is(RoleEnum.Guesser))
            {
                var role2 = (Guesser)role;
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
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - (local.GetRole() is RoleEnum.Anarchist or RoleEnum.Sidekick or RoleEnum.Rebel &&
                        !role2.HoldsDrive ? CustomGameOptions.AnarchKillCooldown : CustomGameOptions.ChaosDriveKillCooldown));
                }
                else if (meeting)
                {
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.MeetingCooldowns - (local.GetRole() is RoleEnum.Anarchist or RoleEnum.Sidekick or RoleEnum.Rebel &&
                        !role2.HoldsDrive ? CustomGameOptions.AnarchKillCooldown : CustomGameOptions.ChaosDriveKillCooldown));
                }
                else
                    role2.LastKilled = DateTime.UtcNow;
            }

            var obj = Objectifier.GetObjectifier(local);

            if (local.Is(ObjectifierEnum.Corrupted))
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

            if (local.Is(AbilityEnum.ButtonBarry))
            {
                var ab2 = (ButtonBarry)ab;

                if (start)
                    ab2.LastButtoned = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ButtonCooldown);
                else
                    ab2.LastButtoned = DateTime.UtcNow;
            }
        }
    }
}