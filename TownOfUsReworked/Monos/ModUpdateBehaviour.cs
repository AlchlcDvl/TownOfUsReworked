using System.Net.Http;
using System.Threading.Tasks;
using BepInEx.Unity.IL2CPP.Utils;
using Newtonsoft.Json.Linq;
using Version = SemanticVersioning.Version;

namespace TownOfUsReworked.Monos;

//Based off of The Other Roles' auto updater
public class ModUpdateBehaviour : MonoBehaviour
{
    private static bool UpdateInProgress;
    private static bool Updated;

    public static ModUpdateBehaviour Instance { get; private set; }

    public ModUpdateBehaviour(IntPtr ptr) : base(ptr) {}

    private UpdateData ReworkedUpdate;

    [HideFromIl2Cpp]
    private bool UpdateExists => RequiredUpdateData != null;

    [HideFromIl2Cpp]
    private UpdateData RequiredUpdateData => ReworkedUpdate;

    public void Awake()
    {
        if (Instance)
            this.Destroy();

        Instance = this;
        SceneManager.add_sceneLoaded(new Action<Scene, LoadSceneMode>((scene, _) => OnSceneLoaded(scene)));
        this.StartCoroutine(CoCheckUpdates());
    }

    private void OnSceneLoaded(Scene scene)
    {
        if (UpdateInProgress || Updated || scene.name != "MainMenu")
            return;

        DownloadAssets();

        if (!UpdateExists)
            return;

        var template = GameObject.Find("ExitGameButton");

        if (!template)
            return;

        var button = Instantiate(template, null);
        button.GetComponent<AspectPosition>().anchorPoint = new(0.458f, 0.124f);

        var passiveButton = button.GetComponent<PassiveButton>();
        passiveButton.OnClick = new();
        passiveButton.OnClick.AddListener((Action)(() =>
        {
            this.StartCoroutine(CoUpdate());
            button.SetActive(false);
        }));

        var text = button.transform.GetComponentInChildren<TMP_Text>();
        var t = "Update Reworked";
        StartCoroutine(Effects.Lerp(0.1f, new Action<float>(_ => text.SetText(t))));
        passiveButton.OnMouseOut.AddListener((Action)(() => text.color = Color.red));
        passiveButton.OnMouseOver.AddListener((Action)(() => text.color = Color.white));
    }

    [HideFromIl2Cpp]
    public IEnumerator CoUpdate()
    {
        UpdateInProgress = true;
        var popup = Instantiate(TwitchManager.Instance.TwitchPopup);
        popup.TextAreaTMP.fontSize *= 0.7f;
        popup.TextAreaTMP.enableAutoSizing = false;
        popup.Show();
        var button = popup.transform.GetChild(2).gameObject;
        button.SetActive(false);
        popup.TextAreaTMP.text = Translate("Updates.Mod.Updating").Replace("%mod%", Translate("Mod.Reworked"));
        var download = Task.Run(DownloadUpdate);

        while (!download.IsCompleted)
            yield return null;

        button.SetActive(true);
        popup.TextAreaTMP.text = Translate($"Updates.Mod.{(download.Result ? "" : "No")}Success");
        UpdateInProgress = false;
        Updated = download.Result;
    }

    [HideFromIl2Cpp]
    public static IEnumerator CoCheckUpdates()
    {
        var rewUpdateCheck = Task.Run(Instance.GetGithubUpdate);

        while (!rewUpdateCheck.IsCompleted)
            yield return null;
        
        var result = rewUpdateCheck.Result;

        if (result != null && result.IsNewer(Version.Parse(TownOfUsReworked.VersionS)) && !ForbiddenUpdate(result))
            Instance.ReworkedUpdate = result;
    }

    [HideFromIl2Cpp]
    public async Task<UpdateData> GetGithubUpdate()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Reworked Updater");

        try
        {
            var req = await client.GetAsync("https://api.github.com/repos/AlchlcDvl/TownOfUsReworked/releases/latest", HttpCompletionOption.ResponseContentRead);

            if (!req.IsSuccessStatusCode)
                return null;

            var dataString = await req.Content.ReadAsStringAsync();
            var data = JObject.Parse(dataString);
            return new(data);
        }
        catch
        {
            return null;
        }
    }

    [HideFromIl2Cpp]
    public async Task<bool> DownloadUpdate()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Reworked Updater");
        var assets = ReworkedUpdate.Request["assets"];
        var downloadURI = "";

        for (var current = assets.First; current != null; current = current.Next) 
        {
            var download = current["download"]?.ToString();
            var content = current["content_type"]?.ToString();

            if (download != null && content != null)
            {
                if (content.Equals("application/x-msdownload") && download.EndsWith(".dll"))
                {
                    downloadURI = download;
                    break;
                }
            }
        }

        if (downloadURI.Length == 0)
            return false;

        var res = await client.GetAsync(downloadURI, HttpCompletionOption.ResponseContentRead);
        var filePath = Path.Combine(Paths.PluginPath, "Reworked.dll");

        if (File.Exists(filePath + ".old"))
            File.Delete(filePath + ".old");

        if (File.Exists(filePath))
            File.Move(filePath, filePath + ".old");

        await using var responseStream = await res.Content.ReadAsStreamAsync();
        await using var fileStream = File.Create(filePath);
        await responseStream.CopyToAsync(fileStream);
        return true;
    }

    [HideFromIl2Cpp]
    private void DownloadAssets() => AssetLoader.LaunchFetchers(UpdateExists);

    [HideFromIl2Cpp]
    private static bool ForbiddenUpdate(UpdateData data) => data.Content.Contains("[Manual]");
}