using DataTable;
using UnityEngine;
using UnityEngine.AI;

public class HumanController : MonoBehaviour
{
    private float _speed;
    public float Cooldown { get; private set; }
    public float MinFatigueInflicted { get; private set; }
    public float MaxFatigueInflicted { get; private set; }
    
    public Animator animator;
    public NavMeshAgent Agent { get; private set; }
    public Transform TargetMonster { get; private set; }
    public int SpawnedPointIdx { get; set; }
    
    public Transform humanEffect;
    public ParticleSystem attackParticle;
    
    public HumanStateMachine stateMachine;
    public WalkHumanState WalkHumanState { get; private set; }
    public RunHumanState RunHumanState { get; private set; }
    public BattleHumanState BattleHumanState { get; private set; }

    public bool isSurprising;
    
    private void Awake()
    {
        // 애니메이터 & NavMeshAgent 세팅
        animator = GetComponentInChildren<Animator>();
        // if (animator == null)
        //      Debug.LogAssertion("Animator not found");
        Agent = GetComponent<NavMeshAgent>();
        // if (Agent == null)
        //     Debug.LogError("NavMeshAgent not found");
        // Sprite가 화면상에 보이도록 조정
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;
        
        // 상태머신 세팅
        stateMachine = new HumanStateMachine();
        WalkHumanState = new WalkHumanState(this);
        RunHumanState = new RunHumanState(this);
        BattleHumanState = new BattleHumanState(this);
    }

    private void OnEnable()
    {
        // 초기화 설정
        Agent.enabled = false;
        if (StageManager.Instance.StartPointList == null) return;
        transform.position = StageManager.Instance.StartPointList[SpawnedPointIdx].position;   // 시작 위치 설정
        ClearTargetMonster();   // 타겟 몬스터 삭제
        Agent.enabled = true;
        Agent.ResetPath();  // 경로 초기화
        Agent.speed = _speed;   // 속도 초기화
        stateMachine.ChangeState(WalkHumanState);   // 걷는 상태로 전환
        // 애니메이션 초기화
        animator.SetBool("IsWalk", true);
        animator.SetBool("IsBattle", false);
        animator.speed = 1.0f;
        // 파티클 초기화
        humanEffect = HumanParticle.Instance.humanEffect;
        attackParticle = HumanParticle.Instance.attackParticle;
    }
    
    private void Update()
    {
        stateMachine.CurrentHumanState?.Update();   // 상태 머신에서 현재 상태를 계속 갱신
        
        if (TargetMonster != null)  // 타겟 몬스터가 있으면
        {
            AnimationDirectionChange(TargetMonster.position);   // 몬스터 방향으로 회전
        }
        else    // 타겟 몬스터가 없으면
        {
            AnimationDirectionChange(Agent.steeringTarget); // 다음 목적지 방향으로 회전
        }
    }
    
    public void SetHumanData(Human_Data humanData)
    {
        _speed = humanData.speed;
        Cooldown = humanData.cooldown;
        MinFatigueInflicted = humanData.minFatigueInflicted;
        MaxFatigueInflicted = humanData.maxFatigueInflicted;
    }

    // 목표 지점 기준으로 애니메이션의 방향 전환
    private void AnimationDirectionChange(Vector3 targetPosition)
    {
        Vector2 direction = ((Vector2)targetPosition - (Vector2)transform.position).normalized;
        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
    }
    
    public void SetTargetMonster(Transform monster)
    {
        TargetMonster = monster;
    }

    public void ClearTargetMonster()
    {
        TargetMonster = null;
    }
    
    // 타겟 몬스터가 있는 방향으로 파티클 실행
    public void PlayAttackParticle()
    {
        if (TargetMonster == null) return;  // 타겟 몬스터 없으면 리턴
        
        humanEffect.transform.position = transform.position;    // 파티클 시작 위치 설정
        
        // 현재 위치에서 타겟 몬스터로의 방향 구하고 정규화
        Vector3 directionToTarget = (TargetMonster.position - gameObject.transform.position).normalized;

        // 방향 벡터 -> 라디안 -> 각도
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        humanEffect.rotation = Quaternion.Euler(0, 0, angle);   // 파티클 부모 오브젝트를 계산된 각도로 회전
        
        attackParticle.Play();
    }
}