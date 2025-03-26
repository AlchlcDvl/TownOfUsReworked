namespace TownOfUsReworked.Managers;

public static class AssetManager
{
    public static readonly Sprite[] PortalAnimation = new Sprite[205];
    public static readonly string[] PortalPaths = new string[205];
    public static readonly Dictionary<string, AssetBundle> Bundles = [];
    public static readonly Dictionary<string, string> AssetToBundle = [];
    private static readonly Dictionary<string, HashSet<UObject>> LoadedAssets = [];
    private static readonly Dictionary<string, HashSet<string>> UnloadedAssets = [];

    public static AudioClip GetAudio(string path, bool placeholder = true) => Get<AudioClip>(path) ?? (placeholder ? Get<AudioClip>("Placeholder") : null);

    public static Sprite GetSprite(string path) => Get<Sprite>(path) ?? Get<Sprite>((Meeting() ? "Meeting" : "") + "Placeholder");

    public static TMP_FontAsset GetFont(string path) => Get<TMP_FontAsset>(path) ?? Get<TMP_FontAsset>("Placeholder");

    public static RoleEffectAnimation GetRoleAnim(string path) => Get<RoleEffectAnimation>(path);

    public static AnimationClip GetAnim(string path) => Get<AnimationClip>(path);

    public static Material GetMaterial(string path) => Get<Material>(path);

    public static void Play(string path, bool loop = false, float volume = 1f, float pitch = float.NaN) => Play(GetAudio(path), loop, volume, pitch);

    public static void Play(AudioClip audio, bool loop = false, float volume = 1f, float pitch = float.NaN)
    {
        Stop(audio);

        if (!Constants.ShouldPlaySfx())
            return;

        var source = SoundManager.Instance.PlaySound(audio, loop, volume);

        if (!float.IsNaN(pitch))
            source.pitch = pitch;
    }

    private static void Stop(AudioClip audio)
    {
        if (Constants.ShouldPlaySfx())
            SoundManager.Instance.StopSound(audio);
    }

    public static void StopAll() => GetAll<AudioClip>().ForEach(Stop);

    private static Texture2D EmptyTexture() => new(2, 2, TextureFormat.ARGB32, true)
    {
        filterMode = FilterMode.Bilinear,
        wrapMode = TextureWrapMode.Clamp
    };

    // private static Texture2D LoadTexture(string path) => path.StartsWith(TownOfUsReworked.Resources) ? LoadResourceTexture(path) : LoadDiskTexture(path);

    public static Texture2D LoadDiskTexture(string path) => LoadTexture(File.ReadAllBytes(path), path.SanitisePath());

    private static Texture2D LoadResourceTexture(string path) => LoadTexture(TownOfUsReworked.Core.GetManifestResourceStream(path)!.ReadFully(), path.SanitisePath());

    private static Texture2D LoadTexture(byte[] data, string name)
    {
        var texture = EmptyTexture();
        texture.name = name;
        return !texture.LoadImage(data, !GetReadable(name)) ? null : texture.DontDestroy();
    }

    private static Sprite LoadSprite(string path) => path.StartsWith(TownOfUsReworked.Resources) ? LoadResourceSprite(path) : LoadDiskSprite(path);

    public static Sprite LoadDiskSprite(string path) => LoadSprite(LoadDiskTexture(path), path.SanitisePath());

    private static Sprite LoadResourceSprite(string path) => LoadSprite(LoadResourceTexture(path), path.SanitisePath());

    public static Sprite LoadSprite(Texture2D tex, string name, float size = float.NaN, SpriteMeshType meshType = SpriteMeshType.Tight)
    {
        var sprite = Sprite.Create(tex, new(0, 0, tex.width, tex.height), new(0.5f, 0.5f), float.IsNaN(size) ? GetSize(name) : size, 0, meshType);
        sprite.name = name;
        return sprite.DontDestroy();
    }

    public static AssetBundle LoadBundle(byte[] data) => AssetBundle.LoadFromMemory(data);

    public static void SaveText(string fileName, string textToSave, string diskLocation) => SaveText(fileName, textToSave, true, diskLocation);

    private static void SaveText(string fileName, string textToSave, bool overrideText = true, string diskLocation = null)
    {
        try
        {
            File.WriteAllText(Path.Combine(diskLocation ?? Application.persistentDataPath, fileName), (overrideText ? "" : ReadDiskText(fileName, diskLocation)) + textToSave);
        }
        catch
        {
            Failure($"Unable to save text to {fileName}{(diskLocation != null ? $" in {diskLocation}" : "")}");
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
            Failure($"Error reading {fileName}{(diskLocation != null ? $" from {diskLocation}" : "")}");
            return "";
        }
    }

