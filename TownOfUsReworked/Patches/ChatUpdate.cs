namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class ChatUpdate
    {
        private static float _time;

        public static void Postfix(HudManager __instance)
        {
            if (!__instance.Chat)
                return;

            if (_time >= CustomGameOptions.ChatCooldown)
            {
                __instance.Chat.TimeSinceLastMessage = 3f;
                _time -= CustomGameOptions.ChatCooldown;
            }
            else
                _time += Time.deltaTime;

            foreach (var bubble in __instance.Chat.chatBubPool.activeChildren)
            {
                if (bubble.Cast<ChatBubble>().NameText != null)
                {
                    bubble.Cast<ChatBubble>().NameText.alignment = TextAlignmentOptions.Left;
                    bubble.Cast<ChatBubble>().TextArea.alignment = TextAlignmentOptions.TopLeft;
                }
            }

            if (ConstantVariables.IsLobby)
                return;

            foreach (var bubble in __instance.Chat.chatBubPool.activeChildren)
            {
                var chat = bubble.Cast<ChatBubble>();

                if (chat.NameText != null)
                {
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (chat.NameText.text.Contains(player.Data.PlayerName))
                        {
                            var role = Role.GetRole(player);

                            if (role != null)
                            {
                                if ((((PlayerControl.LocalPlayer.GetFaction() == player.GetFaction() && !player.Is(Faction.Crew) && !player.Is(Faction.Neutral)) ||
                                    (PlayerControl.LocalPlayer.GetSubFaction() == player.GetSubFaction() && !player.Is(SubFaction.None))) && CustomGameOptions.FactionSeeRoles) || player ==
                                    PlayerControl.LocalPlayer)
                                {
                                    chat.NameText.color = role.Color;
                                }
                                else if (PlayerControl.LocalPlayer.GetFaction() == player.GetFaction() && !player.Is(Faction.Crew) && !player.Is(Faction.Neutral) &&
                                    !CustomGameOptions.FactionSeeRoles)
                                {
                                    chat.NameText.color = role.FactionColor;
                                }
                                else if (PlayerControl.LocalPlayer.GetSubFaction() == player.GetSubFaction() && !player.Is(SubFaction.None) && !CustomGameOptions.FactionSeeRoles)
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
}