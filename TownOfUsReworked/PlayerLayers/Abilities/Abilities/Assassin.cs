using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ExecutionerMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuardianAngelMod;

namespace TownOfUsReworked.PlayerLayers.Abilities.Abilities
{
    public class Assassin : Ability
    {
        public Dictionary<byte, (GameObject, GameObject, GameObject, TMP_Text)> Buttons = new Dictionary<byte, (GameObject, GameObject, GameObject, TMP_Text)>();
        private Dictionary<string, Color> ColorMapping = new Dictionary<string, Color>();
        public Dictionary<string, Color> SortedColorMapping;
        public Dictionary<byte, string> Guesses = new Dictionary<byte, string>();
        public bool GuessedThisMeeting { get; set; } = false;
        public int RemainingKills { get; set; }
        public List<string> PossibleGuesses => SortedColorMapping.Keys.ToList();

        public Assassin(PlayerControl player) : base(player)
        {
            Name = "Assassin";
            TaskText = "Guess and shoot";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Assassin : Colors.Ability;
            AbilityType = AbilityEnum.Assassin;
            RemainingKills = CustomGameOptions.AssassinKills;
            AddToAbilityHistory(AbilityType);

            //Adds all the roles that have a non-zero chance of being in the game
            if (!PlayerControl.LocalPlayer.Is(Faction.Crew))
            {
                ColorMapping.Add("Crewmate", Colors.Crew);

                if (CustomGameOptions.MayorOn > 0)
                    ColorMapping.Add("Mayor", Colors.Mayor);

                if (CustomGameOptions.SheriffOn > 0)
                    ColorMapping.Add("Sheriff", Colors.Sheriff);

                if (CustomGameOptions.EngineerOn > 0)
                    ColorMapping.Add("Engineer", Colors.Engineer);

                if (CustomGameOptions.SwapperOn > 0)
                    ColorMapping.Add("Swapper", Colors.Swapper);

                if (CustomGameOptions.InvestigatorOn > 0)
                    ColorMapping.Add("Investigator", Colors.Investigator);

                if (CustomGameOptions.TimeLordOn > 0)
                    ColorMapping.Add("Time Lord", Colors.TimeLord);

                if (CustomGameOptions.MedicOn > 0)
                    ColorMapping.Add("Medic", Colors.Medic);

                if (CustomGameOptions.AgentOn > 0)
                    ColorMapping.Add("Agent", Colors.Agent);

                if (CustomGameOptions.AltruistOn > 0)
                    ColorMapping.Add("Altruist", Colors.Altruist);

                if (CustomGameOptions.VeteranOn > 0)
                    ColorMapping.Add("Veteran", Colors.Veteran);
                
                if (CustomGameOptions.TrackerOn > 0)
                    ColorMapping.Add("Tracker", Colors.Tracker);

                if (CustomGameOptions.OperativeOn > 0)
                    ColorMapping.Add("Operative", Colors.Operative);

                if (CustomGameOptions.TransporterOn > 0)
                    ColorMapping.Add("Transporter", Colors.Transporter);

                if (CustomGameOptions.MediumOn > 0)
                    ColorMapping.Add("Medium", Colors.Medium);

                if (CustomGameOptions.CoronerOn > 0)
                    ColorMapping.Add("Coroner", Colors.Coroner);

                if (CustomGameOptions.DetectiveOn > 0)
                    ColorMapping.Add("Detective", Colors.Detective);

                if (CustomGameOptions.ShifterOn > 0)
                    ColorMapping.Add("Shifter", Colors.Shifter);

                if (CustomGameOptions.InspectorOn > 0)
                    ColorMapping.Add("Inspector", Colors.Inspector);

                if (CustomGameOptions.EscortOn > 0)
                    ColorMapping.Add("Escort", Colors.Escort);

                if (CustomGameOptions.VigilanteOn > 0)
                    ColorMapping.Add("Vigilante", Colors.Vigilante);

                if (CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0 && !PlayerControl.LocalPlayer.Is(SubFaction.Undead))
                    ColorMapping.Add(("Vampire Hunter"), Colors.VampireHunter);
            }

            if (!PlayerControl.LocalPlayer.Is(Faction.Intruder))
            {
                ColorMapping.Add("Impostor", Colors.Intruder);

                if (CustomGameOptions.JanitorOn > 0)
                    ColorMapping.Add("Janitor", Colors.Janitor);

                if (CustomGameOptions.MorphlingOn > 0)
                    ColorMapping.Add("Morphling", Colors.Morphling);

                if (CustomGameOptions.MinerOn > 0)
                    ColorMapping.Add("Miner", Colors.Miner);

                if (CustomGameOptions.WraithOn > 0)
                    ColorMapping.Add("Wraith", Colors.Wraith);

                if (CustomGameOptions.UndertakerOn > 0)
                    ColorMapping.Add("Undertaker", Colors.Undertaker);

                if (CustomGameOptions.GrenadierOn > 0)
                    ColorMapping.Add("Grenadier", Colors.Grenadier);

                if (CustomGameOptions.PoisonerOn > 0)
                    ColorMapping.Add("Poisoner", Colors.Poisoner);

                if (CustomGameOptions.BlackmailerOn > 0)
                    ColorMapping.Add("Blackmailer", Colors.Blackmailer);

                if (CustomGameOptions.CamouflagerOn > 0)
                    ColorMapping.Add("Camouflager", Colors.Camouflager);

                if (CustomGameOptions.DisguiserOn > 0)
                    ColorMapping.Add("Disguiser", Colors.Disguiser);

                if (CustomGameOptions.TimeMasterOn > 0)
                    ColorMapping.Add("Time Master", Colors.TimeMaster);

                if (CustomGameOptions.DisguiserOn > 0)
                    ColorMapping.Add("Consigliere", Colors.Consigliere);

                if (CustomGameOptions.ConsortOn > 0)
                    ColorMapping.Add("Consort", Colors.Consort);

                if (CustomGameOptions.GodfatherOn > 0)
                {
                    ColorMapping.Add("Godfather", Colors.Godfather);
                    ColorMapping.Add("Mafioso", Colors.Mafioso);
                }
            }

            if (!PlayerControl.LocalPlayer.Is(Faction.Syndicate))
            {
                ColorMapping.Add("Anarchist", Colors.Syndicate);

                if (CustomGameOptions.PuppeteerOn > 0)
                    ColorMapping.Add("Puppeteer", Colors.Puppeteer);

                if (CustomGameOptions.WarperOn > 0)
                    ColorMapping.Add("Warper", Colors.Warper);

                if (CustomGameOptions.ConcealerOn > 0)
                    ColorMapping.Add("Concealer", Colors.Concealer);

                if (CustomGameOptions.GorgonOn > 0)
                    ColorMapping.Add("Gorgon", Colors.Gorgon);

                if (CustomGameOptions.ShapeshifterOn > 0)
                    ColorMapping.Add("Shapeshifter", Colors.Shapeshifter);

                if (CustomGameOptions.RebelOn > 0)
                {
                    ColorMapping.Add("Rebel", Colors.Rebel);
                    ColorMapping.Add("Sidekick", Colors.Sidekick);
                }
            }

            if (CustomGameOptions.ArsonistOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist))
                ColorMapping.Add("Arsonist", Colors.Arsonist);

