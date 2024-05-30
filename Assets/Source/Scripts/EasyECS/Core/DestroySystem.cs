﻿using Source.EasyECS;
using Source.Scripts.SignalSystem;
using UnityEngine;

namespace Source.Scripts.EasyECS.Core
{
    public class DestroySystem : EcsSignalListener<CommandKillEntitySignal>
    {
        private EcsFilter _destroyingFilter;
        
        protected override void OnSignal(CommandKillEntitySignal data)
        {
            Componenter.Get<EcsMonoBehaviorData>(data.Entity).Value.DestroyEcsMonoBehavior();
        }

        protected override void Initialize()
        {
            _destroyingFilter = World.Filter<OnDestroyData>().End();
        }

        protected override void Update()
        {
            foreach (var entity in _destroyingFilter)
            {
                ref var onDestroyData = ref Componenter.Get<OnDestroyData>(entity);
                onDestroyData.TimeRemaining -= DeltaTime;
                
                if (onDestroyData.TimeRemaining <= 0)
                {
                    if (onDestroyData.ObjectToDelete != null) onDestroyData.ObjectToDelete.gameObject.SetActive(false);// Object.Destroy(onDestroyData.ObjectToDelete.gameObject);
                    Componenter.DelEntity(entity);
                }
            }
        }
    }
}