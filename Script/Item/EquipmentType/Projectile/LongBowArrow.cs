using UnityEngine;
using System.Collections;
//�߻�ü
public class LongBowArrow : Projectile
{
    public void Crossroad(float _range, float _damage)
    {
        damage = _damage;
        range = _range;
    }

}