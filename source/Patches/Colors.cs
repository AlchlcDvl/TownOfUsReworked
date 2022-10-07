using UnityEngine;

namespace TownOfUs.Patches
{
    class Colors {

        // Crew Colors
        public readonly static Color Crew = new Color(0.545f, 0.992f, 0.992f, 1f);
        public readonly static Color Mayor = new Color(0.44f, 0.31f, 0.66f, 1f);
        public readonly static Color Vigilante = new Color(1f, 1f, 0f, 1f);
        public readonly static Color Engineer = new Color(1f, 0.65f, 0.04f, 1f);
        public readonly static Color Swapper = new Color(0.4f, 0.9f, 0.4f, 1f);
        public readonly static Color Investigator = new Color(0f, 0.7f, 0.7f, 1f);
        public readonly static Color TimeLord = new Color(0f, 0f, 1f, 1f);
        public readonly static Color Medic = new Color(0f, 0.4f, 0f, 1f);
        public readonly static Color Sheriff = new Color(1f, 0.8f, 0.5f, 1f);
        public readonly static Color Agent = new Color(0.8f, 0.64f, 0.8f, 1f);
        public readonly static Color Snitch = new Color(0.83f, 0.69f, 0.22f, 1f);
        public readonly static Color Altruist = new Color(0.4f, 0f, 0f, 1f);
        public readonly static Color Veteran = new Color(0.6f, 0.5f, 0.25f, 1f);
        public readonly static Color Haunter = new Color(0.83f, 0.83f, 0.83f, 1f);
        public readonly static Color Tracker = new Color(0f, 0.6f, 0f, 1f);
        public readonly static Color Transporter = new Color(0f, 0.93f, 1f, 1f);
        public readonly static Color Medium = new Color(0.65f, 0.5f, 1f, 1f);
        public readonly static Color Mystic = new Color(0.3f, 0.6f, 0.9f, 1f);
        public readonly static Color Operative = new Color(0.65f, 0.82f, 0.7f, 1f);
        public readonly static Color Detective = new Color(0.3f, 0.3f, 1f, 1f);
        public readonly static Color Shifter = new Color(0.876f, 0.523f, 0.123f, 1f);

        // Neutral Colors
        public readonly static Color Neutral = new Color(0.7f, 0.7f, 0.7f, 1f);
        public readonly static Color Jester = new Color(0.969f, 0.702f, 0.855f, 1f);
        public readonly static Color Executioner = new Color(0.8f, 0.8f, 0.8f, 1f);
        public readonly static Color Glitch = new Color(0f, 1f, 0f, 1f);
        public readonly static Color Arsonist = new Color(0.934f, 0.463f, 0f, 1f);
        public readonly static Color Phantom = new Color(0.4f, 0.16f, 0.38f, 1f);
        public readonly static Color Amnesiac = new Color(0.134f, 1f, 1f, 1f);
        public readonly static Color Survivor = new Color(0.867f, 0.867f, 0f, 1f);
        public readonly static Color GuardianAngel = new Color(1f, 1f, 1f, 1f);
        public readonly static Color Plaguebearer = new Color(0.812f, 0.996f, 0.380f, 1f);
        public readonly static Color Pestilence = new Color(0.259f, 0.259f, 0.259f, 1f);
        public readonly static Color Werewolf = new Color(0.624f, 0.439f, 0.227f, 1f);
        public readonly static Color Cannibal = new Color(0.55f, 0.25f, 0.02f, 1f);
        public readonly static Color Taskmaster = new Color(0.67f, 0.67f, 1f, 1f);
        public readonly static Color Juggernaut = new Color(0.631f, 0.167f, 0.337f, 1f);
        public readonly static Color Vampire = new Color(0.482f, 0.537f, 0.408f, 1f);
        public readonly static Color Dracula = new Color(0.675f, 0.541f, 0f, 1f);

        //Imposter Colors
        public readonly static Color Impostor = new Color(1f, 0f, 0f, 1f);
        public readonly static Color Consigliere = new Color(1f, 1f, 0.6f, 1f);
        public readonly static Color Grenadier = new Color(0.523f, 0.667f, 0.356f, 1f);
        public readonly static Color Morphling = new Color(0.732f, 0.269f, 0.689f, 1f);
        public readonly static Color Wraith = new Color(1f, 0.722f, 0.459f, 1f);
        public readonly static Color Poisoner = new Color(0.711f, 0f, 0.171f, 1f);
        public readonly static Color Undertaker = new Color(0f, 0.338f, 0.264f, 1f);
        public readonly static Color Camouflager = new Color(0.215f, 0.543f, 0.752f, 1f);
        public readonly static Color Traitor = new Color(0.215f, 0.051f, 0.263f, 1f);
        public readonly static Color Underdog = new Color(0.517f, 0.101f, 0.497f, 1f);
        public readonly static Color Janitor = new Color(0.148f, 0.279f, 0.777f, 1f);
        public readonly static Color Miner = new Color(0.456f, 0.764f, 0.878f, 1f);
        public readonly static Color Blackmailer = new Color(0.009f, 0.656f, 0.323f, 1f);
        public readonly static Color Disguiser = new Color(0f, 0.4f, 0f, 1f);
        public readonly static Color TimeMaster = new Color(0f, 0f, 0.654f, 1f);

        //Modifiers
        public readonly static Color Bait = new Color(0f, 0.7f, 0.7f, 1f);
        public readonly static Color Lighter = new Color(0.1f, 1f, 0.456f, 1f);
        public readonly static Color Coward = new Color(0.6f, 0.6f, 0.6f, 1f);
        public readonly static Color Diseased = new Color(0.7f, 0.7f, 0.7f, 1f);
        public readonly static Color Torch = new Color(1f, 1f, 0.6f, 1f);
        public readonly static Color Drunk = new Color(0.46f, 0.5f, 0f, 1f);
        public readonly static Color ButtonBarry = new Color(0.9f, 0f, 1f, 1f);
        public readonly static Color Dwarf = new Color(1f, 0.5f, 0.5f, 1f);
        public readonly static Color Giant = new Color(1f, 0.7f, 0.3f, 1f);
        public readonly static Color Sleuth = new Color(0.5f, 0.2f, 0.2f);
        public readonly static Color Tiebreaker = new Color(0.6f, 0.9f, 0.6f);
        public readonly static Color Lovers = new Color(1f, 0.4f, 0.8f, 1f);
        public readonly static Color Volatile = new Color(1f, 0.65f, 0.44f, 1f);
        public readonly static Color Modifier = new Color(0.5f, 0.5f, 0.5f, 1f);

        //Objectifiers
        public readonly static Color Objectifier = new Color(0.865f, 0.345f, 0.355f, 1f);
        public readonly static Color Assassin = new Color(0.027f, 0.216f, 0.388f);

    }
}
