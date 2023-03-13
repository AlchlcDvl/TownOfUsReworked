using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace TownOfUsReworked.Classes
{
    //Thanks to The Other Roles
    public static class SoundEffectsManager
    {
        private static Dictionary<string, AudioClip> SoundEffects;

        public static void Load()
        {
            SoundEffects = new Dictionary<string, AudioClip>();
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();
            SoundEffects.Clear();

            foreach (string resourceName in resourceNames)
            {
                if (resourceName.StartsWith($"{TownOfUsReworked.Sounds}") && resourceName.EndsWith(".raw"))
                    SoundEffects.Add(resourceName, Utils.CreateAudio(resourceName));
            }
        }

        public static AudioClip Get(string path)
        {
            // Convenience: As as SoundEffects are stored in the same folder, allow using just the name as well
            if (!path.Contains("."))
                path = $"{TownOfUsReworked.Sounds}{path}.raw";

            foreach (var (location, audio) in SoundEffects)
            {
                if (location == path)
                    return audio;
            }

            return null;
        }

        public static void Play(string path)
        {
            var clipToPlay = Get(path);
            Stop(path);

            if (Constants.ShouldPlaySfx() && clipToPlay != null)
                SoundManager.Instance.PlaySound(clipToPlay, false, 1);
        }

        public static void Stop(string path)
        {
            if (Constants.ShouldPlaySfx())
                SoundManager.Instance.StopSound(Get(path));
        }

        public static void StopAll()
        {
            if (SoundEffects == null)
                return;

            foreach (var path in SoundEffects.Keys)
                Stop(path);
        }
    }
}