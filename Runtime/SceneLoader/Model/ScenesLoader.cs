using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ScenesLoaderSystem
{
    public class ScenesLoader : ISceneLoader
    {
        public Action OnAllScenesLoad { get; set; }

        private SceneData _loadingScreenData;
        private SceneData _currentSceneData;

        private List<AsyncOperation> _operationsAsync;

        private const int _waitAsync = 10;

        public ScenesLoader(SceneData loadingScreenData)
        {
            _loadingScreenData = loadingScreenData;

            InitOperationAsync();
        }

        private void InitOperationAsync()
        {
            _operationsAsync = new List<AsyncOperation>();
        }

        public async void LoadScene(SceneData sceneData)
        {
            _operationsAsync.Clear();
            _currentSceneData = sceneData;

            await LoadProcessAsync();
        }

        private async Task LoadProcessAsync()
        {
            await LoadSceneLoadingScreenAsync();
            await Task.Delay(_waitAsync);

            AddAllSceneToLoadToOperations();
            await WaitToAllOperationsDoneAsync();

            UnloadLoadingScreen();
            await Task.Delay(_waitAsync);

            OnAllOperationsDone();
        }

        private async Task LoadSceneLoadingScreenAsync()
        {
            if (_currentSceneData.useLoadingScreen == false)
                return;

            AsyncOperation loadLoadingOperation = SceneManager.LoadSceneAsync(_loadingScreenData.nameScene, LoadSceneMode.Additive);

            while (!loadLoadingOperation.isDone)
            {
                await Task.Delay(_waitAsync);
            }
        }

        private void AddAllSceneToLoadToOperations()
        {
            SceneData[] scenesToLoad = _currentSceneData.GetAllScenesToOpen();

            foreach (var sceneData in scenesToLoad)
            {
                AddSceneToOperation(sceneData);
            }
        }

        private void AddSceneToOperation(SceneData sceneData)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneData.nameScene, LoadSceneMode.Additive);

            _operationsAsync.Add(operation);
        }

        private async Task WaitToAllOperationsDoneAsync()
        {
            foreach (var operation in _operationsAsync)
            {
                while (!operation.isDone)
                    await Task.Delay(_waitAsync);
            }
        }

        private void UnloadLoadingScreen()
        {
            if (_currentSceneData.useLoadingScreen)
                SceneManager.UnloadSceneAsync(_loadingScreenData.nameScene);
        }

        private void OnAllOperationsDone()
        {
            OnAllScenesLoad?.Invoke();
            Debug.Log("All scene are loaded");
        }
    }
}