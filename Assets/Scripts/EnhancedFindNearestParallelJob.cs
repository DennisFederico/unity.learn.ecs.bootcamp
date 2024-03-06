using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct EnhancedFindNearestParallelJob : IJobParallelFor {

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
    // An IJobParallelFor Execute() method takes an index parameter and is called once for each index, from 0 up to the index count:
    public void Execute(int index) {
        var seekerPos = seekerPositions[index];

        // Find the target with the closest X coordinate.
        int startIdx = targetPositions.BinarySearch(seekerPos, new AxisXComparer());

        // When no precise match is found, BinarySearch returns the bitwise negation of the last-searched offset.
        // So when startIdx is negative, we flip the bits again, but we then must ensure the index is within bounds.
        if (startIdx < 0) startIdx = ~startIdx;
        if (startIdx >= targetPositions.Length) startIdx = targetPositions.Length - 1;

        // The position of the target with the closest X coordinate.
        var nearestTargetPos = targetPositions[startIdx];
        var nearestDistSq = math.distancesq(seekerPos, nearestTargetPos);

        // Searching upwards through the array for a closer target.
        Search(seekerPos, startIdx + 1, targetPositions.Length, +1, ref nearestTargetPos, ref nearestDistSq);

        // Search downwards through the array for a closer target.
        Search(seekerPos, startIdx - 1, -1, -1, ref nearestTargetPos, ref nearestDistSq);

        nearestTargetPositions[index] = nearestTargetPos;
    }

    [BurstCompile]
    void Search(float3 seekerPos, int startIdx, int endIdx, int step, ref float3 nearestTargetPos, ref float nearestDistSq) {
        for (int i = startIdx; i != endIdx; i += step) {
            float3 targetPos = targetPositions[i];
            float xDiff = seekerPos.x - targetPos.x;

            // If the square of the x distance is greater than the current nearest, we can stop searching.
            if ((xDiff * xDiff) > nearestDistSq) break;

            float distSq = math.distancesq(targetPos, seekerPos);

            if (distSq < nearestDistSq) {
                nearestDistSq = distSq;
                nearestTargetPos = targetPos;
            }
        }
    }
}

[BurstCompile]
public struct AxisXComparer : IComparer<float3> {

    [BurstCompile]
    public int Compare(float3 a, float3 b) {
        return a.x.CompareTo(b.x);
    }
}