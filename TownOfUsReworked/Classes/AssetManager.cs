namespace TownOfUsReworked.Classes
{
    [HarmonyPatch]
    public static class AssetManager
    {
        public static readonly Dictionary<string, AudioClip> SoundEffects = new();
        public static readonly Dictionary<string, Sprite> Sprites = new();
        private static readonly Dictionary<string, float> Sizes = new();
        public static readonly Sprite[] PortalAnimation = new Sprite[205];
        public static readonly Dictionary<string, string> Presets = new()
        {
            { "Casual", CreateText("Casual", "Presets") },
            { "Chaos", CreateText("Chaos", "Presets") },
            { "Default", TryLoadingDataPreset("Default") },
            { "Last Used", TryLoadingDataPreset("LastUsed") },
            { "Ranked", CreateText("Ranked", "Presets") }
        };
        public static readonly Dictionary<int, string> Slots = new()
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
                LogSomething($"Error Loading {itemName}");
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
                LogSomething($"Error Loading Slot {slotId}");
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

        public static Sprite GetSprite(string path) => !Sprites.ContainsKey(path) ? (Meeting ? Sprites["MeetingPlaceholder"] : Sprites["Placeholder"]) : Sprites[path];

        public static void Play(string path)
        {
            try
            {
                var clipToPlay = GetAudio(path);
                Stop(path);

                if (Constants.ShouldPlaySfx())
                    SoundManager.Instance.PlaySound(clipToPlay, false);
            }
            catch
            {
                LogSomething($"Error playing because {path}");
            }
        }

        public static void Stop(string path)
        {
            if (Constants.ShouldPlaySfx())
                SoundManager.Instance.StopSound(GetAudio(path));
        }

        public static void StopAll() => SoundEffects.Keys.ToList().ForEach(Stop);

        public static string GetLanguage() => (uint)DataManager.Settings.Language.CurrentLanguage switch
        {
            13U => "SChinese",
            _ => "English"
        };

        public static Texture2D LoadDiskTexture(string path)
        {
            try
            {
                var texture = EmptyTexture();
                var byteTexture = Il2CppSystem.IO.File.ReadAllBytes(path);
                _ = ImageConversion.LoadImage(texture, byteTexture, false);
                texture.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
                return texture;
            }
            catch
            {
                LogSomething("Error loading texture from disk: " + path);
                return null;
            }
        }

        private static Texture2D EmptyTexture() => new(2, 2, TextureFormat.ARGB32, true);

        public static unsafe Texture2D LoadResourceTexture(string path)
        {
            try
            {
                var sname = path.Replace(".png", "").Replace(TownOfUsReworked.Buttons, "").Replace(TownOfUsReworked.Misc, "").Replace(TownOfUsReworked.Portal, "");
                var texture = EmptyTexture();
                var stream = TownOfUsReworked.Executing.GetManifestResourceStream(path);
                var length = stream.Length;
                var byteTexture = new Il2CppStructArray<byte>(length);
                _ = stream.Read(new(IntPtr.Add(byteTexture.Pointer, IntPtr.Size * 4).ToPointer(), (int)length));
                _ = ImageConversion.LoadImage(texture, byteTexture, false);
                texture.name = sname;
                texture.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
                return texture;
            }
            catch
            {
                LogSomething("Error loading texture from resources: " + path);
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
                _ = sprite.DontDestroy();
                return sprite;
            }
            catch
            {
                LogSomething($"Error Loading {name}");
                return null;
            }
        }

        public static void LoadAssets()
        {
            SoundEffects.Clear();
            Sizes.Clear();
            Sprites.Clear();

            foreach (var resourceName in TownOfUsReworked.Core.GetManifestResourceNames())
            {
                if (resourceName.EndsWith(".png"))
                {
                    var name = resourceName.Replace(".png", "").Replace(TownOfUsReworked.Buttons, "").Replace(TownOfUsReworked.Misc, "").Replace(TownOfUsReworked.Portal, "");

                    if (name is "CurrentSettings" or "Help" or "Plus" or "Minus" or "Wiki")
                        Sizes.Add(name, 180);
                    else if (name == "Phone")
                        Sizes.Add(name, 200);
                    else if (name == "Cursor")
                        Sizes.Add(name, 115);
                    else
                        Sizes.Add(name, 100);
                }
            }

            var position2 = 0;

            foreach (var resourceName in TownOfUsReworked.Core.GetManifestResourceNames())
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