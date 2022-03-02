using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    //시야의 각도를 제한해줬다.

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
    }//코루틴을 사용하여 delay마다 시야 내 타겟 감지

    void LateUpdate()
    {
        DrawFieldOfView();
    }//플레이어의 시야 변동에 따라 즉각 업데이트가 일어나도록 모든 업데이트 직후 일어나는 lateupdate를 사용하였다.

    void FindVisibleTargets()
    {
        visibleTargets.Clear();        
        Collider[] inViewRadiusTarget = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        //시야 반경 안의 타겟 오브젝트들을 모두 저장한다.

        for (int i = 0; i < inViewRadiusTarget.Length; i++)
        {
            Transform target = inViewRadiusTarget[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            //타겟 오브젝트 방향 벡터를 구한다.

            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {//자신의 정면방향과 타겟 방향의 각도가 시야 각도보다 좁다면, 즉 시야안에 타겟이 있다면
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obtacleMask))
                {
                    visibleTargets.Add(target);
                }//시야 사이에 장애물이 없다면 타겟 리스트에 추가한다.
            }
        }
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        //시야 해상도와 시야 각도를 곱한 뒤 int로 바꿈으로써 몇개의 세분화된 시야 각도가 나오는지 구하였다.
        float stepAngleSize = viewAngle / stepCount;
        //세분화된 각 시야 각도를 구한것이다.
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
                }//코드 맨 밑 주석 참고
            }
            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }//세분화 된 시야 각도들을 리스트에 저장한다.(10,20,30,40...)

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];
        vertices[0] = Vector3.zero;
        //시야각을 세분화 함으로써, 여러개의 꼭지점이 생겼고, 그 꼭지점을들 연결하여 여러개의 삼각형이 생겼다.
        //각 점들과 삼각형들을 저장할 배열들을 사이즈에 맞게 선언해주었다.


        for(int i = 0; i<vertexCount -1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }//시야 각도가 세분화 됨에 따라, 생기게 된 여러 꼭지점과, 삼각형의 좌표들을 플레이어를 기준점 삼아 저장했다.

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
        //시야 매쉬에 구현한 시야를 시각적으로 보이도록 업데이트한다.
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
    }//각 시야각에서 ray를 쏘아 나오는 결과들을 구조체로 저장한다.

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
    }//코드 맨 밑 주석 참고


    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {//부울값이 이용하여 변환에 회전값이 포함되어있는지 판별한다.
            angleInDegrees += transform.eulerAngles.y;
        }//y축을 기준으로 회전치를 업데이트한다.
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
    }//hit 여부  끝나는 점 위치, 점까지의 거리,  ray가 쏘아진 각도를 저장한다.
       
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
     시야각을 세분화하여 시야를 구현하다 보니 문제가 생겼다.
     세분화 된 시야각이 장애물에 걸쳐 화면을 이동하면 각 시야가 뚝뚝 끊기면서 최신화되는 문제였다.
     그래서 두 연속된 시야각에서 쏘아진 ray의 충돌체가 같은 장애물이 아닐 때, 그러니까 장애물에 시야가 걸쳐있을 때
     연속된 시야 각 정 중앙에 새로운 ray를 쏘아 장애물이 걸친 지점에만 새롭게 더 세분화 된 시야각을 생성하는 방법으로 해결하였다.
     시야가 부드럽게 최신화 될 떄까지 임의의 숫자 edgeFind만큼 반복한다(해상도를 올리는것에 비해 비용적으로 효율적이다.)
     
    또한 두개의 장애물에 시야가 걸쳐 있을 때 역시 똑같은 문제가 생겼는데
    두번째 장애물에 부딧쳤다는것을 인식하지 못해서 생기는 문제였다.
    그렇기에 서로 다른 hit 값을 가졌을 때 뿐만 아니라 다른 조건일때도 인식하게 해주었다.
    시야각마다 hit한 지점의 거리를 저장하고 있는데, 그 거리의 차의 절댓값이 일정 거리보다 길다면 edge를  찾는 과정이 진행되게 해줬다.
     */
}
