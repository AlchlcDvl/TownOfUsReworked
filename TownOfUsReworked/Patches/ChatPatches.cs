namespace TownOfUsReworked.Patches;

// Thanks to Town Of Host for the Chat History code
[HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
public static class ChatUpdate
{
    private static int CurrentHistorySelection = -1;
    public static readonly List<string> ChatHistory = [];

    public static bool Prefix(ChatController __instance)
    {
        UpdateHistory(__instance);
        UpdateBubbles(__instance);
        UpdateChatTimer(__instance);
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
                                CustomPlayer.Local.GetSubFaction() == player.GetSubFaction())) && GameModifiers.FactionSeeRoles) || player == CustomPlayer.Local)
                            {
                                chat.NameText.color = role.Color;
                            }
                            else if (CustomPlayer.Local.GetFaction() == player.GetFaction() && player.GetFaction() is not (Faction.Crew or Faction.Neutral) &&
                                !GameModifiers.FactionSeeRoles)
                            {
                                chat.NameText.color = role.FactionColor;
                            }
                            else if (CustomPlayer.Local.GetSubFaction() == player.GetSubFaction() && !player.Is(SubFaction.None) && !GameModifiers.FactionSeeRoles)
                                chat.NameText.color = role.SubFactionColor;
                            else
                                chat.NameText.color = UColor.white;
                        }

                        if (GameModifiers.Whispers && !chat.NameText.text.Contains($"[{player.PlayerId}] "))
                            chat.NameText.text = $"[{player.PlayerId}] " + chat.NameText.text;
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
    public static bool Prefix(ChatController __instance, ref PlayerControl sourcePlayer, ref string chatText)
    {
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

        return (Meeting() || Lobby() || localPlayer.Data.IsDead || sourcePlayer == localPlayer || sourcerole.CurrentChannel == ChatChannel.All || shouldSeeMessage) && !(Meeting()
            && CustomPlayer.Local.IsSilenced());
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
            var args = __instance.freeChatField.Text.Split(' ');
            Execute(Find(args), args, __instance.freeChatField.Text);
        }
        else if (CustomPlayer.Local.IsBlackmailed() && text != "i am blackmailed.")
        {
            chatHandled = true;
            Run("<color=#02A752FF>米 Shhhh 米</color>", "You are blackmailed.");
        }
        else if (CustomPlayer.Local.SilenceActive() && text != "i am silenced.")
        {
            chatHandled = true;
            Run("<color=#AAB43EFF>米 Shhhh 米</color>", "You are silenced.");
        }
        else if (MeetingPatches.GivingAnnouncements && !CustomPlayer.LocalCustom.Dead)
        {
            chatHandled = true;
            Run("<color=#00CB97FF>米 Shhhh 米</color>", "You cannot talk right now.");
        }
        else if (!CustomPlayer.LocalCustom.Dead && !IsNullEmptyOrWhiteSpace(text))
        {
            Notify(CustomPlayer.Local.PlayerId);
            CallRpc(CustomRPC.Misc, MiscRPC.Notify, CustomPlayer.Local.PlayerId);
        }

        if (chatHandled)
            Clear(__instance);
        else
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

                Clear(__instance);
            }
        }

        return false;
    }

    private static void Clear(ChatController __instance)
    {
        __instance.freeChatField.Clear();
        __instance.quickChatMenu.Clear();
        __instance.quickChatField.Clear();
        __instance.UpdateChatMode();
    }

    public static void Notify(byte targetPlayerId)
    {
        if (!Meeting() || Notifs.ContainsKey(targetPlayerId))
            return;

        var playerVoteArea = VoteAreaById(targetPlayerId);
        var chat = UObject.Instantiate(playerVoteArea.Megaphone, playerVoteArea.transform.parent);
        chat.name = $"Notification{targetPlayerId}";
        chat.transform.localPosition = new(-1.3f, 0.1f, -1f);
        chat.sprite = GetSprite("Chat()");
        chat.gameObject.SetActive(true);
        Notifs.Add(targetPlayerId, chat);
        Coroutines.Start(PerformTimedAction(2, p =>
        {
            if (p == 1)
            {
                chat.gameObject.SetActive(false);
                chat.gameObject.Destroy();
                chat.Destroy();
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
        __instance.TextArea.text = Info.ColorIt(__instance.TextArea.text);
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

    public static void Postfix(ChatController __instance) => AddAsset("Chat()", __instance.messageSound);
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.Toggle))]
public static class ChatFontPatch
{
    public static void Postfix(ChatController __instance)
    {
        AddAsset("ChatFont", __instance.scroller.transform.GetChild(1).GetChild(5).GetComponent<TextMeshPro>().font);
        __instance.freeChatField.textArea.GetComponent<TextMeshPro>().font = GetFont("ChatFont");
    }
}

[HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.JoinGame))]
public static class InnerNetClientJoinPatch
{
    public static void Prefix() => DataManager.Settings.Multiplayer.ChatMode = QuickChatModes.FreeChatOrQuickChat;
}