    public static void LoadAssets()
    {
        TownOfUsReworked.Core.GetManifestResourceNames().ForEach(x => AddPath(x.SanitisePath(), x));
        Cursor.SetCursor(GetSprite("Cursor").texture, CursorMode.Auto);
    }

    public static void LoadVanillaSounds()
    {
        AddAsset("EngineerIntro", GetIntroSound(RoleTypes.Engineer));
        AddAsset("MorphlingIntro", GetIntroSound(RoleTypes.Shapeshifter));
        AddAsset("MedicIntro", GetIntroSound(RoleTypes.Scientist));
        AddAsset("CrewmateIntro", GetIntroSound(RoleTypes.Crewmate));
        AddAsset("ImpostorIntro", GetIntroSound(RoleTypes.Impostor));
        AddAsset("TrollIntro", GetIntroSound(RoleTypes.Noisemaker));
        AddAsset("TrackerIntro", GetIntroSound(RoleTypes.Tracker));
        AddAsset("WraithIntro", GetIntroSound(RoleTypes.Phantom));
        AddAsset("MorphAnim", RoleManager.Instance.shapeshiftAnim);
        AddAsset("ProtectAnimLoop", RoleManager.Instance.protectLoopAnim);
        AddAsset("ProtectAnim", RoleManager.Instance.protectAnim);
        AddAsset("VanishPoofAnim", RoleManager.Instance.vanish_PoofAnim);
        AddAsset("PoofChargeAnim", RoleManager.Instance.vanish_ChargeAnim);
        AddAsset("AppearPoofAnim", RoleManager.Instance.appear_PoofAnim);
    }

    private static float GetSize(string name) => name switch
    {
        "CurrentSettings" or "Client" or "Plus" or "Minus" or "Wiki" => 180,
        "Phone" => 200,
        "Cursor" => 115,
        "NightVision" => 350,
        "Assets" or "GitHub" or "Discord" => 525,
        _ => 100
    };

    private static T Get<T>(string name) where T : UObject
    {
        if (LoadedAssets.TryGetValue(name, out var objList) && objList.TryFinding(x => x is T, out var result) && result)
            return result as T;

        if (AssetToBundle.TryGetValue(name.ToLower(), out var bundle))
        {
            result = LoadAsset<T>(Bundles[bundle], name);

            if (result)
                return (T)result;
        }

        if (!UnloadedAssets.TryGetValue(name, out var strings))
            return null;

        var tType = typeof(T);

        if (tType == typeof(Sprite) && strings.TryFinding(x => x.EndsWith(".png"), out var path))
            result = AddAsset(name, LoadSprite(path));
        else if (tType == typeof(AudioClip) && strings.TryFinding(x => x.EndsWith(".wav"), out path))
            result = AddAsset(name, LoadAudio(path));
        // else if (tType == typeof(Texture2D) && strings.TryFinding(x => x.EndsWith(".png"), out path))
        //     result = AddAsset(name, LoadTexture(path));
        else
            return null;

        strings.Remove(path);

        if (strings.Count == 0)
            UnloadedAssets.Remove(name);

        if (result)
            return result as T;

        // Failure($"{name} does not exist");
        return null;
    }

    public static IEnumerable<T> GetAll<T>() where T : UObject => LoadedAssets.Values.GetAll().OfType<T>();

    private static T LoadAsset<T>(AssetBundle assetBundle, string name) where T : UObject
    {
        var asset = assetBundle.LoadAsset<T>(name)?.DontUnload();
        AddAsset(name, asset);
        AssetToBundle.Remove(name);

        if (Bundles.Keys.Any(AssetToBundle.Values.Contains))
            return asset;

        Bundles.Remove(assetBundle.name);
        assetBundle.Unload(false);
        return asset;
    }

    private static AudioClip GetIntroSound(RoleTypes roleType) => RoleManager.Instance.GetRole(roleType)?.IntroSound;

    public static T AddAsset<T>(string name, T obj) where T : UObject => AddAsset(name, (UObject)obj) as T;

    private static UObject AddAsset(string name, UObject obj)
    {
        if (!obj)
            return null;

        if (!LoadedAssets.TryGetValue(name, out var value))
            LoadedAssets[name] = [ obj ];
        else
            value.Add(obj);

        return obj;
    }

    public static void AddPath(string name, string path)
    {
        if (!UnloadedAssets.TryGetValue(name, out var value))
            UnloadedAssets[name] = [ path ];
        else
            value.Add(path);
    }

