using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using Hazel;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuardianAngelMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ExecutionerMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuesserMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.BountyHunterMod;

namespace TownOfUsReworked.PlayerLayers.Roles.AllRoles
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__29), nameof(IntroCutscene._CoBegin_d__29.MoveNext))]
    public static class Start
    {
        public static void Postfix(IntroCutscene._CoBegin_d__29 __instance)
        {
            //Crew starting cooldowns
            foreach (var role in Role.GetRoles(RoleEnum.Chameleon))
            {
                var chameleon = (Chameleon)role;
                chameleon.LastSwooped = DateTime.UtcNow;
                chameleon.LastSwooped = chameleon.LastSwooped.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SwoopCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Detective))
            {
                var detective = (Detective)role;
                detective.LastExamined = DateTime.UtcNow;
                detective.LastExamined = detective.LastExamined.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ExamineCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Escort))
            {
                var esc = (Escort)role;
                esc.LastBlock = DateTime.UtcNow;
                esc.LastBlock = esc.LastBlock.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.EscRoleblockCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Inspector))
            {
                var inspector = (Inspector)role;
                inspector.LastInspected = DateTime.UtcNow;
                inspector.LastInspected = inspector.LastInspected.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InspectCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Medium))
            {
                var medium = (Medium)role;
                medium.LastMediated = DateTime.UtcNow;
                medium.LastMediated = medium.LastMediated.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MediateCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Operative))
            {
                var op = (Operative)role;
                op.LastBugged = DateTime.UtcNow;
                op.LastBugged = op.LastBugged.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BugCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Sheriff))
            {
                var sheriff = (Sheriff)role;
                sheriff.LastInterrogated = DateTime.UtcNow;
                sheriff.LastInterrogated = sheriff.LastInterrogated.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InterrogateCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Shifter))
            {
                var shifter = (Shifter)role;
                shifter.LastShifted = DateTime.UtcNow;
                shifter.LastShifted = shifter.LastShifted.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ShifterCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.TimeLord))
            {
                var TimeLord = (TimeLord)role;
                TimeLord.FinishRewind = DateTime.UtcNow;
                TimeLord.StartRewind = DateTime.UtcNow;
                TimeLord.FinishRewind = TimeLord.FinishRewind.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.RewindCooldown);
                TimeLord.StartRewind = TimeLord.StartRewind.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.RewindCooldown);
                TimeLord.StartRewind = TimeLord.StartRewind.AddSeconds(-10.0f);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Tracker))
            {
                var tracker = (Tracker)role;
                tracker.LastTracked = DateTime.UtcNow;
                tracker.LastTracked = tracker.LastTracked.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TrackCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Transporter))
            {
                var transporter = (Transporter)role;
                transporter.LastTransported = DateTime.UtcNow;
                transporter.LastTransported = transporter.LastTransported.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TransportCooldown);
            }
            
            foreach (var role in Role.GetRoles(RoleEnum.VampireHunter))
            {
                var vampireHunter = (VampireHunter)role;
                vampireHunter.LastStaked = DateTime.UtcNow;
                vampireHunter.LastStaked = vampireHunter.LastStaked.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.StakeCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Veteran))
            {
                var veteran = (Veteran)role;
                veteran.LastAlerted = DateTime.UtcNow;
                veteran.LastAlerted = veteran.LastAlerted.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.AlertCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Retributionist))
            {
                var retributionist = (Retributionist)role;
                retributionist.RevivedRole = null;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Vigilante))
            {
                var vigilante = (Vigilante)role;
                vigilante.FirstRound = CustomGameOptions.RoundOneNoShot;
                vigilante.LastKilled = DateTime.UtcNow;
                vigilante.LastKilled = vigilante.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.VigiKillCd);
            }

            //Intruder starting cooldowns
            foreach (var role in Role.GetRoles(RoleEnum.Blackmailer))
            {
                var blackmailer = (Blackmailer)role;
                blackmailer.LastBlackmailed = DateTime.UtcNow;
                blackmailer.LastBlackmailed = blackmailer.LastBlackmailed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BlackmailCd);
                blackmailer.LastKilled = DateTime.UtcNow;
                blackmailer.LastKilled = blackmailer.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Camouflager))
            {
                var camouflager = (Camouflager)role;
                camouflager.LastCamouflaged = DateTime.UtcNow;
                camouflager.LastCamouflaged = camouflager.LastCamouflaged.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CamouflagerCd);
                camouflager.LastKilled = DateTime.UtcNow;
                camouflager.LastKilled = camouflager.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Consigliere))
            {
                var consig = (Consigliere)role;
                consig.LastInvestigated = DateTime.UtcNow;
                consig.LastInvestigated = consig.LastInvestigated.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConsigCd);
                consig.LastKilled = DateTime.UtcNow;
                consig.LastKilled = consig.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Consort))
            {
                var consort = (Consort)role;
                consort.LastBlock = DateTime.UtcNow;
                consort.LastBlock = consort.LastBlock.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConsRoleblockCooldown);
                consort.LastKilled = DateTime.UtcNow;
                consort.LastKilled = consort.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Impostor))
            {
                var impostor = (Impostor)role;
                impostor.LastKilled = DateTime.UtcNow;
                impostor.LastKilled = impostor.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Godfather))
            {
                var godfather = (Godfather)role;
                godfather.LastKilled = DateTime.UtcNow;
                godfather.LastKilled = godfather.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
                godfather.FormerRole = null;
            }
            
            foreach (var role in Role.GetRoles(RoleEnum.Janitor))
            {
                var janitor = (Janitor)role;
                janitor.LastCleaned = DateTime.UtcNow;
                janitor.LastCleaned = janitor.LastCleaned.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.JanitorCleanCd);
                janitor.LastKilled = DateTime.UtcNow;
                janitor.LastKilled = janitor.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Disguiser))
            {
                var disguiser = (Disguiser)role;
                disguiser.LastDisguised = DateTime.UtcNow;
                disguiser.LastDisguised = disguiser.LastDisguised.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DisguiseCooldown);
                disguiser.LastKilled = DateTime.UtcNow;
                disguiser.LastKilled = disguiser.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Grenadier))
            {
                var grenadier = (Grenadier)role;
                grenadier.LastFlashed = DateTime.UtcNow;
                grenadier.LastFlashed = grenadier.LastFlashed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.GrenadeCd);
                grenadier.LastKilled = DateTime.UtcNow;
                grenadier.LastKilled = grenadier.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Miner))
            {
                var miner = (Miner)role;
                miner.LastMined = DateTime.UtcNow;
                miner.LastMined = miner.LastMined.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MineCd);
                var vents = Object.FindObjectsOfType<Vent>();
                miner.VentSize = Vector2.Scale(vents[0].GetComponent<BoxCollider2D>().size, vents[0].transform.localScale) * 0.75f;
                miner.LastKilled = DateTime.UtcNow;
                miner.LastKilled = miner.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Morphling))
            {
                var morphling = (Morphling)role;
                morphling.LastMorphed = DateTime.UtcNow;
                morphling.LastMorphed = morphling.LastMorphed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MorphlingCd);
                morphling.LastKilled = DateTime.UtcNow;
                morphling.LastKilled = morphling.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Teleporter))
            {
                var teleporter = (Teleporter)role;
                teleporter.LastTeleport = DateTime.UtcNow;
                teleporter.LastTeleport = teleporter.LastTeleport.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TeleportCd);
                teleporter.LastKilled = DateTime.UtcNow;
                teleporter.LastKilled = teleporter.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.TimeMaster))
            {
                var tm = (TimeMaster)role;
                tm.LastFrozen = DateTime.UtcNow;
                tm.LastFrozen = tm.LastFrozen.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.FreezeCooldown);
                tm.LastKilled = DateTime.UtcNow;
                tm.LastKilled = tm.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Undertaker))
            {
                var undertaker = (Undertaker)role;
                undertaker.LastDragged = DateTime.UtcNow;
                undertaker.LastDragged = undertaker.LastDragged.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DragCd);
                undertaker.LastKilled = DateTime.UtcNow;
                undertaker.LastKilled = undertaker.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Wraith))
            {
                var wraith = (Wraith)role;
                wraith.LastInvis = DateTime.UtcNow;
                wraith.LastInvis = wraith.LastInvis.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InvisCd);
                wraith.LastKilled = DateTime.UtcNow;
                wraith.LastKilled = wraith.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            //Syndicate starting cooldowns
            foreach (var role in Role.GetRoles(RoleEnum.Anarchist))
            {
                var an = (Anarchist)role;
                an.LastKilled = DateTime.UtcNow;
                an.LastKilled = an.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ChaosDriveKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Concealer))
            {
                var concealer = (Concealer)role;
                concealer.LastConcealed = DateTime.UtcNow;
                concealer.LastConcealed = concealer.LastConcealed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConcealCooldown);
                concealer.LastKilled = DateTime.UtcNow;
                concealer.LastKilled = concealer.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ChaosDriveKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Beamer))
            {
                var beamer = (Beamer)role;
                beamer.LastBeamed = DateTime.UtcNow;
                beamer.LastBeamed = beamer.LastBeamed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConcealCooldown);
                beamer.LastKilled = DateTime.UtcNow;
                beamer.LastKilled = beamer.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ChaosDriveKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Bomber))
            {
                var bomber = (Bomber)role;
                bomber.LastPlaced = DateTime.UtcNow;
                bomber.LastPlaced = bomber.LastPlaced.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BombCooldown);
                bomber.LastDetonated = DateTime.UtcNow;
                bomber.LastDetonated = bomber.LastDetonated.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DetonateCooldown);
                bomber.LastKilled = DateTime.UtcNow;
                bomber.LastKilled = bomber.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ChaosDriveKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Framer))
            {
                var framer = (Framer)role;
                framer.LastFramed = DateTime.UtcNow;
                framer.LastFramed = framer.LastFramed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.FrameCooldown);
                framer.LastKilled = DateTime.UtcNow;
                framer.LastKilled = framer.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ChaosDriveKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Gorgon))
            {
                var gorgon = (Gorgon)role;
                gorgon.LastGazed = DateTime.UtcNow;
                gorgon.LastGazed = gorgon.LastGazed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.GazeCooldown);
                gorgon.LastKilled = DateTime.UtcNow;
                gorgon.LastKilled = gorgon.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ChaosDriveKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Poisoner))
            {
                var poisoner = (Poisoner)role;
                poisoner.LastPoisoned = DateTime.UtcNow;
                poisoner.LastPoisoned = poisoner.LastPoisoned.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.PoisonCd);
                poisoner.LastKilled = DateTime.UtcNow;
                poisoner.LastKilled = poisoner.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ChaosDriveKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Shapeshifter))
            {
                var ss = (Shapeshifter)role;
                ss.LastShapeshifted = DateTime.UtcNow;
                ss.LastShapeshifted = ss.LastShapeshifted.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ShapeshiftCooldown);
                ss.LastKilled = DateTime.UtcNow;
                ss.LastKilled = ss.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ChaosDriveKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Rebel))
            {
                var rebel = (Rebel)role;
                rebel.LastKilled = DateTime.UtcNow;
                rebel.LastKilled = rebel.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ChaosDriveKillCooldown);
                rebel.FormerRole = null;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Warper))
            {
                var warper = (Warper)role;
                warper.LastWarped = DateTime.UtcNow;
                warper.LastWarped = warper.LastWarped.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.WarpCooldown);
                warper.LastKilled = DateTime.UtcNow;
                warper.LastKilled = warper.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ChaosDriveKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Drunkard))
            {
                var drunkard = (Drunkard)role;
                drunkard.LastConfused = DateTime.UtcNow;
                drunkard.LastConfused = drunkard.LastConfused.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConfuseCooldown);
                drunkard.LastKilled = DateTime.UtcNow;
                drunkard.LastKilled = drunkard.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ChaosDriveKillCooldown);
            }

            //Neutral starting cooldowns
            foreach (var role in Role.GetRoles(RoleEnum.Arsonist))
            {
                var arsonist = (Arsonist)role;
                arsonist.LastDoused = DateTime.UtcNow;
                arsonist.LastDoused = arsonist.LastDoused.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DouseCd);
                arsonist.LastIgnited = DateTime.UtcNow;
                arsonist.LastIgnited = arsonist.LastIgnited.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IgniteCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Cannibal))
            {
                var cann = (Cannibal)role;
                cann.LastEaten = DateTime.UtcNow;
                cann.LastEaten = cann.LastEaten.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CannibalCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Cryomaniac))
            {
                var cryo = (Cryomaniac)role;
                cryo.LastDoused = DateTime.UtcNow;
                cryo.LastDoused = cryo.LastDoused.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DouseCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Whisperer))
            {
                var whisperer = (Whisperer)role;
                whisperer.LastWhispered = DateTime.UtcNow;
                whisperer.LastWhispered = whisperer.LastWhispered.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.WhisperCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Werewolf))
            {
                var ww = (Werewolf)role;
                ww.LastMauled = DateTime.UtcNow;
                ww.LastMauled = ww.LastMauled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MaulCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Troll))
            {
                var troll = (Troll)role;
                troll.LastInteracted = DateTime.UtcNow;
                troll.LastInteracted = troll.LastInteracted.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InteractCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Thief))
            {
                var thief = (Thief)role;
                thief.LastKilled = DateTime.UtcNow;
                thief.LastKilled = thief.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ThiefKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Survivor))
            {
                var surv = (Survivor)role;
                surv.LastVested = DateTime.UtcNow;
                surv.LastVested = surv.LastVested.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.VestCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.SerialKiller))
            {
                var sk = (SerialKiller)role;
                sk.LastLusted = DateTime.UtcNow;
                sk.LastKilled = DateTime.UtcNow;
                sk.LastLusted = sk.LastLusted.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BloodlustCd);
                sk.LastKilled = sk.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.LustKillCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Plaguebearer))
            {
                var plaguebearer = (Plaguebearer)role;
                plaguebearer.LastInfected = DateTime.UtcNow;
                plaguebearer.LastInfected = plaguebearer.LastInfected.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InfectCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Pestilence))
            {
                var pestilence = (Pestilence)role;
                pestilence.LastKilled = DateTime.UtcNow;
                pestilence.LastKilled = pestilence.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.PestKillCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Murderer))
            {
                var murd = (Murderer)role;
                murd.LastKilled = DateTime.UtcNow;
                murd.LastKilled = murd.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MurdKCD);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Juggernaut))
            {
                var juggernaut = (Juggernaut)role;
                juggernaut.LastKilled = DateTime.UtcNow;
                juggernaut.LastKilled = juggernaut.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.JuggKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Jackal))
            {
                var jack = (Jackal)role;
                jack.LastRecruited = DateTime.UtcNow;
                jack.LastRecruited = jack.LastRecruited.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.RecruitCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel)role;

                if (ga.TargetPlayer == null)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                    writer.Write((byte)TurnRPC.GAToSurv);
                    writer.Write(ga.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    GATargetColor.GAToSurv(ga.Player);
                    continue;
                }

                ga.LastProtected = DateTime.UtcNow;
                ga.LastProtected = ga.LastProtected.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ProtectCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Glitch))
            {
                var glitch = (Glitch)role;
                glitch.LastHack = DateTime.UtcNow;
                glitch.LastHack = glitch.LastHack.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.HackCooldown);
                glitch.LastMimic = DateTime.UtcNow;
                glitch.LastMimic = glitch.LastMimic.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MimicCooldown);
                glitch.LastKilled = DateTime.UtcNow;
                glitch.LastKilled = glitch.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.GlitchKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Executioner))
            {
                var exe = (Executioner)role;

                if (exe.TargetPlayer == null)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                    writer.Write((byte)TurnRPC.ExeToJest);
                    writer.Write(exe.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    TargetColor.ExeToJest(exe.Player);
                }
            }

            foreach (var role in Role.GetRoles(RoleEnum.Guesser))
            {
                var guess = (Guesser)role;

                if (guess.TargetPlayer == null)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                    writer.Write((byte)TurnRPC.GuessToAct);
                    writer.Write(guess.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    GuessTargetColor.GuessToAct(guess.Player);
                }
            }

            foreach (var role in Role.GetRoles(RoleEnum.BountyHunter))
            {
                var bh = (BountyHunter)role;

                if (bh.TargetPlayer == null)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                    writer.Write((byte)TurnRPC.BHToTroll);
                    writer.Write(bh.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    BHTargetColor.BHToTroll(bh.Player);
                }

                bh.LastChecked = DateTime.UtcNow;
                bh.LastChecked = bh.LastChecked.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BiteCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Necromancer))
            {
                var necromancer = (Necromancer)role;
                necromancer.LastResurrected = DateTime.UtcNow;
                necromancer.LastResurrected = necromancer.LastResurrected.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ResurrectCooldown);
                necromancer.LastKilled = DateTime.UtcNow;
                necromancer.LastKilled = necromancer.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.NecroKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Dracula))
            {
                var drac = (Dracula)role;
                drac.LastBitten = DateTime.UtcNow;
                drac.LastBitten = drac.LastBitten.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BiteCd);
            }
        }
    }
}