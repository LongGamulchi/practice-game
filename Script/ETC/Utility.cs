using System.Collections;
using UnityEngine;

//어드 프로젝트에서든 유용하게 쓰이는 알고리즘들을 저장한것이다.

public static class Utility
{
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random prng = new System.Random(seed);
        for(int i = 0; i<array.Length -1; i++)
        {
            int randomIndex = prng.Next(i, array.Length);//i~최대값의 위치중 랜덤한 위치의 원소를 가져와서.
            T tempItem = array[randomIndex];//그 원소을 임시로 저장한다.
            array[randomIndex] = array[i];//랜덤위치의 원소를 기존 i위치에 넣고
            array[i] = tempItem;//i의 위치에 랜덤위치의 원소를 넣는다.
        }//이 알고리즘의 장점은 중복 없이 배열의 모든 위치를 랜덤하게 섞는다는 점이다.
         return array;
    }

    public static Vector3 SerchMiddlePoint(Vector3 objectF, Vector3 objectS)
    {
        Vector3 middlePoint = new Vector3((objectF.x + objectS.x) / 2, (objectF.y + objectS.y) / 2, (objectF.z + objectS.z) / 2);
        return middlePoint;
    }//두 좌표의 중점찾기

    public static int compare(Item x, Item y)
    {
        if (x.sortValue == y.sortValue)
        {
            return y.itemStack.CompareTo(x.itemStack);
        }
        else return x.sortValue.CompareTo(y.sortValue);
    }//밸류가 낮은 아이템일수록 앞에오며 밸류가 같다면 스택이 높은 아이템일수록 앞에오도록 정렬한다.

}
