using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Arrow", menuName = "UI Effect/CubicBezier Arrow")]
public class ArrowSO : ScriptableObject
{
    [SerializeField, Range(2, 20)]
    private int m_length = 3;

    [SerializeField,
        Tooltip("Sprite of head arrow")]
    private Sprite m_headSprite;

    [SerializeField,
        Tooltip("Sprite of each node, default is the same as head arrow")]
    private Sprite m_nodeSprite;

    private Transform m_anchor;

    /// <summary>
    /// Pool the rectTransform of nodes
    /// </summary>
    private RectTransform[] m_nodes;

    public Sprite ArrowImage => m_headSprite;

    public Sprite NodeImage
    {
        get
        {
            if (m_nodeSprite == null)
                return m_headSprite;

            return m_nodeSprite;
        }
    }

    /// <summary>
    /// Anchor, aslo as p0, (t = 0), in cubic bezier curve.
    /// </summary>
    public Transform Anchor
    {
        get
        {
            if (m_anchor == null)
            {
                Debug.LogWarning("Please SetAnchor First");
                return null;
            }
            return m_anchor;
        }
    }

    /// <summary>
    /// Each node represents the point on cubic bezier spline
    /// </summary>
    public RectTransform[] Nodes
    {
        get
        {
            if ((m_nodes?.Length ?? 0) == 0)
            {
                Debug.LogWarning("Please SetAnchor First");
                return null;
            }
            return m_nodes;
        }
    }

    /// <summary>
    /// Initialize nodes.
    /// </summary>
    /// <param name="canvas"> </param>
    private void InitNodes(Canvas canvas)
    {
        m_nodes = new RectTransform[m_length];

        m_nodes[0] = InitNode(head: false, root: true, canvas: canvas.transform);
        m_anchor = m_nodes[0];

        for (int i = 1; i < m_length - 1; i++)
            m_nodes[i] = InitNode(head: false);

        m_nodes[m_length - 1] = InitNode(head: true);
    }

    /// <summary>
    /// Initialize a node
    /// </summary>
    /// <param name="head"> if node is head, get the head aroow of node </param>
    /// <param name="root"> if node is root, please provide transform of canvas </param>
    /// <param name="canvas"> To avoid culling, set root node as child of transform of root canvas. </param>
    /// <returns> RectTransform of a node </returns>
    private RectTransform InitNode(bool head, bool root = false, Transform canvas = null)
    {
        var node = new GameObject("node");

        var image = node.AddComponent<Image>();
        image.sprite = head ? ArrowImage : NodeImage;
        image.SetNativeSize();

        node.transform.SetParent(root ? canvas : m_anchor, false);
        node.transform.SetAsLastSibling();

        return node.transform as RectTransform;
    }

    /// <summary>
    /// Set anchor of the Spline. If <see cref="Nodes"/> is null, <see cref="InitNodes(Canvas)"/>.
    /// </summary>
    /// <param name="anchor"> </param>
    public void SetAnchor(RectTransform anchor)
    {
        if (m_anchor == null)
        {
            InitNodes(anchor.gameObject.GetComponentInParent<Canvas>());
        }

        Nodes[0].position = anchor.position;
    }

    /// <summary>
    /// Set Active of <see cref="Nodes"/>
    /// </summary>
    public void SetActive(bool value)
        => Anchor.gameObject.SetActive(value);

    /// <summary>
    /// Please Reset this Scriptable Object when referred object get destroyed
    /// </summary>
    public void Reset()
    {
        m_nodes = null;
        m_anchor = null;
    }
}