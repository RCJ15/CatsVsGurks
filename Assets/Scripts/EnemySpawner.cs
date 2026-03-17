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

    private float _currentValue;
    private float _spawnTimer;

    private void Start()
    {
        // Start wave 1 test
        StartWave(0);
    }

    private void Update()
    {
        if (_currentValue <= 0)
        {
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
            Instantiate(enemy, pos, Quaternion.LookRotation(transform.position - pos, Vector3.up));

            _currentValue -= enemy.WaveWeight;
        }
        else
        {
            _spawnTimer -= Time.deltaTime;
        }
    }

    private Vector3 GetRandomPos()
    {
        /*
        bool isXAxis = Random.Range(0, 2) == 1;
        bool invert = Random.Range(0, 2) == 1;

        if (isXAxis)
        {
            float x = 
        }
        else
        {

        }

        Vector3 result = Vector3.zero;

        */
        return Vector3.zero;
    }
    
    public void StartWave(int wave)
    {
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
