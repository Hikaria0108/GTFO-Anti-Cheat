using System;
using System.Collections.Generic;
using CellMenu;
using UnityEngine;

namespace Hikaria.GTFO_Anti_Cheat.Utils
{
    internal class GameEventLogManager : MonoBehaviour
    {
        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (this._interval > 0f)
            {
                this._interval -= Time.deltaTime;
                return;
            }
            if (_gameEventLogs.Count > 0)
            {
                this._puiGameEventLog.AddLogItem(_gameEventLogs[0], eGameEventChatLogType.GameEvent);
                GuiManager.Current.m_playerLayer.m_gameEventLog.AddLogItem(_gameEventLogs[0], eGameEventChatLogType.GameEvent);
                _gameEventLogs.RemoveAt(0);
                this._interval = 1f;
            }
        }

        public static void AddLog(string log)
        {
            Instance._gameEventLogs.Add(log);
        }

        private void Start()
        {
            this._puiGameEventLog = CM_PageLoadout.Current.m_gameEventLog;
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

        public static void AddLogInSplit(string str, int len = 50)
        {
            if (str.Length > len)
            {
                string[] array = getstr(str, len);
                for (int i = 0; i < array.Length; i++)
                {
                    AddLog(array[i]);
                }
                return;
            }
            AddLog(str);
        }

        public static void AddLogInSplit(string str, char ch)
        {
            string[] array = str.Split(ch);
            for (int i = 0; i < array.Length; i++)
            {
                AddLog(array[i]);
            }
            return;
        }

        public static GameEventLogManager Instance;

        public List<string> _gameEventLogs = new List<string>();

        private float _interval;

        private PUI_GameEventLog _puiGameEventLog;
    }
}
