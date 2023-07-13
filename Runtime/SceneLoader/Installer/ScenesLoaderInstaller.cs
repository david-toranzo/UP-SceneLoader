using Patterns.ServiceLocator;
using Toranzo.Installer;
using UnityEngine;

namespace ScenesLoaderSystem
{
    public class ScenesLoaderInstaller : MonoInstaller<ISceneLoader>
    {
        [Header("References")]
        [SerializeField] private SceneDataSO _loadingScreenData;
        [SerializeField] private SceneDataSO _firstOpenSceneDataSo;

        protected override ISceneLoader GetDataType()
        {
            ScenesLoader scenesLoader = new ScenesLoader(_loadingScreenData.SceneData, _firstOpenSceneDataSo.SceneData);

            ServiceLocator.Instance.Register<ISceneLoader>(scenesLoader);

            return scenesLoader;
        }
    }
}