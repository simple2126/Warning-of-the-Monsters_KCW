using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum MonsterState
{
    Idle,
    Scaring, //scare human distance
    Walking, //for minion
    ReturningVillage
}

[System.Serializable]
public class MonsterData
{
    public int id;
    public int monsterId;
    public int currentLevel;
    public string poolTag;
    public float currentFatigue; //현재 피로도
    public float fatigue; //몬스터 피로도바 최대치
    public float minFearInflicted;
    public float maxFearInflicted;
    public float cooldown; //몬스터 놀래킴 쿨타임
    public float humanDetectRange;
    public float humanScaringRange; //적(인간)을 놀래킬 수 있는 범위
    public float walkSpeed; //미니언 걷는 속도
    public int requiredCoins; //필요재화
    public int maxLevel; // 최대 레벨 -> 진화
    public MonsterType monsterType;
    public int summonerId;
    public EvolutionType evolutionType;
}

public abstract class Monster : MonoBehaviour
{
    public MonsterData data;
    protected List<Human> TargetHumanList = new List<Human>();
    private SpriteRenderer _spriteRenderer;
    protected Animator Animator;
    protected MonsterState MonsterState;
    private float _fadeDuration = 1f;
    protected float _lastScareTime;
    private Coroutine _coroutine;
    [SerializeField] private GameObject _fatigueGauge;

    public Action OnAttacked;
    
    protected virtual void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
        SetState(MonsterState.Idle);
        var baseMonsterData = DataManager.Instance.GetBaseMonsterByIdx(data.id - 1);
        if (baseMonsterData != null)
        {
            SetMonsterData(baseMonsterData);
        }
        
