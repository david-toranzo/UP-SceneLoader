using ScenesLoaderSystem;
using System.Collections;
using UnityEngine;

namespace ScenesLoaderSystem
{
    public class AfterStartSceneLoader : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LoadScene _loadScene;

        private void Start()
        {
            StartCoroutine(StartSceneLoad());
        }

        private IEnumerator StartSceneLoad()
        { 
            yield return new WaitForEndOfFrame();
            _loadScene.Load();
        }
    }
}