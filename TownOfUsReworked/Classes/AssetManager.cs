namespace TownOfUsReworked.Classes;

public static class AssetManager
{
    public static readonly Dictionary<string, AudioClip> SoundEffects = new();
    public static readonly Dictionary<string, Sprite> Sprites = new();
    public static readonly Dictionary<string, float> Sizes = new();
    public static readonly Dictionary<string, int> Frequencies = new();
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

        return SoundEffects[path];
    }

    public static Sprite GetSprite(string path)
    {
        if (!Sprites.ContainsKey(path))
        {
            //LogError($"{path} does not exist");
            return Meeting ? Sprites["MeetingPlaceholder"] : Sprites["Placeholder"];
        }

        return Sprites[path];
    }

    public static void Play(string path, bool loop = false, float volume = 1f)
    {
        try
        {
            Stop(path);

            if (Constants.ShouldPlaySfx())
                SoundManager.Instance.PlaySound(GetAudio(path), loop, volume);
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
        }
        catch
        {
            LogError($"Error stopping because {path} was null");
        }
    }

    public static void StopAll() => SoundEffects.Keys.ForEach(Stop);

    public static Texture2D LoadDiskTexture(string path)
    {
        try
        {
            var texture = EmptyTexture();
            _ = ImageConversion.LoadImage(texture, File.ReadAllBytes(path), false);
            texture.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
            texture.name = path;
            _ = texture.DontDestroy();
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
            var stream = TownOfUsReworked.Core.GetManifestResourceStream(path);
            var length = stream.Length;
            var byteTexture = new Il2CppStructArray<byte>(length);
            _ = stream.Read(new(IntPtr.Add(byteTexture.Pointer, IntPtr.Size * 4).ToPointer(), (int)length));
            _ = ImageConversion.LoadImage(texture, byteTexture, false);
            texture.name = sname;
            texture.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
            _ = texture.DontDestroy();
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
            LogError($"Error loading {name} as a sprite");
            return null;
        }
    }

    public static AudioClip CreateAudio(string path)
    {
        try
        {
            var sname = path.Replace(".raw", "").Replace(TownOfUsReworked.Sounds, "");
            var stream = TownOfUsReworked.Core.GetManifestResourceStream(path);
            var byteAudio = new byte[stream.Length];
            _ = stream.Read(byteAudio, 0, (int)stream.Length);
            var samples = new float[byteAudio.Length / 4];

            for (var i = 0; i < samples.Length; i++)
                samples[i] = (float)BitConverter.ToInt32(byteAudio, i * 4) / int.MaxValue;

            var audioClip = AudioClip.Create(sname, samples.Length / 2, 2, Frequencies[sname], false);
            audioClip.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
            audioClip.SetData(samples, 0);
            _ = audioClip.DontDestroy();
            return audioClip;
        }
        catch
        {
            LogError($"Error loading {path} from resources");
            return null;
        }
    }

    public static string CreateText(string itemName, string folder = "")
    {
        try
        {
            var resourceName = $"{TownOfUsReworked.Resources}{(folder == "" ? "" : $"{folder}.")}{itemName}";
            var stream = TownOfUsReworked.Core.GetManifestResourceStream(resourceName);
            var reader = new StreamReader(stream);
            var text = reader.ReadToEnd();
            KeyWords.ForEach(x => text = text.Replace(x.Key, x.Value));
            return text;
        }
        catch
        {
            LogError($"Error loading {itemName} from resources");
            return "";
        }
    }

    public static void SaveText(string fileName, string textToSave, string diskLocation) => SaveText(fileName, textToSave, true, diskLocation);

    public static void SaveText(string fileName, string textToSave, bool overrideText = true, string diskLocation = null)
    {
        try
        {
            File.WriteAllText(Path.Combine(diskLocation ?? Application.persistentDataPath, fileName), (overrideText ? "" : ReadText(fileName)) + textToSave);
        }
        catch
        {
            LogError($"Unable to save text to {fileName}{(diskLocation != null ? $" in {diskLocation}" : "")}");
        }
    }

    public static string ReadText(string fileName, string diskLocation = null)
    {
        try
        {
            return File.ReadAllText(Path.Combine(diskLocation ?? Application.persistentDataPath, fileName));
        }
        catch
        {
            LogError($"Error reading {fileName}{(diskLocation != null ? $" from {diskLocation}" : "")}");
            return "";
        }
    }
}