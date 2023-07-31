namespace TownOfUsReworked.Patches
{
    //Thanks to Town Of Host for the Chat History code
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    public static class ChatUpdate
    {
        private static int CurrentHistorySelection = -1;
        public static readonly List<string> ChatHistory = new();

        public static bool Prefix(ChatController __instance)
        {
            UpdateHistory(__instance);
            UpdateBubbles(__instance);
            UpdateAlignment(__instance);
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

        private static void UpdateAlignment(ChatController __instance)
        {
            foreach (var bubble in __instance.chatBubblePool.activeChildren)
            {
                var chat = bubble.Cast<ChatBubble>();

                if (chat.NameText != null)
                {
                    chat.NameText.alignment = TextAlignmentOptions.Left;
                    chat.TextArea.alignment = TextAlignmentOptions.TopLeft;
                }
            }
        }

        private static void UpdateBubbles(ChatController __instance)
        {
            foreach (var bubble in __instance.chatBubblePool.activeChildren)
            {
                var chat = bubble.Cast<ChatBubble>();

                if (chat.NameText != null)
                {
                    if (ChatCommand.BubbleModifications.TryGetValue(chat, out var pair))
                        (chat.NameText.text, chat.NameText.color) = (pair.Item1, pair.Item2);
                    else if (IsInGame)
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

                if (chat.TextArea != null && !chat.TextArea.richText)
                {
                    chat.TextArea.richText = true;
                    chat.TextArea.text = Info.ColorIt(chat.TextArea.text);
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

    [HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
    public static class OverrideSendChatPatch
    {
        public static bool Prefix(ChatController __instance)
        {
            if (__instance.freeChatField.Text.Length == 0 && __instance.quickChatField.text.text.Length == 0)
                return false;

            var f = CustomGameOptions.ChatCooldown - __instance.timeSinceLastMessage;

            if (f > 0 && CustomGameOptions.ChatCooldown > 0)
            {
                __instance.sendRateMessageText.gameObject.SetActive(true);
                __instance.sendRateMessageText.text = TranslationController.Instance.GetString(StringNames.ChatRateLimit, Mathf.CeilToInt(f));
            }
            else
            {
                if (__instance.quickChatMenu.CanSend)
                    __instance.SendQuickChat();
                else
                {
                    if (string.IsNullOrWhiteSpace(__instance.freeChatField.Text) || DataManager.Settings.Multiplayer.ChatMode != QuickChatModes.FreeChatOrQuickChat)
                        return false;

                    __instance.SendFreeChat();
                }

                __instance.freeChatField.Clear();
                __instance.quickChatMenu.Clear();
                __instance.quickChatField.Clear();
                __instance.UpdateChatMode();
            }

            return false;
        }
    }
}