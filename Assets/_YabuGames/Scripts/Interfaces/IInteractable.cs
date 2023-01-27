using UnityEngine;

namespace _YabuGames.Scripts.Interfaces
{
    public interface IInteractable
    {
        void Merge(int level);
        void AllyMerge(int level);
        void TempMerge();
        int GetLevel();
    }
}