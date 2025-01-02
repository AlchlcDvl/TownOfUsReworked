namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(TaskAdderGame))]
public static class FreeplayPatches
{
    private const SystemTypes LayersType = (SystemTypes)255;

    public static readonly List<PlayerLayer> PreviouslySelected = [];
    private static readonly List<string> FolderNames = [];
    public static readonly Dictionary<string, LayerEnum> RoleButtons = [];

    private static TaskFolder CreateFolder(TaskAdderGame __instance, string name, TaskFolder parent)
    {
        var folder = UObject.Instantiate(__instance.RootFolderPrefab, __instance.transform);
        folder.gameObject.SetActive(false);
        folder.FolderName = name;
        FolderNames.Add(name);
        parent.SubFolders.Add(folder);
        return folder;
    }

    private static void CreateRoleButton(TaskAdderGame __instance, TaskFolder folder, LayerEnum layer, ref float num, ref float num2, ref float num3)
    {
        if (!LayerDictionary.TryGetValue(layer, out var entry))
            return;

        var type = layer.GetLayerType();
        var extension = type switch
        {
            PlayerLayerEnum.Role => "r",
            PlayerLayerEnum.Modifier => "m",
            PlayerLayerEnum.Ability => "a",
            PlayerLayerEnum.Disposition => "d",
            _ => "exe",
        };
        var button = UObject.Instantiate(__instance.RoleButton);
        button.SafePositionWorld = __instance.SafePositionWorld;
        button.Text.SetText(button.name = $"{TranslationManager.Translate($"CustomOption.{layer}")}.{extension}");
        button.Role = RoleManager.Instance.AllRoles[^1];
        button.FileImage.color = button.RolloverHandler.OutColor = entry.Color;
        button.RolloverHandler.OverColor = entry.Color.Alternate(0.4f);
        button.Overlay.enabled = CustomPlayer.Local.Is(layer);
        button.Overlay.sprite = button.CheckImage;
        button.Button.OverrideOnClickListeners(() =>
        {
            var layers = PlayerLayer.LocalLayers();

            if (layers.TryFindingAll(x => !PreviouslySelected.Contains(x), out var missing))
                PreviouslySelected.AddRange(missing);

            if (layers.TryFinding(x => x.Type == layer, out var changing))
                changing.End();

            if (!PreviouslySelected.TryFinding(x => x.Type == layer, out var selected))
                selected = RoleGenManager.SetLayer(layer, type);

            selected.Start(CustomPlayer.Local);
            ButtonUtils.Reset();

            if (CustomPlayer.Local.Data.Role is LayerHandler handler)
                handler.SetUpLayers();
        });
        __instance.AddFileAsChild(folder, button, ref num, ref num2, ref num3);
        RoleButtons[button.name] = layer;
    }

    [HarmonyPatch(nameof(TaskAdderGame.Begin))]
    public static void Prefix(/*TaskAdderGame __instance*/)
    {
        FolderNames.Clear();
        // Info($"File Width: {__instance.fileWidth} Line Width: {__instance.lineWidth} Folder Width {__instance.folderWidth}");
    }

    [HarmonyPatch(nameof(TaskAdderGame.PopulateRoot))]
    public static void Postfix(TaskAdderGame __instance, ISystem.Dictionary<SystemTypes, TaskFolder> folders, TaskFolder rootFolder)
    {
        if (!folders.TryGetValue(LayersType, out var taskFolder))
        {
            taskFolder = folders[LayersType] = CreateFolder(__instance, "_Reworked", rootFolder);

            for (var i = 0; i < 4; i++)
            {
                var folder = CreateFolder(__instance, $"{(PlayerLayerEnum)i}", taskFolder);

                if (folder.FolderName == "Role")
                {
                    for (var j = 0; j < 5; j++)
                        CreateFolder(__instance, $"{(Faction)j}", folder);
                }
            }
        }
    }

