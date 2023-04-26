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
        private readonly static Dictionary<string, float> Sizes = new();
        private readonly static string[] TranslationKeys = Utils.CreateText("Keys", "Languages").Split("\n");

        #pragma warning disable
        public static Sprite Use;

        public static Material BombMaterial;
        public static Material BugMaterial;

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
                return MeetingHud.Instance ? ButtonSprites["MeetingPlaceholder"] : ButtonSprites["Placeholder"];
            }
            else
                return ButtonSprites[path];
        }

        public static float GetSize(string path)
        {
            if (!Sizes.ContainsKey(path))
            {
                Utils.LogSomething($"{path} does not exist");
                return 100;
            }
            else
                return Sizes[path];
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
            var stream1 = TownOfUsReworked.Assembly.GetManifestResourceStream($"{TownOfUsReworked.Misc}Bomber");
            var stream2 = TownOfUsReworked.Assembly.GetManifestResourceStream($"{TownOfUsReworked.Misc}Operative");
            BombBundle = AssetBundle.LoadFromMemory(stream1.ReadFully());
            BugBundle = AssetBundle.LoadFromMemory(stream2.ReadFully());
            BombMaterial = BombBundle.LoadAsset<Material>("bomb").DontUnload();
            BugMaterial = BugBundle.LoadAsset<Material>("trap").DontUnload();

            SoundEffects.Clear();
            Sounds.Clear();

            foreach (var resourceName in TownOfUsReworked.Assembly.GetManifestResourceNames())
            {
                if (resourceName.StartsWith($"{TownOfUsReworked.Misc}") || resourceName.StartsWith($"{TownOfUsReworked.Buttons}"))
                {
                    var name = resourceName.Replace(".png", "").Replace($"{TownOfUsReworked.Sounds}", "").Replace($"{TownOfUsReworked.Buttons}", "");

                    if (name == "NightVisionOverlay")
                        Sizes.Add(name, 350);
                    else
                        Sizes.Add(name, 100);
                }
            }

            foreach (var resourceName in TownOfUsReworked.Assembly.GetManifestResourceNames())
            {
                if (resourceName.StartsWith($"{TownOfUsReworked.Sounds}") && resourceName.EndsWith(".raw"))
                {
                    var name = resourceName.Replace(".raw", "").Replace($"{TownOfUsReworked.Sounds}", "");
                    SoundEffects.Add(name, Utils.CreateAudio(resourceName));
                    Sounds.Add(name);
                }
                else if (resourceName.StartsWith($"{TownOfUsReworked.Buttons}") && resourceName.EndsWith(".png"))
                {
                    var name = resourceName.Replace(".png", "").Replace($"{TownOfUsReworked.Buttons}", "");
                    ButtonSprites.Add(name, Utils.CreateSprite(resourceName));
                }
                else if (resourceName.StartsWith($"{TownOfUsReworked.Misc}") && resourceName.EndsWith(".png"))
                {
                    var name = resourceName.Replace(".png", "").Replace($"{TownOfUsReworked.Misc}", "");
                    ButtonSprites.Add(name, Utils.CreateSprite(resourceName));
                }
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