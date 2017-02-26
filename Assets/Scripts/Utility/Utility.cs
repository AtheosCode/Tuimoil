using UnityEngine;
using System.Collections;

//两条线段的状态
public enum LineCrossState
{
    Cross,//相交
    NotCross,//不相交
    Parallel,//平行
}

/// <summary>
/// 辅助工具类
/// </summary>
public class Utility
{
    #region 计算两条线段的交点
    //两个2D向量的叉积
    public static float CrossProduct2D(Vector2 p1, Vector2 p2)
    {
        return p1.x * p2.y - p1.y * p2.x;
    }

    //检查两条线段是否相交,术语：共线，一定在线段上
    public static bool CheckLineCross(Vector2 sp1, Vector2 ep1, Vector2 sp2, Vector2 ep2)
    {
        //1、可快速排斥实验，先做个初步判断，判断是否相交
        if (Mathf.Max(sp1.x, ep1.x) < Mathf.Min(sp2.x, ep2.x))
        {
            return false;
        }
        if (Mathf.Max(sp1.y, ep1.y) < Mathf.Min(sp2.y, ep2.y))
        {
            return false;
        }
        if (Mathf.Min(sp1.x, ep1.x) > Mathf.Max(sp2.x, ep2.x))
        {
            return false;
        }
        if (Mathf.Min(sp1.y, ep1.y) > Mathf.Max(sp2.y, ep2.y))
        {
            return false;
        }

        //2、跨立实验，就是一条线段的两个点，分布在另一条线段的两侧（包括线段上）
        float temp1 = CrossProduct2D(sp2 - sp1, ep1 - sp1) * CrossProduct2D(ep1 - sp1, ep2 - sp1);
        float temp2 = CrossProduct2D(sp1 - sp2, ep2 - sp2) * CrossProduct2D(ep2 - sp2, ep1 - sp2);

        if (temp1 >= 0 && temp2 >= 0)
        {
            //Debug.Log("line cross");
            return true;
        }

        //Debug.Log("not Cross");
        return false;
    }

    //计算交点
    public static LineCrossState LineCrossWithCrossPoint(Vector2 sp1, Vector2 ep1, Vector2 sp2, Vector2 ep2, out Vector2 outPoint)
    {
        outPoint.x = outPoint.y = float.NaN;

        if (!CheckLineCross(sp1, ep1, sp2, ep2))
        {
            return LineCrossState.NotCross;
        }

        float A1 = ep1.y - sp1.y;
        float B1 = ep1.x - sp1.x;
        float C1 = ep1.x * sp1.y - ep1.y * sp1.x;

        float A2 = ep2.y - sp2.y;
        float B2 = ep2.x - sp2.x;
        float C2 = ep2.x * sp2.y - ep2.y * sp2.x;

        outPoint.x = (B1 * C2 - B2 * C1) / (A1 * B2 - A2 * B1);
        outPoint.y = (A1 * C2 - A2 * C1) / (A1 * B2 - A2 * B1);

        return LineCrossState.Cross;
    }

    #endregion

}