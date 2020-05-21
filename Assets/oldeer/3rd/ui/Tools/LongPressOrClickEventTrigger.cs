using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LongPressOrClickEventTrigger : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler,IDragHandler
{
    public float durationThreshold = 1.0f;

    public UnityEvent onLongPress = new UnityEvent();
    public UnityEvent onClick = new UnityEvent();
    public UnityEvent onClickUp = new UnityEvent();
    public UnityEvent onClickDown = new UnityEvent();

    private bool isPointerDown = false;
    private bool longPressTriggered = false;
    private bool onDrag = false;
    private float timePressStarted;

    private void Update()
    {
        if (isPointerDown && !longPressTriggered && !onDrag)
        {
            if (Time.time - timePressStarted > durationThreshold && (first_pos - stay_pos).magnitude < Screen.width * 0.05f)
            {
                longPressTriggered = true;
                onLongPress.Invoke();
            }
        }
    }

    Vector2 first_pos = Vector2.zero;
    Vector2 stay_pos = Vector2.zero;
    public void OnPointerDown(PointerEventData eventData)
    {
        timePressStarted = Time.time;
        isPointerDown = true;
        longPressTriggered = false;
        onDrag = false;
        first_pos = eventData.position;
        stay_pos = eventData.position;
        onClickDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        onClickUp?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerDown = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!longPressTriggered && !onDrag)
        {
            onClick.Invoke();
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        stay_pos = eventData.position;
        //isPointerDown = false;
        onDrag = true;
    }
}