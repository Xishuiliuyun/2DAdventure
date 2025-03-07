
using System.Security.Permissions;

public abstract class BaseState
{
    protected Enemy currentEnemy;
    public abstract void OnEnter(Enemy enemy);//状态进入时执行该函数内代码
    public abstract void LogicUpdate();//逻辑更新，在Update函数内执行，一般用于状态的判断修改
    public abstract void PhysicsUpdate();//物理更新，在FixedUpdate函数内执行，一般用于与物理判断相关的功能执行
    public abstract void OnExit();//状态退出时执行该函数内代码


}
