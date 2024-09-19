using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct PlayerProjectileDataAspect : IAspect {

    public readonly Entity entity;
    public readonly RefRW<LocalTransform> transform;
    public readonly RefRW<PlayerProjectileData> data;

}
