using UnityEngine;
using UnityEngine.UI;

namespace UAudio.UI
{
    [RequireComponent(typeof(Button))]
    public class AudioButton : AudioInvocator
    {
        private Button m_button;

        private void OnEnable()
        {
            m_button = GetComponent<Button>();
            m_button.onClick.AddListener(Play);
        }

        private void OnDisable()
        {
            m_button.onClick.RemoveListener(Play);
        }
    }
}