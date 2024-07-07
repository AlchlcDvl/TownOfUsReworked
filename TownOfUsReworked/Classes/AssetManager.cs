namespace TownOfUsReworked.Classes;

public static class AssetManager
{
    public static readonly Dictionary<string, AudioClip> SoundEffects = [];
    public static readonly Dictionary<string, Sprite> Sprites = [];
    public static readonly List<Sprite> PortalAnimation = [];
    public static readonly Dictionary<string, TMP_FontAsset> Fonts = [];
    public static readonly Dictionary<string, AssetBundle> Bundles = [];
    public static readonly Dictionary<string, string> ObjectToBundle = [];
    public static readonly Dictionary<string, List<UObject>> LoadedObjects = [];

    public static AudioClip GetAudio(string path)
    {
        if (!SoundEffects.TryGetValue(path, out var sound))
        {
            // LogError($"{path} does not exist");
            return SoundEffects["Placeholder"];
        }

        return sound ?? SoundEffects["Placeholder"];
    }

    public static Sprite GetSprite(string path)
    {
        if (!Sprites.TryGetValue(path, out var sprite))
        {
            // LogError($"{path} does not exist");
            return Sprites[(Meeting ? "Meeting" : "") + "Placeholder"];
        }

        return sprite ?? Sprites[(Meeting ? "Meeting" : "") + "Placeholder"];
    }

    public static void Play(string path, bool loop = false, float volume = 1f)
    {
        Stop(path);

        if (Constants.ShouldPlaySfx())
            SoundManager.Instance.PlaySound(GetAudio(path), loop, volume);
    }

    public static void Stop(string path)
    {
        if (Constants.ShouldPlaySfx())
            SoundManager.Instance.StopSound(GetAudio(path));
    }

    public static void StopAll() => SoundEffects.Keys.ForEach(Stop);

    public static Texture2D LoadDiskTexture(string path)
    {
        var texture = EmptyTexture();
        ImageConversion.LoadImage(texture, File.ReadAllBytes(path), false);
        texture.name = path.SanitisePath();
        return texture.DontDestroy();
    }

    private static Texture2D EmptyTexture() => new(2, 2, TextureFormat.ARGB32, true);

    public static unsafe Texture2D LoadResourceTexture(string path)
    {
        var sname = path.SanitisePath();
        var texture = EmptyTexture();
        var stream = TownOfUsReworked.Core.GetManifestResourceStream(path);
        var length = stream.Length;
        var byteTexture = new Il2CppStructArray<byte>(length);
        stream.Read(new(IntPtr.Add(byteTexture.Pointer, IntPtr.Size * 4).ToPointer(), (int)length));
        ImageConversion.LoadImage(texture, byteTexture, false);
        texture.name = sname;
        return texture.DontDestroy();
    }

    public static Sprite CreateResourceSprite(string path) => CreateSprite(LoadResourceTexture(path), path.SanitisePath());

    public static Sprite CreateSprite(Texture2D tex, string name, float size = -1f)
    {
        var sprite = Sprite.Create(tex, new(0, 0, tex.width, tex.height), new(0.5f, 0.5f), size > 0f ? size : GetSize(name));
        sprite.name = name;
        sprite.hideFlags |= HideFlags.DontSaveInEditor;
        return sprite.DontDestroy();
    }

    public static AudioClip CreateResourceAudio(string path) => CreateDiskAudio(path.SanitisePath(), TownOfUsReworked.Core.GetManifestResourceStream(path));

    public static AudioClip CreateDiskAudio(string name, Stream stream)
    {
        var byteAudio = new byte[stream.Length];
        stream.Read(byteAudio, 0, (int)stream.Length);
        var samples = new float[byteAudio.Length / 4];

        for (var i = 0; i < samples.Length; i++)
            samples[i] = (float)BitConverter.ToInt32(byteAudio, i * 4) / int.MaxValue;

        var audioClip = AudioClip.Create(name, samples.Length / 2, 2, GetFrequency(name), false);
        audioClip.SetData(samples, 0);
        audioClip.hideFlags |= HideFlags.DontSaveInEditor;
        return audioClip.DontDestroy();
    }

    public static AssetBundle LoadBundle(Stream stream) => AssetBundle.LoadFromMemory(stream.ReadFully());

    public static string ReadResourceText(string itemName, string folder = "")
    {
        var resourceName = $"{TownOfUsReworked.Resources}{(folder == "" ? "" : $"{folder}.")}{itemName}";
        var stream = TownOfUsReworked.Core.GetManifestResourceStream(resourceName);
        var reader = new StreamReader(stream);
        var text = reader.ReadToEnd();
        KeyWords.ForEach(x => text = text.Replace(x.Key, x.Value));
        return text;
    }

