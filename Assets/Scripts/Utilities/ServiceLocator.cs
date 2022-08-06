using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    public ObjectPooler pool;
    [HideInInspector] public GameController gameController;
    [HideInInspector] public ThemeManager themeManager;
    public static ServiceLocator instance;
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}
