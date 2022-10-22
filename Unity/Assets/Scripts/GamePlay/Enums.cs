namespace SunAndMoon
{
    public enum MapType
    {
        Green,
        Pink,
        Yellow,
    }

    public enum RoleType
    {
        Unknown,
        Sun,
        Moon,
    }

    public enum GroundType
    {
        Unknown,
        Normal,
        Ice,
    }

    public enum InteractorType
    {
        Unknown,
        Collider,//基础碰撞块
        LeftRotate,//向左转向
        RightRotate,//向右转向
        UpRotate,//向上转向
        DownRotate,//向下转向
        HorizonAccessUp,//横向可通过（在单元格上边界）
        HorizonAccessBottom,//横向可通过(在单元格下边界）
        VerticalAccessLeft,//纵向可通过（在单元格左边界）
        VerticalAccessRight,//纵向可通过（在单元格有边界）
        KnockBreakCollider,//可撞坏
        LeftUpWindmill,//风车（左上角）
        RightUpWindmill,//风车（右上角）
        RightBottomWindmill,//风车（右下角）
        LeftBottomWindmill,//风车（左下角）
    }
}