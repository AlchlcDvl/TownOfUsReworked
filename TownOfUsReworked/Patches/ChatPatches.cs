namespace TownOfUsReworked.Patches;

// Thanks to Town Of Host for the Chat History code
[HarmonyPatch(typeof(ChatController))]
public static class ChatPatches
{
    private static int CurrentHistorySelection = -1;
    public static TextMeshPro SuggestionText;
    public static readonly List<string> ChatHistory = [];

    [HarmonyPatch(nameof(ChatController.Update)), HarmonyPrefix]
    public static bool UpdatePrefix(ChatController __instance)
    {
        if (!__instance.IsOpenOrOpening)
            return false;

        UpdateHistory(__instance);
        UpdateBubbles(__instance);
        UpdateChatTimer(__instance);

        __instance.freeChatField.background.color = ClientOptions.UseDarkTheme ? new Color32(40, 40, 40, 255) : UColor.white;
        __instance.quickChatField.background.color = ClientOptions.UseDarkTheme ? new Color32(40, 40, 40, 255) : UColor.white;

        var text = __instance.freeChatField.Text;

        if (__instance.freeChatField.textArea.hasFocus && text.StartsWith("/") && text != "/")
        {
            var lower = text.ToLower();
            var split = lower.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var first = split[0][1..];
            var closestCommand = Find(first);

            if (closestCommand != null)
            {
                var parts = split[1..];
                var result = "";

                for (var i = 0; i < parts.Length; i++)
                    result += $"{parts[i]} ";

                if (result.Length > 0)
                    result = $"/{first} {result.Trim()} {closestCommand.ConstructParameters(split)}";
                else
                    result = $"/{closestCommand.FindAlias(first)} {closestCommand.ConstructParameters(split)}";

                SuggestionText.SetText(result);

                if (Input.GetKeyDown(KeyCode.Tab))
                    __instance.freeChatField.textArea.SetText(result.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[0]);
            }
            else
                SuggestionText.SetText($"{text} UNKNOWN COMMAND");
        }
        else
            SuggestionText.SetText("");

        return false;
    }

    private static void UpdateChatTimer(ChatController __instance)
    {
        __instance.freeChatField.textArea.characterLimit = GameSettings.ChatCharacterLimit == 0 ? 1000000000 : GameSettings.ChatCharacterLimit;
        __instance.timeSinceLastMessage += Time.deltaTime;

        if (!__instance.sendRateMessageText.isActiveAndEnabled)
            return;

        if (__instance.timeSinceLastMessage >= GameSettings.ChatCooldown)
            __instance.sendRateMessageText.gameObject.SetActive(false);
        else
        {
            __instance.sendRateMessageText.SetText(TranslationController.Instance.GetString(StringNames.ChatRateLimit, Mathf.CeilToInt(GameSettings.ChatCooldown -
                __instance.timeSinceLastMessage)));
        }
    }

    private static void UpdateBubbles(ChatController __instance)
    {
        foreach (var bubble in __instance.chatBubblePool.activeChildren)
        {
            var chat = bubble.Cast<ChatBubble>();

            if (chat.NameText && IsInGame())
            {
                foreach (var player in AllPlayers())
                {
                    if (chat.NameText.text.Contains(player.name))
                    {
                        var role = player.GetRole();

                        if (role)
                        {
                            if ((((CustomPlayer.Local.GetFaction() == player.GetFaction() && player.GetFaction() is not (Faction.Crew or Faction.Neutral)) || (!player.Is(SubFaction.None) &&
                                CustomPlayer.Local.GetSubFaction() == player.GetSubFaction())) && GameModifiers.FactionSeeRoles) || player.AmOwner)
                            {
                                chat.NameText.color = role.Color;
                            }
                            else if (CustomPlayer.Local.GetFaction() == player.GetFaction() && player.GetFaction() is not (Faction.Crew or Faction.Neutral) && !GameModifiers.FactionSeeRoles)
                                chat.NameText.color = role.FactionColor;
                            else if (CustomPlayer.Local.GetSubFaction() == player.GetSubFaction() && !player.Is(SubFaction.None) && !GameModifiers.FactionSeeRoles)
                                chat.NameText.color = role.SubFactionColor;
                            else
                                chat.NameText.color = UColor.white;
                        }

                        if (GameModifiers.Whispers && !chat.NameText.text.StartsWith($"[#{player.PlayerId}]"))
                            chat.NameText.SetText($"[#{player.PlayerId}] {chat.NameText.text}");
                    }
                }
            }
        }
    }

