namespace TownOfUsReworked.Patches;

// Thanks to Town Of Host for the Chat History code
[HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
public static class ChatUpdate
{
    private static int CurrentHistorySelection = -1;
    public static TextMeshPro SuggestionText;
    public static readonly List<string> ChatHistory = [];

    public static bool Prefix(ChatController __instance)
    {
        if (!__instance.IsOpenOrOpening)
            return false;

        UpdateHistory(__instance);
        UpdateBubbles(__instance);
        UpdateChatTimer(__instance);

        __instance.freeChatField.background.color = ClientOptions.UseDarkTheme ? new Color32(40, 40, 40, 255) : new Color32(255, 255, 255, 255);
        __instance.quickChatField.background.color = ClientOptions.UseDarkTheme ? new Color32(40, 40, 40, 255) : new Color32(255, 255, 255, 255);

        var text = __instance.freeChatField.Text;

        if (__instance.freeChatField.textArea.hasFocus && text.StartsWith("/") && text != "/")
        {
            var lower = text.ToLower();
            var closestCommand = Find(lower.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[0]);

            if (closestCommand != null)
            {
                SuggestionText.SetText($"/{closestCommand.Aliases.Find(x => lower.StartsWith($"/{x}") && text.Length - 1 <= x.Length)} {closestCommand.Parameters}");

                if (Input.GetKeyDown(KeyCode.Tab))
                    __instance.freeChatField.textArea.SetText(SuggestionText.text.Split(' ')[0]);
            }
            else
                SuggestionText.SetText("");
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
            __instance.sendRateMessageText.text = TranslationController.Instance.GetString(StringNames.ChatRateLimit, Mathf.CeilToInt(GameSettings.ChatCooldown -
                __instance.timeSinceLastMessage));
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
                    if (chat.NameText.text.Contains(player.Data.PlayerName))
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

                        if (GameModifiers.Whispers && !chat.NameText.text.Contains($"[{player.PlayerId}] "))
                            chat.NameText.text = $"[#{player.PlayerId}] " + chat.NameText.text;
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
}

[HarmonyPatch(typeof(FreeChatInputField), nameof(FreeChatInputField.UpdateCharCount))]
public static class OverrideCharCountPatch
{
    public static bool Prefix(FreeChatInputField __instance)
    {
        if (GameSettings.ChatCharacterLimit == 0)
        {
            __instance.charCountText.text = "";
            return false;
        }

        var length = __instance.Text.Length;
        __instance.charCountText.text = $"{length}/{GameSettings.ChatCharacterLimit}";

        if (length <= GameSettings.ChatCharacterLimit / 2)
            __instance.charCountText.color = UColor.black;
        else if (length < GameSettings.ChatCharacterLimit)
            __instance.charCountText.color = UColor.yellow;
        else if (length >= GameSettings.ChatCharacterLimit)
            __instance.charCountText.color = UColor.red;

        return false;
    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
public static class ChatChannels
{
    public static readonly List<byte> Ignored = [];

    public static bool Prefix(ChatController __instance, PlayerControl sourcePlayer, string chatText)
    {
        if (Ignored.Contains(sourcePlayer.PlayerId))
            return false;

        if ((ChatUpdate.ChatHistory.Count == 0 || ChatUpdate.ChatHistory[^1].Contains(chatText)) && !chatText.StartsWith("/"))
            ChatUpdate.ChatHistory.Add($"{sourcePlayer.Data.PlayerName}: {chatText}");

        if (__instance != Chat())
            return true;

        var localPlayer = CustomPlayer.Local;

        if (!localPlayer)
            return true;

        var sourcerole = sourcePlayer.GetRole();
        var shouldSeeMessage = (sourcePlayer.IsOtherLover(localPlayer) && Lovers.LoversChat && sourcerole.CurrentChannel == ChatChannel.Lovers) || (sourcePlayer.IsOtherRival(localPlayer) &&
            Rivals.RivalsChat && sourcerole.CurrentChannel == ChatChannel.Rivals) || (sourcePlayer.IsOtherLink(localPlayer) && Linked.LinkedChat && sourcerole.CurrentChannel ==
            ChatChannel.Linked);

        if (DateTime.UtcNow - MeetingStart.MeetingStartTime < TimeSpan.FromSeconds(1))
            return shouldSeeMessage;

        return (Meeting() || Lobby() || localPlayer.Data.IsDead || sourcePlayer == localPlayer || sourcerole.CurrentChannel == ChatChannel.All || shouldSeeMessage) && !(Meeting() &&
            CustomPlayer.Local.IsSilenced());
    }

    public static void Postfix(ChatController __instance, ref bool __runOriginal)
    {
        if (__runOriginal)
            ChatControllerAwakePatch.SetChatBubble(__instance.GetComponentsInChildren<ChatBubble>().Last());
    }
}

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
public static class MeetingStart
{
    public static DateTime MeetingStartTime = DateTime.MinValue;

    public static void Prefix() => MeetingStartTime = DateTime.UtcNow;
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
public static class ChatCommands
{
    private static readonly Dictionary<byte, SpriteRenderer> Notifs = [];

    public static bool Prefix(ChatController __instance)
    {
        var text = __instance.freeChatField.Text.ToLower();
        var chatHandled = false;

        // Chat command system
        if (text.StartsWith("/"))
        {
            chatHandled = true;
            var args = __instance.freeChatField.Text.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            Execute(Find(args[0].ToLower()), args, __instance.freeChatField.Text);
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
                __instance.sendRateMessageText.text = TranslationController.Instance.GetString(StringNames.ChatRateLimit, Mathf.CeilToInt(GameSettings.ChatCooldown -
                    __instance.timeSinceLastMessage));
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
}

[HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetText))]
public static class ColorTheMessage
{
    public static void Postfix(ChatBubble __instance)
    {
        __instance.TextArea.richText = true;
        __instance.TextArea.text = Modules.Info.ColorIt(__instance.TextArea.text);
    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.Awake))]
public static class ChatControllerAwakePatch
{
    public static void Prefix()
    {
        if (!EOSManager.Instance.isKWSMinor)
            DataManager.Settings.Multiplayer.ChatMode = QuickChatModes.FreeChatOrQuickChat;
    }

    public static void Postfix(ChatController __instance)
    {
        AddAsset("Chat", __instance.messageSound);
        AddAsset("Warning", __instance.warningSound);

        if (!ChatUpdate.SuggestionText)
        {
            var outputText = __instance.freeChatField.textArea.outputText;
            ChatUpdate.SuggestionText = UObject.Instantiate(outputText, outputText.transform.parent);
            ChatUpdate.SuggestionText.transform.localPosition -= new Vector3(0.02f, 0f, 0f);
            ChatUpdate.SuggestionText.transform.SetSiblingIndex(outputText.transform.GetSiblingIndex());
            ChatUpdate.SuggestionText.transform.DestroyChildren();
            ChatUpdate.SuggestionText.name = "SuggestionText";
            ChatUpdate.SuggestionText.color = UColor.white.SetAlpha(0.5f);
        }

        SetTheme(__instance);
    }

    public static void SetTheme(ChatController __instance)
    {
        __instance.freeChatField.background.color = __instance.quickChatField.background.color = ClientOptions.UseDarkTheme ? new Color32(40, 40, 40, 255) : new Color32(255, 255, 255, 255);

        __instance.freeChatField.textArea.compoText.Color(ClientOptions.UseDarkTheme ? UColor.white : UColor.black);
        __instance.freeChatField.textArea.outputText.color = __instance.quickChatField.text.color = ClientOptions.UseDarkTheme ? UColor.white : UColor.black;

        __instance.quickChatButton.transform.Find("QuickChatIcon").GetComponent<SpriteRenderer>().color = __instance.openKeyboardButton.transform.Find("OpenKeyboardIcon")
            .GetComponent<SpriteRenderer>().color = __instance.openKeyboardButton.transform.parent.Find("BanMenuButton").Find("OpenBanMenuIcon").GetComponent<SpriteRenderer>().color =
                ClientOptions.UseDarkTheme ? new(0.5f, 0.5f, 0.5f, 1f) : UColor.white;

        __instance.GetComponentsInChildren<ChatBubble>().ForEach(SetChatBubble);
    }

    public static void SetChatBubble(ChatBubble bubble)
    {
        var theme = ClientOptions.UseDarkTheme ? UColor.white : UColor.black;
        var invert = ClientOptions.UseDarkTheme ? UColor.black : UColor.white;

        bubble.TextArea.color = bubble.TextArea.text.ContainsAny($"#{CustomPlayer.Local.PlayerId}", $"#({CustomPlayer.Local.Data.PlayerName})") ? invert : theme;
        bubble.Background.color = (bubble.TextArea.text.ContainsAny($"#{CustomPlayer.Local.PlayerId}", $"#({CustomPlayer.Local.Data.PlayerName})") ? theme : invert)
            .SetAlpha(bubble.Xmark.enabled ? 0.5f : 1f);
    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.Toggle))]
public static class ChatFontPatch
{
    public static void Postfix(ChatController __instance) => ChatControllerAwakePatch.SetTheme(__instance);
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
        __instance.playerColorText.text = __instance.player.ColorBlindName;
        __instance.playerNameText.text = sender.Data.PlayerName.IsNullOrWhiteSpace() ? "..." : sender.Data.PlayerName;
        __instance.playerNameText.color = Palette.TextColors[__instance.player.ColorId];
        __instance.playerNameText.outlineColor = Palette.TextOutlineColors[__instance.player.ColorId];
        __instance.chatText.richText = true;
        __instance.chatText.text = text;
        return false;
    }
}