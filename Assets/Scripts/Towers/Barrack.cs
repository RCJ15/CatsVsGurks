using System.Collections.Generic;
using UnityEngine;

public class Barrack : Tower
{
    [Space]
    [SerializeField] protected Transform spawnPoint;
    [SerializeField] protected Cat catToSpawn;
    [SerializeField] protected int catLimit;
    [SerializeField] protected float timeBtwSpawns;
    [SerializeField] protected float firstSpawnTimer;
    private float _spawnTimer;

    private List<Cat> _cats = new();
    private int _catsSpawned;

    protected override void Awake()
    {
        base.Awake();

        _spawnTimer = firstSpawnTimer;
    }

    private void Update()
    {
        if (_catsSpawned >= catLimit)
        {
            return;
        }

        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0)
        {
            _spawnTimer = timeBtwSpawns;

            _cats.Add(Instantiate(catToSpawn, spawnPoint.position, transform.rotation));
            _catsSpawned++;
        }
    }
}
