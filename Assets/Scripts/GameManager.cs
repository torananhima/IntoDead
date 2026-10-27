using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static Player player;
    private static GameManager instance;

    public static GameManager GetInstance()
    {
        if(instance == null)
        {
            instance = new GameManager();
        }
        return instance;
    }
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        player = FindAnyObjectByType<Player>().GetComponent<Player>();
    }
}
