using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class ConsoleReadout : MonoBehaviour
    {
        [SerializeField]private int maxLines = 100;
        [SerializeField]private bool ignoreMessages, ignoreWarnings = true, ignoreErrors;
    
        private TMP_Text text;
        private ScrollRect scrollRect;
        private string buffer;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
            scrollRect = GetComponentInParent<ScrollRect>();
        }

        private void OnEnable()
        {
            Application.logMessageReceived += OnLogMessageReceived;
            text.text = string.Empty;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
        }

        private void Update()
        {
            if (string.IsNullOrEmpty(buffer)) return;

            text.text += buffer;
            buffer = null;
        
            if (scrollRect)
            {
                scrollRect.normalizedPosition = new Vector2(0.0f, 0.0f);
            }
        
            var lines = 0;
            foreach (var c in text.text)
            {
                if (c != '\n') continue;
                lines++;
            }

            buffer = text.text;
            if (lines < maxLines) return;
            for (var i = 0; i < lines - maxLines; i++)
            {
                var j = buffer.IndexOf('\n') + 1;
                buffer = buffer[j..];
            }

            text.text = buffer;
            buffer = null;
        }

        private void OnLogMessageReceived(string condition, string stacktrace, LogType type)
        {
            switch (type)
            {
                default:
                case LogType.Log:
                    if (ignoreMessages) return;
                    break;
                case LogType.Warning:
                    if (ignoreWarnings) return;
                    break;
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    if (ignoreErrors) return;
                    break;
            }
        
            buffer += $"[{DateTime.Now:hh:mm:ss}] ";
            switch (type)
            {
                default:
                case LogType.Log:
                    buffer += condition;
                    break;
                case LogType.Warning:
                    buffer += $"<color=#FFFF00>{condition}</color>";
                    break;
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    buffer += $"<color=#FF0000>{condition}</color>";
                    break;
            }

            buffer += "\n\n";
        }
    }
}