using UnityEngine;

public interface IDamageble{
    void TakeHitEnemy(float damage, RaycastHit hit, GameObject _attackedObj);
    //피격시 인터페이스
    void TakeDamagePlayer(float damage);
    void TakeDamageEnemy(float damage, GameObject _attackedObj);
    //TakeHit 간략버젼
}
 