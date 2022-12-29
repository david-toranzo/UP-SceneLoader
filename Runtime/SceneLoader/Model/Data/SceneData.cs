using System.Collections.Generic;

namespace ScenesLoaderSystem
{
    [System.Serializable]
    public class SceneData
    {
        public string nameScene;
        public bool useLoadingScreen = true;

        public SceneDataSO[] scenesData;

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

                scenesToOpen.Add(sceneData);
            }

            scenesToOpen.Add(this);

            return scenesToOpen.ToArray();
        }
    }
}