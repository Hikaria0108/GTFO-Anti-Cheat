using Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hikaria.GTFO_Anti_Cheat.Utils
{
    internal class ChatManager : MonoBehaviour
    {
        private void FixedUpdate()
        {
            try
            {
                if (ChatManager.queue[ChatManager.front] != null)
                {
                    ChatManager.Speak(ChatManager.queue[ChatManager.front]);
                    ChatManager.front++;
                }
            }
            catch (Exception)
            {
            }
        }

        private static string[] getstr(string strs, int len)
        {
            string[] array = new string[int.Parse(Math.Ceiling((double)strs.Length / (double)len).ToString())];
            for (int i = 0; i < array.Length; i++)
            {
                len = ((len <= strs.Length) ? len : strs.Length);
                array[i] = strs.Substring(0, len);
                strs = strs.Substring(len, strs.Length - len);
            }
            return array;
        }

        public static void SpeakInSeparate(string str, int len = 50)
        {
            if (str.Length > len)
            {
                foreach (string item in ChatManager.getstr(str, len))
                {
                    ChatManager.AddQueue(item);
                }
                return;
            }
            ChatManager.AddQueue(str);
        }

        public static void AddQueue(string msg)
        {
            queue.Add(msg);
        }

        public static void ClearQueue()
        {
            queue.Clear();
        }

        public static void Speak(string text)
        {
            PlayerChatManager.WantToSentTextMessage(PlayerManager.GetLocalPlayerAgent(), text, null);
        }

        private static List<string> queue = new List<string>();

        private static int front = 0;

        public static void DetectBroadcast(string playerName, string message)
        {
            if (!EntryPoint.EnableBroadcast)
                return;

            ChatManager.Speak(string.Format(EntryPoint.Language.CHEATER_DETECTED_MESSAGE, playerName));
            ChatManager.Speak(string.Format(EntryPoint.Language.CHEATING_BEHAVIOR_MESSAGE, message));
        }

        public static void KickBanBroadcast(string playerName, string reason)
        {
            if (!EntryPoint.EnableBroadcast)
                return;

            ChatManager.Speak(string.Format(EntryPoint.Language.CHEATER_DETECTED_MESSAGE, playerName));
            ChatManager.Speak(string.Format(EntryPoint.Language.KICK_OR_BAN_REASON, reason));
        }
    }
}
