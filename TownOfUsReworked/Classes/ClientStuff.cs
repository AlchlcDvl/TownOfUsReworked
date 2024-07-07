// using Cpp2IL.Core.Extensions;

// namespace TownOfUsReworked.Classes;

// // This class only exists because some IL2Cpp bs won't let me put this into the mono instead
// public static class ClientStuff
// {
//     public static void ClickZoom()
//     {
//         CloseMenus(SkipEnum.Zooming);
//         ClientHandler.Instance.Zooming = !ClientHandler.Instance.Zooming;
//         Coroutines.Start(Zoom(ClientHandler.Instance.Zooming));
//     }

//     private static IEnumerator Zoom(bool inOut)
//     {
//         var change = 0.3f * (inOut ? 1 : -1);
//         var limit = inOut ? 12f : 3f;

//         for (var i = Camera.main.orthographicSize; inOut ? (i < 12f) : (i > 3f); i += change)
//         {
//             var size = Meeting ? 3f : i;
//             Camera.main.orthographicSize = size;
//             Camera.allCameras.ForEach(x => x.orthographicSize = size);
//             ResolutionManager.ResolutionChanged.Invoke(Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
//             yield return EndFrame();
//         }

//         Camera.main.orthographicSize = limit;
//         Camera.allCameras.ForEach(x => x.orthographicSize = limit);
//         ResolutionManager.ResolutionChanged.Invoke(Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
//         yield break;
//     }

//     public static void OpenSettings()
//     {
//         CloseMenus(SkipEnum.Settings);

//         if (LocalBlocked)
//             return;

//         ClientHandler.Instance.SettingsActive = !ClientHandler.Instance.SettingsActive;
//         HUD.GameSettings.gameObject.SetActive(ClientHandler.Instance.SettingsActive);
//     }

//     public static void OpenRoleCard()
//     {
//         CloseMenus(SkipEnum.RoleCard);

//         if (LocalBlocked)
//             return;

//         if (ClientHandler.Instance.PhoneText)
//             ClientHandler.Instance.PhoneText.gameObject.Destroy();

//         ClientHandler.Instance.PhoneText = UObject.Instantiate(HUD.KillButton.cooldownTimerText, ClientHandler.Instance.Phone.transform);
//         ClientHandler.Instance.PhoneText.enableWordWrapping = false;
//         ClientHandler.Instance.PhoneText.transform.localScale = Vector3.one * 0.4f;
//         ClientHandler.Instance.PhoneText.transform.localPosition = new(0, 0, -50f);
//         ClientHandler.Instance.PhoneText.gameObject.layer = 5;
//         ClientHandler.Instance.PhoneText.alignment = TextAlignmentOptions.Center;
//         ClientHandler.Instance.PhoneText.name = "PhoneText";

//         if (!ClientHandler.Instance.ToTheWiki)
//         {
//             ClientHandler.Instance.ToTheWiki = CreateButton("ToTheWiki", "Mod Wiki", () =>
//             {
//                 OpenRoleCard();
//                 OpenWiki();
//             });
//         }

//         ClientHandler.Instance.RoleCardActive = !ClientHandler.Instance.RoleCardActive;
//         ClientHandler.Instance.PhoneText.text = CustomPlayer.Local.RoleCardInfo();
//         ClientHandler.Instance.PhoneText.gameObject.SetActive(ClientHandler.Instance.RoleCardActive);
//         ClientHandler.Instance.Phone.gameObject.SetActive(ClientHandler.Instance.RoleCardActive);
//         ClientHandler.Instance.ToTheWiki.gameObject.SetActive(ClientHandler.Instance.RoleCardActive && IsNormal && IsInGame);
//     }

//     public static void Open()
//     {
//         if (!ClientHandler.Instance.Phone)
//         {
//             ClientHandler.Instance.Phone = new GameObject("Phone") { layer = 5 }.AddComponent<SpriteRenderer>();
//             ClientHandler.Instance.Phone.sprite = GetSprite("Phone");
//             ClientHandler.Instance.Phone.transform.SetParent(HUD.transform);
//             ClientHandler.Instance.Phone.transform.localPosition = new(0, 0, -49f);
//             ClientHandler.Instance.Phone.transform.localScale *= 1.25f;
//         }

