using UnityEngine;
using UnityEngine.EventSystems;

// Use EventSystems OnDrag Interface for better performance
[RequireComponent(typeof(RectTransform))]
public class BezierTracing : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Fields

    [SerializeField,
        Tooltip("ScriptableObject of Arrow. \n" +
        "Caches nodes property and image data")]
    private ArrowSO m_arrowSO;

    [SerializeField,
        Tooltip("Anchor, aslo as p0, (t = 0), in cubic bezier curve. \n" +
        "Default is this gameObject")]
    private RectTransform m_anchor;

    private RectTransform m_draggableArea;

    [SerializeField,
        Tooltip("factors for dynamic generating p1,p2 of cubic bezier curve")]
    private Vector2 p1factor, p2factor;

    /// <summary>
    /// instance of spline based on current control vertices
    /// </summary>
    private CubicBezierCurve m_spline;

    /// <summary>
    /// Destination vertex, aslo as p3 (t = 1) in cubic bezier curve.
    /// </summary>
    private Vector3 m_destVert = Vector3.zero;

    /// <summary>
    /// Control verteices for cubic bezier curve. length = 4
    /// </summary>
    private Vector3[] m_controlVerts;

    #endregion Fields

    #region Properties

    /// <summary>
    /// Anchor position for the arrow
    /// </summary>
    public RectTransform Anchor
    {
        get
        {
            if (m_anchor == null)
                m_anchor = transform as RectTransform;
            return m_anchor;
        }
    }

    /// <summary>
    /// Interactable area/panel/canvas
    /// </summary>
    public RectTransform DraggableArea
    {
        get
        {
            if (m_draggableArea == null)
                m_draggableArea = Anchor.GetComponentInParent<Canvas>().transform as RectTransform;
            return m_draggableArea;
        }
    }

    /// <summary>
    /// Get current control vertices. Please update destination Vertex before using the getter.
    /// </summary>
    public Vector3[] CubicControlVerts
    {
        get
        {
            if (m_controlVerts == null)
            {
                m_controlVerts = new Vector3[4];
                m_controlVerts[0] = Anchor.position;
            }
            m_controlVerts[1] = GetDynamicVerts(p1factor);
            m_controlVerts[2] = GetDynamicVerts(p2factor);
            m_controlVerts[3] = m_destVert;

            return m_controlVerts;
        }
    }

    /// <summary>
    /// Get Spline based on current Cubic Control Vertices
    /// </summary>
    public CubicBezierCurve Spline
    {
        get
        {
            if (m_spline == null)
                m_spline = new CubicBezierCurve(CubicControlVerts);
            else
                m_spline.SetControlVerts(CubicControlVerts);

            return m_spline;
        }
    }

    #endregion Properties

    #region Unity Events

    private void OnValidate()
    {
        if (m_arrowSO == null
            || m_arrowSO.ArrowImage == null)
        {
            Debug.LogWarning($"Please assign a sprite through {nameof(ArrowSO)}");
        }
    }

    /// <summary>
    /// Remove reference in Arrow Scriptable Object
    /// </summary>
    private void OnDestroy()
    {
        if (m_arrowSO != null)
            m_arrowSO.Reset();
    }

    public void OnEndDrag(PointerEventData eventData)
        => m_arrowSO.SetActive(false);

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_arrowSO.SetAnchor(Anchor);

        SetDraggedPosition(eventData);
        BezierArrowUtility.SetBezierArrow(m_arrowSO, Spline);

        m_arrowSO.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
        BezierArrowUtility.SetBezierArrow(m_arrowSO, Spline);
    }

    #endregion Unity Events

    private void SetDraggedPosition(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                DraggableArea,
                eventData.position,
                eventData.pressEventCamera,
                out var globalMousePos))
        {
            m_destVert = globalMousePos;
        }
    }

    private Vector3 GetDynamicVerts(Vector2 factor)
        => GetDynamicVerts(Anchor.position, m_destVert, factor);

    public static Vector3 GetDynamicVerts(Vector2 origin, Vector2 dest, Vector2 factor)
    {
        //Debug.Log($"origin: {origin}, dest: {dest}, diff: {dest - origin}, f.o: {origin * factor}, f.diff: {(dest - origin) * factor} ");
        return origin + (dest - origin) * factor;
    }
}