        if (_fatigueGauge == null)
        {
            _fatigueGauge = gameObject.transform.Find("FatigueCanvas/FatigueGauge").gameObject;
        }
    }
    
    private void OnEnable()
    {
        GameManager.Instance.activeObjects.Add(this.gameObject);
        ResetMonster();
        _fatigueGauge.SetActive(true);
    }
    
    protected virtual void Update()
    {
        Transform nearestHuman = GetNearestHuman();
        
        switch (MonsterState)
        {
            case MonsterState.Idle:
                UpdateAnimatorParameters(Vector2.zero);
                break;
            case MonsterState.Scaring:
                if (nearestHuman != null)
                {
                    Vector2 directionToHuman = ((Vector2)nearestHuman.position - (Vector2)transform.position).normalized;
                    UpdateAnimatorParameters(directionToHuman);
                    Scaring();
                }
                else
                {
                    SetState(MonsterState.Idle);
                }
                break;
            case MonsterState.ReturningVillage:
                UpdateAnimatorParameters(StageManager.Instance.EndPoint.position);
                ReturnToVillage();
                break;
        }
    }

    private void ResetMonster()
    {
        SetState(MonsterState.Idle);
        if (_spriteRenderer.color.a == 0f)
        {
            Color color = _spriteRenderer.color;
            color.a = 1f;
            _spriteRenderer.color = color;
        }
    }

    // 처음 데이터 저장
    private void SetMonsterData(DataTable.Monster_Data monsterData)
    {
        data.id = monsterData.id;
        data.currentLevel = 0;
        data.poolTag = monsterData.name;
        data.currentFatigue = 0f;
        data.fatigue = monsterData.fatigue;
        data.minFearInflicted = monsterData.minFearInflicted;
        data.maxFearInflicted = monsterData.maxFearInflicted;
        data.cooldown = monsterData.cooldown;
        data.humanDetectRange = monsterData.humanDetectRange;
        data.humanScaringRange = monsterData.humanScaringRange;
        data.requiredCoins = monsterData.requiredCoins;
        data.maxLevel = monsterData.maxLevel;
        data.walkSpeed = monsterData.walkSpeed;
        data.monsterType = monsterData.monsterType;
    }

    // // 현재 데이터 변경
    // public void SetMonsterDataToMonsterData(MonsterData newMonsterData)
    // {
    //     data = newMonsterData;
    //     data.currentFatigue = 0f;
    // }

    public void Upgrade(DataTable.Upgrade_Data upgradeData)
    {
        data.monsterId = upgradeData.monsterId;
        data.currentLevel = upgradeData.upgradeLevel;
        data.fatigue = upgradeData.fatigue;
        data.minFearInflicted = upgradeData.minFearInflicted;
        data.maxFearInflicted = upgradeData.maxFearInflicted;
        data.cooldown = upgradeData.cooldown;
        data.humanScaringRange = upgradeData.humanScaringRange;
        data.requiredCoins = upgradeData.requiredCoins;
    }

    public void Evolution(DataTable.Evolution_Data evolutionData)
    {
        data.monsterId = evolutionData.evolutionId;
        data.currentLevel = evolutionData.upgradeLevel;
        data.poolTag = evolutionData.name;
        data.currentFatigue = 0f;
        data.fatigue = evolutionData.fatigue;
        data.minFearInflicted = evolutionData.minFearInflicted;
        data.maxFearInflicted = evolutionData.maxFearInflicted;
        data.cooldown = evolutionData.cooldown;
        data.humanScaringRange = evolutionData.humanScaringRange;
        data.requiredCoins = evolutionData.requiredCoins;
        data.evolutionType = evolutionData.evolutionType;
        SetState(MonsterState.Idle);
    }

    protected Transform GetNearestHuman()
    {
        Collider2D[] detectedHumans = Physics2D.OverlapCircleAll(transform.position, data.humanScaringRange, LayerMask.GetMask("Human"));
        if (detectedHumans.Length > 0)
        {
            Transform nearestHuman = detectedHumans[0].transform;
            float minDistance = Vector2.Distance(transform.position, nearestHuman.position);

            foreach (Collider2D human in detectedHumans)
            {
                float distance = Vector2.Distance(transform.position, human.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestHuman = human.transform;
                }
            }
            return nearestHuman;
        }
        return null;
    }

    protected virtual void SetState(MonsterState state)
    {
        if (MonsterState == state) return;
        
        MonsterState = state;
        switch (MonsterState)
        {
            case MonsterState.Idle:
                Animator.SetBool("Scare", false);
                break;

            case MonsterState.Scaring:
                Animator.SetBool("Scare", true);
                break;

            case MonsterState.ReturningVillage:
                Animator.SetBool("Return", true);
                _coroutine = StartCoroutine(FadeOutAndReturnToPool());
                break;
        }
    }
    
    private void UpdateAnimatorParameters(Vector2 direction)
    {
        if (Animator == null) return;
        
        Animator.SetFloat("Horizontal", direction.x);
        Animator.SetFloat("Vertical", direction.y);
    }

    protected virtual void Scaring()
    {
        // 단일 공격
        foreach (Human human in TargetHumanList)
        {
            if (human == null) continue;
        
            if (Time.time - _lastScareTime > data.cooldown)
            {
                human.IncreaseFear(Random.Range(data.minFearInflicted, data.maxFearInflicted));
                _lastScareTime = Time.time;
            }
        }

        // 범위 공격
        // if (Time.time - _lastScareTime > data.cooldown)
        // {
        //     foreach (Human human in _targetHumanList)
        //     {
        //         if (human == null) continue;
        //
        //         human.IncreaseFear(data.fearInflicted);
        //         //human.controller.SetTargetMonster(transform);
        //     }
        //     _lastScareTime = Time.time;
        // }
        
        if (TargetHumanList.Count == 0)
        {
            SetState(MonsterState.Idle);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (MonsterState == MonsterState.ReturningVillage) return;
        if (other.CompareTag("Human"))
        {
            Human human = other.GetComponent<Human>();
            if (human != null && !TargetHumanList.Contains(human))
            {
                TargetHumanList.Add(human);
                SetState(MonsterState.Scaring);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Human"))
        {
            Human human = other.GetComponent<Human>();
            human.controller.ClearTargetMonster();
            if (human != null && TargetHumanList.Contains(human))
            {
                TargetHumanList.Remove(human);
            }
        }
    }
    
    public void IncreaseFatigue(float value)
    {
        data.currentFatigue += value;
        Animator.SetTrigger("Hit");
        if (data.currentFatigue >= data.fatigue)
        {
            data.currentFatigue = data.fatigue;
            SetState(MonsterState.ReturningVillage);
        }
        OnAttacked?.Invoke();
    }

    public void SetFatigue(float value)
    {
        data.currentFatigue = value;
    }

    public virtual void ReturnToVillage()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        StartCoroutine(FadeOutAndReturnToPool());
    }

    private IEnumerator FadeOutAndReturnToPool()
    {
        foreach (var human in TargetHumanList)
        {
            if (human != null)
            {
                human.controller.ClearTargetMonster();
            }
        }
        TargetHumanList.Clear();
        
        _fatigueGauge.SetActive(false);
        
        // Fade out
        float startAlpha = _spriteRenderer.color.a;
        float elapsedTime = 0f;
        Color startColor = _spriteRenderer.color;

        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / _fadeDuration);
            startColor.a = newAlpha;
            _spriteRenderer.color = startColor;
            yield return null;
        }

        startColor.a = 0f;
        _spriteRenderer.color = startColor;
        PoolManager.Instance.ReturnToPool(data.poolTag, gameObject);
    }
    
    public void Reset()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        data.currentFatigue = 0f;
        TargetHumanList.Clear();
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 1f);
        SetState(MonsterState.Idle);
    }

    private void OnDisable()
    {
        GameManager.Instance.activeObjects.Remove(gameObject);
    }
}