using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    //�þ��� ������ ���������.

    public LayerMask targetMask;
    public LayerMask obtacleMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    public float meshResolution;
    public int edgeFind;
    public float edgeDstThreshold;
    public MeshFilter viewMeshFilter;
    Mesh viewMesh;


    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        StartCoroutine("FindTarget", .2f);
    }
    IEnumerator FindTarget(float delay) {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }//�ڷ�ƾ�� ����Ͽ� delay���� �þ� �� Ÿ�� ����

    void LateUpdate()
    {
        DrawFieldOfView();
    }//�÷��̾��� �þ� ������ ���� �ﰢ ������Ʈ�� �Ͼ���� ��� ������Ʈ ���� �Ͼ�� lateupdate�� ����Ͽ���.

    void FindVisibleTargets()
    {
        visibleTargets.Clear();        
        Collider[] inViewRadiusTarget = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        //�þ� �ݰ� ���� Ÿ�� ������Ʈ���� ��� �����Ѵ�.

        for (int i = 0; i < inViewRadiusTarget.Length; i++)
        {
            Transform target = inViewRadiusTarget[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            //Ÿ�� ������Ʈ ���� ���͸� ���Ѵ�.

            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {//�ڽ��� �������� Ÿ�� ������ ������ �þ� �������� ���ٸ�, �� �þ߾ȿ� Ÿ���� �ִٸ�
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obtacleMask))
                {
                    visibleTargets.Add(target);
                }//�þ� ���̿� ��ֹ��� ���ٸ� Ÿ�� ����Ʈ�� �߰��Ѵ�.
            }
        }
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        //�þ� �ػ󵵿� �þ� ������ ���� �� int�� �ٲ����ν� ��� ����ȭ�� �þ� ������ �������� ���Ͽ���.
        float stepAngleSize = viewAngle / stepCount;
        //����ȭ�� �� �þ� ������ ���Ѱ��̴�.
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);
            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit &&edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if(edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }//�ڵ� �� �� �ּ� ����
            }
            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }//����ȭ �� �þ� �������� ����Ʈ�� �����Ѵ�.(10,20,30,40...)

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];
        vertices[0] = Vector3.zero;
        //�þ߰��� ����ȭ �����ν�, �������� �������� �����, �� ���������� �����Ͽ� �������� �ﰢ���� �����.
        //�� ����� �ﰢ������ ������ �迭���� ����� �°� �������־���.


        for(int i = 0; i<vertexCount -1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }//�þ� ������ ����ȭ �ʿ� ����, ����� �� ���� ��������, �ﰢ���� ��ǥ���� �÷��̾ ������ ��� �����ߴ�.

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
        //�þ� �Ž��� ������ �þ߸� �ð������� ���̵��� ������Ʈ�Ѵ�.
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obtacleMask)){
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }//�� �þ߰����� ray�� ��� ������ ������� ����ü�� �����Ѵ�.

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;
        for(int i = 0; i < edgeFind; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);
            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }
        return new EdgeInfo(minPoint, maxPoint);
    }//�ڵ� �� �� �ּ� ����


    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {//�οﰪ�� �̿��Ͽ� ��ȯ�� ȸ������ ���ԵǾ��ִ��� �Ǻ��Ѵ�.
            angleInDegrees += transform.eulerAngles.y;
        }//y���� �������� ȸ��ġ�� ������Ʈ�Ѵ�.
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo{
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }//hit ����  ������ �� ��ġ, �������� �Ÿ�,  ray�� ����� ������ �����Ѵ�.
       
    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
    /*
     �þ߰��� ����ȭ�Ͽ� �þ߸� �����ϴ� ���� ������ �����.
     ����ȭ �� �þ߰��� ��ֹ��� ���� ȭ���� �̵��ϸ� �� �þ߰� �Ҷ� ����鼭 �ֽ�ȭ�Ǵ� ��������.
     �׷��� �� ���ӵ� �þ߰����� ����� ray�� �浹ü�� ���� ��ֹ��� �ƴ� ��, �׷��ϱ� ��ֹ��� �þ߰� �������� ��
     ���ӵ� �þ� �� �� �߾ӿ� ���ο� ray�� ��� ��ֹ��� ��ģ �������� ���Ӱ� �� ����ȭ �� �þ߰��� �����ϴ� ������� �ذ��Ͽ���.
     �þ߰� �ε巴�� �ֽ�ȭ �� ������ ������ ���� edgeFind��ŭ �ݺ��Ѵ�(�ػ󵵸� �ø��°Ϳ� ���� ��������� ȿ�����̴�.)
     
    ���� �ΰ��� ��ֹ��� �þ߰� ���� ���� �� ���� �Ȱ��� ������ ����µ�
    �ι�° ��ֹ��� �ε��ƴٴ°��� �ν����� ���ؼ� ����� ��������.
    �׷��⿡ ���� �ٸ� hit ���� ������ �� �Ӹ� �ƴ϶� �ٸ� �����϶��� �ν��ϰ� ���־���.
    �þ߰����� hit�� ������ �Ÿ��� �����ϰ� �ִµ�, �� �Ÿ��� ���� ������ ���� �Ÿ����� ��ٸ� edge��  ã�� ������ ����ǰ� �����.
     */
}
