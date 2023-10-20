using UnityEngine;

public class spawner : MonoBehaviour
{
    public GameObject prefab;
    public float minTime = 2f;
    public float maxTime = 4f;

    private void Start()
    {
        Spawn();
    }

    private void Spawn()
    {
        Instantiate(prefab, transform.position, Quaternion.identity);  //ʵ���� ���� Quaternion.identity=Quaternion(0,0,0,0)
        Invoke(nameof(Spawn), Random.Range(minTime, maxTime));         //�ڹ涨ʱ�䷶Χ��������ѵ���
    }
}
