using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using Hazel;

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
                chameleon.LastSwooped = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SwoopCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Detective))
            {
                var detective = (Detective)role;
                detective.LastExamined = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ExamineCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Coroner))
            {
                var coroner = (Coroner)role;
                coroner.LastAutopsied = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - 10f);
                coroner.LastCompared = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CompareCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Escort))
            {
                var esc = (Escort)role;
                esc.LastBlock = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.EscRoleblockCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Inspector))
            {
                var inspector = (Inspector)role;
                inspector.LastInspected = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InspectCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Medium))
            {
                var medium = (Medium)role;
                medium.LastMediated = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MediateCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Operative))
            {
                var op = (Operative)role;
                op.LastBugged = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BugCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Sheriff))
            {
                var sheriff = (Sheriff)role;
                sheriff.LastInterrogated = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InterrogateCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Shifter))
            {
                var shifter = (Shifter)role;
                shifter.LastShifted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ShifterCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.TimeLord))
            {
                var TimeLord = (TimeLord)role;
                TimeLord.FinishRewind = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.RewindCooldown);
                TimeLord.StartRewind = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.RewindCooldown).AddSeconds(-10.0f);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Tracker))
            {
                var tracker = (Tracker)role;
                tracker.LastTracked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TrackCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Transporter))
            {
                var transporter = (Transporter)role;
                transporter.LastTransported = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TransportCooldown);
            }
            
            foreach (var role in Role.GetRoles(RoleEnum.VampireHunter))
            {
                var vampireHunter = (VampireHunter)role;
                vampireHunter.LastStaked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.StakeCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Veteran))
            {
                var veteran = (Veteran)role;
                veteran.LastAlerted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.AlertCd);
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
                vigilante.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.VigiKillCd);
            }

            //Intruder starting cooldowns
            foreach (var role in Role.GetRoles(RoleEnum.Blackmailer))
            {
                var blackmailer = (Blackmailer)role;
                blackmailer.LastBlackmailed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BlackmailCd);
                blackmailer.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Camouflager))
            {
                var camouflager = (Camouflager)role;
                camouflager.LastCamouflaged = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CamouflagerCd);
                camouflager.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Consigliere))
            {
                var consig = (Consigliere)role;
                consig.LastInvestigated = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConsigCd);
                consig.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Consort))
            {
                var consort = (Consort)role;
                consort.LastBlock = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConsRoleblockCooldown);
                consort.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Impostor))
            {
                var impostor = (Impostor)role;
                impostor.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Godfather))
            {
                var godfather = (Godfather)role;
                godfather.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
                godfather.FormerRole = null;
            }
            
            foreach (var role in Role.GetRoles(RoleEnum.Janitor))
            {
                var janitor = (Janitor)role;
                janitor.LastCleaned = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.JanitorCleanCd);
                janitor.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Disguiser))
            {
                var disguiser = (Disguiser)role;
                disguiser.LastDisguised = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DisguiseCooldown);
                disguiser.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
                disguiser.LastMeasured = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MeasureCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Grenadier))
            {
                var grenadier = (Grenadier)role;
                grenadier.LastFlashed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.GrenadeCd);
                grenadier.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Miner))
            {
                var miner = (Miner)role;
                miner.LastMined = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MineCd);
                miner.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Morphling))
            {
                var morphling = (Morphling)role;
                morphling.LastMorphed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MorphlingCd);
                morphling.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
                morphling.LastSampled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SampleCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Teleporter))
            {
                var teleporter = (Teleporter)role;
                teleporter.LastTeleport = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TeleportCd);
                teleporter.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
                teleporter.LastMarked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MarkCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.TimeMaster))
            {
                var tm = (TimeMaster)role;
                tm.LastFrozen = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.FreezeCooldown);
                tm.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Undertaker))
            {
                var undertaker = (Undertaker)role;
                undertaker.LastDragged = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DragCd);
                undertaker.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Wraith))
            {
                var wraith = (Wraith)role;
                wraith.LastInvis = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InvisCd);
                wraith.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
            }

            //Syndicate starting cooldowns
            foreach (var role in Role.GetRoles(RoleEnum.Anarchist))
            {
                var an = (Anarchist)role;
                an.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ChaosDriveKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Concealer))
            {
                var concealer = (Concealer)role;
                concealer.LastConcealed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConcealCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Beamer))
            {
                var beamer = (Beamer)role;
                beamer.LastBeamed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConcealCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Bomber))
            {
                var bomber = (Bomber)role;
                bomber.LastPlaced = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BombCooldown);
                bomber.LastDetonated = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DetonateCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Framer))
            {
                var framer = (Framer)role;
                framer.LastFramed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.FrameCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Gorgon))
            {
                var gorgon = (Gorgon)role;
                gorgon.LastGazed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.GazeCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Poisoner))
            {
                var poisoner = (Poisoner)role;
                poisoner.LastPoisoned = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.PoisonCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Shapeshifter))
            {
                var ss = (Shapeshifter)role;
                ss.LastShapeshifted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ShapeshiftCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Rebel))
            {
                var rebel = (Rebel)role;
                rebel.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ChaosDriveKillCooldown);
                rebel.FormerRole = null;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Warper))
            {
                var warper = (Warper)role;
                warper.LastWarped = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.WarpCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Drunkard))
            {
                var drunkard = (Drunkard)role;
                drunkard.LastConfused = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConfuseCooldown);
            }

            //Neutral starting cooldowns
            foreach (var role in Role.GetRoles(RoleEnum.Arsonist))
            {
                var arsonist = (Arsonist)role;
                arsonist.LastDoused = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DouseCd);
                arsonist.LastIgnited = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IgniteCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Cannibal))
            {
                var cann = (Cannibal)role;
                cann.LastEaten = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CannibalCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Cryomaniac))
            {
                var cryo = (Cryomaniac)role;
                cryo.LastDoused = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DouseCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Whisperer))
            {
                var whisperer = (Whisperer)role;
                whisperer.LastWhispered = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.WhisperCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Werewolf))
            {
                var ww = (Werewolf)role;
                ww.LastMauled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MaulCooldown);
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
                thief.LastStolen = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ThiefKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Survivor))
            {
                var surv = (Survivor)role;
                surv.LastVested = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.VestCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.SerialKiller))
            {
                var sk = (SerialKiller)role;
                sk.LastLusted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BloodlustCd);
                sk.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.LustKillCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Plaguebearer))
            {
                var plaguebearer = (Plaguebearer)role;
                plaguebearer.LastInfected = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InfectCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Pestilence))
            {
                var pestilence = (Pestilence)role;
                pestilence.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.PestKillCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Murderer))
            {
                var murd = (Murderer)role;
                murd.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MurdKCD);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Juggernaut))
            {
                var juggernaut = (Juggernaut)role;
                juggernaut.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.JuggKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Jackal))
            {
                var jack = (Jackal)role;
                jack.LastRecruited = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.RecruitCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel)role;

                if (!ga.TargetAlive)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                    writer.Write((byte)TurnRPC.TurnSurv);
                    writer.Write(ga.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    ga.TurnSurv();
                }
                else
                    ga.LastProtected = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ProtectCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Glitch))
            {
                var glitch = (Glitch)role;
                glitch.LastHack = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.HackCooldown);
                glitch.LastMimic = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MimicCooldown);
                glitch.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.GlitchKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Executioner))
            {
                var exe = (Executioner)role;

                if (exe.Failed)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                    writer.Write((byte)TurnRPC.TurnJest);
                    writer.Write(exe.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    exe.TurnJest();
                }
            }

            foreach (var role in Role.GetRoles(RoleEnum.Guesser))
            {
                var guess = (Guesser)role;

                if (guess.Failed)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                    writer.Write((byte)TurnRPC.TurnAct);
                    writer.Write(guess.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    guess.TurnAct();
                }
            }

            foreach (var role in Role.GetRoles(RoleEnum.BountyHunter))
            {
                var bh = (BountyHunter)role;

                if (bh.Failed)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                    writer.Write((byte)TurnRPC.TurnTroll);
                    writer.Write(bh.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    bh.TurnTroll();
                }
                else
                    bh.LastChecked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BiteCd);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Necromancer))
            {
                var necromancer = (Necromancer)role;
                necromancer.LastResurrected = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ResurrectCooldown);
                necromancer.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.NecroKillCooldown);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Dracula))
            {
                var drac = (Dracula)role;
                drac.LastBitten = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BiteCd);
            }
        }
    }
}