using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct EnemyDataAspect : IAspect {

    public readonly RefRO<EnemyData> data;
    public readonly RefRW<LocalTransform> transform;

}
