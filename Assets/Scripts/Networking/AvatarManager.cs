using UnityEngine;

namespace Networking
{
    [CreateAssetMenu(fileName = "AvatarVariants", menuName = "ScriptableObjects/AvatarManager", order = 1)]
    public class AvatarManager : ScriptableObject
    {
        public Sprite[] AvatarVariants;
    }
}