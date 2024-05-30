using Source.EasyECS;
using Source.Scripts.EasyECS.Core;
using UnityEngine;

namespace Source.Scripts.ECS.Views
{
    [AddComponentMenu("ECS View/Hero")]
    public class Hero : MonoBehaviour, IEcsComponentInitialize
    {
        public void Initialize(int entity, Componenter componenter)
        {
            componenter.Add<HeroData>(entity);
            Debug.Log($"Initialized hero : {entity}");
        }
    }

    public struct HeroData
    {
        
    }
}