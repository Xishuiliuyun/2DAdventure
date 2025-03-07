
using System.Security.Permissions;

public abstract class BaseState
{
    protected Enemy currentEnemy;
    public abstract void OnEnter(Enemy enemy);//״̬����ʱִ�иú����ڴ���
    public abstract void LogicUpdate();//�߼����£���Update������ִ�У�һ������״̬���ж��޸�
    public abstract void PhysicsUpdate();//������£���FixedUpdate������ִ�У�һ�������������ж���صĹ���ִ��
    public abstract void OnExit();//״̬�˳�ʱִ�иú����ڴ���


}
