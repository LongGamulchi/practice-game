using System.Collections;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class PlayerController : MonoBehaviour{

    Vector3 velocity;
    Rigidbody myRigidbody;
    public Camera playerCamera;





    void Start()    {
        myRigidbody = GetComponent<Rigidbody>();
    }
    public void Move (Vector3 _velocity){
        velocity = _velocity;

    }//�̵�

    public void LookAt(Vector3 lookPoint){
        Vector3 fixedYRocationvalue = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(fixedYRocationvalue);
    }//Ŀ���� �ٶ󺸰� �Ѵ�.

    public void CameraMove(Vector3 playerPosition, Vector3 mousePosition) {
        float distance;
        distance = Vector3.Distance(playerPosition, mousePosition);

        Vector3 oneThirdPosition = Utility.SerchMiddlePoint(playerPosition, mousePosition);
        oneThirdPosition = Utility.SerchMiddlePoint(oneThirdPosition, mousePosition);
        oneThirdPosition = Utility.SerchMiddlePoint(playerPosition, oneThirdPosition);
        Vector3 cameraPosition = new Vector3(oneThirdPosition.x, playerPosition.y + 20, oneThirdPosition.z - 6.5f);
        if (distance < 70)
        {
            playerCamera.transform.position = Vector3.MoveTowards(playerCamera.transform.position, cameraPosition, 5f);
        }
        else
            playerCamera.transform.position = new Vector3(playerPosition.x, playerPosition.y+20, playerPosition.z - 6.5f);
    }//ī�޶� ����
    
    public FieldItems ItemDstance(Vector3 playerPosition, Collider[] initialCollisions)
    {
        float shortDis = Vector3.Distance(playerPosition, initialCollisions[0].transform.position);
        FieldItems item = initialCollisions[0].GetComponent<FieldItems>();
        foreach (Collider found in initialCollisions)
        {
            float distance = Vector3.Distance(playerPosition, found.transform.position);
            if(distance < shortDis)
            {
                item = found.GetComponent<FieldItems>();
                shortDis = distance;
            }
        }
        return item;
    }//������ �������� ���� ����� �������� ��ȯ

    private void FixedUpdate(){
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.fixedDeltaTime);
    }


}
