using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;
using Reactor.Utilities.Extensions;

namespace TownOfUsReworked.Classes
{
    public static class AssetManager
    {
        private static Dictionary<string, AudioClip> SoundEffects;
        private static bool MaterialsLoaded = false;
        public static List<string> Sounds;

        public static Sprite Clean;
        public static Sprite Fix;
        public static Sprite SwapperSwitch;
        public static Sprite SwapperSwitchDisabled;
        public static Sprite Camouflage;
        public static Sprite Rewind;
        public static Sprite Shield;
        public static Sprite Eat;
        public static Sprite Shift;
        public static Sprite Seer;
        public static Sprite Sample;
        public static Sprite Morph;
        public static Sprite Mine;
        public static Sprite Invis;
        public static Sprite Douse;
        public static Sprite Ignite;
        public static Sprite Revive;
        public static Sprite Button;
        public static Sprite CycleBack;
        public static Sprite CycleForward;
        public static Sprite Guess;
        public static Sprite Drag;
        public static Sprite Drop;
        public static Sprite Flash;
        public static Sprite Alert;
        public static Sprite Remember;
        public static Sprite Track;
        public static Sprite Poison;
        public static Sprite Poisoned;
        public static Sprite Transport;
        public static Sprite Mediate;
        public static Sprite Vest;
        public static Sprite Protect;
        public static Sprite Blackmail;
        public static Sprite BlackmailLetter;
        public static Sprite BlackmailOverlay;
        public static Sprite Infect;
        public static Sprite Bug;
        public static Sprite Examine;
        public static Sprite Hack;
        public static Sprite Mimic;
        public static Sprite Maul;
        public static Sprite Shoot;
        public static Sprite Assault;
        public static Sprite Obliterate;
        public static Sprite Neutralise;
        public static Sprite Disguise;
        public static Sprite TimeFreeze;
        public static Sprite CryoFreeze;
        public static Sprite Measure;
        public static Sprite Warp;
        public static Sprite Promote;
        public static Sprite Teleport;
        public static Sprite Mark;
        public static Sprite Placeholder;
        public static Sprite MeetingPlaceholder;
        public static Sprite Stab;
        public static Sprite SyndicateKill;
        public static Sprite Plant;
        public static Sprite Detonate;
        public static Sprite Ressurect;
        public static Sprite Whisper;
        public static Sprite CrewVent;
        public static Sprite IntruderVent;
        public static Sprite SyndicateVent;
        public static Sprite NeutralVent;
        public static Sprite Recruit;
        public static Sprite Bite;
        public static Sprite Sidekick;
        public static Sprite Haunt;
        public static Sprite CorruptedKill;
        public static Sprite IntruderKill;
        public static Sprite Report;
        public static Sprite Use;
        public static Sprite Sabotage;
        public static Sprite Shapeshift;
        public static Sprite Pet;
        public static Sprite SyndicateSabotage;
        public static Sprite Interrogate;
        public static Sprite Swoop;
        public static Sprite EscortRoleblock;
        public static Sprite Reveal;
        public static Sprite Inspect;
        public static Sprite Stake;
        public static Sprite Plus;
        public static Sprite Minus;

        public static Sprite Lighter;
        public static Sprite Blocked;
        public static Sprite Darker;
        public static Sprite Footprint;
        public static Sprite Arrow;

        public static Sprite Vault;
        public static Sprite Cokpit;
        public static Sprite Task;
        public static Sprite Medical;

        public static AnimationClip VaultAnim;
        public static AnimationClip CokpitAnim;
        public static AnimationClip MedicalAnim;

        public static GameObject CallPlateform;

        public static Material BombMaterial;
        public static Material BugMaterial;

        public static Sprite SettingsButton;
        public static Sprite CrewSettingsButton;
        public static Sprite NeutralSettingsButton;
        public static Sprite IntruderSettingsButton;
        public static Sprite SyndicateSettingsButton;
        public static Sprite ModifierSettingsButton;
        public static Sprite ObjectifierSettingsButton;
        public static Sprite AbilitySettingsButton;
        public static Sprite ToUBanner;
        public static Sprite UpdateTOUButton;
        public static Sprite UpdateSubmergedButton;

        public static Sprite UpdateImage;
        public static Sprite DiscordImage;

        public static AssetBundle AirshipBundle;
        public static AssetBundle BugBundle;
        public static AssetBundle BombBundle;

        public static AudioClip Get(string path)
        {
            Utils.LogSomething($"Looking For Sound: {path}");

            if (!SoundEffects.ContainsKey(path))
            {
                Utils.LogSomething($"{path} does not exist");
                return null;
            }

            return SoundEffects[path];
        }

        public static void Play(string path)
        {
            var clipToPlay = Get(path);
            Stop(path);

            if (clipToPlay != null && Constants.ShouldPlaySfx())
            {   
                try
                {
                    SoundManager.Instance.PlaySound(clipToPlay, false);
                }
                catch
                {
                    Utils.LogSomething($"Error Playing: {path}");
                }
            }
            else
                Utils.LogSomething($"Error Playing Because Sound Was null: {path}");
        }