    private static void UpdateHistory(ChatController __instance)
    {
        if (!__instance.freeChatField.textArea.hasFocus)
            return;

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.C))
            ClipboardHelper.PutClipboardString(__instance.freeChatField.textArea.text);

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.V))
            __instance.freeChatField.textArea.SetText(__instance.freeChatField.textArea.text + GUIUtility.systemCopyBuffer);

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.X))
        {
            ClipboardHelper.PutClipboardString(__instance.freeChatField.textArea.text);
            __instance.freeChatField.textArea.SetText("");
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && ChatHistory.Any())
        {
            CurrentHistorySelection--;

            if (CurrentHistorySelection >= 0)
                __instance.freeChatField.textArea.SetText(ChatHistory[CurrentHistorySelection]);
            else
            {
                __instance.freeChatField.textArea.SetText("");
                CurrentHistorySelection = ChatHistory.Count - 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && ChatHistory.Any())
        {
            CurrentHistorySelection++;

            if (CurrentHistorySelection < ChatHistory.Count)
                __instance.freeChatField.textArea.SetText(ChatHistory[CurrentHistorySelection]);
            else if (CurrentHistorySelection == ChatHistory.Count)
            {
                __instance.freeChatField.textArea.SetText("");
                CurrentHistorySelection = 0;
            }
        }
    }

    public static readonly List<byte> Ignored = [];

    [HarmonyPatch(nameof(ChatController.AddChat)), HarmonyPrefix]
    public static bool AddChatPrefix(ChatController __instance, PlayerControl sourcePlayer, string chatText)
    {
        if (Ignored.Contains(sourcePlayer.PlayerId))
            return false;

        if ((ChatHistory.Count == 0 || ChatHistory[^1].Contains(chatText)) && !chatText.StartsWith("/"))
            ChatHistory.Add($"{sourcePlayer.name}: {chatText}");

        if (__instance != Chat())
            return true;

        var localPlayer = CustomPlayer.Local;

        if (!localPlayer)
            return true;

        var sourcerole = sourcePlayer.GetRole();
        var shouldSeeMessage = (sourcePlayer.IsOtherLover(localPlayer) && Lovers.LoversChat && sourcerole.CurrentChannel == ChatChannel.Lovers) || (sourcePlayer.IsOtherRival(localPlayer) &&
            Rivals.RivalsChat && sourcerole.CurrentChannel == ChatChannel.Rivals) || (sourcePlayer.IsOtherLink(localPlayer) && Linked.LinkedChat && sourcerole.CurrentChannel ==
            ChatChannel.Linked);

        if (Time.time - MeetingPatches.MeetingStartTime < 1f)
            return shouldSeeMessage;

        return (Meeting() || Lobby() || localPlayer.Data.IsDead || sourcePlayer == localPlayer || sourcerole.CurrentChannel == ChatChannel.All || shouldSeeMessage) && !(Meeting() &&
            CustomPlayer.Local.IsSilenced());
    }

    private static readonly Dictionary<byte, SpriteRenderer> Notifs = [];

    [HarmonyPatch(nameof(ChatController.SendChat)), HarmonyPrefix]
    public static bool SendChatPrefix(ChatController __instance)
    {
        var text = __instance.freeChatField.Text.ToLower();
        var chatHandled = false;

        // Chat command system
        if (text.StartsWith("/"))
        {
            chatHandled = true;
            var args = __instance.freeChatField.Text.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            Execute(args, __instance.freeChatField.Text);
        }
        else if (CustomPlayer.Local.IsBlackmailed() && text != "i am blackmailed.")
        {
            chatHandled = true;
            Run("<#02A752FF>米 Shhhh 米</color>", "You are blackmailed.");
        }
        else if (CustomPlayer.Local.SilenceActive() && text != "i am silenced.")
        {
            chatHandled = true;
            Run("<#AAB43EFF>米 Shhhh 米</color>", "You are silenced.");
        }
        else if (MeetingPatches.GivingAnnouncements && !CustomPlayer.LocalCustom.Dead)
        {
            chatHandled = true;
            Run("<#00CB97FF>米 Shhhh 米</color>", "You cannot talk right now.");
        }
        else if (!CustomPlayer.LocalCustom.Dead && !IsNullEmptyOrWhiteSpace(text))
        {
            Notify(CustomPlayer.Local.PlayerId);
            CallRpc(CustomRPC.Misc, MiscRPC.Notify, CustomPlayer.Local.PlayerId);
        }

        if (!chatHandled)
        {
            if (GameSettings.ChatCooldown > __instance.timeSinceLastMessage)
            {
                __instance.sendRateMessageText.gameObject.SetActive(true);
                __instance.sendRateMessageText.SetText(TranslationController.Instance.GetString(StringNames.ChatRateLimit, Mathf.CeilToInt(GameSettings.ChatCooldown -
                    __instance.timeSinceLastMessage)));
            }
            else if (!IsNullEmptyOrWhiteSpace(text))
            {
                if (__instance.quickChatMenu.CanSend)
                    __instance.SendQuickChat();
                else
                {
                    if (IsNullEmptyOrWhiteSpace(__instance.freeChatField.Text) || DataManager.Settings.Multiplayer.ChatMode != QuickChatModes.FreeChatOrQuickChat)
                        return false;

                    __instance.SendFreeChat();
                }

                chatHandled = true;
            }
        }

        if (chatHandled)
        {
            __instance.freeChatField.Clear();
            __instance.quickChatMenu.Clear();
            __instance.quickChatField.Clear();
            __instance.UpdateChatMode();
        }

        return false;
    }

    public static void Notify(byte targetPlayerId)
    {
        if (!Meeting() || Notifs.ContainsKey(targetPlayerId))
            return;

        var playerVoteArea = VoteAreaById(targetPlayerId);
        var chat = UObject.Instantiate(playerVoteArea.Megaphone, playerVoteArea.transform.parent);
        chat.name = $"Notification{targetPlayerId}";
        chat.transform.localPosition = new(-1.3f, 0.1f, -1f);
        chat.sprite = GetSprite("Chat");
        chat.gameObject.SetActive(true);
        Notifs.Add(targetPlayerId, chat);
        Coroutines.Start(PerformTimedAction(2, p =>
        {
            if (p == 1)
            {
                chat.gameObject.SetActive(false);
                chat.gameObject.Destroy();
                Notifs.Remove(targetPlayerId);
            }
        }));
    }

    [HarmonyPatch(nameof(ChatController.Awake))]
    public static void Prefix()
    {
        if (!EOSManager.Instance.isKWSMinor)
            DataManager.Settings.Multiplayer.ChatMode = QuickChatModes.FreeChatOrQuickChat;
    }

    [HarmonyPatch(nameof(ChatController.Awake)), HarmonyPostfix]
    public static void AwakePostfix(ChatController __instance)
    {
        AddAsset("Chat", __instance.messageSound);
        AddAsset("Warning", __instance.warningSound);

        if (!SuggestionText)
        {
            var outputText = __instance.freeChatField.textArea.outputText;
            SuggestionText = UObject.Instantiate(outputText, outputText.transform.parent);
            SuggestionText.transform.localPosition -= new Vector3(0.02f, 0f, 0f);
            SuggestionText.transform.SetSiblingIndex(outputText.transform.GetSiblingIndex());
            SuggestionText.transform.DestroyChildren();
            SuggestionText.name = "SuggestionText";
            SuggestionText.color = UColor.white.SetAlpha(0.5f);
        }

        SetTheme(__instance);
    }

    public static void SetTheme(ChatController __instance)
    {
        __instance.freeChatField.background.color = __instance.quickChatField.background.color = ClientOptions.UseDarkTheme ? new Color32(40, 40, 40, 255) : UColor.white;

        __instance.freeChatField.textArea.compoText.Color(ClientOptions.UseDarkTheme ? UColor.white : UColor.black);
        __instance.freeChatField.textArea.outputText.color = __instance.quickChatField.text.color = ClientOptions.UseDarkTheme ? UColor.white : UColor.black;

        __instance.quickChatButton.transform.Find("QuickChatIcon").GetComponent<SpriteRenderer>().color = __instance.openKeyboardButton.transform.Find("OpenKeyboardIcon")
            .GetComponent<SpriteRenderer>().color = __instance.openKeyboardButton.transform.parent.FindRecursive("OpenBanMenuIcon").GetComponent<SpriteRenderer>().color =
                ClientOptions.UseDarkTheme ? new(0.5f, 0.5f, 0.5f, 1f) : UColor.white;

        __instance.GetComponentsInChildren<ChatBubble>().ForEach(SetChatBubble);
    }

    public static void SetChatBubble(ChatBubble bubble)
    {
        var theme = ClientOptions.UseDarkTheme ? UColor.white : UColor.black;
        var invert = ClientOptions.UseDarkTheme ? UColor.black : UColor.white;

        bubble.TextArea.color = bubble.TextArea.text.ContainsAny($"#{CustomPlayer.Local.PlayerId}", $"#{CustomPlayer.Local.name}", $"#({CustomPlayer.Local.name})") ? invert : theme;
        bubble.Background.color = (bubble.TextArea.text.ContainsAny($"#{CustomPlayer.Local.PlayerId}", $"#{CustomPlayer.Local.name}", $"#({CustomPlayer.Local.name})") ? theme : invert)
            .SetAlpha(bubble.Xmark.enabled ? 0.5f : 1f);
    }

    [HarmonyPatch(nameof(ChatController.Toggle)), HarmonyPostfix]
    public static void TogglePostfix(ChatController __instance) => SetTheme(__instance);
}

