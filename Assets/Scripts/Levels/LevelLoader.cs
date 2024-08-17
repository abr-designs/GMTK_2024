using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using Utilities;

namespace Levels
{
    public class LevelLoader : HiddenSingleton<LevelLoader>
    {
        public static LevelDataContainer CurrentLevelDataContainer { get; private set; }
        public static int CurrentLevelIndex { get; private set; } = -1;
        
        private GameObject _currentLevelGameObject;

        [SerializeField]
        private LevelDataContainer[] levels;

        private void LoadLevel(int indexToLoad)
        {
            var count = levels.Length;
            Assert.IsTrue(indexToLoad >= 0);
            Assert.IsTrue(indexToLoad < count);

            TryCleanCurrentLevel();

            var levelInstance = Instantiate(levels[indexToLoad], transform);

            _currentLevelGameObject = levelInstance.gameObject;
            CurrentLevelIndex = indexToLoad;
            CurrentLevelDataContainer = levelInstance;
        }

        private bool TryLoadNextLevel()
        {
            if (CurrentLevelIndex + 1 >= levels.Length)
                return false;

            LoadLevel(CurrentLevelIndex + 1);
            return true;
        }

        private void RestartLevel()
        {
            TryCleanCurrentLevel();

            LoadLevel(CurrentLevelIndex);
        }
        //============================================================================================================//

        private void TryCleanCurrentLevel()
        {
            if (_currentLevelGameObject == null)
                return;
            
            Destroy(_currentLevelGameObject);
        }
        //============================================================================================================//
        public static bool OnLastLevel()
        {
            return CurrentLevelIndex == Instance.levels.Length - 1;
        }

        public static bool LoadNextLevel() => Instance.TryLoadNextLevel();
        
        public static void Restart() => Instance.RestartLevel();

        public static void LoadFirstLevel() => Instance.LoadLevel(0);
        
        //============================================================================================================//

#if UNITY_EDITOR
        public LevelDataContainer[] GetDataContainers()
        {
            return levels;
        }
#endif
        
    }
}