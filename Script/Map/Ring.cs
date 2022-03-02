using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Ring : MonoBehaviour
{

    public static float ringDamage;
    public GameObject nextRing;


    public int ringIndex;
    public float[] ringDamages;
    public float[] ringTime;
    public float[] ringReductionTime;
    public Vector3[] ringSize;
    public Vector3 nextRingPosition;
    float radius;
    private void Start()
    {
        ringIndex = 0;
        ringDamage = ringDamages[ringIndex];
        this.transform.localScale = ringSize[ringIndex];
        UpdateNextRing();
    }//시작 라운드 세팅

    private void Update()
    {
        if(Time.time>ringTime[ringIndex] && ringIndex<5)
        {
            ringIndex++;
            RingUpdate();
        }
    }//정해진 라운드 시간이 지나면 작동한다.

    public void RingUpdate()
    {
        this.transform.DOScale(ringSize[ringIndex],ringReductionTime[ringIndex-1]);
        this.transform.DOMove(nextRingPosition, ringReductionTime[ringIndex - 1]);
        StartCoroutine(DamageUpate());
    }//링의 위치와 크기를 업데이트해준다.

    IEnumerator DamageUpate()
    {
        yield return new WaitForSeconds(ringReductionTime[ringIndex - 1]);
        ringDamage = ringDamages[ringIndex];
        UpdateNextRing();
    }//링 축소가 끝나면 링 데미지를 업데이트해준다.

    public void UpdateNextRing()
    {
        radius = Random.Range(0, (ringSize[ringIndex].x - ringSize[ringIndex + 1].x) / 2);
        nextRingPosition = RandomPosition(Random.Range(-radius, radius), radius) + this.transform.position;
        nextRing.transform.position = nextRingPosition + Vector3.up * 10;
        nextRing.transform.localScale = ringSize[ringIndex + 1];
    }//다음 라운드 링의 정보를 생성한다.

    public Vector3 RandomPosition(float x, float r)
    {
        float y = Mathf.Sqrt((Mathf.Pow(r, 2) - Mathf.Pow(x, 2)));
        int negariveDiscrimination = Random.Range(0, 2);
        if (negariveDiscrimination == 0)
        {
            return new Vector3(x, 0, y);
        }
        else
            return new Vector3(x, 0, -y);
    }//x좌표에 따라 y좌표를 찾아준다. x^2 + y^2 = r^2

    public void OnTriggerEnter(Collider collider)
    {        
        if (collider.gameObject.CompareTag("Player"))
        {
            collider.gameObject.GetComponent<Player>().outsideRing = false;
        }
    }
    public void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            collider.gameObject.GetComponent<Player>().outsideRing = true;
        }
    }
}