//         if (IsInGame)
//             OpenRoleCard();
//         else
//             OpenWiki();
//     }

//     public static void OpenWiki()
//     {
//         CloseMenus(SkipEnum.Wiki);

//         if (LocalBlocked)
//             return;

//         if (!ClientHandler.Instance.PagesSet)
//         {
//             var clone = Info.AllInfo.Clone();
//             clone.RemoveAll(x => x.Name is "Invalid" or "None" || x.Type == InfoType.Lore);
//             clone.Reverse();
//             clone = [ .. clone.Distinct() ];
//             var i = 0;
//             var j = 0;
//             var k = 0;

//             foreach (var pair in clone)
//             {
//                 ClientHandler.Instance.Sorted.Add(j, new(pair is SymbolInfo symbol ? symbol.Symbol : pair.Name, pair));
//                 j++;
//                 k++;

//                 if (k >= 28)
//                 {
//                     i++;
//                     k = 0;
//                 }
//             }

//             ClientHandler.Instance.MaxPage = i;
//             ClientHandler.Instance.PagesSet = true;
//         }

//         if (!ClientHandler.Instance.NextButton)
//         {
//             ClientHandler.Instance.NextButton = CreateButton("WikiNextButton", "Next Page", () =>
//             {
//                 ClientHandler.Instance.Page = CycleInt(ClientHandler.Instance.MaxPage, 0, ClientHandler.Instance.Page, true);
//                 ResetButtonPos();
//             });
//         }

//         if (!ClientHandler.Instance.BackButton)
//         {
//             ClientHandler.Instance.BackButton = CreateButton("WikiBack", "Previous Page", () =>
//             {
//                 if (ClientHandler.Instance.Selected == null)
//                     ClientHandler.Instance.Page = CycleInt(ClientHandler.Instance.MaxPage, 0, ClientHandler.Instance.Page, false);
//                 else if (ClientHandler.Instance.LoreActive)
//                 {
//                     ClientHandler.Instance.PhoneText.gameObject.SetActive(false);
//                     AddInfo();
//                     ClientHandler.Instance.LoreActive = false;
//                 }
//                 else
//                 {
//                     ClientHandler.Instance.Selected = null;
//                     ClientHandler.Instance.SelectionActive = false;
//                     ClientHandler.Instance.LoreButton.gameObject.SetActive(false);
//                     ClientHandler.Instance.NextButton.gameObject.SetActive(true);
//                     ClientHandler.Instance.NextButton.transform.localPosition = new(2.5f, 1.6f, 0f);
//                     ClientHandler.Instance.PhoneText.gameObject.SetActive(false);
//                     ClientHandler.Instance.Entry.Clear();
//                 }

//                 ResetButtonPos();
//             });
//         }

//         if (!ClientHandler.Instance.YourStatus)
//         {
//             ClientHandler.Instance.YourStatus = CreateButton("YourStatus", "Your Status", () =>
//             {
//                 OpenWiki();
//                 OpenRoleCard();
//             });
//         }

//         if (!ClientHandler.Instance.LoreButton)
//         {
//             ClientHandler.Instance.LoreButton = CreateButton("WikiLore", "Lore", () =>
//             {
//                 ClientHandler.Instance.LoreActive = !ClientHandler.Instance.LoreActive;
//                 SetEntryText(Info.ColorIt(WrapText(LayerInfo.AllLore.Find(x => x.Name == ClientHandler.Instance.Selected.Name || x.Short == ClientHandler.Instance.Selected.Short)
//                     .Description)));
//                 ClientHandler.Instance.PhoneText.text = ClientHandler.Instance.Entry[0];
//                 ClientHandler.Instance.PhoneText.transform.localPosition = new(-2.6f, 0.45f, -5f);
//                 ClientHandler.Instance.SelectionActive = true;
//             });
//         }

//         if (ClientHandler.Instance.Buttons.Count == 0)
//         {
//             var i = 0;
//             var j = 0;