            if (CustomGameOptions.GlitchOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Glitch))
                ColorMapping.Add("Glitch", Colors.Glitch);

            if (CustomGameOptions.PlaguebearerOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer))
                ColorMapping.Add("Plaguebearer", Colors.Plaguebearer);

            if (CustomGameOptions.SerialKillerOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller))
                ColorMapping.Add("Serial Killer", Colors.SerialKiller);

            if (CustomGameOptions.JuggernautOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut))
                ColorMapping.Add("Juggernaut", Colors.Juggernaut);

            if (CustomGameOptions.MurdererOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Murderer))
                ColorMapping.Add("Murderer", Colors.Murderer);

            if (CustomGameOptions.CryomaniacOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Murderer))
                ColorMapping.Add("Cryomaniac", Colors.Cryomaniac);

            if (CustomGameOptions.WerewolfOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf))
                ColorMapping.Add("Werewolf", Colors.Werewolf);

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Pestilence) && CustomGameOptions.AssassinGuessPest && CustomGameOptions.PlaguebearerOn > 0)
                ColorMapping.Add("Pestilence", Colors.Pestilence);
            
            if (CustomGameOptions.DraculaOn > 0 && !PlayerControl.LocalPlayer.Is(SubFaction.Undead))
            {
                ColorMapping.Add("Dracula", Colors.Dracula);
                ColorMapping.Add("Vampire", Colors.Vampire);
                ColorMapping.Add("Dampyr", Colors.Dampyr);
            }
            
            if (CustomGameOptions.JackalOn > 0 && !PlayerControl.LocalPlayer.Is(SubFaction.Cabal))
            {
                ColorMapping.Add("Jackal", Colors.Jackal);
                ColorMapping.Add("Recruit", Colors.Recruit);
            }

            //Add certain Neutral roles if enabled
            if (CustomGameOptions.AssassinGuessNeutralBenign)
            {
                if (CustomGameOptions.AmnesiacOn > 0 | (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Amnesiac) |
                    (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Amnesiac))
                    ColorMapping.Add("Amnesiac", Colors.Amnesiac);

                if (CustomGameOptions.SurvivorOn > 0 | (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Survivor) |
                    (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Survivor))
                        ColorMapping.Add("Survivor", Colors.Survivor);

                if (CustomGameOptions.GuardianAngelOn > 0)
                    ColorMapping.Add("Guardian Angel", Colors.GuardianAngel);

                if (CustomGameOptions.ThiefOn > 0)
                    ColorMapping.Add("Thief", Colors.Thief);
            }

            if (CustomGameOptions.AssassinGuessNeutralEvil)
            {
                if (CustomGameOptions.CannibalOn > 0)
                    ColorMapping.Add("Cannibal", Colors.Cannibal);

                if (CustomGameOptions.ExecutionerOn > 0)
                    ColorMapping.Add("Executioner", Colors.Executioner);

                /*if (CustomGameOptions.PirateOn > 0)
                    ColorMapping.Add("Pirate", Colors.Pirate);*/

                if (CustomGameOptions.TrollOn > 0)
                    ColorMapping.Add("Troll", Colors.Troll);

                if (CustomGameOptions.JesterOn > 0 | (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Jester) |
                    (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Jester))
                    ColorMapping.Add("Jester", Colors.Jester);
            }

            //Add Modifiers if enabled
            if (CustomGameOptions.AssassinGuessModifiers)
            {
                if (CustomGameOptions.BaitOn > 0)
                    ColorMapping.Add("Bait", Colors.Bait);

                if (CustomGameOptions.DiseasedOn > 0)
                    ColorMapping.Add("Diseased", Colors.Diseased);

                if (CustomGameOptions.ProfessionalOn > 0)
                    ColorMapping.Add("Professional", Colors.Professional);

                if (CustomGameOptions.CowardOn > 0)
                    ColorMapping.Add("Coward", Colors.Coward);

                if (CustomGameOptions.ShyOn > 0)
                    ColorMapping.Add("Shy", Colors.Shy);

                if (CustomGameOptions.VIPOn > 0)
                    ColorMapping.Add("VIP", Colors.VIP);
            }

            //Add Objectifiers if enabled
            if (CustomGameOptions.AssassinGuessObjectifiers)
            {
                if (CustomGameOptions.LoversOn > 0)
                    ColorMapping.Add("Lover", Colors.Lovers);

                if (CustomGameOptions.TaskmasterOn > 0)
                    ColorMapping.Add("Tasmaster", Colors.Taskmaster);
                    
                if (CustomGameOptions.TraitorOn > 0)
                    ColorMapping.Add("Traitor", Colors.Traitor);

                if (CustomGameOptions.FanaticOn > 0)
                    ColorMapping.Add("Fanatic", Colors.Lovers);

                if (CustomGameOptions.RivalsOn > 0)
                    ColorMapping.Add("Rivals", Colors.Traitor);

                if (CustomGameOptions.PhantomOn > 0)
                    ColorMapping.Add("Phantom", Colors.Phantom);
            }

            //Add Abilities if enabled
            if (CustomGameOptions.AssassinGuessAbilities)
            {
                if (CustomGameOptions.AssassinOn > 0)
                    ColorMapping.Add("Assassin", Colors.Assassin);

                if (CustomGameOptions.TorchOn > 0)
                    ColorMapping.Add("Torch", Colors.Torch);

                if (CustomGameOptions.LighterOn > 0)
                    ColorMapping.Add("Lighter", Colors.Lighter);

                if (CustomGameOptions.UnderdogOn > 0)
                    ColorMapping.Add("Underdog", Colors.Underdog);

                if (CustomGameOptions.RadarOn > 0)
                    ColorMapping.Add("Radar", Colors.Radar);

                if (CustomGameOptions.TiebreakerOn > 0)
                    ColorMapping.Add("Tiebreaker", Colors.Tiebreaker);

                if (CustomGameOptions.MultitaskerOn > 0)
                    ColorMapping.Add("Multitasker", Colors.Multitasker);

                if (CustomGameOptions.SnitchOn > 0)
                    ColorMapping.Add("Snitch", Colors.Snitch);

                if (CustomGameOptions.RevealerOn > 0)
                    ColorMapping.Add("Revealer", Colors.Revealer);

                if (CustomGameOptions.TunnelerOn > 0)
                    ColorMapping.Add("Tunneler", Colors.Tunneler);

                if (CustomGameOptions.ButtonBarryOn > 0)
                    ColorMapping.Add("Button Barry", Colors.ButtonBarry);
            }

            //Sorts the list alphabetically. 
            SortedColorMapping = ColorMapping.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
