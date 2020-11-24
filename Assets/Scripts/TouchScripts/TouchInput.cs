using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class TouchInput : MonoBehaviour
{
    public LayerMask touchInputMask;
    private List<GameObject> touchList = new List<GameObject>();
    private GameObject[] touchesOld;
    private GameObject draggedObject;
    private static TouchCreator lastFakeTouch;

    private Camera _camera;

    private void Start()
    {
        _camera = transform.GetComponent<Camera>();
        if (_camera == null)
        {
            _camera = Camera.main;
        }
    }

    void Update()
    {
        List<Touch> touches = new List<Touch>();
        touches.AddRange(Input.touches);
#if UNITY_EDITOR
        if (lastFakeTouch == null) lastFakeTouch = new TouchCreator();
        if (Input.GetMouseButtonDown(0))
        {
            lastFakeTouch.phase = TouchPhase.Began;
            lastFakeTouch.deltaPosition = new Vector2(0, 0);
            lastFakeTouch.deltaTime = 0;
            lastFakeTouch.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            lastFakeTouch.fingerId = 0;
            lastFakeTouch.fakeTime = Time.time;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            lastFakeTouch.phase = TouchPhase.Ended;
            Vector2 newPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            lastFakeTouch.deltaPosition = newPosition - lastFakeTouch.position;
            lastFakeTouch.position = newPosition;
            lastFakeTouch.fingerId = 0;
            lastFakeTouch.deltaTime = Time.time - lastFakeTouch.fakeTime;
        }
        else if (Input.GetMouseButton(0))
        {
            lastFakeTouch.phase = TouchPhase.Moved;
            Vector2 newPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            lastFakeTouch.deltaPosition = newPosition - lastFakeTouch.position;
            lastFakeTouch.position = newPosition;
            lastFakeTouch.fingerId = 0;
            lastFakeTouch.deltaTime = Time.time - lastFakeTouch.fakeTime;
        }
        else
        {
            lastFakeTouch = null;
        }
        if (lastFakeTouch != null) touches.Add(lastFakeTouch.Create());
#endif
        if (touches.Count > 0)
        {
            touchesOld = new GameObject[touchList.Count];
            touchList.CopyTo(touchesOld);
            touchList.Clear();
            foreach (Touch t in touches)
            {
                Vector2 touchOnWorld = _camera.ScreenToWorldPoint(t.position);
                RaycastHit2D hit;
                if (hit = Physics2D.Linecast(touchOnWorld, touchOnWorld, touchInputMask))
                {
                    GameObject racipient = hit.transform.gameObject;
                    touchList.Add(racipient);
                    string methodName = "";
                    switch (t.phase)
                    {
                        case TouchPhase.Began:
                            methodName = "OnTouchDown";
                            if(draggedObject == null)
                            {
                                draggedObject = racipient;
                            }
                            break;

                        case TouchPhase.Ended:
                            methodName = "OnTouchUp";
                            if (draggedObject != null && racipient != draggedObject)
                            {
                                draggedObject.SendMessage("OnTouchUp", new object[] { t.deltaPosition, t.deltaTime, new Vector2(), t.position }, SendMessageOptions.DontRequireReceiver);
                            }
                            draggedObject = null;
                            break;

                        case TouchPhase.Moved:
                            methodName = "OnTouchMove";
                            if (draggedObject == null)
                            {
                                draggedObject = racipient;
                            }
                            break;
                        case TouchPhase.Stationary:
                            methodName = "OnTouchStay";
                            break;

                        case TouchPhase.Canceled:
                            methodName = "OnTouchExit";
                            break;
                    }
                    racipient.SendMessage(methodName, new object[] {t.deltaPosition, t.deltaTime, hit.point }, SendMessageOptions.DontRequireReceiver);
                }
                else if (draggedObject != null && t.phase == TouchPhase.Ended)
                {
                    draggedObject.SendMessage("OnTouchUp", new object[] { t.deltaPosition, t.deltaTime, new Vector2(), t.position }, SendMessageOptions.DontRequireReceiver);
                    draggedObject = null;
                }

                foreach (GameObject g in touchesOld)
                {
                    if (!touchList.Contains(g))
                        g.SendMessage("OnTouchExit", new object[] {t.deltaPosition, t.deltaTime, hit.point }, SendMessageOptions.DontRequireReceiver);
                }
                if (draggedObject != null)
                {
                    draggedObject.SendMessage("OnTouchDrag", new object[] { t.deltaPosition, t.deltaTime, new Vector2(), t.position }, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}

public class TouchCreator
{
    static Dictionary<string, FieldInfo> fields;

    object touch;
    public float fakeTime;
    public float deltaTime { get { return ((Touch)touch).deltaTime; } set { fields["m_TimeDelta"].SetValue(touch, value); } }
    public int tapCount { get { return ((Touch)touch).tapCount; } set { fields["m_TapCount"].SetValue(touch, value); } }
    public TouchPhase phase { get { return ((Touch)touch).phase; } set { fields["m_Phase"].SetValue(touch, value); } }
    public Vector2 deltaPosition { get { return ((Touch)touch).deltaPosition; } set { fields["m_PositionDelta"].SetValue(touch, value); } }
    public int fingerId { get { return ((Touch)touch).fingerId; } set { fields["m_FingerId"].SetValue(touch, value); } }
    public Vector2 position { get { return ((Touch)touch).position; } set { fields["m_Position"].SetValue(touch, value); } }
    public Vector2 rawPosition { get { return ((Touch)touch).rawPosition; } set { fields["m_RawPosition"].SetValue(touch, value); } }

    public Touch Create()
    {
        return (Touch)touch;
    }

    public TouchCreator()
    {
        touch = new Touch();
    }

    static TouchCreator()
    {
        fields = new Dictionary<string, FieldInfo>();
        foreach (var f in typeof(Touch).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
        {
            fields.Add(f.Name, f);
        }
    }
}
