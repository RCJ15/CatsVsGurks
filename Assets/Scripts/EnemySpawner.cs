using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public bool WaveCompleted => _currentValue <= 0;

    [SerializeField] private Vector2 min;
    [SerializeField] private Vector2 max;
    [SerializeField] private Wave[] waves;

    private Wave _currentWave;

    private int _gurksRemaining;
    private float _currentValue;
    private float _spawnTimer;

    private bool _isPlaying;

    private void Start()
    {
        // TEMP
        Debug.LogWarning("DELETE THIS");
        Begin();
    }

    private void Update()
    {
        if (!_isPlaying)
        {
            return;
        }

        if (_currentValue <= 0)
        {
            if (_gurksRemaining <= 0)
            {
                // Start next wave
                Debug.Log("WAVE COMPLETE");
            }
            return;
        }

        if (_spawnTimer <= 0)
        {
            _spawnTimer = Random.Range(_currentWave.TimeBtwSpawns.x, _currentWave.TimeBtwSpawns.y);

            // Determine enemy to spawn
            Gurk enemy = _currentWave.Enemies[Random.Range(0, _currentWave.EnemiesLength)];

            // Spawn location
            Vector3 pos = GetRandomPos();

            // Spawn enemy
            Gurk newEnemy = Instantiate(enemy, pos, Quaternion.LookRotation(transform.position - pos, Vector3.up));

            _gurksRemaining++;
            newEnemy.OnDie += () => _gurksRemaining--;

            _currentValue -= enemy.WaveWeight;
        }
        else
        {
            _spawnTimer -= Time.deltaTime;
        }
    }

    private Vector3 GetRandomPos()
    {
        bool CoinFlip() => Random.Range(0, 2) == 1;

        bool isXAxis = CoinFlip();
        bool invert = CoinFlip();

        float x;
        float z;

        if (isXAxis)
        {
            x = Random.Range(-max.x, max.x) / 2f;

            z = Random.Range(min.y, max.y) * (invert ? -1 : 1) / 2f;
        }
        else
        {
            z = Random.Range(-max.y, max.y) / 2f;

            x = Random.Range(min.x, max.x) * (invert ? -1 : 1) / 2f;
        }

        Vector3 result = new Vector3(x, 0, z);

        return result;
    }

    public void Begin()
    {
        _isPlaying = true;
        StartWave(0);
    }

    public void StartWave(int wave)
    {
        Debug.Log("STARTING WAVE " + wave);
        _currentWave = waves[wave];

        _currentValue = _currentWave.Value;
        _spawnTimer = _currentWave.StartDelay;
    }

    [Serializable]
    public class Wave
    {
        public int EnemiesLength
        {
            get
            {
                if (!_enemiesLength.HasValue)
                {
                    _enemiesLength = Enemies.Length;
                }

                return _enemiesLength.Value;
            }
        }
        private int? _enemiesLength = null;
        public Gurk[] Enemies => enemies;
        public float Value => value;
        public Vector2 TimeBtwSpawns => timeBtwSpawns;
        public float StartDelay => startDelay;

        [SerializeField] private Gurk[] enemies;
        [SerializeField] private float value;
        [SerializeField] private Vector2 timeBtwSpawns;
        [SerializeField] private float startDelay;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(transform.position, new(min.x, 0, min.y));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new(max.x, 0, max.y));
    }
}
