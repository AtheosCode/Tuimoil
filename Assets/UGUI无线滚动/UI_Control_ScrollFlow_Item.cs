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
    public float v = 0;
    // public float index = 0,index_value;
    private Color color;
    private Vector3 p, s;
    private UI_Control_ScrollFlow parent;

    public void Drag(float value)
    {
        v += value;
        p = rect.localPosition;
        p.x = parent.GetPosition(v);
        rect.localPosition = p;

        color.a = parent.GetApa(v);
        img.color = color;
        sv = parent.GetScale(v);
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