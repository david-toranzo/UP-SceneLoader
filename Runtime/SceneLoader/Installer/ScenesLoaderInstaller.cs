using Patterns.ServiceLocator;
using Toranzo.Installer;
using UnityEngine;

namespace ScenesLoaderSystem
{
    public class ScenesLoaderInstaller : MonoInstaller<ISceneLoader>
    {
        [Header("Data scene")]
        [SerializeField] private SceneDataSO _loadingScreenData;

        protected override ISceneLoader GetDataType()
        {
            ScenesLoader scenesLoader = new ScenesLoader(_loadingScreenData.SceneData);

            ServiceLocator.Instance.Register<ISceneLoader>(scenesLoader);

            return scenesLoader;
        }
    }
}