//             for (var k = 0; k < ClientHandler.Instance.Sorted.Count; k++)
//             {
//                 if (!ClientHandler.Instance.Buttons.ContainsKey(i))
//                     ClientHandler.Instance.Buttons.Add(i, []);

//                 var cache = ClientHandler.Instance.Sorted[k].Value;
//                 var cache2 = ClientHandler.Instance.Sorted[k].Key;
//                 var button = CreateButton($"{cache2}Info", cache2, () =>
//                 {
//                     foreach (var buttons in ClientHandler.Instance.Buttons.Values)
//                     {
//                         if (buttons.Any())
//                             buttons.ForEach(x => x?.gameObject?.SetActive(false));
//                     }

//                     ClientHandler.Instance.Selected = cache;
//                     ClientHandler.Instance.NextButton.gameObject.SetActive(false);
//                     AddInfo();
//                 }, cache.Color);

//                 ClientHandler.Instance.Buttons[i].Add(button);
//                 j++;

//                 if (j >= 28)
//                 {
//                     i++;
//                     j = 0;
//                 }
//             }
//         }

//         ClientHandler.Instance.WikiActive = !ClientHandler.Instance.WikiActive;
//         ClientHandler.Instance.Phone.gameObject.SetActive(ClientHandler.Instance.WikiActive);
//         ClientHandler.Instance.NextButton.gameObject.SetActive(ClientHandler.Instance.WikiActive);
//         ClientHandler.Instance.BackButton.gameObject.SetActive(ClientHandler.Instance.WikiActive);
//         ClientHandler.Instance.YourStatus.gameObject.SetActive(ClientHandler.Instance.WikiActive && IsNormal && IsInGame);
//         ResetButtonPos();
//         ClientHandler.Instance.Selected = null;

//         if (!ClientHandler.Instance.WikiActive && ClientHandler.Instance.PhoneText)
//             ClientHandler.Instance.PhoneText.gameObject.SetActive(false);
//     }

//     public static void ResetButtonPos()
//     {
//         if (ClientHandler.Instance.BackButton)
//             ClientHandler.Instance.BackButton.transform.localPosition = new(-2.6f, 1.6f, 0f);

//         if (ClientHandler.Instance.NextButton)
//             ClientHandler.Instance.NextButton.transform.localPosition = new(2.5f, 1.6f, 0f);

//         if (ClientHandler.Instance.YourStatus)
//             ClientHandler.Instance.YourStatus.transform.localPosition = new(0f, 1.6f, 0f);

//         if (ClientHandler.Instance.ToTheWiki)
//             ClientHandler.Instance.ToTheWiki.transform.localPosition = new(-2.6f, 1.6f, 0f);

//         if (ClientHandler.Instance.Selected != null)
//         {
//             if (LayerInfo.AllLore.Any(x => x.Name == ClientHandler.Instance.Selected.Name || x.Short == ClientHandler.Instance.Selected.Short) && ClientHandler.Instance.LoreButton)
//             {
//                 ClientHandler.Instance.LoreButton.gameObject.SetActive(!ClientHandler.Instance.LoreActive);
//                 ClientHandler.Instance.LoreButton.transform.localPosition = new(0f, -1.7f, 0f);
//             }

//             return;
//         }

//         var m = 0;

//         foreach (var pair in ClientHandler.Instance.Buttons)
//         {
//             foreach (var button in pair.Value)
//             {
//                 if (!button)
//                     continue;

//                 var row = m / 4;
//                 var col = m % 4;
//                 button.transform.localPosition = new(-2.6f + (1.7f * col), 1f - (0.45f * row), -1f);
//                 button.gameObject.SetActive(ClientHandler.Instance.Page == pair.Key && ClientHandler.Instance.WikiActive);
//                 m++;

//                 if (m >= 28)
//                     m -= 28;
//             }
//         }
//     }

//     public static void AddInfo()
//     {
//         if (ClientHandler.Instance.PhoneText)
//             ClientHandler.Instance.PhoneText.gameObject.Destroy();

