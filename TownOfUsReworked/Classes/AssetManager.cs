namespace TownOfUsReworked.Classes
{
    [HarmonyPatch]
    public static class AssetManager
    {
        public readonly static Dictionary<string, AudioClip> SoundEffects = new();
        public readonly static Dictionary<string, Sprite> Sprites = new();
        //private static readonly Dictionary<string, string> Translations = new();
        private static readonly Dictionary<string, float> Sizes = new();
        //private static readonly string[] TranslationKeys = Utils.CreateText("Keys", "Languages").Split("\n");
        public readonly static Sprite[] PortalAnimation = new Sprite[205];
        //public readonly static Dictionary<string, Material> Materials = new();
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
        //public readonly static Dictionary<string, string> MaterialNames = new() { {"Vision", "GlitchedPlayer"} };

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

        /*public static Material GetMaterial(string path)
        {
            if (!Materials.ContainsKey(path))
            {
                Utils.LogSomething($"{path} does not exist");
                return null;
            }
            else
                return Materials[path];
        }*/

        public static Sprite GetSprite(string path) => !Sprites.ContainsKey(path) ? Utils.Meeting ? Sprites["MeetingPlaceholder"] : Sprites["Placeholder"] : Sprites[path];

        /*public static string Translate(string id)
        {
            if (!Translations.ContainsKey(id))
            {
                Utils.LogSomething($"{id} does not exist");
                return "DNE";
            }
            else
                return Translations[id];
        }*/

        public static void Play(string path)
        {
            try
            {
                var clipToPlay = GetAudio(path);
                Stop(path);

                if (clipToPlay && Constants.ShouldPlaySfx())
                    SoundManager.Instance.PlaySound(clipToPlay, false);
                else
                    Utils.LogSomething($"Error playing because sound was null: {path}");
            } catch {}
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
            13U => "SChinese",
            _ => "English",
        };

        public static Texture2D LoadDiskTexture(string path)
        {
            try
            {
                var texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                var byteTexture = Il2CppSystem.IO.File.ReadAllBytes(path);
                _ = ImageConversion.LoadImage(texture, byteTexture, false);
                return texture;
            }
            catch
            {
                Utils.LogSomething("Error loading texture from disk: " + path);
                return null;
            }
        }

        public static unsafe Texture2D LoadResourceTexture(string path)
        {
            try
            {
                var texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                var stream = TownOfUsReworked.Executing.GetManifestResourceStream(path);
                var length = stream.Length;
                var byteTexture = new Il2CppStructArray<byte>(length);
                _ = stream.Read(new(IntPtr.Add(byteTexture.Pointer, IntPtr.Size * 4).ToPointer(), (int)length));
                _ = ImageConversion.LoadImage(texture, byteTexture, false);
                return texture;
            }
            catch
            {
                Utils.LogSomething("Error loading texture from resources: " + path);
                return null;
            }
        }

        /*public static AudioClip CreateAudio(string path)
        {
            try
            {
                var sname = path.Replace(".raw", "").Replace(TownOfUsReworked.Sounds, "");
                var stream = TownOfUsReworked.Executing.GetManifestResourceStream(path);
                var byteAudio = new byte[stream.Length];
                _ = stream.Read(byteAudio, 0, (int)stream.Length);
                var samples = new float[byteAudio.Length / 4];

                for (var i = 0; i < samples.Length; i++)
                    samples[i] = (float)BitConverter.ToInt32(byteAudio, i * 4) / int.MaxValue;

                var audioClip = AudioClip.Create(sname, samples.Length / 2, 2, 48000, false);
                _ = audioClip.SetData(samples, 0);
                return audioClip;
            }
            catch
            {
                Utils.LogSomething($"Error loading {path}");
                return null;
            }
        }*/

        public static Sprite CreateSprite(string name)
        {
            try
            {
                var sname = name.Replace(".png", "").Replace(TownOfUsReworked.Buttons, "").Replace(TownOfUsReworked.Misc, "").Replace(TownOfUsReworked.Portal, "")
                    /*.Replace(TownOfUsReworked.Icons, "")*/;
                var tex = LoadResourceTexture(name);
                var sprite = Sprite.Create(tex, new(0, 0, tex.width, tex.height), new(0.5f, 0.5f), Sizes[sname]);
                sprite.name = sname;
                sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
                _ = sprite.DontDestroy();
                return sprite;
            }
            catch
            {
                Utils.LogSomething($"Error Loading {name}");
                return null;
            }
        }

        /*public static Material CreateMaterial(string path, string materialName)
        {
            try
            {
                var stream = TownOfUsReworked.Executing.GetManifestResourceStream(path);
                var assets = stream.ReadFully();
                var bundle = AssetBundle.LoadFromMemory(assets);
                var mat = bundle.LoadAsset<Material>(materialName).DontUnload();
                mat.name = materialName;
                return mat;
            }
            catch
            {
                Utils.LogSomething("Unable to load material: " + path);
                return null;
            }
        }*/

        public static void Load()
        {
            SoundEffects.Clear();
            Sizes.Clear();
            Sprites.Clear();
            //Translations.Clear();

            foreach (var resourceName in TownOfUsReworked.Assembly.GetManifestResourceNames())
            {
                if (resourceName.EndsWith(".png"))
                {
                    var name = resourceName.Replace(".png", "").Replace(TownOfUsReworked.Buttons, "").Replace(TownOfUsReworked.Misc, "").Replace(TownOfUsReworked.Portal, "")
                        /*.Replace(TownOfUsReworked.Icons, "")*/;

                    if (name is "CurrentSettings" or "Help" or "Plus" or "Minus")
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
                if ((resourceName.StartsWith(TownOfUsReworked.Buttons) || resourceName.StartsWith(TownOfUsReworked.Misc)/* || resourceName.StartsWith(TownOfUsReworked.Icons)*/) &&
                    resourceName.EndsWith(".png"))
                {
                    var name = resourceName.Replace(".png", "").Replace(TownOfUsReworked.Buttons, "").Replace(TownOfUsReworked.Misc, "")/*.Replace(TownOfUsReworked.Icons, "")*/;
                    Sprites.Add(name, CreateSprite(resourceName));
                }
                else if (resourceName.StartsWith(TownOfUsReworked.Portal) && resourceName.EndsWith(".png"))
                {
                    if (PortalAnimation[position2] == null)
                        PortalAnimation[position2] = CreateSprite(resourceName);

                    position2++;
                }
                /*else if (resourceName.StartsWith(TownOfUsReworked.Materials))
                {
                    var name = resourceName.Replace(TownOfUsReworked.Materials, "");
                    Materials.Add(name, CreateMaterial(resourceName, MaterialNames[name]));
                }
                else if (resourceName.StartsWith(TownOfUsReworked.Sounds) && resourceName.EndsWith(".raw"))
                {
                    var name = resourceName.Replace(TownOfUsReworked.Sounds, "").Replace(".raw", "");
                    SoundEffects.Add(name, CreateAudio(resourceName));
                }*/
            }

            /*var translation = Utils.CreateText(GetLanguage(), "Languages").Split("\n");

            if (TranslationKeys.Length != 0 && translation.Length != 0 && TranslationKeys.Length == translation.Length)
            {
                var position = 0;

                foreach (var key in TranslationKeys)
                {
                    Translations.Add(key, translation[position]);
                    position++;
                }
            }*/
        }
    }
}