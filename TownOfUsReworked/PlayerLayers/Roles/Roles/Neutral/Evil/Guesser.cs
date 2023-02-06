using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Guesser : Role
    {
        public Dictionary<byte, (GameObject, GameObject, GameObject, TMP_Text)> MoarButtons = new Dictionary<byte, (GameObject, GameObject, GameObject, TMP_Text)>();
        private Dictionary<string, Color> ColorMapping = new Dictionary<string, Color>();
        public Dictionary<string, Color> SortedColorMapping;
        public Dictionary<byte, string> Guesses = new Dictionary<byte, string>();
        public PlayerControl TargetPlayer;
        public bool TargetGuessed = false;
        public bool GuessedThisMeeting { get; set; } = false;
        public int RemainingGuesses { get; set; }
        public int RemainingHints { get; set; }
        public List<string> PossibleGuesses => SortedColorMapping.Keys.ToList();
        public bool GuesserWins { get; set; }

        public Guesser(PlayerControl player) : base(player)
        {
            Name = "Guesser";
            RoleType = RoleEnum.Guesser;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = "Neutral (Evil)";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Guesser : Colors.Neutral;
            RemainingGuesses = CustomGameOptions.GuessCount;
            RemainingHints = CustomGameOptions.MeetingHintCount;

            //Adds all the roles that have a non-zero chance of being in the game
            ColorMapping.Add("Crewmate", Colors.Crew);

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

            /*if (CustomGameOptions.SeerOn > 0)
                ColorMapping.Add("Seer", Colors.Seer);*/

            if (CustomGameOptions.MysticOn > 0)
                ColorMapping.Add("Mystic", Colors.Mystic);

            if (CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0)
                ColorMapping.Add(("Vampire Hunter"), Colors.VampireHunter);

            if (!CustomGameOptions.AltImps && CustomGameOptions.IntruderCount > 0)
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

            if ( CustomGameOptions.SyndicateCount > 0)
            {
                ColorMapping.Add("Anarchist", Colors.Syndicate);

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

                if (CustomGameOptions.PoisonerOn > 0)
                    ColorMapping.Add("Poisoner", Colors.Poisoner);

                if (CustomGameOptions.BomberOn > 0)
                    ColorMapping.Add("Bomber", Colors.Bomber);

                if (CustomGameOptions.RebelOn > 0)
                {
                    ColorMapping.Add("Rebel", Colors.Rebel);
                    ColorMapping.Add("Sidekick", Colors.Sidekick);
                }
            }

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
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Pestilence))
                    ColorMapping.Add("Pestilence", Colors.Pestilence);
                
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer))
                    ColorMapping.Add("Plaguebearer", Colors.Plaguebearer);
            }
            
            if (CustomGameOptions.DraculaOn > 0)
            {
                ColorMapping.Add("Dracula", Colors.Dracula);
                ColorMapping.Add("Vampire", Colors.Vampire);
                ColorMapping.Add("Dampyr", Colors.Dampyr);
            }
            
            if (CustomGameOptions.JackalOn > 0)
            {
                ColorMapping.Add("Jackal", Colors.Jackal);
                ColorMapping.Add("Recruit", Colors.Cabal);
            }
            
            if (CustomGameOptions.NecromancerOn > 0)
            {
                ColorMapping.Add("Necromancer", Colors.Necromancer);
                ColorMapping.Add("Ressurected", Colors.Reanimated);
            }
            
            if (CustomGameOptions.WhispererOn > 0)
            {
                ColorMapping.Add("Whisperer", Colors.Whisperer);
                ColorMapping.Add("Persuaded", Colors.Sect);
            }

            SortedColorMapping = ColorMapping.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }

        public override void Wins()
        {
            if (Player.Data.IsDead || Player.Data.Disconnected || TargetPlayer == null)
                return;
                
            GuesserWins = true;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (!TargetGuessed || !Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (TargetGuessed)
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                writer.Write((byte)WinLoseRPC.GuesserWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }   
            
            return true;
        }
    }
}
