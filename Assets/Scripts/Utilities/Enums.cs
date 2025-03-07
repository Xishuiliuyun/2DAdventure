//���״̬
/*public enum PlayerState
{
    Move,Jump,Fall,SlipWall,ClimbWall,Slid,Attack,Dead
}*/

//����NPC״̬ PatrolѲ�� Chase׷�� Skill�ͷż��� Show��ʾ(��ţ) KeepHide���ڿ�����(��ţ)
public enum NPCState
{
    Patrol,Chase,Skill, Show,KeepHide

}

//�ɻ����������ͣ�DeftĬ�ϡ�Item������Ʒ��NPC�ɶԻ���
public enum InteractableType
{ 
    Deft,Item,NPC
}

//�����豸���ͣ�DeftĬ�ϡ�Keyboard����XBOX��PS��Switch
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

//��ɫ����ֵ HPѪ�� Att������ Def������ Energy����ֵ Exp����ֵ
public enum AttributeType
{
    HP,Att,Def,Energy,Exp,Null
}

//��ɫ���� jumpTimes��Ծ���� slipWall��ǽ climbWall��ǽ slid����
public enum AbilityType
{
    jumpTimes, slipWall, climbWall, slid
}


//Ԥ������Ʒ����
public enum PrefabType
{
    HPBottle,Key, Coin
}

//Static�̶��ı����� Dynamic���������豸��仯���ı�����
public enum TextType
{ 
    Static, Dynamic
}

public enum TipsType
{
    MoveAndAtt,ClimbWall, SlipWall, Slid
}


//�������ͣ������������������������л����������¼���
public enum SaveType
{
    SaveEnemy, NotSaveEnemy
}


public enum PrefabPath
{ 

}

//enum�������õ�����Language����ResolvingPower�ֱ���
public enum EnumSettingsType
{
    Language, ResolvingPower
}

//��������
public enum Language
{
    Chinese,English,Japanese,Korean
}

//�ֱ�������
public enum ResolvingPower
{
    Type1,Type2, Type3, Type4, Type5, Type6, Type7, Type8, Type9, Type10
}

//���̰���
public enum Keys_Keyboard
{ 
    Null, MoveUp1, MoveUp2, MoveDown1, MoveDown2, MoveLeft1, MoveLeft2, MoveRight1, MoveRight2,
    Jump, Attack1, Attack2, Confirm_E, Confirm_F, Slid, ESC, Back
}
//�ֱ�����
public enum Keys_Gamepad
{
    Null, Jump, Attack, Confirm_E, Confirm_F, Slid, ESC, Back
}



