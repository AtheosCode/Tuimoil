using UnityEngine;
using UnityEngine.UI;

public class UI_Control_ScrollFlow_Item : MonoBehaviour
{
    public Image img;
    [HideInInspector]
    public RectTransform rect;
    /// <summary>
    /// 缩放值
    /// </summary>
    public float sv;
    [Tooltip("Position of the queue")]
    /// <summary>
    /// 队列位置
    /// </summary>
    public float positonValue = 0;
    // public float index = 0,index_value;
    private Color color;
    private Vector3 p, s;
    private UI_Control_ScrollFlow parent;

    public void Drag(float value)
    {
        positonValue += value;
        p = rect.localPosition;
        p.x = parent.GetPosition(positonValue);
        rect.localPosition = p;

        color.a = parent.GetApa(positonValue);
        img.color = color;
        sv = parent.GetScale(positonValue);
        s.x = sv;
        s.y = sv;
        s.z = 1;
        rect.localScale = s;
    }

    public void Init(UI_Control_ScrollFlow _parent)
    {
        rect = this.GetComponent<RectTransform>();
        img = this.GetComponent<Image>();
        parent = _parent;
        color = img.color;
    }
}