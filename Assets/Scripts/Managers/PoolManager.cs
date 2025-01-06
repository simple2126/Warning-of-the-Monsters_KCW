using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class PoolManager : SingletonBase<PoolManager>
{
    // Inspector에서 여러 풀 한번에 관리
    [Serializable]
    public class PoolConfig
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    
    private List<PoolConfig> _poolConfigs = new List<PoolConfig>();
    private Dictionary<string, object> _pools = new Dictionary<string, object>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void CreatePool<T>(string tag, GameObject prefab, int size) where T : Component
    {
        // 풀 딕셔너리에 이미 해당 태그와 일치하는 풀 있으면 리턴(중복 풀 생성 방지)
        if (_pools.ContainsKey(tag))
        {
            Debug.Log($"Pool with tag {tag} already exists.");
            return;
        }

        // 계층 구조 생성하여 정리
        GameObject poolObject = new GameObject($"Pool_{tag}"); // 풀 관리할 빈 게임오브젝트 생성하고 태그로 이름 구별
        poolObject.transform.SetParent(transform); // PoolManager의 자식으로 설정

        //UI일 시
        if (typeof(UIBase).IsAssignableFrom(typeof(T)))
        {
            var canvas = poolObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var canvasScaler = poolObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(UIManager.Instance.screenWidth, UIManager.Instance.screenHeight);
            canvasScaler.matchWidthOrHeight = 1f;
        }

        // Inspector에서 받아온 설정 정보 기반으로 새로운 오브젝트 풀 생성
        IObjectPool<T> objectPool = new ObjectPool<T>(
            createFunc: () =>
            {
                GameObject obj = Instantiate(prefab);
                obj.name = tag; // 생성되는 풀링 오브젝트의 이름을 태그명과 동일하게 설정
                obj.transform.SetParent(poolObject.transform);
                return obj.GetComponent<T>();
            },
            actionOnGet: obj => obj.gameObject.SetActive(true),
            actionOnRelease: obj => obj.gameObject.SetActive(false),
            actionOnDestroy: obj => Destroy(obj.gameObject),
            defaultCapacity: size,
            maxSize: 100
        );
        
        ExpandPool(objectPool, prefab, tag, poolObject.transform, size);    // size만큼 미리 생성

        _pools.Add(tag, objectPool);    // 풀 딕셔너리에 새로운 오브젝트 풀 추가
    }
    
    private void ExpandPool<T>(IObjectPool<T> pool, GameObject prefab, string tag, Transform parent, int count) where T : Component
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.name = tag;
            obj.transform.SetParent(parent);
            obj.SetActive(false);
            pool.Release(obj.GetComponent<T>());
        }
    }
    
    public void AddPools<T>(PoolConfig[] newPools) where T : Component
    {
        if (newPools == null) return;
        
        foreach (var pool in newPools)
        {
            if (_pools.ContainsKey(pool.tag)) continue; // 이미 있으면 추가 X
            _poolConfigs.Add(pool); // 외부 클래스에서 받아온 풀 정보 리스트에 추가
        }
    }

    public T SpawnFromPool<T>(string tag, Vector3 position, Quaternion rotation) where T : Component
    {
        // 태그와 일치하는 풀이 없으면 풀 생성
        if (!_pools.TryGetValue(tag, out var pool))
        {
            foreach (var poolConfig in _poolConfigs)
            {
                if (poolConfig.tag == tag)
                {
                    CreatePool<T>(poolConfig.tag, poolConfig.prefab, poolConfig.size);
                }
            }
        }
        
        // 풀 생성 실패 시 에러메시지 출력 후 null 반환
        if (!_pools.TryGetValue(tag, out pool))
        {
            Debug.LogAssertion($"Pool with tag {tag} cannot be created.");
            return null;
        }

        // 태그와 일치하는 풀이 있으면 
        if (pool is IObjectPool<T> typedPool)
        {
            // 모든 오브젝트가 사용 중이면 풀 확장
            if (typedPool.CountInactive == 0)   
            {
                var poolConfig = _poolConfigs.Find(config => config.tag == tag);
                if (poolConfig != null)
                {
                    Transform parentTransform = transform.Find($"Pool_{tag}");
                    ExpandPool(typedPool, poolConfig.prefab, tag, parentTransform, poolConfig.size);
                }
            }
            
            // 풀에서 오브젝트 가져와 Transform 설정 후 반환
            T obj = typedPool.Get();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            return obj;
        }

        Debug.LogAssertion($"Type Error: Pool with tag {tag} is {typeof(T)}.");
        return null;
    }

    public T SpawnFromPool<T>(string tag) where T : Component
    {
        // 태그와 일치하는 풀이 없으면 풀 생성
        if (!_pools.TryGetValue(tag, out var pool))
        {
            foreach (var poolConfig in _poolConfigs)
            {
                if (poolConfig.tag == tag)
                {
                    CreatePool<T>(poolConfig.tag, poolConfig.prefab, poolConfig.size);
                }
            }
        }
        
        // 풀 생성 실패 시 에러메시지 출력 후 null 반환
        if (!_pools.TryGetValue(tag, out pool))
        {
            Debug.LogAssertion($"Pool with tag {tag} cannot be created.");
            return null;
        }

        // 태그와 일치하는 풀이 있으면 
        if (pool is IObjectPool<T> typedPool)
        {
            // 모든 오브젝트가 사용 중이면 풀 확장
            if (typedPool.CountInactive == 0)
            {
                var poolConfig = _poolConfigs.Find(config => config.tag == tag);
                if (poolConfig != null)
                {
                    Transform parentTransform = transform.Find($"Pool_{tag}");
                    ExpandPool(typedPool, poolConfig.prefab, tag, parentTransform, poolConfig.size);
                }
            }
            
            // 풀에서 오브젝트 가져와 반환
            T obj = typedPool.Get();
            return obj;
        }

        Debug.LogAssertion($"Type Error: Pool with tag {tag} is {typeof(T)}.");
        return null;
    }
    
    public void ReturnToPool<T>(string tag, T obj) where T : Component
    {
        if (obj == null) return;

        // 태그와 일치하는 풀이 있는지 유효성 검사
        if (!_pools.TryGetValue(tag, out var pool))
        {
            Debug.LogAssertion($"Pool with tag {tag} does not exist.");
            return;
        }

        // 오브젝트 풀에 반환
        if (pool is IObjectPool<T> typedPool)
        {
            typedPool.Release(obj);
            return;
        }

        Debug.LogAssertion($"Type Error: Pool with tag {tag} is {typeof(T)}.");
    }

    // 특정 태그의 오브젝트 풀을 삭제
    public void DeletePool(string tag)
    {
        // 태그와 일치하는 풀이 있는지 유효성 검사
        if (!_pools.ContainsKey(tag))
        {
            Debug.LogAssertion($"Pool with tag {tag} does not exist.");
            return;
        }

        if (_pools[tag] is IObjectPool<Component> pool)
        {
            pool.Clear();   // 퓰 딕셔너리에서 제거
        }

        Transform poolObject = transform.Find($"Pool_{tag}");
        if (poolObject != null)
        {
            Destroy(poolObject.gameObject);  // 오브젝트 풀 삭제
        }
    }

    // 풀 딕셔너리에 등록된 모든 오브젝트 풀 삭제
    public void DeleteAllPools()
    {
        // 풀 딕셔너리에 있는 오브젝트 풀로 생성된 게임오브젝트 삭제
        foreach (var key in _pools.Keys)
        {
            Transform poolObject = transform.Find($"Pool_{key}");
            if (poolObject != null)
            {
                Destroy(poolObject.gameObject);
            }
        }
        _pools.Clear(); // 풀 딕셔너리에서 모든 항목 삭제
    }
}