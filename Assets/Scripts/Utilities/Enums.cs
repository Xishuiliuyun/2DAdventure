//玩家状态
/*public enum PlayerState
{
    Move,Jump,Fall,SlipWall,ClimbWall,Slid,Attack,Dead
}*/

//怪物NPC状态 Patrol巡逻 Chase追击 Skill释放技能 Show显示(蜗牛) KeepHide缩在壳里面(蜗牛)
public enum NPCState
{
    Patrol,Chase,Skill, Show,KeepHide

}

//可互动物体类型，Deft默认、Item宝箱物品、NPC可对话类
public enum InteractableType
{ 
    Deft,Item,NPC
}

//输入设备类型，Deft默认、Keyboard键鼠、XBOX、PS、Switch
public enum InputControlType
{
    Deft,Keyboard,XBOX,PS,Switch
}


public enum SceneType
{
    Loaction,Menu,Inventory
}


public enum PersistentType
{
    ReadWrite,DoNotPersiste
}

//角色属性值 HP血量 Att攻击力 Def防御力 Energy能量值 Exp经验值
public enum AttributeType
{
    HP,Att,Def,Energy,Exp,Null
}

//角色能力 jumpTimes跳跃次数 slipWall滑墙 climbWall爬墙 slid滑铲
public enum AbilityType
{
    jumpTimes, slipWall, climbWall, slid
}


//预制体物品类型
public enum PrefabType
{
    HPBottle,Key, Coin
}

//Static固定文本内容 Dynamic根据输入设备需变化的文本内容
public enum TextType
{ 
    Static, Dynamic
}

public enum TipsType
{
    MoveAndAtt,ClimbWall, SlipWall, Slid
}


//保存类型，敌人死亡永久死亡，敌人切换场景后重新加载
public enum SaveType
{
    SaveEnemy, NotSaveEnemy
}


public enum PrefabPath
{ 

}

//enum类型设置的类型Language语言ResolvingPower分辨率
public enum EnumSettingsType
{
    Language, ResolvingPower
}

//语言类型
public enum Language
{
    Chinese,English,Japanese,Korean
}

//分辨率类型
public enum ResolvingPower
{
    Type1,Type2, Type3, Type4, Type5, Type6, Type7, Type8, Type9, Type10
}

//键盘按键
public enum Keys_Keyboard
{ 
    Null, MoveUp1, MoveUp2, MoveDown1, MoveDown2, MoveLeft1, MoveLeft2, MoveRight1, MoveRight2,
    Jump, Attack1, Attack2, Confirm_E, Confirm_F, Slid, ESC, Back
}
//手柄按键
public enum Keys_Gamepad
{
    Null, Jump, Attack, Confirm_E, Confirm_F, Slid, ESC, Back
}



