using UnityEngine;

namespace _YabuGames.Scripts.Interfaces
{
    public interface IInteractable
    {
        void Merge(int level,IInteractable script);
        void AllyMerge(int takenLevel,IInteractable script);
        void TempMerge(int takenLevel);
        int GetLevel();
    }
}