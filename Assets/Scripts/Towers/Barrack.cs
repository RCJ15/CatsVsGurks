using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Barrack : Tower
{
    [Space]
    [SerializeField] protected Transform spawnPoint;
    [SerializeField] protected Cat catToSpawn;
    [SerializeField] protected int catLimit;
    [SerializeField] protected float timeBtwSpawns;
    [SerializeField] protected float firstSpawnTimer;
    private float _spawnTimer;

    [Space]
    [SerializeField] private Transform visual;
    [SerializeField] private Image timer;
    [SerializeField] private TMP_Text text;

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
            _spawnTimer = timeBtwSpawns;
            return;
        }

        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0)
        {
            _spawnTimer = timeBtwSpawns;

            Cat cat = Instantiate(catToSpawn, spawnPoint.position, transform.rotation);
            cat.OnDie += OnCatDie;

            _catsSpawned++;
        }
    }

    private void LateUpdate()
    {
        visual.transform.forward = visual.transform.position - HeadPosition.Pos;
        timer.fillAmount = Mathf.Lerp(1, 0, _spawnTimer / timeBtwSpawns);
        text.text = _catsSpawned.ToString() + "/" + catLimit.ToString();
    }

    private void OnCatDie()
    {
        _catsSpawned--;
    }
    public override void Hurt(float Damage, Entity from)
    {
        StartCoroutine(Vibrate(0.5f, 0.2f, 0.2f));
        base.Hurt(Damage, from);
    }
    IEnumerator Vibrate(float frequency, float amplitude, float duration)
    {
        OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.RTouch);
        yield return new WaitForSecondsRealtime(duration);
        OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.RTouch);
    }
}
