using UnityEngine;
using System.Collections;

public class CoroutineHandler : MonoBehaviour
{
    private static CoroutineHandler instance;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public static void StartStaticCoroutine(IEnumerator coroutine)
    {
        if (instance == null)
        {
            GameObject obj = new GameObject("CoroutineHandler");
            instance = obj.AddComponent<CoroutineHandler>();
            DontDestroyOnLoad(obj);
        }

        instance.StartCoroutine(coroutine);
    }
}
