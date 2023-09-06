namespace TownOfUsReworked.Classes;

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
        { "Default", ReadText("DefaultSettings") },
        { "Last Used", ReadText("LastUsedSettings") },
        { "Ranked", CreateText("Ranked", "Presets") }
    };
    public static readonly Dictionary<int, string> Slots = new()
    {
        { 1, ReadText("GameSettings-Slot1-ToU-Rew") },
        { 2, ReadText("GameSettings-Slot2-ToU-Rew") },
        { 3, ReadText("GameSettings-Slot3-ToU-Rew") },
        { 4, ReadText("GameSettings-Slot4-ToU-Rew") },
        { 5, ReadText("GameSettings-Slot5-ToU-Rew") },
        { 6, ReadText("GameSettings-Slot6-ToU-Rew") },
        { 7, ReadText("GameSettings-Slot7-ToU-Rew") },
        { 8, ReadText("GameSettings-Slot8-ToU-Rew") },
        { 9, ReadText("GameSettings-Slot9-ToU-Rew") },
        { 10, ReadText("GameSettings-Slot10-ToU-Rew") }
    };

    public static AudioClip GetAudio(string path)
    {
        if (!SoundEffects.ContainsKey(path))
        {
            LogError($"{path} does not exist");
            return null;
        }
        else
            return SoundEffects[path];
    }

    public static Sprite GetSprite(string path) => !Sprites.ContainsKey(path) || path == "" ? (Meeting ? Sprites["MeetingPlaceholder"] : Sprites["Placeholder"]) : Sprites[path];

    public static void Play(string path, bool loop = false)
    {
        try
        {
            Stop(path);

            if (Constants.ShouldPlaySfx())
                SoundManager.Instance.PlaySound(GetAudio(path), loop);
        }
        catch
        {
            LogError($"Error playing because {path} was null");
        }
    }

    public static void Stop(string path)
    {
        try
        {
            if (Constants.ShouldPlaySfx())
                SoundManager.Instance.StopSound(GetAudio(path));
        } catch {}
    }

    public static void StopAll() => SoundEffects.Keys.ForEach(Stop);

    public static Texture2D LoadDiskTexture(string path)
    {
        try
        {
            var texture = EmptyTexture();
            _ = ImageConversion.LoadImage(texture, File.ReadAllBytes(path), false);
            texture.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
            return texture;
        }
        catch
        {
            LogError($"Error loading {path} from disk");
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
            LogError($"Error loading {path} from resources");
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
            LogError($"Error Loading {name}");
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