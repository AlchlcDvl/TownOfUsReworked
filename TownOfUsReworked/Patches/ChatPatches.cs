namespace TownOfUsReworked.Patches;

//Thanks to Town Of Host for the Chat History code
[HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
public static class ChatUpdate
{
    private static int CurrentHistorySelection = -1;
    public static readonly List<string> ChatHistory = new();

    public static bool Prefix(ChatController __instance)
    {
        if (!SoundEffects.ContainsKey("Chat"))
            SoundEffects.Add("Chat", __instance.messageSound);

        UpdateHistory(__instance);
        UpdateBubbles(__instance);
        UpdateChatTimer(__instance);
        return false;
    }

    private static void UpdateChatTimer(ChatController __instance)
    {
        __instance.freeChatField.textArea.characterLimit = CustomGameOptions.ChatCharacterLimit;
        __instance.timeSinceLastMessage += Time.deltaTime;

        if (!__instance.sendRateMessageText.isActiveAndEnabled)
            return;

        var f = CustomGameOptions.ChatCooldown - __instance.timeSinceLastMessage;

        if (f <= 0 || CustomGameOptions.ChatCooldown == 0)
            __instance.sendRateMessageText.gameObject.SetActive(false);
        else
            __instance.sendRateMessageText.text = TranslationController.Instance.GetString(StringNames.ChatRateLimit, Mathf.CeilToInt(f));
    }

    private static void UpdateBubbles(ChatController __instance)
    {
        foreach (var bubble in __instance.chatBubblePool.activeChildren)
        {
            var chat = bubble.Cast<ChatBubble>();

            if (chat.NameText != null)
            {
                if (IsInGame)
                {
                    foreach (var player in CustomPlayer.AllPlayers)
                    {
                        if (chat.NameText.text.Contains(player.Data.PlayerName))
                        {
                            var role = Role.GetRole(player);

                            if (role != null)
                            {
                                if ((((CustomPlayer.Local.GetFaction() == player.GetFaction() && !player.Is(Faction.Crew) && !player.Is(Faction.Neutral)) ||
                                    (CustomPlayer.Local.GetSubFaction() == player.GetSubFaction() && !player.Is(SubFaction.None))) && CustomGameOptions.FactionSeeRoles) || player ==
                                    CustomPlayer.Local)
                                {
                                    chat.NameText.color = role.Color;
                                }
                                else if (CustomPlayer.Local.GetFaction() == player.GetFaction() && !player.Is(Faction.Crew) && !player.Is(Faction.Neutral) &&
                                    !CustomGameOptions.FactionSeeRoles)
                                {
                                    chat.NameText.color = role.FactionColor;
                                }
                                else if (CustomPlayer.Local.GetSubFaction() == player.GetSubFaction() && !player.Is(SubFaction.None) && !CustomGameOptions.FactionSeeRoles)
                                    chat.NameText.color = role.SubFactionColor;
                                else
                                    chat.NameText.color = UColor.white;
                            }

                            if (CustomGameOptions.Whispers && !chat.NameText.text.Contains($"[{player.PlayerId}] "))
                                chat.NameText.text = $"[{player.PlayerId}] " + chat.NameText.text;
                        }
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

        if (Input.GetKeyDown(KeyCode.UpArrow) && ChatHistory.Count > 0)
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

        if (Input.GetKeyDown(KeyCode.DownArrow) && ChatHistory.Count > 0)
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
        var length = __instance.Text.Length;
        __instance.charCountText.text = $"{length}/{CustomGameOptions.ChatCharacterLimit}";

        if (length <= CustomGameOptions.ChatCharacterLimit / 2)
            __instance.charCountText.color = UColor.black;
        else if (length < CustomGameOptions.ChatCharacterLimit)
            __instance.charCountText.color = UColor.yellow;
        else if (length >= CustomGameOptions.ChatCharacterLimit)
            __instance.charCountText.color = UColor.red;

        return false;
    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
public static class ChatChannels
{
    public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer)
    {
        if (__instance != HUD.Chat)
            return true;

        var localPlayer = CustomPlayer.Local;

        if (localPlayer == null)
            return true;

        var sourcerole = Role.GetRole(sourcePlayer);
        var shouldSeeMessage = (sourcePlayer.IsOtherLover(localPlayer) && CustomGameOptions.LoversChat && sourcerole.CurrentChannel == ChatChannel.Lovers) ||
            (sourcePlayer.IsOtherRival(localPlayer) && CustomGameOptions.RivalsChat && sourcerole.CurrentChannel == ChatChannel.Rivals) || (sourcePlayer.IsOtherLink(localPlayer) &&
            CustomGameOptions.LinkedChat && sourcerole.CurrentChannel == ChatChannel.Linked);

        if (DateTime.UtcNow - MeetingStart.MeetingStartTime < TimeSpan.FromSeconds(1))
            return shouldSeeMessage;

        return (Meeting || LobbyBehaviour.Instance || localPlayer.Data.IsDead || sourcePlayer == localPlayer || sourcerole.CurrentChannel == ChatChannel.All || shouldSeeMessage) && !(Meeting
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
    private static SpriteRenderer Chat;

    public static bool Prefix(ChatController __instance)
    {
        var text = __instance.freeChatField.Text.ToLower();
        var chatHandled = false;

        if ((ChatUpdate.ChatHistory.Count == 0 || ChatUpdate.ChatHistory[^1] != text) && !text.StartsWith("/"))
            ChatUpdate.ChatHistory.Add(text);

        //Chat command system
        if (text.StartsWith("/"))
        {
            chatHandled = true;
            var args = __instance.freeChatField.Text.Split(' ');
            Execute(Find(args), __instance, args);
        }
        else if (CustomPlayer.Local.IsBlackmailed() && text != "i am blackmailed.")
        {
            chatHandled = true;
            Run(__instance, "<color=#02A752FF>米 Shhhh 米</color>", "You are blackmailed.");
        }
        else if (!CustomPlayer.Local.IsSilenced() && text != "i am silenced." && CustomPlayer.AllPlayers.Any(x => x.IsSilenced() && x.GetSilencer().HoldsDrive))
        {
            chatHandled = true;
            Run(__instance, "<color=#AAB43EFF>米 Shhhh 米</color>", "You are silenced.");
        }
        else if (!CustomPlayer.Local.IsSilenced() && text != "i am silenced." && CustomPlayer.AllPlayers.Any(x => x.IsSilenced() && x.GetRebSilencer().HoldsDrive))
        {
            chatHandled = true;
            Run(__instance, "<color=#AAB43EFF>米 Shhhh 米</color>", "You are silenced.");
        }
        else if (MeetingPatches.GivingAnnouncements && !CustomPlayer.LocalCustom.IsDead)
        {
            chatHandled = true;
            Run(__instance, "<color=#00CB97FF>米 Shhhh 米</color>", "You cannot talk right now.");
        }
        else if (!CustomPlayer.LocalCustom.IsDead && !IsNullEmptyOrWhiteSpace(text))
        {
            Notify(CustomPlayer.Local.PlayerId);
            CallRpc(CustomRPC.Misc, MiscRPC.Notify, CustomPlayer.Local.PlayerId);
        }

        if (chatHandled)
            Clear(__instance);
        else
        {
            var f = CustomGameOptions.ChatCooldown - __instance.timeSinceLastMessage;

            if (f > 0 && CustomGameOptions.ChatCooldown > 0)
            {
                __instance.sendRateMessageText.gameObject.SetActive(true);
                __instance.sendRateMessageText.text = TranslationController.Instance.GetString(StringNames.ChatRateLimit, Mathf.CeilToInt(f));
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
        if (!Meeting || Chat)
            return;

        var playerVoteArea = VoteAreaById(targetPlayerId);
        Chat = UObject.Instantiate(playerVoteArea.Megaphone, playerVoteArea.Megaphone.transform.parent);
        Chat.name = "Notification";
        Chat.transform.localPosition = new(-2f, 0.1f, -1f);
        Chat.sprite = GetSprite("Chat");
        Chat.gameObject.SetActive(true);
        HUD.StartCoroutine(Effects.Lerp(2, new Action<float>(p =>
        {
            if (p == 1)
            {
                Chat.gameObject.SetActive(false);
                Chat.gameObject.Destroy();
                Chat.Destroy();
                Chat = null;
            }
        })));
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
}