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
        Instantiate(prefab, transform.position, Quaternion.identity);  //实例化 其中 Quaternion.identity=Quaternion(0,0,0,0)
        Invoke(nameof(Spawn), Random.Range(minTime, maxTime));         //在规定时间范围内随机唤醒调用
    }
}
