using UnityEngine;
using System.Collections;

public class GlobalDefine
{
    public enum PanelType
    {
        //节点1
        Default = 0,
        RootNode,
        //节点2
        node_1 = 5,
        Login,
        FPS,
        Sphere,
        Final,
    }
    public enum GlobalEvent
    {
        //UI
        UI_CalMap = 1000,//1000开始，避免与网络Show冲突
        UI_CreatePlaneSign,

        UI_EnterGroup,//进入房间
        UI_LeaveGruop,//离开房间
        UI_OtherLeave,//其他玩家离开
        UI_GetGroupList,//显示房间列表
        UI_ObjectEntry,//显示进入房间的玩家

        UI_GameOver,//游戏结束
        UI_ShowKillTips,//死亡提示
        UI_ShowWeaponTips,//获得武器提示
        UI_WeaponCoolTips,//武器冷却提示

        UI_BloodBar,//非玩家血条
        UI_PlayerBloodBar,//玩家血条
        UI_PlayerEnergyBar,//玩家能量条
        UI_RefreshRank,//刷新排名
        UI_ShowOverHead,//OverHead
        UI_TravelUIOpen,//跳越时空，开关UI

        UI_ShowItemIcon,//显示道具图标
        UI_CountDown,//倒计时
        UI_ShowCalInfo,//显示结算面板信息

        UI_ResetTalent,//重置天赋
        UI_BuyItem,//购买商品成功
        UI_FriendList,//好友列表
        UI_ApplyList,//好友申请列表
        UI_RecommList,//好友推荐列表
        UI_FriendAdd,//申请添加好友
        UI_FriendAgree,//同意添加好友
        UI_FriendDisagree,//拒绝添加好友
        UI_FriendDelete,//删除好友
        UI_SaleItem,//卖出材料
        UI_UpgradeAircraft,//升级飞机
        UI_PlayerInfo,//获取玩家详细信息
        UI_RankScore,
        UI_RankKill,
        UI_RankTeamWin,
        UI_RankChamp,
        UI_RegularScore,
        UI_RegularKill,
        UI_RegularTeamWin,
        EGMI_ACK_WORLD_LIST_BIG,
        EGMI_ACK_WORLD_LIST_SMALL,
        EGMI_ACK_ROLE_LIST,

        EGMI_ACK_LOGIN,
        EGMI_ACK_CONNECT_WORLD,
        EGMI_ACK_SELECT_SERVER,
        UI_EquipAircraft,
    }

}