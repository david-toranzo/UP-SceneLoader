using System.Collections.Generic;

namespace ScenesLoaderSystem
{
    [System.Serializable]
    public class SceneData
    {
        public string nameScene;
        public bool useLoadingScreen = true;
        public bool isLockedScene;
        public bool removeLockedScenes;
        public bool isPrincipal;
        public bool dontCloseOthersScenes;
        public bool hasToKeepOpen;

        public SceneDataSO[] scenesData;
        public SceneDataSO[] _scenesDataToRemove;

        public string Name => nameScene;

        public SceneData[] GetAllScenesToOpen()
        {
            List<SceneData> scenesToOpen = new List<SceneData>();

            foreach (var sceneDataSO in scenesData)
            {
                SceneData sceneData = sceneDataSO.SceneData;
                SceneData[] scenesIntoSceneData = sceneData.GetAllScenesToOpen();

                foreach (var sceneDataInto in scenesIntoSceneData)
                {
                    if (scenesToOpen.Contains(sceneDataInto))
                        continue;

                    scenesToOpen.Add(sceneDataInto);
                }

                if (scenesToOpen.Contains(sceneData))
                    continue;

                scenesToOpen.Add(sceneData);
            }

            scenesToOpen.Add(this);

            return scenesToOpen.ToArray();
        }

        public SceneData[] GetAllSceneDatasToRemove()
        {
            if (ReferenceEquals(_scenesDataToRemove, null))
                return null;

            List<SceneData> scenesToRemove = new List<SceneData>();

            foreach (var sceneDataSO in _scenesDataToRemove)
            {
                scenesToRemove.Add(sceneDataSO.SceneData);
            }

            return scenesToRemove.ToArray();
        }
    }
}