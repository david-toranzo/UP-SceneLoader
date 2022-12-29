using System;
using Patterns.ServiceLocator;
using UnityEngine;

namespace ScenesLoaderSystem
{
    public class LoadScene : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SceneDataSO _sceneDataSO;

        public void Load()
        {
            ServiceLocator.Instance.GetService<ISceneLoader>().LoadScene(_sceneDataSO.SceneData);
        }
    }
}