        public static void Stop(string path)
        {
            if (Constants.ShouldPlaySfx())
                SoundManager.Instance.StopSound(Get(path));
        }

        public static void StopAll()
        {
            foreach (var path in SoundEffects.Keys)
                Stop(path);
        }

        public static AudioClip CreateAudio(string path, string name = "NoName")
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var stream = assembly.GetManifestResourceStream(path);
                var byteAudio = new byte[stream.Length];
                _ = stream.Read(byteAudio, 0, (int)stream.Length);
                var samples = new float[byteAudio.Length / 4];

                for (var i = 0; i < samples.Length; i++)
                {
                    var offset = i * 4;
                    samples[i] = (float)BitConverter.ToInt32(byteAudio, offset) / Int32.MaxValue;
                }

                var audioClip = AudioClip.Create(name, samples.Length, 2, 24000, false);
                audioClip.SetData(samples, 0);
                return audioClip;
            }
            catch
            {
                Utils.LogSomething($"Error Loading Sound From: {path}");
                return null;
            }
        }

        public static void LoadAndReload()
        {
            //Ability buttons
            Clean = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Clean");
            Fix = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Fix");
            SwapperSwitch = Utils.CreateSprite($"{TownOfUsReworked.Buttons}SwapActive");
            SwapperSwitchDisabled = Utils.CreateSprite($"{TownOfUsReworked.Buttons}SwapDisabled");
            Rewind = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Rewind");
            Shield = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Shield");
            Seer = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Reveal");
            Sample = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Sample");
            Morph = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Morph");
            Mine = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Mine");
            Invis = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Invis");
            Douse = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Douse");
            Ignite = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Ignite");
            Revive = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Revive");
            Button = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Button");
            Drag = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Drag");
            Drop = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Drop");
            CycleBack = Utils.CreateSprite($"{TownOfUsReworked.Buttons}CycleBack");
            CycleForward = Utils.CreateSprite($"{TownOfUsReworked.Buttons}CycleForward");
            Guess = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Guess");
            Flash = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Flash");
            Alert = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Alert");
            Remember = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Remember");
            Track = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Track");
            Poison = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Poison");
            Poisoned = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Poisoned");
            Transport = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Transport");
            Plant = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Plant");
            Detonate = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Detonate");
            Mediate = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Mediate");
            Vest = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Vest");
            Protect = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Protect");
            Blackmail = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Blackmail");
            BlackmailLetter = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Blackmailed");
            Infect = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Infect");
            Bug = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Bug");
            Examine = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Examine");
            Hack = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Hack");
            Mimic = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Mimic");
            Camouflage = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Camouflage");
            Stab = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Stab");
            Shift = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Shift");
            Shoot = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Shoot");
            Maul = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Maul");
            Obliterate = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Obliterate");
            Assault = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Assault");
            Neutralise = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Neutralise");
            Disguise = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Disguise");
            Eat = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Eat");
            TimeFreeze = Utils.CreateSprite($"{TownOfUsReworked.Buttons}TimeFreeze");
            CryoFreeze = Utils.CreateSprite($"{TownOfUsReworked.Buttons}CryoFreeze");
            Measure = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Measure");
            Teleport = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Recall");
            Mark = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Mark");
            Warp = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Warp");
            Placeholder = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Placeholder");
            MeetingPlaceholder = Utils.CreateSprite($"{TownOfUsReworked.Buttons}MeetingPlaceholder");
            SyndicateKill = Utils.CreateSprite($"{TownOfUsReworked.Buttons}SyndicateKill");
            Ressurect = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Ressurect");
            Whisper = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Whisper");
            CorruptedKill = Utils.CreateSprite($"{TownOfUsReworked.Buttons}CorruptedKill");
            CrewVent = Utils.CreateSprite($"{TownOfUsReworked.Buttons}CrewVent");
            IntruderVent = Utils.CreateSprite($"{TownOfUsReworked.Buttons}IntruderVent");
            SyndicateVent = Utils.CreateSprite($"{TownOfUsReworked.Buttons}SyndicateVent");
            NeutralVent = Utils.CreateSprite($"{TownOfUsReworked.Buttons}NeutralVent");
            Recruit = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Recruit");
            Bite = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Bite");
            Promote = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Promote");
            Haunt = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Haunt");
            Sidekick = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Sidekick");
            IntruderKill = Utils.CreateSprite($"{TownOfUsReworked.Buttons}IntruderKill");
            Report = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Report");
            Use = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Use");
            Sabotage = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Sabotage");
            Shapeshift = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Shapeshift");
            Pet = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Pet");
            SyndicateSabotage = Utils.CreateSprite($"{TownOfUsReworked.Buttons}SyndicateSabotage");
            Interrogate = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Interrogate");
            Swoop = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Swoop");
            Reveal = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Reveal");
            EscortRoleblock = Utils.CreateSprite($"{TownOfUsReworked.Buttons}EscortRoleblock");
            Inspect = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Inspect");
            Stake = Utils.CreateSprite($"{TownOfUsReworked.Buttons}Stake");

            //Misc Stuff
            BlackmailOverlay = Utils.CreateSprite($"{TownOfUsReworked.Misc}BlackmailOverlay");
            Lighter = Utils.CreateSprite($"{TownOfUsReworked.Misc}Lighter");
            Darker = Utils.CreateSprite($"{TownOfUsReworked.Misc}Darker");
            Arrow = Utils.CreateSprite($"{TownOfUsReworked.Misc}Arrow");
            Footprint = Utils.CreateSprite($"{TownOfUsReworked.Misc}Footprint");
            Blocked = Utils.CreateSprite($"{TownOfUsReworked.Misc}Blocked");
            Plus = Utils.CreateSprite($"{TownOfUsReworked.Misc}Plus");
            Minus = Utils.CreateSprite($"{TownOfUsReworked.Misc}Minus");

            //Settings buttons
            SettingsButton = Utils.CreateSprite($"{TownOfUsReworked.Misc}SettingsButton");
            CrewSettingsButton = Utils.CreateSprite($"{TownOfUsReworked.Misc}Crew");
            NeutralSettingsButton = Utils.CreateSprite($"{TownOfUsReworked.Misc}Neutral");
            IntruderSettingsButton = Utils.CreateSprite($"{TownOfUsReworked.Misc}Intruders");
            SyndicateSettingsButton = Utils.CreateSprite($"{TownOfUsReworked.Misc}Syndicate");
            ModifierSettingsButton = Utils.CreateSprite($"{TownOfUsReworked.Misc}Modifiers");
            ObjectifierSettingsButton = Utils.CreateSprite($"{TownOfUsReworked.Misc}Objectifiers");
            AbilitySettingsButton = Utils.CreateSprite($"{TownOfUsReworked.Misc}Abilities");
            ToUBanner = Utils.CreateSprite($"{TownOfUsReworked.Misc}TownOfUsReworkedBanner");
            UpdateTOUButton = Utils.CreateSprite($"{TownOfUsReworked.Misc}UpdateToUButton");
            UpdateSubmergedButton = Utils.CreateSprite($"{TownOfUsReworked.Misc}UpdateSubmergedButton");

            if (!MaterialsLoaded)
            {
                var stream1 = TownOfUsReworked.assembly.GetManifestResourceStream($"{TownOfUsReworked.Misc}Bomber");
                var stream2 = TownOfUsReworked.assembly.GetManifestResourceStream($"{TownOfUsReworked.Misc}Operative");
                BombBundle = AssetBundle.LoadFromMemory(stream1.ReadFully());
                BugBundle = AssetBundle.LoadFromMemory(stream2.ReadFully());
                BombMaterial = BombBundle.LoadAsset<Material>("bomb").DontUnload();
                BugMaterial = BugBundle.LoadAsset<Material>("trap").DontUnload();

                //Better Airship stuff
                var stream = TownOfUsReworked.assembly.GetManifestResourceStream($"{TownOfUsReworked.Misc}Airship");
                AirshipBundle = AssetBundle.LoadFromMemory(stream.ReadFully());

                //Menu settings
                DiscordImage = Utils.CreateSprite($"{TownOfUsReworked.Misc}Discord");
                UpdateImage = Utils.CreateSprite($"{TownOfUsReworked.Misc}Update");

                //Better Aiship Resources
                Vault = AirshipBundle.LoadAsset<Sprite>("Vault").DontDestroy();
                Cokpit = AirshipBundle.LoadAsset<Sprite>("Cokpit").DontDestroy();
                Medical = AirshipBundle.LoadAsset<Sprite>("Medical").DontDestroy();
                Task = AirshipBundle.LoadAsset<Sprite>("task-shields").DontDestroy();

                VaultAnim = AirshipBundle.LoadAsset<AnimationClip>("Vault.anim").DontDestroy();
                CokpitAnim = AirshipBundle.LoadAsset<AnimationClip>("Cokpit.anim").DontDestroy();
                MedicalAnim = AirshipBundle.LoadAsset<AnimationClip>("Medical.anim").DontDestroy();

                CallPlateform = AirshipBundle.LoadAsset<GameObject>("call.prefab").DontDestroy();

                MaterialsLoaded = true;
            }

            SoundEffects = new Dictionary<string, AudioClip>();
            Sounds = new List<string>();
            SoundEffects.Clear();
            Sounds.Clear();
            var resourceNames = TownOfUsReworked.assembly.GetManifestResourceNames();

            foreach (var resourceName in resourceNames)
            {
                if (resourceName.StartsWith($"{TownOfUsReworked.Sounds}") && resourceName.EndsWith(".raw"))
                {
                    //Convenience: As as SoundEffects are stored in the same folder, allow using just the name as well
                    var name = resourceName.Replace(".raw", "");
                    name = name.Replace($"{TownOfUsReworked.Sounds}", "");
                    SoundEffects.Add(name, CreateAudio(resourceName));
                    Sounds.Add(name);
                }
            }
        }
    }
}