    private static bool GetReadable(string name) => name is "Cursor";

    private static AudioClip LoadAudio(string path) => path.StartsWith(TownOfUsReworked.Resources) ? LoadResourceAudio(path) : LoadDiskAudio(path);

    private static AudioClip LoadResourceAudio(string path) => LoadAudio(path.SanitisePath(), TownOfUsReworked.Core.GetManifestResourceStream(path)!.ReadFully());

    private static AudioClip LoadDiskAudio(string path) => LoadAudio(path.SanitisePath(), File.ReadAllBytes(path));

    // Lord help my soul, got the code from here: https://github.com/deadlyfingers/UnityWav/blob/master/WavUtility.cs

    private static AudioClip LoadAudio(string name, byte[] fileBytes)
    {
        var chunk = BitConverter.ToInt32(fileBytes, 16) + 24;
        var channels = BitConverter.ToUInt16(fileBytes, 22);
        var sampleRate = BitConverter.ToInt32(fileBytes, 24);
        var bitDepth = BitConverter.ToUInt16(fileBytes, 34);
        var wavSize = BitConverter.ToInt32(fileBytes, chunk);
        var data = bitDepth switch
        {
            8 => Convert8BitByteArrayToAudioClipData(fileBytes, wavSize),
            16 => Convert16BitByteArrayToAudioClipData(fileBytes, chunk, wavSize),
            24 => Convert24BitByteArrayToAudioClipData(fileBytes, chunk, wavSize),
            32 => Convert32BitByteArrayToAudioClipData(fileBytes, chunk, wavSize),
            _ => throw new(bitDepth + " bit depth is not supported."),
        };

        var audioClip = AudioClip.Create(name, data.Length, channels, sampleRate, false);
        return audioClip.SetData(data, 0) ? audioClip.DontDestroy() : null;
    }

    private static float[] Convert8BitByteArrayToAudioClipData(byte[] source, int wavSize)
    {
        var data = new float[wavSize];

        for (var i = 0; i < wavSize; i++)
            data[i] = (float)source[i] / sbyte.MaxValue;

        return data;
    }

    private static float[] Convert16BitByteArrayToAudioClipData(byte[] source, int headerOffset, int wavSize)
    {
        headerOffset += sizeof(int);
        const int x = sizeof(short);
        var convertedSize = wavSize / x;
        var data = new float[convertedSize];

        for (var i = 0; i < convertedSize; i++)
            data[i] = (float)BitConverter.ToInt16(source, (i * x) + headerOffset) / short.MaxValue;

        return data;
    }

    private static float[] Convert24BitByteArrayToAudioClipData(byte[] source, int headerOffset, int wavSize)
    {
        const int intSize = sizeof(int);
        headerOffset += intSize;
        var convertedSize = wavSize / 3;
        var data = new float[convertedSize];
        var block = new byte[intSize]; // Using a 4-byte block for copying 3 bytes, then copy bytes with 1 offset

        for (var i = 0; i < convertedSize; i++)
        {
            Buffer.BlockCopy(source, (i * 3) + headerOffset, block, 1, 3);
            data[i] = (float)BitConverter.ToInt32(block, 0) / int.MaxValue;
        }

        return data;
    }

    private static float[] Convert32BitByteArrayToAudioClipData(byte[] source, int headerOffset, int wavSize)
    {
        headerOffset += sizeof(int);
        var convertedSize = wavSize / 4;
        var data = new float[convertedSize];

        for (var i = 0; i < convertedSize; i++)
            data[i] = (float)BitConverter.ToInt32(source, (i * 4) + headerOffset) / int.MaxValue;

        return data;
    }

    // This is all for mainly debugging stuff when I want to dump assets from the main game

    // public static void Dump(this Sprite sprite, string path) => File.WriteAllBytes(path, sprite.texture.Decompress().EncodeToPNG());

    // public static Texture2D Decompress(this Texture2D source)
    // {
    //     var renderTex = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
    //     Graphics.Blit(source, renderTex);
    //     var previous = RenderTexture.active;
    //     RenderTexture.active = renderTex;
    //     var readableText = new Texture2D(source.width, source.height);
    //     readableText.ReadPixels(new(0, 0, renderTex.width, renderTex.height), 0, 0);
    //     readableText.Apply();
    //     RenderTexture.active = previous;
    //     RenderTexture.ReleaseTemporary(renderTex);
    //     readableText.name = source.name;
    //     return readableText;
    // }
}