namespace TownOfUsReworked.Classes
{
    [HarmonyPatch]
    public static class AssetManager
    {
        public readonly static List<string> Sounds = new();
        private readonly static Dictionary<string, AudioClip> SoundEffects = new();
        private readonly static Dictionary<string, Sprite> Sprites = new();
        private readonly static Dictionary<string, string> Translations = new();
        private readonly static Dictionary<string, float> Sizes = new();
        //private readonly static string[] TranslationKeys = Utils.CreateText("Keys", "Languages").Split("\n");
        public readonly static Sprite[] PortalAnimation = new Sprite[205];
        public static Sprite Use;
        public readonly static Dictionary<string, string> Presets = new()
        {
            { "Casual", Utils.CreateText("Casual", "Presets") },
            { "Chaos", Utils.CreateText("Chaos", "Presets") },
            { "Default", TryLoadingDataPreset("Default") },
            { "Last Used", TryLoadingDataPreset("LastUsed") },
            { "Ranked", Utils.CreateText("Ranked", "Presets") }
        };
        public readonly static Dictionary<int, string> Slots = new()
        {
            { 1, TryLoadingSlotSettings(1) },
            { 2, TryLoadingSlotSettings(2) },
            { 3, TryLoadingSlotSettings(3) },
            { 4, TryLoadingSlotSettings(4) },
            { 5, TryLoadingSlotSettings(5) },
            { 6, TryLoadingSlotSettings(6) },
            { 7, TryLoadingSlotSettings(7) },
            { 8, TryLoadingSlotSettings(8) },
            { 9, TryLoadingSlotSettings(9) },
            { 10, TryLoadingSlotSettings(10) }
        };

        public static string TryLoadingDataPreset(string itemName)
        {
            try
            {
                return File.ReadAllText(Path.Combine(Application.persistentDataPath, $"{itemName}Settings"));
            }
            catch
            {
                Utils.LogSomething($"Error Loading {itemName}");
                return "";
            }
        }

        public static string TryLoadingSlotSettings(int slotId)
        {
            try
            {
                return File.ReadAllText(Path.Combine(Application.persistentDataPath, $"GameSettings-Slot{slotId}-ToU-Rew"));
            }
            catch
            {
                Utils.LogSomething($"Error Loading Slot {slotId}");
                return "";
            }
        }

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
            if (!Sprites.ContainsKey(path))
                return MeetingHud.Instance ? Sprites["MeetingPlaceholder"] : Sprites["Placeholder"];
            else
                return Sprites[path];
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

        public static string GetLanguage() => (uint)DataManager.Settings.Language.CurrentLanguage switch
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

        public static Texture2D LoadDiskTexture(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    var texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                    var byteTexture = Il2CppSystem.IO.File.ReadAllBytes(path);
                    ImageConversion.LoadImage(texture, byteTexture, false);
                    return texture;
                }
            }
            catch
            {
                Utils.LogSomething("Error loading texture from disk: " + path);
            }

            return null;
        }

        public static unsafe Texture2D LoadResourceTexture(string path)
        {
            try
            {
                var texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                var stream = TownOfUsReworked.Executing.GetManifestResourceStream(path);
                var length = stream.Length;
                var byteTexture = new Il2CppStructArray<byte>(length);
                stream.Read(new(IntPtr.Add(byteTexture.Pointer, IntPtr.Size * 4).ToPointer(), (int)length));
                ImageConversion.LoadImage(texture, byteTexture, false);
                return texture;
            }
            catch
            {
                Utils.LogSomething("Error loading texture from resources: " + path);
            }

            return null;
        }

        public static AudioClip CreateAudio(string path, string name = "NoName")
        {
            try
            {
                var stream = TownOfUsReworked.Executing.GetManifestResourceStream(path);
                var byteAudio = new byte[stream.Length];
                stream.Read(byteAudio, 0, (int)stream.Length);
                var samples = new float[byteAudio.Length / 4];

                for (var i = 0; i < samples.Length; i++)
                    samples[i] = (float)BitConverter.ToInt32(byteAudio, i * 4) / int.MaxValue;

                var audioClip = AudioClip.Create(name, samples.Length, 2, 24000, false);
                audioClip.SetData(samples, 0);
                return audioClip;
            }
            catch
            {
                Utils.LogSomething($"Error Loading {path}");
                return null;
            }
        }

        public static Sprite CreateSprite(string name)
        {
            try
            {
                var sname = name.Replace(".png", "").Replace(TownOfUsReworked.Buttons, "").Replace(TownOfUsReworked.Misc, "").Replace(TownOfUsReworked.Portal, "");
                var tex = LoadResourceTexture(name);
                var sprite = Sprite.Create(tex, new(0, 0, tex.width, tex.height), new(0.5f, 0.5f), Sizes[sname]);
                sprite.name = sname;
                sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
                sprite.DontDestroy();
                return sprite;
            }
            catch
            {
                Utils.LogSomething($"Error Loading {name}");
                return null;
            }
        }

        public static void Load()
        {
            SoundEffects.Clear();
            Sounds.Clear();

            foreach (var resourceName in TownOfUsReworked.Assembly.GetManifestResourceNames())
            {
                if (resourceName.EndsWith(".png"))
                {
                    var name = resourceName.Replace(".png", "").Replace(TownOfUsReworked.Buttons, "").Replace(TownOfUsReworked.Misc, "").Replace(TownOfUsReworked.Portal, "");

                    if (name is "CurrentSettings" or "Help" or "Cross" or "Plus" or "Minus")
                        Sizes.Add(name, 180);
                    else if (name == "RoleCard")
                        Sizes.Add(name, 200);
                    else
                        Sizes.Add(name, 100);
                }
            }

            var position2 = 0;

            foreach (var resourceName in TownOfUsReworked.Assembly.GetManifestResourceNames())
            {
                if ((resourceName.StartsWith(TownOfUsReworked.Buttons) || resourceName.StartsWith(TownOfUsReworked.Misc)) && resourceName.EndsWith(".png"))
                {
                    var name = resourceName.Replace(".png", "").Replace(TownOfUsReworked.Buttons, "").Replace(TownOfUsReworked.Misc, "");
                    Sprites.Add(name, CreateSprite(resourceName));
                }
                else if (resourceName.StartsWith(TownOfUsReworked.Portal) && resourceName.EndsWith(".png"))
                {
                    if (PortalAnimation[position2] == null)
                        PortalAnimation[position2] = CreateSprite(resourceName);

                    position2++;
                }
            }
        }
    }
}