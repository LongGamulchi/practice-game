using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Map[] maps;
    public int mapIndex;
    public Transform tilePrefab;
    public Transform[] obstaclePrefabs;
    public Transform navmeshFloor;
    public Transform navmeshMaskPrefab;
    public Vector2 maxMapSize;
    [Range(0,1)]
    public float outlinePercent;
    public float tileSize;
    float[] euler = { 0, 90, 180 ,270 };
    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;
    Map currentMap;


    void Start()
    {
        GenerateMap();
    }


    public void GenerateMap()
    {        
        currentMap = maps[mapIndex];
        System.Random prng = new System.Random(currentMap.seed);
        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize, 0.05f, currentMap.mapSize.y * tileSize);
        allTileCoords = new List<Coord>();
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }//모든xy좌표를 저장
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));
        //유틸리티의 알고리즘을 이용해 랜덤으로 바뀐 좌표를 큐에 저장
        

        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }//에디터에서 호출할것이기때문에 destroy대신 다른걸 사용했다.
        //holderName에 생성된 타일과 장애물을 묶어 자식이 존재한다면 파괴한다는 뜻.
        //변경점이 있을떄마다 맵에디터에서 이것이 포함된 함수를 호출할것이고 그에따라 중복되는 타일이 생기지 않도록 파괴한다.


        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for(int x = 0; x< currentMap.mapSize.x; x++)
        {
            for(int y = 0; y< currentMap.mapSize.y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;//소환할 타일
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;//타일이 할당한 테두리 크기만큼 작아진다.
                newTile.parent = mapHolder;
            }
        }//맵 사이즈 크기의 맵 타일 생성


        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];
        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int currentObstacleCount = 0;
        for (int i = 0; i<obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != currentMap.mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))//맵중앙은 비어있고 장애물로 완전히 고립된 구역이 없도록.
            {
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(obstaclePrefabs[Random.Range(0,obstaclePrefabs.Length)], obstaclePosition + Vector3.up * obstacleHeight/2, Quaternion.Euler(0,euler[Random.Range(0,4)],0)) as Transform;
                newObstacle.parent = mapHolder;
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, (1-outlinePercent)*tileSize);

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colourPercent = randomCoord.y / (float)currentMap.mapSize.y;
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColour, currentMap.backgroundColour, colourPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;
            }//가져온 랜덤 좌표가 조건에 맞을때만 그 좌표에 장애물이 생성된다.
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }//조건에 맞지 않다면  장애물이 생성될 위치를 false로 바꾸고 총 장애물 수 --
        }// 랜덤 좌표에 장애물 생성


        Transform maskLeft = Instantiate(navmeshMaskPrefab, Vector3.left * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskRight = Instantiate(navmeshMaskPrefab, Vector3.right * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskTop = Instantiate(navmeshMaskPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        Transform maskBottom = Instantiate(navmeshMaskPrefab, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;
        //최대 맵 사이즈와 현재 맵 사이즈의 가장자리를 위치를 구하고 최대 맵 사이즈와 현재 맵사이즈 사이의 공백 부분을 길찾기 알고리즘에서 커팅한다.
        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;//x각도를 건드렸기에 xy로 크기조절
    }


    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {//Foold fill 알고리즘 만약 이 자리에 장애물이 생성된다면 존재해야 할 비장애물 타일 갯수와, 도달할 수 있는 타일 갯수가 같은지 비교하여 장애물을 생성할지 판단한다.
        //이미 탐색한 타일과 장애물이 생성된 자리를 판단하기 위해 bool배열을 이용하여 장애물이 없거나 아직 탐색하지 않은 타일을 false로 한다.
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();

        queue.Enqueue(currentMap.mapCenter);
        mapFlags[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;
        int accessibleTileCount = 1;
        //맵중앙은 늘 비어있게 만들것임으로 큐에 넣어준다.

        while (queue.Count > 0){
            Coord tile = queue.Dequeue();
            for (int x = -1; x<=1; x++)
            {
                for(int y =-1; y<=1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    //이중 for문으로 랜덤좌표와 그 주변 8좌표를 neighbourXY좌표를 이용하여 확인하게해준다.

                    if (x == 0 || y == 0)//이 조건을 넣음으로써 대각선 좌표는 확인하지 않기로 하였다.
                    {
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >=0 && neighbourY < obstacleMap.GetLength(1))
                        {//좌표의 위치가 장애물 맵 배열보다 크거나 작은경우를 방지한다.
                            if (!mapFlags[neighbourX,neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {//맵 좌표와 장애물 좌표 둘다 flase여야한다 즉 장애물이 없거나 지나온 타일이 아니여야한다.
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }//도달할 수 있는 타일의 수를 세고, 다음 탐색을 시작할 위치를 큐에 저장한다.
                        }
                    }
                }
            }
        } 
        int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }//이러한 과정을 통해 비장애물 타일수 == 도달할 수 있는 타일수 라면 true를 반환한다.


    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
    }//vector2좌표를 벡터3값으로 바꿔준다.


    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }//랜덤 좌표를 순서대로 내보낸다.


    [System.Serializable]
    public struct Coord
    {
        public int x;
        public int y;
        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }
        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }//좌표가 같은지 비교하기 위해 연산자를 따로 정의했다.
    }


    [System.Serializable]
    public class Map
    {
        public Coord mapSize;
        [Range(0, 1)]
        public float obstaclePercent;
        public int seed;
        public float minObstacleHeight;
        public float maxObstacleHeight;
        public Color foregroundColour;
        public Color backgroundColour;
        public Coord mapCenter
        {
            get
            {
                return new Coord(mapSize.x / 2, mapSize.y / 2);
            }
        }
    }
}
