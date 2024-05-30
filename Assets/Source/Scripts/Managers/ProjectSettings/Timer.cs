using System;
using System.Collections.Generic;
using Source.Scripts.Extensions;
using UnityEngine;

namespace Source.Scripts.Managers.ProjectSettings
{
    public class Timer : MonoBehaviour
    {
        private static Timer _instance;
        private static bool _isInitialized;
        private readonly List<Process> _busyProcesses = new List<Process>();
        private readonly List<Process> _freeProcesses = new List<Process>();

        private static void TryInitialize()
        {
            if (!_isInitialized)
            {
                _instance = new GameObject().AddComponent<Timer>();
                _instance.gameObject.name = "ProjectTimer";
                DontDestroyOnLoad(_instance);
            }
        }

        private void FixedUpdate()
        {
            for (var index = _busyProcesses.Count - 1; index >= 0; index--)
            {
                var process = _busyProcesses[index];
                process.TimeRemaining -= Time.fixedDeltaTime;
                if (process.TimeRemaining <= 0)
                {
                    process.OnEnd.Invoke();
                    if (process.IsLoop) process.TimeRemaining += process.Delay;
                    else Release(process);
                }
            }
        }

        private void Release(Process process)
        {
            _busyProcesses.Remove(process);
            _freeProcesses.Add(process);
        }

        private Process Get(float delay, Action onEnd, bool isLoop)
        {
            if (_instance._freeProcesses.Count > 0)
            {
                var process = _instance._freeProcesses.PopFirst();
                _instance._busyProcesses.Add(process);
                return process.SetProcess(delay, onEnd, isLoop);
            }
            else
            {
                var process = new Process(delay, onEnd, isLoop);
                _busyProcesses.Add(process);
                return process;
            }
        }

        public static void Run(float delay, Action onEnd, bool isLoop = false)
        {
            TryInitialize();
            _instance._freeProcesses.Add(_instance.Get(delay, onEnd, isLoop));
        }

        public static void RunOne(float delay, Action onEnd)
        {
            TryInitialize();
            _instance._freeProcesses.Add(_instance.Get(delay, onEnd, false));
        }

        public static void RunLoop(float delay, Action onEnd)
        {
            TryInitialize();
            _instance._freeProcesses.Add(_instance.Get(delay, onEnd, true));
        }
        
        public class Process
        {
            public Process(float delay, Action onEnd, bool isLoop)
            {
                TimeRemaining = delay;
                IsLoop = isLoop;
                Delay = delay;
                OnEnd = onEnd;
            }

            public Process SetProcess(float delay, Action onEnd, bool isLoop)
            {
                TimeRemaining = delay;
                IsLoop = isLoop;
                Delay = delay;
                OnEnd = onEnd;
                return this;
            }

            public float TimeRemaining;
            public bool IsLoop;
            public float Delay;
            public Action OnEnd { get; private set; }
        }
    }
}