using System;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }

    private class TimerData
    {
        public float timeRemaining;
        public Action onComplete;
        public bool active;
    }

    private List<TimerData> activeTimers = new List<TimerData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        for (int i = activeTimers.Count - 1; i >= 0; i--)
        {
            if (!activeTimers[i].active) continue;

            activeTimers[i].timeRemaining -= Time.deltaTime;
            if (activeTimers[i].timeRemaining <= 0)
            {
                activeTimers[i].onComplete?.Invoke();
                activeTimers.RemoveAt(i);
            }
        }
    }

    public void StartTimer(float duration, Action onComplete)
    {
        activeTimers.Add(new TimerData { timeRemaining = duration, onComplete = onComplete, active = true });
    }
}
