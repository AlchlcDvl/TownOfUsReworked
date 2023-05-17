namespace TownOfUsReworked.Custom
{
    [HarmonyPatch]
    public static class ButtonUtils
    {
        public static void DisableButtons(this PlayerControl player)
        {
            foreach (var button in CustomButton.AllButtons.Where(x => x.Owner.Player == player))
                button.Disable();
        }

        public static void EnableButtons(this PlayerControl player)
        {
            foreach (var button in CustomButton.AllButtons.Where(x => x.Owner.Player == player))
                button.Enable();
        }

        public static void DestroyButtons(this PlayerControl player)
        {
            foreach (var button in CustomButton.AllButtons.Where(x => x.Owner.Player == player))
                button.Destroy();
        }

        public static bool CannotUse(this PlayerControl player) => player.onLadder || player.IsBlocked() || player.inVent || player.inMovingPlat;

        public static float GetModifiedCooldown(this PlayerControl player, float cooldown, float difference = 0f, float factor = 1f) => (cooldown * factor * player.GetMultiplier()) +
            difference + player.GetUnderdogChange();

        public static float GetUnderdogChange(this PlayerControl player)
        {
            if (!player.Is(AbilityEnum.Underdog))
                return 0f;

            var last = LayerExtentions.Last(player);
            var lowerKC = -CustomGameOptions.UnderdogKillBonus;
            var upperKC = CustomGameOptions.UnderdogKillBonus;

            if (CustomGameOptions.UnderdogIncreasedKC && !last)
                return upperKC;
            else if (last)
                return lowerKC;
            else
                return 0f;
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

        public static void ResetCustomTimers(bool start)
        {
            var local = PlayerControl.LocalPlayer;
            var role = Role.LocalRole;
            local.RegenTask();
            Utils.RoundOne = start;

            if (!start && Role.SyndicateHasChaosDrive)
                RoleGen.AssignChaosDrive();

            if (local.Is(RoleEnum.Chameleon))
            {
                var role2 = (Chameleon)role;

                if (start)
                    role2.LastSwooped = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SwoopCooldown);
                else
                    role2.LastSwooped = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Detective))
            {
                var role2 = (Detective)role;

                if (start)
                    role2.LastExamined = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ExamineCd);
                else
                    role2.LastExamined = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Escort))
            {
                var role2 = (Escort)role;
                role2.BlockTarget = null;

                if (start)
                    role2.LastBlock = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.EscRoleblockCooldown);
                else
                    role2.LastBlock = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Inspector))
            {
                var role2 = (Inspector)role;

                if (start)
                    role2.LastInspected = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InspectCooldown);
                else
                    role2.LastInspected = DateTime.UtcNow;

                if (local.Data.IsDead && !CustomGameOptions.DeadSeeEverything)
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
                else
                {
                    role2.LastCompared = DateTime.UtcNow;
                    role2.LastAutopsied = DateTime.UtcNow;
                }

                if (role2.UsesLeft == 0)
                    role2.ReferenceBody = null;
            }
            else if (local.Is(RoleEnum.Medium))
            {
                var role2 = (Medium)role;
                role2.OnLobby();

                if (start)
                    role2.LastMediated = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MediateCooldown);
                else
                    role2.LastMediated = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Operative))
            {
                var role2 = (Operative)role;
                role2.BuggedPlayers.Clear();

                if (CustomGameOptions.BugsRemoveOnNewRound)
                    role2.OnLobby();

                if (start)
                    role2.LastBugged = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BugCooldown);
                else
                    role2.LastBugged = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Sheriff))
            {
                var role2 = (Sheriff)role;

                if (start)
                    role2.LastInterrogated = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InterrogateCd);
                else
                    role2.LastInterrogated = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Shifter))
            {
                var role2 = (Shifter)role;

                if (start)
                    role2.LastShifted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ShifterCd);
                else
                    role2.LastShifted = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Tracker))
            {
                var role2 = (Tracker)role;

                if (CustomGameOptions.ResetOnNewRound)
                {
                    role2.UsesLeft = CustomGameOptions.MaxTracks + (role.TasksDone ? 1 : 0);
                    role2.OnLobby();
                }

                if (start)
                    role2.LastTracked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TrackCd);
                else
                    role2.LastTracked = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Transporter))
            {
                var role2 = (Transporter)role;
                role2.TransportPlayer1 = null;
                role2.TransportPlayer2 = null;

                if (start)
                    role2.LastTransported = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TransportCooldown);
                else
                    role2.LastTransported = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Altruist))
            {
                var role2 = (Altruist)role;

                if (start)
                    role2.LastRevived = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ReviveCooldown);
                else
                    role2.LastRevived = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.VampireHunter))
            {
                var role2 = (VampireHunter)role;

                if (start)
                    role2.LastStaked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.StakeCooldown);
                else
                    role2.LastStaked = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Veteran))
            {
                var role2 = (Veteran)role;

                if (start)
                    role2.LastAlerted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.AlertCd);
                else
                    role2.LastAlerted = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Mystic))
            {
                var role2 = (Mystic)role;

                if (start)
                    role2.LastRevealed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.RevealCooldown);
                else
                    role2.LastRevealed = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Seer))
            {
                var role2 = (Seer)role;

                if (start)
                    role2.LastSeered = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SeerCooldown);
                else
                    role2.LastSeered = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Vigilante))
            {
                var role2 = (Vigilante)role;
                role2.RoundOne = start && CustomGameOptions.RoundOneNoShot;

                if (start)
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.VigiKillCd);
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
                else
                    role2.LastKnighted = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Retributionist))
            {
                var role2 = (Retributionist)role;

                if (role2.RevivedRole == null)
                    return;

                switch (role2.RevivedRole.RoleType)
                {
                    case RoleEnum.Chameleon:
                        role2.LastSwooped = DateTime.UtcNow;
                        break;

                    case RoleEnum.Detective:
                        role2.LastExamined = DateTime.UtcNow;
                        break;

                    case RoleEnum.Vigilante:
                        role2.LastKilled = DateTime.UtcNow;
                        break;

                    case RoleEnum.VampireHunter:
                        role2.LastStaked = DateTime.UtcNow;
                        break;

                    case RoleEnum.Veteran:
                        role2.LastAlerted = DateTime.UtcNow;
                        break;

                    case RoleEnum.Tracker:
                        role2.LastTracked = DateTime.UtcNow;

                        if (CustomGameOptions.ResetOnNewRound)
                            role2.OnLobby();

                        break;

                    case RoleEnum.Sheriff:
                        role2.LastInterrogated = DateTime.UtcNow;
                        break;

                    case RoleEnum.Medium:
                        role2.LastMediated = DateTime.UtcNow;
                        role2.OnLobby();
                        break;

                    case RoleEnum.Operative:
                        role2.LastBugged = DateTime.UtcNow;
                        role2.BuggedPlayers.Clear();

                        if (CustomGameOptions.BugsRemoveOnNewRound)
                            role2.OnLobby();

                        break;

                    case RoleEnum.Inspector:
                        role2.LastInspected = DateTime.UtcNow;
                        break;

                    case RoleEnum.Escort:
                        role2.LastBlock = DateTime.UtcNow;
                        role2.BlockTarget = null;
                        break;

                    case RoleEnum.Transporter:
                        role2.LastTransported = DateTime.UtcNow;
                        role2.TransportPlayer1 = null;
                        role2.TransportPlayer2 = null;
                        break;
                }

                if (local.Data.IsDead && !CustomGameOptions.DeadSeeEverything)
                    role2.Inspected.Clear();
            }
            else if (local.Is(RoleEnum.Blackmailer))
            {
                var role2 = (Blackmailer)role;
                role2.BlackmailedPlayer = null;

                if (start)
                    role2.LastBlackmailed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BlackmailCd);
                else
                    role2.LastBlackmailed = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Camouflager))
            {
                var role2 = (Camouflager)role;

                if (start)
                    role2.LastCamouflaged = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CamouflagerCd);
                else
                    role2.LastCamouflaged = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Enforcer))
            {
                var role2 = (Enforcer)role;
                role2.BombedPlayer = null;

                if (start)
                    role2.LastBombed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.EnforceCooldown);
                else
                    role2.LastBombed = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Consigliere))
            {
                var role2 = (Consigliere)role;

                if (start)
                    role2.LastInvestigated = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConsigCd);
                else
                    role2.LastInvestigated = DateTime.UtcNow;

                if (local.Data.IsDead && !CustomGameOptions.DeadSeeEverything)
                    role2.Investigated.Clear();
            }
            else if (local.Is(RoleEnum.Consort))
            {
                var role2 = (Consort)role;
                role2.BlockTarget = null;

                if (start)
                    role2.LastBlock = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConsRoleblockCooldown);
                else
                    role2.LastBlock = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Disguiser))
            {
                var role2 = (Disguiser)role;
                role2.MeasuredPlayer = null;
                role2.DisguisedPlayer = null;
                role2.DisguisePlayer = null;

                if (start)
                {
                    role2.LastDisguised = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DisguiseCooldown);
                    role2.LastMeasured = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MeasureCooldown);
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

                if (role2.FormerRole == null || role2.IsImp || start)
                    return;

                switch (role2.FormerRole.RoleType)
                {
                    case RoleEnum.Blackmailer:
                        role2.BlackmailedPlayer = null;
                        role2.LastBlackmailed = DateTime.UtcNow;
                        role2.LastKilled = DateTime.UtcNow;
                        break;

                    case RoleEnum.Camouflager:
                        role2.LastCamouflaged = DateTime.UtcNow;
                        break;

                    case RoleEnum.Consigliere:
                        role2.LastInvestigated = DateTime.UtcNow;
                        break;

                    case RoleEnum.Disguiser:
                        role2.LastDisguised = DateTime.UtcNow;
                        role2.LastMeasured = DateTime.UtcNow;
                        role2.MeasuredPlayer = null;
                        role2.DisguisedPlayer = null;
                        role2.DisguisePlayer = null;
                        break;

                    case RoleEnum.Grenadier:
                        role2.LastFlashed = DateTime.UtcNow;
                        break;

                    case RoleEnum.Miner:
                        role2.LastMined = DateTime.UtcNow;
                        break;

                    case RoleEnum.Janitor:
                        role2.LastCleaned = DateTime.UtcNow;
                        role2.LastDragged = DateTime.UtcNow;
                        role2.CurrentlyDragging = null;
                        break;

                    case RoleEnum.Morphling:
                        role2.LastMorphed = DateTime.UtcNow;
                        role2.LastSampled = DateTime.UtcNow;
                        role2.SampledPlayer = null;
                        role2.MorphedPlayer = null;
                        break;

                    case RoleEnum.Teleporter:
                        role2.LastTeleport = DateTime.UtcNow;
                        role2.TeleportPoint = new(0, 0, 0);
                        break;

                    case RoleEnum.Wraith:
                        role2.LastInvis = DateTime.UtcNow;
                        break;

                    case RoleEnum.Ambusher:
                        role2.LastAmbushed = DateTime.UtcNow;
                        role2.AmbushedPlayer = null;
                        break;

                    case RoleEnum.Consort:
                        role2.LastBlock = DateTime.UtcNow;
                        role2.BlockTarget = null;
                        break;
                }

                if (local.Data.IsDead && !CustomGameOptions.DeadSeeEverything)
                    role2.Investigated.Clear();
            }
            else if (local.Is(RoleEnum.Godfather))
            {
                var role2 = (Godfather)role;

                if (!role2.HasDeclared)
                {
                    if (start)
                        role2.LastDeclared = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - 10f);
                    else
                        role2.LastDeclared = DateTime.UtcNow;
                }
            }
            else if (local.Is(RoleEnum.Grenadier))
            {
                var role2 = (Grenadier)role;

                if (start)
                    role2.LastFlashed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.GrenadeCd);
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
                else
                {
                    role2.LastMorphed = DateTime.UtcNow;
                    role2.LastSampled = DateTime.UtcNow;
                }
            }
            else if (local.Is(RoleEnum.Teleporter))
            {
                var role2 = (Teleporter)role;
                role2.TeleportPoint = new(0, 0, 0);

                if (start)
                {
                    role2.LastTeleport = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TeleportCd);
                    role2.LastMarked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MarkCooldown);
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
                else
                    role2.LastInvis = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Ambusher))
            {
                var role2 = (Ambusher)role;
                role2.AmbushedPlayer = null;

                if (start)
                    role2.LastAmbushed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.AmbushCooldown);
                else
                    role2.LastAmbushed = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Ghoul))
            {
                var role2 = (Ghoul)role;

                if (!role2.Caught)
                    role2.LastMarked = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Concealer))
            {
                var role2 = (Concealer)role;
                role2.ConcealedPlayer = null;

                if (start)
                    role2.LastConcealed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConcealCooldown);
                else
                    role2.LastConcealed = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Spellslinger))
            {
                var role2 = (Spellslinger)role;

                if (start)
                    role2.LastSpelled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SpellCooldown);
                else
                    role2.LastSpelled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Stalker))
            {
                var role2 = (Stalker)role;

                if (start)
                    role2.LastStalked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.StalkCd);
                else
                    role2.LastStalked = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Bomber))
            {
                var role2 = (Bomber)role;

                if (CustomGameOptions.BombsRemoveOnNewRound)
                    Bomb.Clear(role2.Bombs);

                if (start)
                {
                    role2.LastDetonated = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DetonateCooldown);
                    role2.LastPlaced = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BombCooldown);
                }
                else
                {
                    role2.LastDetonated = DateTime.UtcNow;
                    role2.LastPlaced = DateTime.UtcNow;
                }
            }
            else if (local.Is(RoleEnum.Framer))
            {
                var role2 = (Framer)role;

                if (start)
                    role2.LastFramed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.FrameCooldown);
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
                else
                    role2.LastCrusaded = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Poisoner))
            {
                var role2 = (Poisoner)role;
                role2.PoisonedPlayer = null;

                if (start)
                    role2.LastPoisoned = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.PoisonCd);
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
                    else
                        role2.LastDeclared = DateTime.UtcNow;
                }
            }
            else if (local.Is(RoleEnum.PromotedRebel))
            {
                var role2 = (PromotedRebel)role;

                if (role2.FormerRole == null || role2.IsAnarch || start)
                    return;

                switch (role2.FormerRole.RoleType)
                {
                    case RoleEnum.Concealer:
                        role2.LastConcealed = DateTime.UtcNow;
                        role2.ConcealedPlayer = null;
                        break;

                    case RoleEnum.Framer:
                        role2.LastFramed = DateTime.UtcNow;
                        break;

                    case RoleEnum.Poisoner:
                        role2.LastPoisoned = DateTime.UtcNow;
                        role2.PoisonedPlayer = null;
                        break;

                    case RoleEnum.Shapeshifter:
                        role2.LastShapeshifted = DateTime.UtcNow;
                        role2.ShapeshiftPlayer1 = null;
                        role2.ShapeshiftPlayer2 = null;
                        break;

                    case RoleEnum.Warper:
                        role2.LastWarped = DateTime.UtcNow;
                        role2.WarpPlayer1 = null;
                        role2.WarpPlayer2 = null;
                        break;

                    case RoleEnum.Bomber:
                        role2.LastPlaced = DateTime.UtcNow;
                        role2.LastDetonated = DateTime.UtcNow;

                        if (CustomGameOptions.BombsRemoveOnNewRound)
                            Bomb.Clear(role2.Bombs);

                        break;

                    case RoleEnum.Crusader:
                        role2.LastCrusaded = DateTime.UtcNow;
                        role2.CrusadedPlayer = null;
                        break;
                }
            }
            else if (local.Is(RoleEnum.Shapeshifter))
            {
                var role2 = (Shapeshifter)role;
                role2.ShapeshiftPlayer1 = null;
                role2.ShapeshiftPlayer2 = null;

                if (start)
                    role2.LastShapeshifted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ShapeshiftCooldown);
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
                else
                    role2.LastWarped = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Banshee))
            {
                var role2 = (Banshee)role;

                if (!role2.Caught)
                    role2.LastScreamed = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Arsonist))
            {
                var role2 = (Arsonist)role;

                if (start)
                {
                    role2.LastDoused = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DouseCd);
                    role2.LastIgnited = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IgniteCd);
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
                else
                    role2.LastEaten = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Cryomaniac))
            {
                var role2 = (Cryomaniac)role;

                if (start)
                    role2.LastDoused = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CryoDouseCooldown);
                else
                    role2.LastDoused = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Dracula))
            {
                var role2 = (Dracula)role;

                if (start)
                    role2.LastBitten = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BiteCd);
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

                if (start)
                    role2.LastProtected = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ProtectCd);
                else
                    role2.LastProtected = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Jackal))
            {
                var role2 = (Jackal)role;

                if (start)
                    role2.LastRecruited = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.RecruitCooldown);
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
                else
                {
                    role2.LastResurrected = DateTime.UtcNow;
                    role2.LastKilled = DateTime.UtcNow;
                }
            }
            else if (local.Is(RoleEnum.Jester))
            {
                var role2 = (Jester)role;
                role2.LastHaunted = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Juggernaut))
            {
                var role2 = (Juggernaut)role;

                if (start)
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.JuggKillCooldown);
                else
                    role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Murderer))
            {
                var role2 = (Murderer)role;

                if (start)
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MurdKCD);
                else
                    role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Pestilence))
            {
                var role2 = (Pestilence)role;

                if (start)
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.PestKillCd);
                else
                    role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Plaguebearer))
            {
                var role2 = (Plaguebearer)role;

                if (start)
                    role2.LastInfected = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InfectCd);
                else
                    role2.LastInfected = DateTime.UtcNow;

                if (local.Data.IsDead || local.Data.Disconnected)
                    role2.Infected.Clear();
            }
            else if (local.Is(RoleEnum.SerialKiller))
            {
                var role2 = (SerialKiller)role;

                if (start)
                {
                    role2.LastLusted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BloodlustCd);
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.LustKillCd);
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
                else
                    role2.LastVested = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Thief))
            {
                var role2 = (Thief)role;

                if (start)
                    role2.LastStolen = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ThiefKillCooldown);
                else
                    role2.LastStolen = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Troll))
            {
                var role2 = (Troll)role;

                if (start)
                    role2.LastInteracted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InteractCooldown);
                else
                    role2.LastInteracted = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Werewolf))
            {
                var role2 = (Werewolf)role;

                if (start)
                    role2.LastMauled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MaulCooldown);
                else
                    role2.LastMauled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Whisperer))
            {
                var role2 = (Whisperer)role;

                if (start)
                    role2.LastWhispered = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.WhisperCooldown);
                else
                    role2.LastWhispered = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.BountyHunter))
            {
                var role2 = (BountyHunter)role;

                if (start)
                    role2.LastChecked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BountyHunterCooldown);
                else
                    role2.LastChecked = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Executioner))
            {
                var role2 = (Executioner)role;
                role2.LastDoomed = DateTime.UtcNow;
            }

            if (role.BaseFaction == Faction.Intruder)
            {
                var role2 = (IntruderRole)role;

                if (start)
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
                else
                    role2.LastKilled = DateTime.UtcNow;
            }
            else if (role.BaseFaction == Faction.Syndicate)
            {
                var role2 = (SyndicateRole)role;

                if (start)
                {
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - (local.Is(RoleEnum.Anarchist) && !role2.HoldsDrive ?
                        CustomGameOptions.AnarchKillCooldown : CustomGameOptions.ChaosDriveKillCooldown));
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

        public static bool ButtonUsable(this ActionButton button) => button.isActiveAndEnabled && !button.isCoolingDown && !PlayerControl.LocalPlayer.CannotUse() &&
            !MeetingHud.Instance;
    }
}