using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using Reactor.Utilities;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Guesser : NeutralRole
    {
        public Dictionary<byte, (GameObject, GameObject, GameObject, TMP_Text)> MoarButtons = new();
        private readonly Dictionary<string, Color> ColorMapping = new();
        public Dictionary<string, Color> SortedColorMapping;
        public Dictionary<byte, string> Guesses = new();
        public PlayerControl TargetPlayer;
        public bool TargetGuessed;
        public bool GuessedThisMeeting;
        public int RemainingGuesses;
        public List<string> PossibleGuesses => SortedColorMapping.Keys.ToList();
        public bool GuesserWins;
        public bool FactionHintGiven;
        public bool AlignmentHintGiven;
        public bool SubFactionHintGiven;
        public bool Failed => !TargetGuessed && (RemainingGuesses <= 0f || TargetPlayer?.Data.IsDead == true || TargetPlayer?.Data.Disconnected == true);

        public Guesser(PlayerControl player) : base(player)
        {
            Name = "Guesser";
            RoleType = RoleEnum.Guesser;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = NE;
            Color = CustomGameOptions.CustomNeutColors ? Colors.Guesser : Colors.Neutral;
            RemainingGuesses = CustomGameOptions.GuessCount;
            MoarButtons = new();
            ColorMapping = new()
            {
                { "Crewmate", Colors.Crew }
            };

            //Adds all the roles that have a non-zero chance of being in the game
            if (CustomGameOptions.CrewMax > 0 && CustomGameOptions.CrewMin > 0)
            {
                if (CustomGameOptions.MayorOn > 0)
                    ColorMapping.Add("Mayor", Colors.Mayor);

                if (CustomGameOptions.SheriffOn > 0)
                    ColorMapping.Add("Sheriff", Colors.Sheriff);

                if (CustomGameOptions.EngineerOn > 0)
                    ColorMapping.Add("Engineer", Colors.Engineer);

                if (CustomGameOptions.SwapperOn > 0)
                    ColorMapping.Add("Swapper", Colors.Swapper);

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

                if (CustomGameOptions.RetributionistOn > 0)
                    ColorMapping.Add("Retributionist", Colors.Retributionist);

                if (CustomGameOptions.ChameleonOn > 0)
                    ColorMapping.Add("Chameleon", Colors.Chameleon);

                if (CustomGameOptions.SeerOn > 0)
                    ColorMapping.Add("Seer", Colors.Seer);

                if (CustomGameOptions.MysticOn > 0)
                    ColorMapping.Add("Mystic", Colors.Mystic);

                if (CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0)
                    ColorMapping.Add("Vampire Hunter", Colors.VampireHunter);
            }

            if (!CustomGameOptions.AltImps && CustomGameOptions.IntruderCount > 0)
            {
                ColorMapping.Add("Impostor", Colors.Intruder);

                if (CustomGameOptions.IntruderMax > 0 && CustomGameOptions.IntruderMin > 0)
                {
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
            }

            if (CustomGameOptions.SyndicateCount > 0)
            {
                ColorMapping.Add("Anarchist", Colors.Syndicate);

                if (CustomGameOptions.SyndicateMax > 0 && CustomGameOptions.SyndicateMin > 0)
                {
                    if (CustomGameOptions.WarperOn > 0)
                        ColorMapping.Add("Warper", Colors.Warper);

                    if (CustomGameOptions.ConcealerOn > 0)
                        ColorMapping.Add("Concealer", Colors.Concealer);

                    if (CustomGameOptions.GorgonOn > 0)
                        ColorMapping.Add("Gorgon", Colors.Gorgon);

                    if (CustomGameOptions.ShapeshifterOn > 0)
                        ColorMapping.Add("Shapeshifter", Colors.Shapeshifter);

                    if (CustomGameOptions.FramerOn > 0)
                        ColorMapping.Add("Framer", Colors.Framer);

                    if (CustomGameOptions.BomberOn > 0)
                        ColorMapping.Add("Bomber", Colors.Bomber);

                    if (CustomGameOptions.PoisonerOn > 0)
                        ColorMapping.Add("Poisoner", Colors.Poisoner);

                    if (CustomGameOptions.DrunkardOn > 0)
                        ColorMapping.Add("Drunkard", Colors.Drunkard);

                    if (CustomGameOptions.BeamerOn > 0)
                        ColorMapping.Add("Beamer", Colors.Beamer);

                    if (CustomGameOptions.RebelOn > 0)
                    {
                        ColorMapping.Add("Rebel", Colors.Rebel);
                        ColorMapping.Add("Sidekick", Colors.Sidekick);
                    }
                }
            }

            if (CustomGameOptions.NeutralMax > 0 && CustomGameOptions.NeutralMin > 0)
            {
                if (CustomGameOptions.ArsonistOn > 0)
                    ColorMapping.Add("Arsonist", Colors.Arsonist);

                if (CustomGameOptions.GlitchOn > 0)
                    ColorMapping.Add("Glitch", Colors.Glitch);

                if (CustomGameOptions.SerialKillerOn > 0)
                    ColorMapping.Add("Serial Killer", Colors.SerialKiller);

                if (CustomGameOptions.JuggernautOn > 0)
                    ColorMapping.Add("Juggernaut", Colors.Juggernaut);

                if (CustomGameOptions.MurdererOn > 0)
                    ColorMapping.Add("Murderer", Colors.Murderer);

                if (CustomGameOptions.CryomaniacOn > 0)
                    ColorMapping.Add("Cryomaniac", Colors.Cryomaniac);

                if (CustomGameOptions.WerewolfOn > 0)
                    ColorMapping.Add("Werewolf", Colors.Werewolf);

                if (CustomGameOptions.PlaguebearerOn > 0)
                {
                    ColorMapping.Add("Plaguebearer", Colors.Plaguebearer);
                    ColorMapping.Add("Pestilence", Colors.Pestilence);
                }

                if (CustomGameOptions.DraculaOn > 0 && !PlayerControl.LocalPlayer.Is(SubFaction.Undead))
                {
                    ColorMapping.Add("Dracula", Colors.Dracula);
                    ColorMapping.Add("Bitten", Colors.Undead);
                }

                if (CustomGameOptions.JackalOn > 0 && !PlayerControl.LocalPlayer.Is(SubFaction.Cabal))
                {
                    ColorMapping.Add("Jackal", Colors.Jackal);
                    ColorMapping.Add("Recruit", Colors.Cabal);
                }

                if (CustomGameOptions.NecromancerOn > 0 && !PlayerControl.LocalPlayer.Is(SubFaction.Reanimated))
                {
                    ColorMapping.Add("Necromancer", Colors.Necromancer);
                    ColorMapping.Add("Resurrected", Colors.Reanimated);
                }

                if (CustomGameOptions.WhispererOn > 0 && !PlayerControl.LocalPlayer.Is(SubFaction.Sect))
                {
                    ColorMapping.Add("Whisperer", Colors.Whisperer);
                    ColorMapping.Add("Persuaded", Colors.Sect);
                }

                if (CustomGameOptions.AmnesiacOn > 0)
                    ColorMapping.Add("Amnesiac", Colors.Amnesiac);

                if (CustomGameOptions.SurvivorOn > 0 || CustomGameOptions.GuardianAngelOn > 0)
                    ColorMapping.Add("Survivor", Colors.Survivor);

                if (CustomGameOptions.GuardianAngelOn > 0)
                    ColorMapping.Add("Guardian Angel", Colors.GuardianAngel);

                if (CustomGameOptions.ThiefOn > 0)
                    ColorMapping.Add("Thief", Colors.Thief);

                if (CustomGameOptions.CannibalOn > 0)
                    ColorMapping.Add("Cannibal", Colors.Cannibal);

                if (CustomGameOptions.ExecutionerOn > 0)
                    ColorMapping.Add("Executioner", Colors.Executioner);

                if (CustomGameOptions.GuesserOn > 0)
                    ColorMapping.Add("Guesser", Colors.Guesser);

                if (CustomGameOptions.BountyHunterOn > 0)
                    ColorMapping.Add("Bounty Hunter", Colors.BountyHunter);

                if (CustomGameOptions.TrollOn > 0 || CustomGameOptions.BountyHunterOn > 0)
                    ColorMapping.Add("Troll", Colors.Troll);

                if (CustomGameOptions.ActorOn > 0 || CustomGameOptions.GuesserOn > 0)
                    ColorMapping.Add("Actor", Colors.Actor);

                if (CustomGameOptions.JesterOn > 0 || CustomGameOptions.ExecutionerOn > 0)
                    ColorMapping.Add("Jester", Colors.Jester);
            }

            SortedColorMapping = ColorMapping.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        public void TurnAct()
        {
            var targetRole = GetRole(TargetPlayer);
            var newRole  = new Actor(Player) { PretendRoles = targetRole == null ? InspectorResults.IsBasic : targetRole.InspectorResults };
            newRole.RoleUpdate(this);

            if (Player == PlayerControl.LocalPlayer)
                Utils.Flash(Colors.Actor, "Your target died so you have become an <color=#00ACC2FF>Actor</color>!");

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer, "Someone has changed their identity!");
        }
    }
}
