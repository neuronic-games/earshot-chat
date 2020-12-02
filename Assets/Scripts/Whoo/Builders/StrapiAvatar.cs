using UI;
using UnityEngine;
using UnityEngine.UI;
using Whoo.Data;

namespace Whoo.Builders
{
    public class StrapiAvatar : AvatarBase
    {
        public Image genderBase = default;

        public Image hair = default;

        public Image faceA = default;

        public Image faceB = default;

        public Image torso = default;

        public Profile.AvatarComponent comp = default;

        public override void LoadAvatar()
        {
            if (comp == null)
            {
                Sprite transparent = Whoo.Build.Settings.transparent;
                genderBase.overrideSprite = transparent;
                hair.overrideSprite       = transparent;
                faceA.overrideSprite      = transparent;
                faceB.overrideSprite      = transparent;
                torso.overrideSprite      = transparent;
                return;
            }

            AvatarSprites sprites = comp.male ? Whoo.Build.Settings.maleSprites : Whoo.Build.Settings.femaleSprites;
            genderBase.overrideSprite = sprites.baseSprite;
            hair.overrideSprite       = GetSpriteSafe(sprites.hair,  comp.hair);
            faceA.overrideSprite      = GetSpriteSafe(sprites.faceA, comp.faceA);
            faceB.overrideSprite      = GetSpriteSafe(sprites.faceB, comp.faceB);
            torso.overrideSprite      = GetSpriteSafe(sprites.hair,  comp.torso);

            Sprite GetSpriteSafe(Sprite[] s, int index)
            {
                if (index >= 0 && index < s.Length) return s[index];
                else return Whoo.Build.Settings.transparent;
            }
        }
    }
}