//         ClientHandler.Instance.Selected.WikiEntry(out var result);
//         SetEntryText(result);
//         ClientHandler.Instance.PhoneText = UObject.Instantiate(HUD.TaskPanel.taskText, ClientHandler.Instance.Phone.transform);
//         ClientHandler.Instance.PhoneText.color = UColor.white;
//         ClientHandler.Instance.PhoneText.text = ClientHandler.Instance.Entry[0];
//         ClientHandler.Instance.PhoneText.enableWordWrapping = false;
//         ClientHandler.Instance.PhoneText.transform.localScale = Vector3.one * 0.75f;
//         ClientHandler.Instance.PhoneText.transform.localPosition = new(-2.6f, 0.45f, -5f);
//         ClientHandler.Instance.PhoneText.alignment = TextAlignmentOptions.TopLeft;
//         ClientHandler.Instance.PhoneText.fontStyle = FontStyles.Bold;
//         ClientHandler.Instance.PhoneText.gameObject.SetActive(true);
//         ClientHandler.Instance.PhoneText.name = "PhoneText";
//         ClientHandler.Instance.SelectionActive = true;
//     }

//     public static PassiveButton CreateButton(string name, string labelText, Action onClick, UColor? textColor = null)
//     {
//         var button = UObject.Instantiate(HUD.MapButton, ClientHandler.Instance.Phone.transform);
//         button.name = $"{name}Button";
//         button.transform.localScale = new(0.5f, 0.5f, 1f);
//         button.GetComponent<BoxCollider2D>().size = new(2.5f, 0.55f);
//         var label = UObject.Instantiate(HUD.TaskPanel.taskText, button.transform);
//         label.color = textColor ?? UColor.white;
//         label.text = labelText;
//         label.enableWordWrapping = false;
//         label.transform.localPosition = new(0f, 0f, label.transform.localPosition.z);
//         label.transform.localScale *= 1.55f;
//         label.alignment = TextAlignmentOptions.Center;
//         label.fontStyle = FontStyles.Bold;
//         label.name = $"{name}Text";
//         var rend = button.GetComponent<SpriteRenderer>();
//         rend.sprite = GetSprite("Plate");
//         rend.color = UColor.white;
//         button.OverrideOnClickListeners(onClick);
//         button.OverrideOnMouseOverListeners(() => rend.color = UColor.yellow);
//         button.OverrideOnMouseOutListeners(() => rend.color = UColor.white);
//         return button;
//     }

//     public static void CloseMenus(SkipEnum skip = SkipEnum.None)
//     {
//         if (ClientHandler.Instance.WikiActive && skip != SkipEnum.Wiki)
//             OpenWiki();

//         if (ClientHandler.Instance.RoleCardActive && skip != SkipEnum.RoleCard)
//             OpenRoleCard();

//         if (ClientHandler.Instance.Zooming && skip != SkipEnum.Zooming)
//             ClickZoom();

//         if (ClientHandler.Instance.SettingsActive && skip != SkipEnum.Settings)
//             OpenSettings();

//         if (MapPatch.MapActive && Map && skip != SkipEnum.Map)
//             Map.Close();

//         if (ActiveTask && ActiveTask && skip != SkipEnum.Task)
//             ActiveTask.Close();

//         if (GameSettingMenu.Instance && skip != SkipEnum.Client)
//             GameSettingMenu.Instance.Close();
//     }

//     public static void SetEntryText(string result)
//     {
//         ClientHandler.Instance.Entry.Clear();
//         ClientHandler.Instance.ResultPage = 0;
//         var texts = result.Split('\n');
//         var pos = 0;
//         var result2 = "";

//         foreach (var text in texts)
//         {
//             result2 += $"{text}\n";
//             pos++;

//             if (pos >= 19)
//             {
//                 ClientHandler.Instance.Entry.Add(result2);
//                 result2 = "";
//                 pos -= 19;
//             }
//         }

//         if (!IsNullEmptyOrWhiteSpace(result2))
//             ClientHandler.Instance.Entry.Add(result2);
//     }
// }