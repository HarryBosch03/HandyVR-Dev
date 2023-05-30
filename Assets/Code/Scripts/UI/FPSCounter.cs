using System.Globalization;
using TMPro;
using UnityEngine;

namespace Code.Scripts.UI
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class FPSCounter : MonoBehaviour
    {
        [SerializeField] private string template;
    
        private TMP_Text text;

        private void Awake()
        {
            text = GetComponentInChildren<TMP_Text>();
        }

        private void Update()
        {
            text.text = string.Format(template, (Mathf.RoundToInt(1.0f / Time.smoothDeltaTime)).ToString(CultureInfo.InvariantCulture));
        }
    }
}
