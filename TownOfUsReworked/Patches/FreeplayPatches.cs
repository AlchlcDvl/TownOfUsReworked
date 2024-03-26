/*namespace TownOfUsReworked.Patches;

// I'll leave the code here for now and get back to it later

public static class FreeplayPatches
{
    private const SystemTypes LayersType = (SystemTypes)255;

    public static readonly List<PlayerLayer> PreviouslySelected = [];
    private static readonly List<string> FolderNames = [];
    private static readonly Dictionary<string, LayerEnum> RoleButtons = [];

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
        var info = LayerInfo.AllRoles.Find(x => x.Role == layer);
        var button = UObject.Instantiate(__instance.RoleButton);
        button.SafePositionWorld = __instance.SafePositionWorld;
        button.Text.text = $"{info.Short.Replace(" (XD)", "")}.exe";
        button.Role = RoleManager.Instance.AllRoles[0];
        button.FileImage.color = button.RolloverHandler.OutColor = info.Color;
        button.Overlay.enabled = CustomPlayer.Local.Is(layer);
        button.Overlay.sprite = button.CheckImage;
        button.Button.OnClick = new();
        button.Button.OnClick.AddListener((Action)(() =>
        {
            var role = Role.LocalRole;

            if (role && !PreviouslySelected.Any(x => x.Type == role.Type))
                PreviouslySelected.Add(role);

            var ability = Ability.LocalAbility;

            if (ability && !PreviouslySelected.Any(x => x.Type == ability.Type))
                PreviouslySelected.Add(ability);

            var modifier = Modifier.LocalModifier;

            if (modifier && !PreviouslySelected.Any(x => x.Type == modifier.Type))
                PreviouslySelected.Add(modifier);

            var objectifier = Objectifier.LocalObjectifier;

            if (objectifier && !PreviouslySelected.Any(x => x.Type == objectifier.Type))
                PreviouslySelected.Add(objectifier);

            var selected = PreviouslySelected.Find(x => x.Type == layer);

            if (selected)
                selected.Player = CustomPlayer.Local;
            else
                selected = RoleGen.SetLayer(layer, layer.GetLayerType()).Start(CustomPlayer.Local);

            if (role?.Type == layer)
            {
                role.ExitingLayer();
                role.Player = null;
            }

            if (ability?.Type == layer)
            {
                ability.ExitingLayer();
                ability.Player = null;
            }

            if (modifier?.Type == layer)
            {
                modifier.ExitingLayer();
                modifier.Player = null;
            }

            if (objectifier?.Type == layer)
            {
                objectifier.ExitingLayer();
                objectifier.Player = null;
            }

            ButtonUtils.Reset();
        }));
        __instance.AddFileAsChild(folder, button, ref num, ref num2, ref num3);
        RoleButtons[button.Text.text] = layer;
    }

    [HarmonyPatch(typeof(TaskAdderGame), nameof(TaskAdderGame.Begin))]
    public static class BeginTaskAdderPatch
    {
        public static void Prefix(TaskAdderGame __instance)
        {
            FolderNames.Clear();
            LogInfo($"File Width: {__instance.fileWidth} Line Width: {__instance.lineWidth} Folder Width {__instance.folderWidth}");
        }
    }

    [HarmonyPatch(typeof(TaskAdderGame), nameof(TaskAdderGame.PopulateRoot))]
    public static class PopulatRootPatch
    {
        public static void Postfix(TaskAdderGame __instance, ref ISystem.Dictionary<SystemTypes, TaskFolder> folders, ref TaskFolder rootFolder)
        {
            if (!folders.TryGetValue(LayersType, out var taskFolder))
            {
                taskFolder = folders[LayersType] = CreateFolder(__instance, "_Reworked", rootFolder);

                for (var i = 0; i < 4; i++)
                {
                    var folder = CreateFolder(__instance, ((PlayerLayerEnum)i).ToString(), taskFolder);

                    if (folder.FolderName == "Role")
                    {
                        for (var j = 0; j < 5; j++)
                            CreateFolder(__instance, ((Faction)j).ToString(), folder);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(TaskAdderGame), nameof(TaskAdderGame.ShowFolder))]
    public static class ShowFolderPatch
    {
        public static bool Prefix(TaskAdderGame __instance, ref TaskFolder taskFolder)
        {
            RoleButtons.Clear();
            var stringBuilder = new StringBuilder();
            __instance.Hierarchy.Add(taskFolder);

            foreach (var folder in __instance.Hierarchy)
            {
                stringBuilder.Append(folder.FolderName);
                stringBuilder.Append('\\');
            }

            __instance.PathText.text = stringBuilder.ToString();
            __instance.ActiveItems.ForEach(x => x.gameObject.Destroy());
            __instance.ActiveItems.Clear();
            var num = 0f;
            var num2 = 0f;
            var num3 = 0f;
            taskFolder.SubFolders = taskFolder.SubFolders.Il2CppToSystem().OrderBy(x => x.FolderName).ToList().SystemToIl2Cpp();

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

                if (taskFolder2 != null && taskFolder2.Button != null)
                {
                    ControllerManager.Instance.AddSelectableUiElement(taskFolder2.Button, false);

                    if (!IsNullEmptyOrWhiteSpace(__instance.restorePreviousSelectionByFolderName) && taskFolder2.FolderName.Equals(__instance.restorePreviousSelectionByFolderName))
                        __instance.restorePreviousSelectionFound = taskFolder2.Button;
                }
            }

            if (FolderNames.Contains(taskFolder.FolderName))
            {
                var layerEnum = Enum.TryParse<PlayerLayerEnum>(taskFolder.FolderName, out var h) ? h : PlayerLayerEnum.None;
                var (start, end) = layerEnum switch
                {
                    PlayerLayerEnum.Modifier => ((int)LayerEnum.Astral, (int)LayerEnum.Yeller),
                    PlayerLayerEnum.Objectifier => ((int)LayerEnum.Allied, (int)LayerEnum.Traitor),
                    PlayerLayerEnum.Ability => ((int)LayerEnum.ButtonBarry, (int)LayerEnum.Underdog),
                    _ => (-1, -1)
                };

                if (Enum.TryParse<Faction>(taskFolder.FolderName, out var faction))
                {
                    (start, end) = faction switch
                    {
                        Faction.Crew => ((int)LayerEnum.Altruist, (int)LayerEnum.Vigilante),
                        Faction.Neutral => ((int)LayerEnum.Actor, (int)LayerEnum.Whisperer),
                        Faction.Intruder => ((int)LayerEnum.Ambusher, (int)LayerEnum.Wraith),
                        Faction.GameMode => ((int)LayerEnum.Hunter, (int)LayerEnum.Runner),
                        Faction.Syndicate => ((int)LayerEnum.Anarchist, (int)LayerEnum.Warper),
                        _ => (-1, -1)
                    };
                }

                if (start != -1 && end != -1)
                {
                    for (var k = start; k <= end; k++)
                    {
                        var layer = (LayerEnum)k;

                        if (layer is LayerEnum.Phantom or LayerEnum.Ghoul or LayerEnum.Banshee or LayerEnum.Revealer)
                            continue;

                        try
                        {
                            CreateRoleButton(__instance, taskFolder, layer, ref num, ref num2, ref num3);
                        }
                        catch (Exception e)
                        {
                            LogError($"Layer: {layer}\n{e}");
                        }
                    }
                }
            }
            else
            {
                var flag = false;
                taskFolder.Children = taskFolder.Children.Il2CppToSystem().OrderBy(t => t.TaskType).ToList().SystemToIl2Cpp();

                foreach (var task in taskFolder.Children)
                {
                    var taskAddButton = UObject.Instantiate(__instance.TaskPrefab);
                    taskAddButton.MyTask = task;

                    if (taskAddButton.MyTask.TaskType == TaskTypes.DivertPower)
                    {
                        var divert = taskAddButton.MyTask.TryCast<DivertPowerTask>();

                        if (divert != null)
                            taskAddButton.Text.text = TranslationController.Instance.GetString(StringNames.DivertPowerTo, TranslationController.Instance.GetString(divert.TargetSystem));
                    }
                    else if (taskAddButton.MyTask.TaskType == TaskTypes.FixWeatherNode)
                    {
                        var node = taskAddButton.MyTask.TryCast<WeatherNodeTask>();

                        if (node != null)
                        {
                            taskAddButton.Text.text = TranslationController.Instance.GetString(StringNames.FixWeatherNode) + " " +
                                TranslationController.Instance.GetString(WeatherSwitchGame.ControlNames[node.NodeId]);
                        }
                    }
                    else
                        taskAddButton.Text.text = TranslationController.Instance.GetString(taskAddButton.MyTask.TaskType);

                    __instance.AddFileAsChild(taskFolder, taskAddButton, ref num, ref num2, ref num3);

                    if (taskAddButton != null && taskAddButton.Button != null)
                    {
                        ControllerManager.Instance.AddSelectableUiElement(taskAddButton.Button, false);

                        if (__instance.Hierarchy.Count != 1 && !flag)
                        {
                            var component = ControllerManager.Instance.CurrentUiState.CurrentSelection.GetComponent<TaskFolder>();

                            if (component != null)
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
            if (RoleButtons.TryGetValue(__instance.Text.text, out var layer))
            {
                __instance.Overlay.enabled = CustomPlayer.Local.Is(layer);
                return false;
            }

            return true;
        }
    }
}*/