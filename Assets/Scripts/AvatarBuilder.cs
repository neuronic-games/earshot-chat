using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Whoo;
using Whoo.Data;

public class AvatarBuilder : MonoBehaviour
{
    #region Serialized

    [SerializeField]
    private WhooSettings settings = default;

    [Header("UI Views")]
    public StrapiAvatar avatar;

    [Header("UI Controls"), SerializeField]
    private HorizontalSelector genderSelector = default;

    [SerializeField]
    private HorizontalSelector hairSelector = default;

    [SerializeField]
    private HorizontalSelector faceASelector = default;

    [SerializeField]
    private HorizontalSelector faceBSelector = default;

    [SerializeField]
    private HorizontalSelector torsoSelector = default;

    [Header("Other Controls")]
    [SerializeField]
    private Button closeButton;

    [SerializeField]
    private Button saveButton;

    #endregion

    public string profileId;

    public void Awake() => Setup();

    #region Setup

    private void Setup()
    {
        if (Whoo.Build.Settings == null) Whoo.Build.Settings = settings;
        SetupUI();
        LoadAvatarAsync(profileId).Forget();
    }

    private void SetupUI()
    {
        AvatarSprites m = settings.maleSprites;
        AvatarSprites f = settings.femaleSprites;
        var maleValue = new HorizontalSelector.Item()
        {
            label  = "Male",
            sprite = m.baseSprite
        };
        maleValue.onValueChanged.AddListener(MaleGroupSelected);
        genderSelector.AddItem(maleValue);
        var femaleValue = new HorizontalSelector.Item()
        {
            label  = "Female",
            sprite = f.baseSprite
        };
        femaleValue.onValueChanged.AddListener(FemaleGroupSelected);
        genderSelector.AddItem(femaleValue);

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(Close);
        closeButton.interactable = true;

        saveButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener(Save);
        saveButton.interactable = true;
    }

    private void MaleGroupSelected()
    {
        AvatarSprites m = settings.maleSprites;
        avatar.genderBase.overrideSprite = m.baseSprite;

        SetupSelectorAndImagePair(m.hair,  hairSelector,  avatar.hair);
        SetupSelectorAndImagePair(m.torso, torsoSelector, avatar.torso);
        SetupSelectorAndImagePair(m.faceA, faceASelector, avatar.faceA);
        SetupSelectorAndImagePair(m.faceB, faceBSelector, avatar.faceB);
    }

    private void FemaleGroupSelected()
    {
        AvatarSprites f = settings.femaleSprites;
        avatar.genderBase.overrideSprite = f.baseSprite;

        SetupSelectorAndImagePair(f.hair,  hairSelector,  avatar.hair);
        SetupSelectorAndImagePair(f.torso, torsoSelector, avatar.torso);
        SetupSelectorAndImagePair(f.faceA, faceASelector, avatar.faceA);
        SetupSelectorAndImagePair(f.faceB, faceBSelector, avatar.faceB);
    }

    //todo -- eliminate all the reinstantiation.

    private void SetupSelectorAndImagePair(Sprite[] sprites, HorizontalSelector selector, Image image)
    {
        selector.Clear();
        foreach (var sprite in sprites)
        {
            var item = new HorizontalSelector.Item() {sprite = sprite};
            item.onValueChanged.AddListener(() => image.overrideSprite = sprite);
            selector.AddItem(item);
        }
    }

    #endregion

    #region Strapi Save/Load

    public async UniTaskVoid LoadAvatarAsync(string profileid)
    {
        Profile profile = new Profile {id = profileid};
        await profile.GetAsync();

        RefreshUI(profile);
    }

    private void RefreshUI(Profile profile)
    {
        avatar.comp = profile.profileAvatar;
        avatar.LoadAvatar();
        genderSelector.Select(profile.profileAvatar.male ? 0 : 1);
        hairSelector.Select(profile.profileAvatar.hair);
        faceASelector.Select(profile.profileAvatar.faceA);
        faceBSelector.Select(profile.profileAvatar.faceB);
        torsoSelector.Select(profile.profileAvatar.torso);
    }

    public async UniTaskVoid SaveAvatarAsync(string profileid)
    {
        saveButton.interactable = false;

        var profile = new Profile {id = profileid};
        await profile.GetAsync();

        var updateObj = new Profile
        {
            profileAvatar = new Profile.AvatarComponent()
            {
                male  = genderSelector.CurrentIndex == 0,
                hair  = hairSelector.CurrentIndex,
                faceA = faceASelector.CurrentIndex,
                faceB = faceBSelector.CurrentIndex,
                torso = torsoSelector.CurrentIndex,
                id    = profile.profileAvatar.id
            }
        };

        await profile.PutAsync(updateObj);

        RefreshUI(profile);

        saveButton.interactable = true;
    }

    public void Close()
    {
        CloseAsync(gameObject.scene, saveButton, closeButton).Forget();
    }

    public void Save()
    {
        SaveAvatarAsync(profileId).Forget();
    }

    public static async UniTaskVoid CloseAsync(Scene scene, params Selectable[] toDisable)
    {
        foreach (var selectable in toDisable)
        {
            selectable.interactable = false;
        }

        await SceneManager.UnloadSceneAsync(scene);
    }

    #endregion
}