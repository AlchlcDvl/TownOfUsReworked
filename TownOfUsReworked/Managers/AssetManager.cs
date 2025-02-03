namespace TownOfUsReworked.Managers;

public static class AssetManager
{
    public static readonly List<Sprite> PortalAnimation = [];
    public static readonly Dictionary<string, AssetBundle> Bundles = [];
    public static readonly Dictionary<string, string> AssetToBundle = [];
    private static readonly Dictionary<string, List<UObject>> LoadedAssets = [];
    public static readonly Dictionary<string, List<string>> UnloadedAssets = [];

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

        if (Constants.ShouldPlaySfx())
        {
            var source = SoundManager.Instance.PlaySound(audio, loop, volume);

            if (!float.IsNaN(pitch))
                source.pitch = pitch;
        }
    }

    public static void Stop(AudioClip audio)
    {
        if (Constants.ShouldPlaySfx())
            SoundManager.Instance.StopSound(audio);
    }

    public static void StopAll() => UnityGetAll<AudioClip>().ForEach(Stop);

    private static Texture2D EmptyTexture() => new(2, 2, TextureFormat.ARGB32, true);

    public static Texture2D LoadTexture(string path) => path.StartsWith(TownOfUsReworked.Resources) ? LoadResourceTexture(path) : LoadDiskTexture(path);

    public static Texture2D LoadDiskTexture(string path) => LoadTexture(File.ReadAllBytes(path), path.SanitisePath());

    public static Texture2D LoadResourceTexture(string path) => LoadTexture(TownOfUsReworked.Core.GetManifestResourceStream(path).ReadFully(), path.SanitisePath());

    public static Texture2D LoadTexture(byte[] data, string name)
    {
        var texture = EmptyTexture();

        if (texture.LoadImage(data, !GetReadable(name)))
        {
            texture.name = name;
            texture.hideFlags |= HideFlags.DontSaveInEditor;
            return texture.DontDestroy();
        }

        return null;
    }

    public static Sprite LoadSprite(string path) => path.StartsWith(TownOfUsReworked.Resources) ? LoadResourceSprite(path) : LoadDiskSprite(path);

    public static Sprite LoadDiskSprite(string path) => LoadSprite(LoadDiskTexture(path), path.SanitisePath());

    public static Sprite LoadResourceSprite(string path) => LoadSprite(LoadResourceTexture(path), path.SanitisePath());

    public static Sprite LoadSprite(Texture2D tex, string name, float size = float.NaN, SpriteMeshType meshType = SpriteMeshType.Tight)
    {
        var sprite = Sprite.Create(tex, new(0, 0, tex.width, tex.height), new(0.5f, 0.5f), float.IsNaN(size) ? GetSize(name) : size, 0, meshType);
        sprite.name = name;
        sprite.hideFlags |= HideFlags.DontSaveInEditor;
        return sprite.DontDestroy();
    }

    public static AssetBundle LoadBundle(byte[] data) => AssetBundle.LoadFromMemory(data);

    public static void SaveText(string fileName, string textToSave, string diskLocation) => SaveText(fileName, textToSave, true, diskLocation);

    public static void SaveText(string fileName, string textToSave, bool overrideText = true, string diskLocation = null)
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
        PortalAnimation.Clear();
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

    public static float GetSize(string name)
    {
        if (name is "CurrentSettings" or "Client" or "Plus" or "Minus" or "Wiki")
            return 180;
        else if (name == "Phone")
            return 200;
        else if (name == "Cursor")
            return 115;
        else if (name == "NightVision")
            return 350;
        else if (name is "Info" or "GitHub" or "Discord")
            return 525;
        else
            return 100;
    }

    private static T Get<T>(string name) where T : UObject
    {
        if (LoadedAssets.TryGetValue(name, out var objList) && objList.TryFinding(x => x is T, out var result) && result)
            return result as T;

        if (AssetToBundle.TryGetValue(name.ToLower(), out var bundle))
        {
            result = LoadAsset<T>(Bundles[bundle], name);

            if (result)
                return result as T;
        }

        if (UnloadedAssets.TryGetValue(name, out var strings))
        {
            var tType = typeof(T);
            result = null;

            if (tType == typeof(Sprite) && strings.TryFinding(x => x.EndsWith(".png"), out var path))
                result = AddAsset(name, LoadSprite(path));
            else if (tType == typeof(AudioClip) && strings.TryFinding(x => x.EndsWith(".wav"), out path))
                result = AddAsset(name, LoadAudio(path));
            else if (tType == typeof(Texture2D) && strings.TryFinding(x => x.EndsWith(".png"), out path))
                result = AddAsset(name, LoadTexture(path));
            else
                return null;

            strings.Remove(path);

            if (strings.Count == 0)
                UnloadedAssets.Remove(name);

            if (result)
                return result as T;
        }

        // Failure($"{name} does not exist");
        return null;
    }

    public static IEnumerable<T> UnityGetAll<T>() where T : UObject => LoadedAssets.Values.GetAll().OfType<T>();

    private static T LoadAsset<T>(AssetBundle assetBundle, string name) where T : UObject
    {
        var asset = assetBundle.LoadAsset<T>(name)?.DontUnload();
        AddAsset(name, asset);
        AssetToBundle.Remove(name);

        if (!Bundles.Keys.Any(AssetToBundle.Values.Contains))
        {
            Bundles.Remove(assetBundle.name);
            assetBundle.Unload(false);
        }

        return asset;
    }

    public static AudioClip GetIntroSound(RoleTypes roleType) => RoleManager.Instance.GetRole(roleType)?.IntroSound;

    public static UObject AddAsset(string name, UObject obj)
    {
        if (!obj)
            return null;

        if (!LoadedAssets.TryGetValue(name, out var value))
            LoadedAssets[name] = [ obj ];
        else if (!value.Contains(obj))
            value.Add(obj);

        return obj;
    }

    public static string AddPath(string name, string path)
    {
        if (!UnloadedAssets.TryGetValue(name, out var value))
            UnloadedAssets[name] = [ path ];
        else if (!value.Contains(path))
            value.Add(path);

        return path;
    }

    private static bool GetReadable(string name) => name is "Cursor";

    public static AudioClip LoadAudio(string path) => path.StartsWith(TownOfUsReworked.Resources) ? LoadResourceAudio(path) : LoadDiskAudio(path);

    public static AudioClip LoadResourceAudio(string path) => LoadAudio(path.SanitisePath(), TownOfUsReworked.Core.GetManifestResourceStream(path).ReadFully());

    public static AudioClip LoadDiskAudio(string path) => LoadAudio(path.SanitisePath(), File.ReadAllBytes(path));

    // Lord help my soul, got the code from here: https://github.com/deadlyfingers/UnityWav/blob/master/WavUtility.cs

    public static AudioClip LoadAudio(string name, byte[] fileBytes)
    {
        var chunk = BitConverter.ToInt32(fileBytes, 16) + 24;
        var channels = BitConverter.ToUInt16(fileBytes, 22);
        var sampleRate = BitConverter.ToInt32(fileBytes, 24);
        var bitDepth = BitConverter.ToUInt16(fileBytes, 34);
        var data = bitDepth switch
        {
            8 => Convert8BitByteArrayToAudioClipData(fileBytes, chunk),
            16 => Convert16BitByteArrayToAudioClipData(fileBytes, chunk),
            24 => Convert24BitByteArrayToAudioClipData(fileBytes, chunk),
            32 => Convert32BitByteArrayToAudioClipData(fileBytes, chunk),
            _ => throw new Exception(bitDepth + " bit depth is not supported."),
        };

        var audioClip = AudioClip.Create(name, data.Length, channels, sampleRate, false);

        if (audioClip.SetData(data, 0))
        {
            audioClip.hideFlags |= HideFlags.DontSaveInEditor;
            return audioClip.DontDestroy();
        }

        return null;
    }

    private static float[] Convert8BitByteArrayToAudioClipData(byte[] source, int headerOffset)
    {
        var wavSize = BitConverter.ToInt32(source, headerOffset);
        headerOffset += sizeof(int);
        var data = new float[wavSize];

        for (var i = 0; i < wavSize; i++)
            data[i] = (float)source[i] / sbyte.MaxValue;

        return data;
    }

    private static float[] Convert16BitByteArrayToAudioClipData(byte[] source, int headerOffset)
    {
        var wavSize = BitConverter.ToInt32(source, headerOffset);
        headerOffset += sizeof(int);
        var x = sizeof(short);
        var convertedSize = wavSize / x;
        var data = new float[convertedSize];

        for (var i = 0; i < convertedSize; i++)
            data[i] = (float)BitConverter.ToInt16(source, (i * x) + headerOffset) / short.MaxValue;

        return data;
    }

    private static float[] Convert24BitByteArrayToAudioClipData(byte[] source, int headerOffset)
    {
        var wavSize = BitConverter.ToInt32(source, headerOffset);
        var intSize = sizeof(int);
        headerOffset += intSize;
        var x = 3; // Block size = 3
        var convertedSize = wavSize / x;
        var data = new float[convertedSize];
        var block = new byte[intSize]; // Using a 4 byte block for copying 3 bytes, then copy bytes with 1 offset

        for (var i = 0; i < convertedSize; i++)
        {
            Buffer.BlockCopy(source, (i * x) + headerOffset, block, 1, x);
            data[i] = (float)BitConverter.ToInt32(block, 0) / int.MaxValue;
        }

        return data;
    }

    private static float[] Convert32BitByteArrayToAudioClipData (byte[] source, int headerOffset)
    {
        var wavSize = BitConverter.ToInt32(source, headerOffset);
        headerOffset += sizeof(int);
        var x = sizeof(float); // Block size = 4
        var convertedSize = wavSize / x;
        var data = new float[convertedSize];

        for (var i = 0; i < convertedSize; i++)
            data[i] = (float)BitConverter.ToInt32(source, (i * x) + headerOffset) / int.MaxValue;

        return data;
    }
}