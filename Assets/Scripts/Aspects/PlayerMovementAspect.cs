using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct PlayerMovementAspect : IAspect {

    public readonly RefRO<PlayerInputData> input;
    public readonly RefRO<PlayerData> data;
    public readonly RefRW<LocalTransform> transform;
}
