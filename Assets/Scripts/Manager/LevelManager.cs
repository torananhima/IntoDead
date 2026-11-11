using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    List<WorldContainer> containers = new();
    public static LevelManager Instance;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        containers = FindObjectsByType<WorldContainer>(FindObjectsSortMode.None).ToList();

        foreach (WorldContainer container in containers)
        {
            //container.InitializeLoot(true);
        }
    }
}
