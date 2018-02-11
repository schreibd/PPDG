using UnityEngine;
using System.Collections;
using System;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static bool _instantiated;

    public static T Instance
    {
        get
        {
            if (_instantiated) return _instance;

            var type = typeof(T);
            var objects = FindObjectsOfType<T>();

            if (objects.Length > 0)
            {
                Instance = objects[0];
                if (objects.Length > 1)
                {
                    Debug.LogWarning("There is more than one instance of Singleton of type \"" + type + "\". Keeping the first. Destroying the others.");
                    for (var i = 1; i < objects.Length; i++) DestroyImmediate(objects[i].gameObject);
                }
                _instantiated = true;
                return _instance;
            }

            var attribute = Attribute.GetCustomAttribute(type, typeof(PrefabAttribute)) as PrefabAttribute;
            if (attribute == null)
            {
                Debug.LogError("There is no Prefab Atrribute for Singleton of type \"" + type + "\".");
                return null;
            }
            var prefabPath = attribute.Name;
            if (String.IsNullOrEmpty(prefabPath))
            {
                Debug.LogError("Prefab name is empty for Singleton of type \"" + type + "\".");
                return null;
            }

            var prefab = Resources.Load<GameObject>(prefabPath);
            var gameObject = Instantiate(prefab) as GameObject;
            if (gameObject == null)
            {
                Debug.LogError("Could not find Prefab \"" + prefabPath + "\" on Resources for Singleton of type \"" + type + "\".");
                return null;
            }
            gameObject.name = prefab.name;
            Instance = gameObject.GetComponent<T>();
            if (!_instantiated)
            {
                Debug.LogWarning("There wasn't a component of type \"" + type + "\" inside prefab \"" + prefabPath + "\". Creating one.");
                Instance = gameObject.AddComponent<T>();
            }
            if (attribute.Persistent)
                DontDestroyOnLoad(_instance.gameObject);
            return _instance;
        }

        private set
        {
            _instance = value;
            _instantiated = value != null;
        }
    }

    private void OnDestroy() { _instantiated = false; }
}

