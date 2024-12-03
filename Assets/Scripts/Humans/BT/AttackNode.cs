public class AttackNode : INode
{
    private HumanController _humanController;

    public AttackNode(HumanController humanController)
    {
        _humanController = humanController;
    }

    public NodeState Evaluate()
    {
        // Debug.Log($"Evaluating {this.GetType().Name}");
        if (!_humanController.HasTargetMonster())   // 타겟몬스터 없어지면
        {
            return NodeState.Failure;
        }
        if (_humanController.CanAttack())
        {
            _humanController.PerformAttack(); // 공격 수행
        }
        return NodeState.Success; 
    }
}