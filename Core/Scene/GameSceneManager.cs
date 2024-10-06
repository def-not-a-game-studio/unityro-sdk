using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Core.Scene
{
    public class GameSceneManager
    {
        private static float _currentProgress = 0f;

        public static UnityAction<float> SceneLoadingProgress;

        public static async UniTask LoadScene(string sceneName, LoadSceneMode mode)
        {
            _currentProgress = 0f;

            await SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
            if (SceneManager.GetActiveScene().buildIndex > 2)
            {
                await UnloadScene(SceneManager.GetActiveScene().name);
            }

            var op = SceneManager.LoadSceneAsync(sceneName, mode);
            ObserveProgress(op).Forget();
            await op.ToUniTask();
            UpdateProgress(1f);

            await SceneManager.UnloadSceneAsync("Loading");

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        }

        private static async UniTask ObserveProgress(AsyncOperation operation)
        {
            while (!operation.isDone)
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (operation.progress != _currentProgress)
                {
                    UpdateProgress(operation.progress);
                }

                await UniTask.Yield();
            }
        }

        private static void UpdateProgress(float progress)
        {
            _currentProgress = progress;
            SceneLoadingProgress?.Invoke(_currentProgress);
        }

        public static UniTask UnloadScene(string sceneName)
        {
            return SceneManager.UnloadSceneAsync(sceneName).ToUniTask();
        }
    }
}