    // https://stackoverflow.com/questions/51315918/how-to-encodetopng-compressed-textures-in-unity courtesy of pat from salem mod loader
    public static Texture2D Decompress(this Texture2D source)
    {
        var renderTex = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        Graphics.Blit(source, renderTex);
        var previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        var readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        readableText.hideFlags |= HideFlags.DontUnloadUnusedAsset;
        return readableText;
    }

    public static void SaveText(string fileName, string textToSave, string diskLocation) => SaveText(fileName, textToSave, true, diskLocation);

    public static void SaveText(string fileName, string textToSave, bool overrideText = true, string diskLocation = null)
    {
        try
        {
            File.WriteAllText(Path.Combine(diskLocation ?? Application.persistentDataPath, fileName), (overrideText ? "" : ReadDiskText(fileName, diskLocation)) + textToSave);
        }
        catch
        {
            LogError($"Unable to save text to {fileName}{(diskLocation != null ? $" in {diskLocation}" : "")}");
        }
    }

    public static string ReadDiskText(string fileName, string diskLocation = null)
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

    public static void LoadAssets()
    {
        SoundEffects.Clear();
        Sprites.Clear();
        PortalAnimation.Clear();

        foreach (var resourceName in TownOfUsReworked.Core.GetManifestResourceNames())
        {
            if (resourceName.StartsWith(TownOfUsReworked.Resources) && resourceName.EndsWith(".png"))
                Sprites.Add(resourceName.SanitisePath(), CreateResourceSprite(resourceName));
            else if (resourceName.StartsWith(TownOfUsReworked.Resources) && resourceName.EndsWith(".raw"))
                SoundEffects.Add(resourceName.SanitisePath(), CreateResourceAudio(resourceName));
        }

        Cursor.SetCursor(GetSprite("Cursor").texture, Vector2.zero, CursorMode.Auto);
    }

    public static void LoadVanillaSounds()
    {
        SoundEffects.TryAdd("EngineerIntro", GetIntroSound(RoleTypes.Engineer));
        SoundEffects.TryAdd("MorphlingIntro", GetIntroSound(RoleTypes.Shapeshifter));
        SoundEffects.TryAdd("MedicIntro", GetIntroSound(RoleTypes.Scientist));
        SoundEffects.TryAdd("CrewmateIntro", GetIntroSound(RoleTypes.Crewmate));
        SoundEffects.TryAdd("ImpostorIntro", GetIntroSound(RoleTypes.Impostor));
        SoundEffects.TryAdd("TrollIntro", GetIntroSound(RoleTypes.Noisemaker));
        SoundEffects.TryAdd("TrackerIntro", GetIntroSound(RoleTypes.Tracker));
        SoundEffects.TryAdd("WraithIntro", GetIntroSound(RoleTypes.Phantom));
    }

    public static float GetSize(string path)
    {
        var name = path.SanitisePath();

        if (name is "CurrentSettings" or "Client" or "Plus" or "Minus" or "Wiki")
            return 180;
        else if (name == "Phone")
            return 200;
        else if (name == "Cursor")
            return 115;
        else if (name == "NightVision")
            return 350;
        else
            return 100;
    }

    public static int GetFrequency(string path)
    {
        var name = path.SanitisePath();

        if (name.Contains("Intro"))
            return 36000;
        else
            return 48000;
    }

    public static TMP_FontAsset GetFont(string path)
    {
        if (!Fonts.TryGetValue(path, out var sound))
        {
            // LogError($"{path} does not exist");
            return Fonts["Placeholder"];
        }

        return sound ?? Fonts["Placeholder"];
    }

    public static T Get<T>(string name) where T : UObject
    {
        if (LoadedObjects.TryGetValue(name, out var objList))
            return (T)objList.Find(x => x.GetType() == typeof(T));

        if (ObjectToBundle.TryGetValue(name.ToLower(), out var bundle))
            return LoadAsset<T>(Bundles[bundle], name);

        return null;
    }

    private static T LoadAsset<T>(AssetBundle assetBundle, string name) where T : UObject
    {
        var asset = assetBundle.LoadAsset(name, Il2CppType.Of<T>())?.Cast<T>().DontUnload();
        LoadedObjects.TryAdd(name, []);
        LoadedObjects[name].Add(asset);
        return asset;
    }

    public static AudioClip GetIntroSound(RoleTypes roleType) => RoleManager.Instance.AllRoles.ToList().Find(x => x.Role == roleType).IntroSound;
}