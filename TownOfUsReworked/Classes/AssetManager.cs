using System.Collections.Generic;
using UnityEngine;
using Reactor.Utilities.Extensions;
using HarmonyLib;
using AmongUs.Data;

namespace TownOfUsReworked.Classes
{
    [HarmonyPatch]
    public static class AssetManager
    {
        public readonly static List<string> Sounds = new();
        private readonly static Dictionary<string, AudioClip> SoundEffects = new();
        private readonly static Dictionary<string, Sprite> ButtonSprites = new();
        private readonly static Dictionary<string, string> Translations = new();
        private readonly static string[] TranslationKeys = Utils.CreateText("Keys", "Languages").Split("\n");

        #pragma warning disable
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
        public static Sprite ArsoDouse;
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
        public static Sprite RetSelect;
        public static Sprite RetDeselect;
        public static Sprite Use;

        public static Sprite Lighter;
        public static Sprite Blocked;
        public static Sprite Darker;
        public static Sprite Footprint;
        public static Sprite Arrow;

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

        private static AssetBundle BugBundle;
        private static AssetBundle BombBundle;
        #pragma warning restore

        public static AudioClip GetAudio(string path)
        {
            if (!SoundEffects.ContainsKey(path))
            {
                Utils.LogSomething($"{path} does not exist");
                return null;
            }
            else
                return SoundEffects[path];
        }

        public static Sprite GetSprite(string path)
        {
            if (!ButtonSprites.ContainsKey(path))
            {
                Utils.LogSomething($"{path} does not exist");
                return null;
            }
            else
                return ButtonSprites[path];
        }

        public static string Translate(string id)
        {
            if (!Translations.ContainsKey(id))
            {
                Utils.LogSomething($"{id} does not exist");
                return "DNE";
            }
            else
                return Translations[id];
        }

        public static void Play(string path)
        {
            var clipToPlay = GetAudio(path);
            Stop(path);

            if (clipToPlay != null && Constants.ShouldPlaySfx())
                SoundManager.Instance.PlaySound(clipToPlay, false);
            else
                Utils.LogSomething($"Error Playing Because Sound Was null: {path}");
        }

        public static void Stop(string path)
        {
            if (Constants.ShouldPlaySfx())
                SoundManager.Instance.StopSound(GetAudio(path));
        }

        public static void StopAll()
        {
            foreach (var path in SoundEffects.Keys)
                Stop(path);
        }

        public static string GetLanguage()
        {
            var language = (uint)DataManager.Settings.Language.CurrentLanguage;

            return language switch
            {
                1U => "Latam",
                2U => "Brazilian",
                3U => "Portuguese",
                4U => "Korean",
                5U => "Russian",
                6U => "Dutch",
                7U => "Filipino",
                8U => "French",
                9U => "German",
                10U => "Italian",
                11U => "Japanese",
                12U => "Spanish",
                13U => "SChinese",
                14U => "TChinese",
                15U => "Irish",
                _ => "English",
            };
        }

        public static void Load()
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
            ArsoDouse = Utils.CreateSprite($"{TownOfUsReworked.Buttons}ArsoDouse");
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
            RetSelect = Utils.CreateSprite($"{TownOfUsReworked.Buttons}RetSelect");
            RetDeselect = Utils.CreateSprite($"{TownOfUsReworked.Buttons}RetDeselect");

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

            var stream1 = TownOfUsReworked.Assembly.GetManifestResourceStream($"{TownOfUsReworked.Misc}Bomber");
            var stream2 = TownOfUsReworked.Assembly.GetManifestResourceStream($"{TownOfUsReworked.Misc}Operative");
            BombBundle = AssetBundle.LoadFromMemory(stream1.ReadFully());
            BugBundle = AssetBundle.LoadFromMemory(stream2.ReadFully());
            BombMaterial = BombBundle.LoadAsset<Material>("bomb").DontUnload();
            BugMaterial = BugBundle.LoadAsset<Material>("trap").DontUnload();

            //Menu settings
            DiscordImage = Utils.CreateSprite($"{TownOfUsReworked.Misc}Discord");
            UpdateImage = Utils.CreateSprite($"{TownOfUsReworked.Misc}Update");

            SoundEffects.Clear();
            Sounds.Clear();

            foreach (var resourceName in TownOfUsReworked.Assembly.GetManifestResourceNames())
            {
                if (resourceName.StartsWith($"{TownOfUsReworked.Sounds}") && resourceName.EndsWith(".raw"))
                {
                    //Convenience: As as SoundEffects are stored in the same folder, allow using just the name as well
                    var name = resourceName.Replace(".raw", "");
                    name = name.Replace($"{TownOfUsReworked.Sounds}", "");
                    SoundEffects.Add(name, Utils.CreateAudio(resourceName));
                    Sounds.Add(name);
                }
                /*else if (resourceName.StartsWith($"{TownOfUsReworked.Buttons}") && resourceName.EndsWith(".png"))
                {
                    //Convenience: As as Sprites are stored in the same folder, allow using just the name as well
                    var name = resourceName.Replace(".png", "");
                    name = name.Replace($"{TownOfUsReworked.Buttons}", "");
                    ButtonSprites.Add(name, Utils.CreateSprite(resourceName));
                }*/
            }

            var translation = Utils.CreateText(GetLanguage(), "Languages").Split("\n");

            if (TranslationKeys.Length > 0 && translation.Length > 0 && TranslationKeys.Length == translation.Length)
            {
                var position = 0;
                Translations.Clear();

                foreach (var key in TranslationKeys)
                {
                    Translations.Add(key, translation[position]);
                    position++;
                }
            }
        }
    }
}