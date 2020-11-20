using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CHARACTER_STATE { 
    CS_BIRTH = 0,
    CS_IDLE,
    CS_MOVE,
    CS_SKILL_PRE,
    CS_SKILL,
    CS_SIT,
    CS_BE_HIT,
    CS_BE_HIT_FREE,
    CS_BE_HITDOWN,
    CS_BE_HITSTUN,
    CS_BE_HITBACK,
    CS_BE_HITFLAY,
    CS_BE_HITNAIR,
    CS_BE_HITACUPOINT,
    CS_DIE,
    CS_RELIVE,
    CS_FIGHTIDLE,
    CS_WATER,
    CS_THREEJUMP,
    CS_FOURJUMP,
    CS_FAST_DROP,
}