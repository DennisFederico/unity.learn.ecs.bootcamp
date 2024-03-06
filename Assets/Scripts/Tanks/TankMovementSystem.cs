using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Tanks {
    public partial struct TankMovementSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<Tank>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var timeDeltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (tankTransform, entity) in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<Tank>().WithNone<PlayerTank>().WithEntityAccess()) {
                var tankPosition = tankTransform.ValueRW.Position;
                
                // This does not modify the actual position of the tank, only the point at which we sample the 3D noise function.
                // This way, every tank is using a different slice and will move along its own different random flow field.
                tankPosition.y = entity.Index;

                var angle = (0.5f + noise.cnoise(tankPosition / 10f)) * 4.0f * math.PI;
                var dir = float3.zero;
                math.sincos(angle, out dir.x, out dir.z);

                // Update the LocalTransform.
                tankTransform.ValueRW.Position += dir * timeDeltaTime * 5.0f;
                tankTransform.ValueRW.Rotation = quaternion.RotateY(angle);
            }
            
            //ROTATE THE TURRET - COULD BE PLACED IN ANOTHER SYSTEM AFTER THIS
            var spin = quaternion.RotateY(SystemAPI.Time.DeltaTime * math.PI);

            foreach (var tank in SystemAPI.Query<RefRW<Tank>>()) {
                var turretTransform = SystemAPI.GetComponentRW<LocalTransform>(tank.ValueRO.Turret);
                // Add a rotation around the Y axis (relative to the parent).
                turretTransform.ValueRW.Rotation = math.mul(spin, turretTransform.ValueRO.Rotation);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }
}