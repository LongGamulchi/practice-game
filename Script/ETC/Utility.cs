using System.Collections;
using UnityEngine;

//��� ������Ʈ������ �����ϰ� ���̴� �˰������ �����Ѱ��̴�.

public static class Utility
{
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random prng = new System.Random(seed);
        for(int i = 0; i<array.Length -1; i++)
        {
            int randomIndex = prng.Next(i, array.Length);//i~�ִ밪�� ��ġ�� ������ ��ġ�� ���Ҹ� �����ͼ�.
            T tempItem = array[randomIndex];//�� ������ �ӽ÷� �����Ѵ�.
            array[randomIndex] = array[i];//������ġ�� ���Ҹ� ���� i��ġ�� �ְ�
            array[i] = tempItem;//i�� ��ġ�� ������ġ�� ���Ҹ� �ִ´�.
        }//�� �˰����� ������ �ߺ� ���� �迭�� ��� ��ġ�� �����ϰ� ���´ٴ� ���̴�.
         return array;
    }

    public static Vector3 SerchMiddlePoint(Vector3 objectF, Vector3 objectS)
    {
        Vector3 middlePoint = new Vector3((objectF.x + objectS.x) / 2, (objectF.y + objectS.y) / 2, (objectF.z + objectS.z) / 2);
        return middlePoint;
    }//�� ��ǥ�� ����ã��

    public static int compare(Item x, Item y)
    {
        if (x.sortValue == y.sortValue)
        {
            return y.itemStack.CompareTo(x.itemStack);
        }
        else return x.sortValue.CompareTo(y.sortValue);
    }//����� ���� �������ϼ��� �տ����� ����� ���ٸ� ������ ���� �������ϼ��� �տ������� �����Ѵ�.

}
