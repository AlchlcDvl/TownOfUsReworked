using System;
using HarmonyLib;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EngineerMod;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.OperativeMod;

namespace TownOfUsReworked.PlayerLayers.Roles.AllRoles
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject)
                return;

            //Crew cooldowns
            foreach (var role in Role.GetRoles(RoleEnum.Chameleon))
            {
                var role2 = (Chameleon)role;
                role2.LastSwooped = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Detective))
            {
                var role2 = (Detective)role;
                role2.LastExamined = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Escort))
            {
                var role2 = (Escort)role;
                role2.LastBlock = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Inspector))
            {
                var role2 = (Inspector)role;
                role2.LastInspected = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Medium))
            {
                var role2 = (Medium)role;
                role2.LastMediated = DateTime.UtcNow;
                role2.MediatedPlayers.Values.DestroyAll();
                role2.MediatedPlayers.Clear();
            }

            foreach (var role in Role.GetRoles(RoleEnum.Operative))
            {
                var role2 = (Operative)role;
                role2.LastBugged = DateTime.UtcNow;
                role2.BuggedPlayers.Clear();
                
                if (CustomGameOptions.BugsRemoveOnNewRound)
                    role2.Bugs.ClearBugs();
            }

            foreach (var role in Role.GetRoles(RoleEnum.Sheriff))
            {
                var role2 = (Sheriff)role;
                role2.LastInterrogated = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Shifter))
            {
                var role2 = (Shifter)role;
                role2.LastShifted = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.TimeLord))
            {
                var role2 = (TimeLord)role;
                role2.FinishRewind = DateTime.UtcNow;
                role2.StartRewind = DateTime.UtcNow;
                role2.StartRewind = role2.StartRewind.AddSeconds(-10.0f);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Tracker))
            {
                var role2 = (Tracker)role;
                role2.LastTracked = DateTime.UtcNow;

                if (CustomGameOptions.ResetOnNewRound)
                {
                    role2.UsesLeft = CustomGameOptions.MaxTracks;
                    role2.TrackerArrows.Values.DestroyAll();
                    role2.TrackerArrows.Clear();
                }
            }

            foreach (var role in Role.GetRoles(RoleEnum.Transporter))
            {
                var role2 = (Transporter)role;
                role2.LastTransported = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.VampireHunter))
            {
                var role2 = (VampireHunter)role;
                role2.LastStaked = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Veteran))
            {
                var role2 = (Veteran)role;
                role2.LastAlerted = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Vigilante))
            {
                var role2 = (Vigilante)role;
                role2.FirstRound = false;
                role2.LastKilled = DateTime.UtcNow;
            }

            //Intruder cooldowns
            foreach (var role in Role.GetRoles(RoleEnum.Blackmailer))
            {
                var role2 = (Blackmailer)role;
                role2.Blackmailed = null;
                role2.LastBlackmailed = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;

                if (role2.Player.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    role2.Blackmailed?.myRend().material.SetFloat("_Outline", 0f);
            }

            foreach (var role in Role.GetRoles(RoleEnum.Camouflager))
            {
                var role2 = (Camouflager)role;
                role2.LastCamouflaged = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Consigliere))
            {
                var role2 = (Consigliere)role;
                role2.LastInvestigated = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Consort))
            {
                var role2 = (Consort)role;
                role2.LastBlock = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Disguiser))
            {
                var role2 = (Disguiser)role;
                role2.DisguiseButton.graphic.sprite = TownOfUsReworked.MeasureSprite;
                role2.MeasuredPlayer = null;
                role2.LastDisguised = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Godfather))
            {
                var role2 = (Godfather)role;
                role2.LastKilled = DateTime.UtcNow;

                if (!role2.HasDeclared && !role2.WasMafioso)
                    role2.LastDeclared = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Grenadier))
            {
                var role2 = (Grenadier)role;
                role2.LastFlashed = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Impostor))
            {
                var role2 = (Impostor)role;
                role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Mafioso))
            {
                var role2 = (Mafioso)role;
                role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Janitor))
            {
                var role2 = Role.GetRole<Janitor>(PlayerControl.LocalPlayer);
                role2.LastCleaned = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Miner))
            {
                var role2 = (Miner)role;
                role2.LastMined = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Morphling))
            {
                var role2 = (Morphling)role;
                role2.MorphButton.graphic.sprite = TownOfUsReworked.SampleSprite;
                role2.SampledPlayer = null;
                role2.LastMorphed = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Teleporter))
            {
                var role2 = (Teleporter)role;
                role2.TeleportButton.graphic.sprite = TownOfUsReworked.MarkSprite;
                role2.LastTeleport = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Undertaker))
            {
                var role2 = (Undertaker)role;
                role2.DragDropButton.graphic.sprite = TownOfUsReworked.DragSprite;
                role2.CurrentlyDragging = null;
                role2.LastDragged = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.TimeMaster))
            {
                var role2 = (TimeMaster)role;
                role2.LastFrozen = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Wraith))
            {
                var role2 = (Wraith)role;
                role2.LastInvis = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }

            //Syndicate cooldowns
            foreach (var role in Role.GetRoles(RoleEnum.Anarchist))
            {
                var role2 = (Anarchist)role;
                role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Concealer))
            {
                var role2 = (Concealer)role;
                role2.LastConcealed = DateTime.UtcNow;

                if (Role.SyndicateHasChaosDrive)
                    role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Framer))
            {
                var role2 = (Framer)role;
                role2.LastFramed = DateTime.UtcNow;

                if (Role.SyndicateHasChaosDrive)
                    role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Gorgon))
            {
                var role2 = (Gorgon)role;
                role2.LastGazed = DateTime.UtcNow;

                if (Role.SyndicateHasChaosDrive)
                    role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Poisoner))
            {
                var role2 = (Poisoner)role;
                role2.LastPoisoned = DateTime.UtcNow;
                role2.PoisonButton.graphic.sprite = TownOfUsReworked.PoisonSprite;

                if (Role.SyndicateHasChaosDrive)
                    role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Rebel))
            {
                var role2 = (Rebel)role;
                role2.LastKilled = DateTime.UtcNow;

                if (!role2.HasDeclared && !role2.WasSidekick)
                    role2.LastDeclared = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Shapeshifter))
            {
                var role2 = (Shapeshifter)role;
                role2.LastShapeshifted = DateTime.UtcNow;

                if (Role.SyndicateHasChaosDrive)
                    role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Sidekick))
            {
                var role2 = (Sidekick)role;
                role2.LastKilled = DateTime.UtcNow;

                if (Role.SyndicateHasChaosDrive)
                    role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Warper))
            {
                var role2 = (Warper)role;
                role2.LastWarped = DateTime.UtcNow;

                if (Role.SyndicateHasChaosDrive)
                    role2.LastKilled = DateTime.UtcNow;
            }

            //Neutral cooldowns
            foreach (var role in Role.GetRoles(RoleEnum.Arsonist))
            {
                var role2 = (Arsonist) role;
                role2.LastDoused = DateTime.UtcNow;
                role2.LastIgnited = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Cannibal))
            {
                var role2 = (Cannibal) role;
                role2.LastEaten = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Cryomaniac))
            {
                var role2 = (Cryomaniac) role;
                role2.LastDoused = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Dampyr))
            {
                var role2 = (Dampyr) role;
                role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Dracula))
            {
                var role2 = (Dracula) role;
                role2.LastBitten = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Glitch))
            {
                var role2 = (Glitch) role;
                role2.LastMimic = DateTime.UtcNow;
                role2.LastHack = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.GuardianAngel))
            {
                var role2 = (GuardianAngel)role;
                role2.LastProtected = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Jackal))
            {
                var role2 = (Jackal)role;
                role2.LastRecruited = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Jester))
            {
                var role2 = (Jester)role;

                if (role2.VotedOut)
                    role2.LastHaunted = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Juggernaut))
            {
                var role2 = (Juggernaut) role;
                role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Murderer))
            {
                var role2 = (Murderer) role;
                role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Pestilence))
            {
                var role2 = (Pestilence) role;
                role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Plaguebearer))
            {
                var role2 = (Plaguebearer) role;
                role2.LastInfected = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.SerialKiller))
            {
                var role2 = (SerialKiller) role;
                role2.LastLusted = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Survivor))
            {
                var role2 = (Survivor) role;
                role2.LastVested = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Thief))
            {
                var role2 = (Thief) role;
                role2.LastKilled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Troll))
            {
                var role2 = (Troll)role;
                role2.LastInteracted = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Werewolf))
            {
                var role2 = (Werewolf) role;
                role2.LastMauled = DateTime.UtcNow;
            }

            foreach (var role in Role.GetRoles(RoleEnum.Whisperer))
            {
                var role2 = (Whisperer) role;
                role2.LastWhispered = DateTime.UtcNow;
            }
        }
    }
}