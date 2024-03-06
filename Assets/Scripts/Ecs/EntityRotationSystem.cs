using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Ecs {
    public partial struct EntityRotationSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<RotationSpeed>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (transform, rotationSpeed) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotationSpeed>>()) {
                var rotation = rotationSpeed.ValueRO.radiansPerSecond * deltaTime;
                var rotateY = transform.ValueRW.RotateY(rotation);
                transform.ValueRW = rotateY.RotateX(rotation);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }
}