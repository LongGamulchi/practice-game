using UnityEngine;

public interface IDamageble{
    void TakeHitEnemy(float damage, RaycastHit hit, GameObject _attackedObj);
    //�ǰݽ� �������̽�
    void TakeDamagePlayer(float damage);
    void TakeDamageEnemy(float damage, GameObject _attackedObj);
    //TakeHit ��������
}
 