[HarmonyPatch(typeof(FreeChatInputField), nameof(FreeChatInputField.UpdateCharCount))]
public static class OverrideCharCountPatch
{
    public static bool Prefix(FreeChatInputField __instance)
    {
        if (GameSettings.ChatCharacterLimit == 0)
        {
            __instance.charCountText.SetText("");
            return false;
        }

        var length = __instance.Text.Length;
        __instance.charCountText.SetText($"{length}/{GameSettings.ChatCharacterLimit}");

        if (length <= GameSettings.ChatCharacterLimit / 2)
            __instance.charCountText.color = UColor.black;
        else if (length < GameSettings.ChatCharacterLimit)
            __instance.charCountText.color = UColor.yellow;
        else if (length >= GameSettings.ChatCharacterLimit)
            __instance.charCountText.color = UColor.red;

        return false;
    }
}

[HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetText))]
public static class ChatBubblePatch
{
    public static void Postfix(ChatBubble __instance)
    {
        __instance.TextArea.richText = true;
        ChatPatches.SetChatBubble(__instance);
    }
}

[HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.JoinGame))]
public static class InnerNetClientJoinPatch
{
    public static void Prefix() => DataManager.Settings.Multiplayer.ChatMode = QuickChatModes.FreeChatOrQuickChat;
}

[HarmonyPatch(typeof(ChatNotification), nameof(ChatNotification.SetUp))]
public static class ChatNotifFixPatch
{
    public static bool Prefix(ChatNotification __instance, PlayerControl sender, string text)
    {
        __instance.timeOnScreen = 4f;
        __instance.gameObject.SetActive(true);
        __instance.SetCosmetics(sender.Data);
        __instance.playerColorText.SetText(__instance.player.ColorBlindName);
        __instance.playerNameText.SetText(sender.name.IsNullOrWhiteSpace() ? "..." : sender.name);
        __instance.playerNameText.color = __instance.player.ColorId.GetColor(false);
        __instance.playerNameText.outlineColor = __instance.player.ColorId.GetColor(true);
        __instance.chatText.richText = true;
        __instance.chatText.SetText(text);
        return false;
    }
}