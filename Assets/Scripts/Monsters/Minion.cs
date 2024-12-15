using UnityEngine;
using UnityEngine.AI;

public class Minion : Monster //졸개
{
    private NavMeshAgent _navMeshAgent;
    private Monster_Data.Minion_Data _minionData;
    
    public void InitializeMinion(Monster_Data.Minion_Data minionData)
    {
        _minionData = minionData;
        data.fatigue = minionData.fatigue;
        data.fearInflicted = minionData.fearInflicted;
        data.cooldown = minionData.cooldown;
        data.humanScaringRange = minionData.humanScaringRange;
        data.speed = minionData.speed;
        
        _navMeshAgent.speed = minionData.speed;
        SetState(MonsterState.Walking);
    }
    
    protected override void Awake()
    {
        base.Awake();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
    }
    
    protected override void Update()
    {
        base.Update();
        if (MonsterState == MonsterState.Walking)
        {
            WalkTowardsNearestHuman();
        }
    }

    protected override void SetState(MonsterState state)
    {
        base.SetState(state);
        switch (MonsterState)
        {
            case MonsterState.Walking:
                Animator.SetBool("Walking", true);
                break;
        }
    }
    
    private void WalkTowardsNearestHuman()
    {
        Transform nearestHuman = GetNearestHuman();
        if (nearestHuman != null)
        {
            _navMeshAgent.SetDestination(nearestHuman.position);
        }
        else
        {
            SetState(MonsterState.Idle);
        }
    }
}
