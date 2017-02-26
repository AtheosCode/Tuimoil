using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 自定义的多级下拉列表（DoTween 做缓动）
/// </summary>
public class ItemList : MonoBehaviour
{
    [System.Serializable]
    private struct TranTran
    {
        //Atheos 这里可以做优化 直接用 LayoutElement 组件 而不是Transform，防止拉错组件
        public Transform item;
        public Transform ceil;
    }
    [SerializeField]
    private List<TranTran> itemList = new List<TranTran> ();
    Dictionary<Transform, Transform> itemDic = new Dictionary<Transform, Transform> ();
    void Awake()
    {
        foreach (var item in itemList)
        {
            itemDic.Add ( item.item, item.ceil );
        }
        itemList = null;
    }
    // Use this for initialization
    void Start()
    {
        foreach (var item in itemDic)
        {
            Tweener tween = item.Value.GetComponent<LayoutElement> ().DOPreferredSize ( new Vector2 ( 0, 0 ), 0.25f );
            tween.SetAutoKill ( false );
            tween.Flip ();
            tween.Pause ();
            item.Key.Find ( "Button" ).GetComponent<Button> ().onClick.AddListener ( delegate ()
            {
                tween.Flip ();
                tween.Play ();
            } );
        }
    }
    // Update is called once per frame
    //void Update()
    //{

    //}
}
