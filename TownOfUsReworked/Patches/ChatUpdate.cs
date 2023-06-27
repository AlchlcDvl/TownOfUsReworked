namespace TownOfUsReworked.Patches
{
    //Thanks to Town Of Host for the Chat History code
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    public static class ChatUpdate
    {
        private static int CurrentHistorySelection = -1;
        public readonly static List<string> ChatHistory = new();

        public static bool Prefix(ChatController __instance)
        {
            UpdateHistory(__instance);
            UpdateBubbles(__instance);
            UpdateAlignment(__instance);
            UpdateChatTimer(__instance);
            UpdateCharCounter(__instance);
            return false;
        }

        private static void UpdateCharCounter(ChatController __instance)
        {
            var length = __instance.TextArea.text.Length;
            __instance.CharCount.text = $"{length}/{CustomGameOptions.ChatCharacterLimit}";

            if (length <= CustomGameOptions.ChatCharacterLimit / 2)
                __instance.CharCount.color = UColor.black;
            else if (length < CustomGameOptions.ChatCharacterLimit)
                __instance.CharCount.color = UColor.yellow;
            else if (length >= CustomGameOptions.ChatCharacterLimit)
                __instance.CharCount.color = UColor.red;
        }

        private static void UpdateChatTimer(ChatController __instance)
        {
            var flag = DataManager.Settings.Multiplayer.ChatMode != QuickChatModes.QuickChatOnly;
            __instance.OpenKeyboardButton.SetActive(flag);
            __instance.TextArea.enabled = flag;
            __instance.TextArea.characterLimit = CustomGameOptions.ChatCharacterLimit;
            __instance.TimeSinceLastMessage += Time.deltaTime;

            if (!__instance.SendRateMessage.isActiveAndEnabled)
                return;

            var f = CustomGameOptions.ChatCooldown - __instance.TimeSinceLastMessage;

            if (f <= 0 || CustomGameOptions.ChatCooldown == 0)
                __instance.SendRateMessage.gameObject.SetActive(false);
            else
                __instance.SendRateMessage.text = TranslationController.Instance.GetString(StringNames.ChatRateLimit, Mathf.CeilToInt(f));
        }

        private static void UpdateAlignment(ChatController __instance)
        {
            foreach (var bubble in __instance.chatBubPool.activeChildren)
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
            if (ConstantVariables.IsLobby)
                return;

            foreach (var bubble in __instance.chatBubPool.activeChildren)
            {
                var chat = bubble.Cast<ChatBubble>();

                if (chat.NameText != null)
                {
                    if (ChatCommand.BubbleModifications.ContainsKey(chat.TextArea.text))
                        (chat.NameText.text, chat.NameText.color) = ChatCommand.BubbleModifications[chat.TextArea.text];
                    else
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
                                        chat.NameText.color = Color.white;
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
            if (!__instance.TextArea.hasFocus)
                return;

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.C))
                ClipboardHelper.PutClipboardString(__instance.TextArea.text);

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.V))
                __instance.TextArea.SetText(__instance.TextArea.text + GUIUtility.systemCopyBuffer);

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.X))
            {
                ClipboardHelper.PutClipboardString(__instance.TextArea.text);
                __instance.TextArea.SetText("");
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) && ChatHistory.Count > 0)
            {
                CurrentHistorySelection--;

                if (CurrentHistorySelection >= 0)
                    __instance.TextArea.SetText(ChatHistory[CurrentHistorySelection]);
                else
                {
                    __instance.TextArea.SetText("");
                    CurrentHistorySelection = ChatHistory.Count - 1;
                }
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) && ChatHistory.Count > 0)
            {
                CurrentHistorySelection++;

                if (CurrentHistorySelection < ChatHistory.Count)
                    __instance.TextArea.SetText(ChatHistory[CurrentHistorySelection]);
                else if (CurrentHistorySelection == ChatHistory.Count)
                {
                    __instance.TextArea.SetText("");
                    CurrentHistorySelection = 0;
                }
            }
        }
    }

    [HarmonyPatch(typeof(ChatController), nameof(ChatController.UpdateCharCount))]
    public static class OverrideCharCountPatch
    {
        public static bool Prefix() => false;
    }

    [HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
    public static class OverrideSendChatPatch
    {
        public static bool Prefix(ChatController __instance)
        {
            SendPatch(__instance);
            return false;
        }

        private static void SendPatch(ChatController __instance)
        {
            var f = CustomGameOptions.ChatCooldown - __instance.TimeSinceLastMessage;

            if (f > 0 && CustomGameOptions.ChatCooldown > 0)
            {
                __instance.SendRateMessage.gameObject.SetActive(true);
                __instance.SendRateMessage.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.ChatRateLimit, Mathf.CeilToInt(f));
            }
            else
            {
                if (__instance.quickChatData.qcType != QuickChatNetType.None)
                    __instance.SendQuickChat();
                else if (DataManager.Settings.Multiplayer.ChatMode == QuickChatModes.FreeChatOrQuickChat)
                    __instance.SendFreeChat();

                __instance.TimeSinceLastMessage = 0f;
                __instance.TextArea.Clear();
                __instance.quickChatMenu.ResetGlyphs();
                __instance.quickChatData.qcType = QuickChatNetType.None;
            }
        }
    }
}