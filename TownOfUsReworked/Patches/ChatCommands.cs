namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
    public static class ChatCommands
    {
        public static SpriteRenderer Chat;

        public static bool Prefix(ChatController __instance)
        {
            var text = __instance.freeChatField.Text.ToLower();
            var chatHandled = false;

            if (ChatUpdate.ChatHistory.Count == 0 || ChatUpdate.ChatHistory[^1] != text)
                ChatUpdate.ChatHistory.Add(text);

            //Chat command system
            if (text.StartsWith("/"))
            {
                chatHandled = true;
                var args = __instance.freeChatField.Text.Split(' ');
                var command = ChatCommand.AllCommands.Find(x => x.Command == args[0] || x.Short == args[0]);

                if (command == null)
                    __instance.AddChat(CustomPlayer.Local, "Invalid command.");
                else if (command.ExecuteArgs == null)
                    command.ExecuteArgless(__instance);
                else if (command.ExecuteArgless == null)
                    command.ExecuteArgs(args, __instance);
                else
                    __instance.AddChat(CustomPlayer.Local, "Huh...weird.");
            }
            else if (CustomPlayer.Local.IsBlackmailed() && !chatHandled && text != "i am blackmailed.")
            {
                chatHandled = true;
                __instance.AddChat(CustomPlayer.Local, "You are blackmailed.");
            }
            else if (!CustomPlayer.Local.IsSilenced() && !chatHandled && text != "i am silenced." && CustomPlayer.AllPlayers.Any(x => x.IsSilenced() && x.GetSilencer().HoldsDrive))
            {
                chatHandled = true;
                __instance.AddChat(CustomPlayer.Local, "You are silenced.");
            }
            else if (MeetingPatches.GivingAnnouncements && !CustomPlayer.Local.Data.IsDead && !chatHandled)
            {
                chatHandled = true;
                __instance.AddChat(CustomPlayer.Local, "You cannot talk right now.");
            }

            if (chatHandled)
            {
                __instance.freeChatField.Clear();
                __instance.quickChatMenu.Clear();
                __instance.quickChatField.Clear();
                __instance.UpdateChatMode();
            }

            if (!CustomPlayer.Local.Data.IsDead && !chatHandled && !MeetingPatches.GivingAnnouncements && text != "")
            {
                Notify(CustomPlayer.Local.PlayerId);
                CallRpc(CustomRPC.Misc, MiscRPC.Notify, CustomPlayer.Local.PlayerId);
            }

            return !chatHandled;
        }

        public static void Notify(byte targetPlayerId)
        {
            if (!Meeting || Chat)
                return;

            var playerVoteArea = VoteAreaById(targetPlayerId);
            Chat = UObject.Instantiate(playerVoteArea.Megaphone, playerVoteArea.Megaphone.transform);
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
}