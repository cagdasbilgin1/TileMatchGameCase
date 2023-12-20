using TileMatchGame.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMatchGame.ScriptableObjects
{
    [CreateAssetMenu(fileName = "BoosterAnimations", menuName = "CollapseBlast/BoosterAnimationsSO", order = 2)]
    public class BoosterAnimationSO : ScriptableObject
    {
        [SerializeField] List<GameObject> boosterAnimations;

        public List<GameObject> BoosterAnimations => boosterAnimations;
    }
}

