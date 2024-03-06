using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Tanks {
    public partial struct CannonBallSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<CannonBallAuthoring.CannonBall>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

            var cannonBallJob = new CannonBallJob {
                Ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged),
                DeltaTime = SystemAPI.Time.DeltaTime
            };
            cannonBallJob.Schedule();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }
    }
    
    // IJobEntity relies on source generation to implicitly define a query from the signature of the Execute method.
    // In this case, the implicit query will look for all entities that have the CannonBall and LocalTransform components. 
    [BurstCompile]
    public partial struct CannonBallJob : IJobEntity {
        
        private static readonly float3 Gravity = new float3(0.0f, -9.82f, 0.0f);
        
        public EntityCommandBuffer Ecb;
        public float DeltaTime;
        
        // Execute will be called once for every entity that has a CannonBall and LocalTransform component.
        private void Execute(Entity entity, ref CannonBallAuthoring.CannonBall cannonBall, ref LocalTransform transform) {
            
            transform.Position += cannonBall.Velocity * DeltaTime;

            // if hit the ground
            if (transform.Position.y <= 0.0f) {
                Ecb.DestroyEntity(entity);
            }

            cannonBall.Velocity += Gravity * DeltaTime;
        }
    }
}