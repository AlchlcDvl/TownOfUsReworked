using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using TownOfUsReworked.Lobby.CustomOption;

//Thanks to The Other Roles for this code
namespace TownOfUsReworked.Extentions
{
    // Class to preload all audio/sound effects that are contained in the embedded resources.
    // The effects are made available through the soundEffects Dict / the get and the play methods.
    public static class SoundEffects
    {
        private static Dictionary<string, AudioClip> soundEffects;

        public static void Load()
        {
            soundEffects = new Dictionary<string, AudioClip>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] resourceNames = assembly.GetManifestResourceNames();

            foreach (string resourceName in resourceNames)
            {
                if (resourceName.Contains("TownOfUsReworked.Resources.Sounds.") && resourceName.Contains(".raw"))
                    soundEffects.Add(resourceName, TownOfUsReworked.LoadAudioClipFromResources(resourceName));
            }
        }

        public static AudioClip Get(string path)
        {
            //Convenience: As as SoundEffects are stored in the same folder, allow using just the name as well
            if (!path.Contains("."))
                path = "TownOfUsReworked.Resources.Sounds." + path + ".raw";

            AudioClip returnValue;
            return soundEffects.TryGetValue(path, out returnValue) ? returnValue : null;
        }

        public static void Play(string path, float volume=0.8f)
        {
            /*if (CustomGameOptions.EnableSFX)
                return;*/

            AudioClip clipToPlay = Get(path);
            // if (false) clipToPlay = get("exampleClip"); for april fools?
            Stop(path);

            if (Constants.ShouldPlaySfx())
                SoundManager.Instance.PlaySound(clipToPlay, false, volume);
        }

        public static void Stop(string path)
        {
            if (Constants.ShouldPlaySfx())
                SoundManager.Instance.StopSound(Get(path));
        }

        public static void StopAll()
        {
            if (soundEffects == null)
                return;

            foreach (var path in soundEffects.Keys)
                Stop(path);
        }
    }
}