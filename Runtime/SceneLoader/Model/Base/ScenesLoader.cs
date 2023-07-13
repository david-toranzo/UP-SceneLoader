using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ScenesLoaderSystem
{
    public class ScenesLoader : ISceneLoader
    {
        private readonly SceneData _loadingScreenSceneData;

        private SceneData _currentSceneData;
        private List<SceneData> _openScenes = new List<SceneData>();
        private SceneRemover _sceneRemover = new SceneRemover();
        private List<AsyncOperation> _operationsAsync;
        private const int _waitAsync = 10;

        public Action OnAllScenesAreLoaded { get; set; }

        public ScenesLoader(SceneData loadingScreenSceneData, SceneData firstOpenSceneData)
        {
            _loadingScreenSceneData = loadingScreenSceneData;

            _openScenes.Add(firstOpenSceneData);
            _operationsAsync = new List<AsyncOperation>();
        }

        public async void RemoveCurrentAndSetPrincipal(SceneData currentSceneData)
        {
            await LoadLoadingScreenAsync();

            await _sceneRemover.RemoveScene(_currentSceneData);
            _openScenes.Remove(_currentSceneData);

            _currentSceneData = currentSceneData;

            AllSceneLoaded();
        }

        public async void LoadScene(SceneData sceneData, bool dontRemoveOpenScenes = false)
        {
            _currentSceneData = sceneData;

            await LoadLoadingScreenAsync();

            bool hastoCloseOtherScenes = !sceneData.dontCloseOthersScenes;
            bool mustRemoveOpenScenes = !dontRemoveOpenScenes;

            if (hastoCloseOtherScenes && mustRemoveOpenScenes)
                _openScenes = await _sceneRemover.RemoveScenes(_openScenes, _currentSceneData.removeLockedScenes);

            await RemoveScenesFromCurrentSceneData();

            _operationsAsync.Clear();
            await LoadProcessAsync();
        }

        private async Task RemoveScenesFromCurrentSceneData()
        {
            SceneData[] sceneDatas = _currentSceneData.GetAllSceneDatasToRemove();

            if (ReferenceEquals(sceneDatas, null))
                return;

            foreach (var sceneData in sceneDatas)
            {
                if (!_openScenes.Contains(sceneData))
                    continue;

                await _sceneRemover.RemoveScene(sceneData);
                await Task.Delay(10);
                _openScenes.Remove(sceneData);
            }
        }

        private async Task LoadLoadingScreenAsync()
        {
            if (_currentSceneData.useLoadingScreen == false)
                return;

            AsyncOperation loadLoadingOperation = SceneManager.LoadSceneAsync(_loadingScreenSceneData.Name, LoadSceneMode.Additive);

            while (!loadLoadingOperation.isDone)
            {
                await Task.Yield();
            }
        }

        private async Task LoadProcessAsync()
        {
            AddAllSceneToLoadToOperations();
            await WaitToAllOperationsDoneAsync();

            AllSceneLoaded();
        }

        private void AddAllSceneToLoadToOperations()
        {
            SceneData[] scenesToLoad = _currentSceneData.GetAllScenesToOpen();

            foreach (var sceneData in scenesToLoad)
            {
                if (_openScenes.Contains(sceneData))
                    continue;

                AddSceneToOperation(sceneData);
            }
        }

        private void AddSceneToOperation(SceneData sceneData)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneData.nameScene, LoadSceneMode.Additive);

            _operationsAsync.Add(operation);
            _openScenes.Add(sceneData);
        }

        private async Task WaitToAllOperationsDoneAsync()
        {
            foreach (var operation in _operationsAsync)
            {
                while (!operation.isDone)
                    await Task.Delay(_waitAsync);
            }
        }

        private async void AllSceneLoaded()
        {
            SetPrincipalScene();

            //Added for security reasons beacause not always load the scene correctly, so we need to wait the main thread
            await Task.Delay(10);

            if (_currentSceneData.useLoadingScreen)
                SceneManager.UnloadSceneAsync(_loadingScreenSceneData.Name);

            OnAllScenesAreLoaded?.Invoke();
        }

        private void SetPrincipalScene()
        {
            foreach (var sceneData in _openScenes)
            {
                if (!sceneData.isPrincipal)
                    continue;

                SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneData.nameScene));
            }
        }

        public bool IsThiSceneDataOpen(SceneData sceneData)
        {
            return _openScenes.Contains(sceneData);
        }
    }
}