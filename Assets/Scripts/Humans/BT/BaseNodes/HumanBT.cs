using UnityEngine;

public class HumanBT : MonoBehaviour
{
    private BehaviorTree _behaviorTree;
    private HumanController _humanController;

    public Vector3 spawnPoint;
    public Vector3 targetPoint;

    private void Awake()
    {
        _humanController = TryGetComponent<HumanController>(out _humanController)
            ? _humanController : gameObject.AddComponent<HumanController>();
        spawnPoint = GameObject.FindWithTag("HumanSpawnPoint").transform.position;
        if (spawnPoint == null)
        {
            Debug.LogAssertion("Could not find spawn point");
        }
        targetPoint = GameObject.FindWithTag("DestinationPoint").transform.position;
        if (targetPoint == null)
        {
            Debug.LogAssertion("Could not find destination point");
        }
    }

    private void Start()
    {
        InitializeBehaviorTree();
    }

    private void Update()
    {
        _behaviorTree.Update();
    }
    /* 참고 사항 */
    // BT 구조 이미지는 팀노션 페이지에 수정하는 대로 업데이트 예정(현재는 스크립트로만 작성됨)
    // TODO: unity editor experimental graph view 활용 or 외부 패키지 사용하여 Edtor에서 확인 가능하게 수정
    private void InitializeBehaviorTree()   // 인간 행동 트리 초기화
    {
        SelectorNode root = new SelectorNode();

        IdleNode idleNode = new IdleNode(_humanController);
        RunNode runNode = new RunNode(_humanController, spawnPoint);
        
        //SelectorNode readySelector = new SelectorNode();
        
        
        // // Idle Node
        //
        // // Walk Node
        // WalkNode walkNode = new WalkNode(_humanController, targetPoint);
        //
        // // Ready To Battle        
         SelectorNode readySelector = new SelectorNode();
         WalkNode walkNode = new WalkNode(_humanController, targetPoint);
         readySelector.AddChild(walkNode);
        // // SetFormation Node
        // SetFormationNode setFormationNode =  new SetFormationNode(_humanController);
        //
        // Battle
        SequenceNode battleSequence = new SequenceNode();
        battleSequence.AddChild(new SetFormationNode(_humanController));
        battleSequence.AddChild(new AttackNode(_humanController));
        //
         readySelector.AddChild(battleSequence);
        // readySelector.AddChild(setFormationNode);
        // readySelector.AddChild(battleSequence);
        //
        // root.AddChild(idleNode);
        // root.AddChild(walkNode);
        // root.AddChild(readySelector);
    
        root.AddChild(idleNode);
        root.AddChild(runNode);
        root.AddChild(readySelector);
        //root.AddChild(walkNode);
        //root.AddChild(battleSequence);
        
        _behaviorTree = new BehaviorTree(root);
    }
}