    [HarmonyPatch(nameof(TaskAdderGame.ShowFolder))]
    public static bool Prefix(TaskAdderGame __instance, TaskFolder taskFolder)
    {
        RoleButtons.Clear();
        var stringBuilder = new Il2CppSystem.Text.StringBuilder();
        __instance.Hierarchy.Add(taskFolder);

        foreach (var folder in __instance.Hierarchy)
        {
            stringBuilder.Append(folder.FolderName);
            stringBuilder.Append('\\');
        }

        __instance.PathText.SetText(stringBuilder);
        __instance.ActiveItems.ForEach(x => x.gameObject.Destroy());
        __instance.ActiveItems.Clear();
        var num = 0f;
        var num2 = 0f;
        var num3 = 0f;
        taskFolder.SubFolders = taskFolder.SubFolders.ToArray().OrderBy(x => x.FolderName).ToIl2Cpp();

        foreach (var sub in taskFolder.SubFolders)
        {
            var taskFolder2 = UObject.Instantiate(sub, __instance.TaskParent);
            taskFolder2.gameObject.SetActive(true);
            taskFolder2.Parent = __instance;
            taskFolder2.transform.localPosition = new(num, num2, 0f);
            taskFolder2.transform.localScale = Vector3.one;
            num3 = Mathf.Max(num3, taskFolder2.Text.bounds.size.y + 1.1f);
            num += __instance.folderWidth;

            if (num > __instance.lineWidth)
            {
                num = 0f;
                num2 -= num3;
                num3 = 0f;
            }

            __instance.ActiveItems.Add(taskFolder2.transform);

            if (taskFolder2 && taskFolder2.Button)
            {
                ControllerManager.Instance.AddSelectableUiElement(taskFolder2.Button, false);

                if (!IsNullEmptyOrWhiteSpace(__instance.restorePreviousSelectionByFolderName) && taskFolder2.FolderName.Equals(__instance.restorePreviousSelectionByFolderName))
                    __instance.restorePreviousSelectionFound = taskFolder2.Button;
            }
        }

        if (FolderNames.Contains(taskFolder.FolderName))
        {
            var layerEnum = Enum.TryParse<PlayerLayerEnum>(taskFolder.FolderName, out var h) ? h : PlayerLayerEnum.None;
            var range = layerEnum switch
            {
                PlayerLayerEnum.Modifier => GetValuesFromTo(LayerEnum.Astral, LayerEnum.Yeller),
                PlayerLayerEnum.Disposition => GetValuesFromTo(LayerEnum.Allied, LayerEnum.Traitor),
                PlayerLayerEnum.Ability => GetValuesFromTo(LayerEnum.ButtonBarry, LayerEnum.Underdog),
                _ => []
            };

            if (Enum.TryParse<Faction>(taskFolder.FolderName, out var faction))
            {
                range = faction switch
                {
                    Faction.Crew => GetValuesFromTo(LayerEnum.Altruist, LayerEnum.Vigilante),
                    Faction.Neutral => GetValuesFromTo(LayerEnum.Actor, LayerEnum.Whisperer),
                    Faction.Intruder => GetValuesFromTo(LayerEnum.Ambusher, LayerEnum.Wraith),
                    Faction.GameMode => GetValuesFromTo(LayerEnum.Hunter, LayerEnum.Runner),
                    Faction.Syndicate => GetValuesFromTo(LayerEnum.Anarchist, LayerEnum.Warper),
                    _ => []
                };
            }

            foreach (var layer in range)
            {
                if (layer is LayerEnum.Phantom or LayerEnum.Ghoul or LayerEnum.Banshee or LayerEnum.Revealer)
                    continue;

                try
                {
                    CreateRoleButton(__instance, taskFolder, layer, ref num, ref num2, ref num3);
                }
                catch (Exception e)
                {
                    Error($"Layer: {layer}\n{e}");
                }
            }
        }
        else
        {
            var flag = false;
            taskFolder.Children = taskFolder.Children.ToArray().OrderBy(t => t.TaskType).ToIl2Cpp();

            foreach (var task in taskFolder.Children)
            {
                var taskAddButton = UObject.Instantiate(__instance.TaskPrefab);
                taskAddButton.MyTask = task;

                if (taskAddButton.MyTask.TaskType == TaskTypes.DivertPower)
                {
                    var divert = taskAddButton.MyTask.TryCast<DivertPowerTask>();

                    if (divert)
                        taskAddButton.Text.SetText(TranslationController.Instance.GetString(StringNames.DivertPowerTo, TranslationController.Instance.GetString(divert.TargetSystem)));
                }
                else if (taskAddButton.MyTask.TaskType == TaskTypes.FixWeatherNode)
                {
                    var node = taskAddButton.MyTask.TryCast<WeatherNodeTask>();

                    if (node)
                    {
                        taskAddButton.Text.SetText(TranslationController.Instance.GetString(StringNames.FixWeatherNode) + " " +
                            TranslationController.Instance.GetString(WeatherSwitchGame.ControlNames[node.NodeId]));
                    }
                }
                else
                    taskAddButton.Text.SetText(TranslationController.Instance.GetString(taskAddButton.MyTask.TaskType));

                __instance.AddFileAsChild(taskFolder, taskAddButton, ref num, ref num2, ref num3);

                if (taskAddButton && taskAddButton.Button)
                {
                    ControllerManager.Instance.AddSelectableUiElement(taskAddButton.Button, false);

                    if (__instance.Hierarchy.Count != 1 && !flag)
                    {
                        if (ControllerManager.Instance.CurrentUiState.CurrentSelection.TryGetComponent<TaskFolder>(out var component) && component)
                            __instance.restorePreviousSelectionByFolderName = component.FolderName;

                        ControllerManager.Instance.SetDefaultSelection(taskAddButton.Button, null);
                        flag = true;
                    }
                }
            }
        }

        ControllerManager.Instance.SetBackButton(__instance.Hierarchy.Count == 1 ? __instance.BackButton : __instance.FolderBackButton);
        return false;
    }
}

[HarmonyPatch(typeof(TaskAddButton), nameof(TaskAddButton.Update))]
public static class UpdateRoleButtons
{
    public static bool Prefix(TaskAddButton __instance)
    {
        if (FreeplayPatches.RoleButtons.TryGetValue(__instance.name, out var layer))
        {
            __instance.Overlay.enabled = CustomPlayer.Local.Is(layer);
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(TutorialManager), nameof(TutorialManager.Awake))]
public static class TutorialManagerRunTutorial
{
    public static void Postfix(TutorialManager __instance) => __instance.StartCoroutine(CustomPlayer.Local.CoSetRole((RoleTypes)100, false));
}