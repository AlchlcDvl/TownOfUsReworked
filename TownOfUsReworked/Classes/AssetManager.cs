namespace TownOfUsReworked.Classes;

public static class AssetManager
{
    public static readonly Dictionary<string, AudioClip> SoundEffects = new();
    public static readonly Dictionary<string, Sprite> Sprites = new();
    public static readonly Dictionary<string, float> Sizes = new();
    public static readonly Dictionary<string, int> Frequencies = new();
    public static readonly Sprite[] PortalAnimation = new Sprite[205];
    public static readonly string[] Presets = { "Casual", "Chaos", "Ranked" };

    public static AudioClip GetAudio(string path)
    {
        if (!SoundEffects.TryGetValue(path, out var sound))
        {
            LogError($"{path} does not exist");
            return null;
        }

        return sound;
    }

    public static Sprite GetSprite(string path)
    {
        if (!Sprites.TryGetValue(path, out var sprite))
        {
            //LogError($"{path} does not exist");
            return Meeting ? Sprites["MeetingPlaceholder"] : Sprites["Placeholder"];
        }

        return sprite ?? Sprites["Placeholder"];
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
        var texture = EmptyTexture();
        _ = ImageConversion.LoadImage(texture, File.ReadAllBytes(path), false);
        texture.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
        texture.name = path;
        _ = texture.DontDestroy();
        return texture;
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
        catch (Exception e)
        {
            LogError($"Error loading {name} as a sprite\n{e}");
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
        catch (Exception e)
        {
            LogError($"Error loading {path} from resources\n{e}");
            return null;
        }
    }

    public static string ReadResourceText(string itemName, string folder = "")
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
        catch (Exception e)
        {
            LogError($"Error loading {itemName} from resources\n{e}");
            return "";
        }
    }

    public static void SaveText(string fileName, string textToSave, string diskLocation) => SaveText(fileName, textToSave, true, diskLocation);

    public static void SaveText(string fileName, string textToSave, bool overrideText = true, string diskLocation = null)
    {
        try
        {
            File.WriteAllText(Path.Combine(diskLocation ?? Application.persistentDataPath, fileName), (overrideText ? "" : ReadDiskText(fileName)) + textToSave);
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
        Sizes.Clear();
        Sprites.Clear();
        Frequencies.Clear();
        Array.Clear(PortalAnimation, 0, 205);

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
                else if (name == "NightVision")
                    Sizes.Add(name, 350);
                else
                    Sizes.Add(name, 100);
            }
            else if (resourceName.EndsWith(".raw"))
            {
                var name = resourceName.Replace(".raw", "").Replace(TownOfUsReworked.Sounds, "");

                if (name.Contains("Intro"))
                    Frequencies.Add(name, 36000);
                else
                    Frequencies.Add(name, 48000);
            }
        }

        var position = 0;

        foreach (var resourceName in TownOfUsReworked.Core.GetManifestResourceNames())
        {
            if ((resourceName.StartsWith(TownOfUsReworked.Buttons) || resourceName.StartsWith(TownOfUsReworked.Misc)) && resourceName.EndsWith(".png"))
                Sprites.Add(resourceName.Replace(".png", "").Replace(TownOfUsReworked.Buttons, "").Replace(TownOfUsReworked.Misc, ""), CreateSprite(resourceName));
            else if (resourceName.StartsWith(TownOfUsReworked.Sounds) && resourceName.EndsWith(".raw"))
                SoundEffects.Add(resourceName.Replace(".raw", "").Replace(TownOfUsReworked.Sounds, ""), CreateAudio(resourceName));
            else if (resourceName.StartsWith(TownOfUsReworked.Portal) && resourceName.EndsWith(".png"))
            {
                if (PortalAnimation[position] == null)
                    PortalAnimation[position] = CreateSprite(resourceName);

                position++;
            }
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
    }
}