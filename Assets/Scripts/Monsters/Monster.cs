using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterState
{
    Idle,
    Scaring, //scare human distance
    Walking, //for minion
    ReturningVillage
}

public abstract class Monster : MonoBehaviour
{
    public MonsterSO data;
    protected List<Human> _targetHumanList = new List<Human>();
    private SpriteRenderer _spriteRenderer;
    protected Animator Animator;
    protected MonsterState MonsterState;
    private float fadeDuration = 0.5f;
    public int currentUpgradeLevel = 0;
    protected float LastScareTime;
    private float currentFatigue; //현재 피로도
    private Coroutine coroutine;
    
    public void Upgrade(Monster_Data.Upgrade_Data upgradeData)
    {
        currentUpgradeLevel = upgradeData.upgrade_level;
        data.fatigue = upgradeData.fatigue;
        data.fearInflicted = upgradeData.fearInflicted;
        data.cooldown = upgradeData.cooldown;
        data.requiredCoins = upgradeData.requiredCoins;
    }
    
    protected virtual void Awake()
    {
        data = MonsterDataManager.Instance.GetMonsterSOByIdx(data.id);
        Debug.Log($"Awake data{data.monsterId}");
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
        SetState(MonsterState.Idle);
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
                ReturnToVillage();
                break;
        }
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
                Animator.SetTrigger("Return");
                coroutine = StartCoroutine(FadeOutAndReturnToPool());
                break;
        }
    }
    
    private void UpdateAnimatorParameters(Vector2 direction)
    {
        if (Animator == null) return;

        Animator.SetFloat("Horizontal", direction.x);
        Animator.SetFloat("Vertical", direction.y);
        
        bool isScaring = direction != Vector2.zero;
        Animator.SetBool("Scare", isScaring);
    }
    
    protected virtual void Scaring()
    {
        // 단일 공격
        foreach (Human human in _targetHumanList)
        {
            if (human == null) continue;
        
            if (Time.time - LastScareTime > data.cooldown)
            {
                human.IncreaseFear(data.fearInflicted);
                //human.controller.SetTargetMonster(transform);
                LastScareTime = Time.time;
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
        
        if (_targetHumanList.Count == 0)
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
            if (human != null && !_targetHumanList.Contains(human))
            {
                _targetHumanList.Add(human);
                human.controller.SetTargetMonster(transform);
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
            if (human != null && _targetHumanList.Contains(human))
            {
                _targetHumanList.Remove(human);
            }
        }
    }
    
    public void IncreaseFatigue(float value)
    {
        currentFatigue += value;
        Animator.SetTrigger("Hit");
        Debug.Log($"Monster curFatigue: {currentFatigue}");
        if (currentFatigue >= data.fatigue)
        {
            currentFatigue = data.fatigue;
            SetState(MonsterState.ReturningVillage);
        }
    }
    
    private void ReturnToVillage()
    {
        if (coroutine != null) StopCoroutine(coroutine);
        StartCoroutine(FadeOutAndReturnToPool());
    }

    private IEnumerator FadeOutAndReturnToPool()
    {
        foreach (var human in _targetHumanList)
        {
            if (human != null)
            {
                human.controller.ClearTargetMonster();
            }
        }
        
        _targetHumanList.Clear();
        
        // Fade out
        float startAlpha = _spriteRenderer.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, newAlpha);
            yield return null;
        }

        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 0f);
        
        gameObject.SetActive(false);
        PoolManager.Instance.ReturnToPool(data.poolTag, gameObject);
    }
    
    public void Reset()
    {
        if (coroutine != null) StopCoroutine(coroutine);
        currentFatigue = 0f;
        _targetHumanList.Clear();
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 1f);
        SetState(MonsterState.Idle);
    }
    
    // public void ResetToBaseState()
    // {
    //     currentUpgradeLevel = 0;
    //     data = MonsterDataManager.Instance.GetBaseMonsterData(data.id);
    //     transform.localScale = Vector3.one;
    // }
}