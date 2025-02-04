using UnityEngine.EventSystems;

namespace UAudio.UI
{
    public class AudioPointerDown : AudioInvocator, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            Play();
        }
    }
}