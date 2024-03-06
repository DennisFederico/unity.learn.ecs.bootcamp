using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct FindNearestJob : IJob {

    // All of the data which a job will access should be included in its fields.
    // In this case, the job needs three arrays of float3.

    // Array and collection fields that are only read in the job should be marked with the ReadOnly attribute.
    // Although not strictly necessary in this case, marking data as ReadOnly may allow the job scheduler to safely run 
    // more jobs concurrently with each other.

    [ReadOnly] public NativeArray<float3> targetPositions;
    [ReadOnly] public NativeArray<float3> seekerPositions;

    // For SeekerPositions[i], we will assign the nearest target position to NearestTargetPositions[i].
    public NativeArray<float3> nearestTargetPositions;

    // 'Execute' is the only method of the IJob interface. When a worker thread executes the job, it calls this method.
    [BurstCompile]
    public void Execute()  {
        // Compute the square distance from each seeker to every target.
        for (var i = 0; i < seekerPositions.Length; i++) {
            float3 seekerPos = seekerPositions[i];
            var nearestDistSq = float.MaxValue;
            foreach (var targetPos in targetPositions) {
                var distSq = math.distancesq(seekerPos, targetPos);
                if (!(distSq < nearestDistSq)) continue;
                nearestDistSq = distSq;
                nearestTargetPositions[i] = targetPos;
            }
        }
    }
}