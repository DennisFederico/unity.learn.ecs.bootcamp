using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Ecs {
    public class RotationSpeedAuthoring : MonoBehaviour {
        
        [Range(90f, 360f)]
        [SerializeField] private float degreesPerSecond = 360f;
        private class RotationSpeedAuthoringBaker : Baker<RotationSpeedAuthoring> {
            public override void Bake(RotationSpeedAuthoring authoring) {
                
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new RotationSpeed {
                    radiansPerSecond = math.radians(authoring.degreesPerSecond)
                });
            }
        }
    }
    
    public struct RotationSpeed : IComponentData {
        public float radiansPerSecond;
    }
}