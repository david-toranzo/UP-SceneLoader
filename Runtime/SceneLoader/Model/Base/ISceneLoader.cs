using System;

namespace ScenesLoaderSystem
{
    public interface ISceneLoader
    {
        Action OnAllScenesAreLoaded { get; set; }
        void LoadScene(SceneData sceneData, bool dontRemoveOpenScenes = false);
        void RemoveCurrentAndSetPrincipal(SceneData currentSceneData);
        bool IsThiSceneDataOpen(SceneData sceneData);
    }
}