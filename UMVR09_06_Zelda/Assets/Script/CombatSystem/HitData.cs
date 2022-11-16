using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem
{


    public enum AttackForce
    {
        light,
        heavy

    }
    public enum SkillState
    {
        normal,
        fever,
        bomb,
        timestop,
        icecube,
    }
    public struct HitData
    {
        public float Damage;
        public AttackForce attackForce;
        public SkillState skillState;
    }

}
