using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
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
    [SerializeField]
    private Image genderBase = default;

    [SerializeField]
    private Image hair = default;

    [SerializeField]
    private Image faceA = default;

    [SerializeField]
    private Image faceB = default;

    [SerializeField]
    private Image torso = default;

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
        genderBase.overrideSprite = m.baseSprite;

        SetupSelectorAndImagePair(m.hair,  hairSelector,  hair);
        SetupSelectorAndImagePair(m.torso, torsoSelector, torso);
        SetupSelectorAndImagePair(m.faceA, faceASelector, faceA);
        SetupSelectorAndImagePair(m.faceB, faceBSelector, faceB);
    }

    private void FemaleGroupSelected()
    {
        AvatarSprites f = settings.femaleSprites;
        genderBase.overrideSprite = f.baseSprite;

        SetupSelectorAndImagePair(f.hair,  hairSelector,  hair);
        SetupSelectorAndImagePair(f.torso, torsoSelector, torso);
        SetupSelectorAndImagePair(f.faceA, faceASelector, faceA);
        SetupSelectorAndImagePair(f.faceB, faceBSelector, faceB);
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
        var profile = await LoadProfile(profileid);

        genderSelector.Select(profile.avatar.male ? 0 : 1);
        hairSelector.Select(profile.avatar.hair);
        faceASelector.Select(profile.avatar.faceA);
        faceBSelector.Select(profile.avatar.faceB);
        torsoSelector.Select(profile.avatar.torso);
    }

    private static async UniTask<Profile> LoadProfile(string profileid)
    {
        var profileEndpoint = StrapiEndpoints.ProfileEndpoint(profileid);
        string response = (await UnityWebRequest.Get(profileEndpoint).SendWebRequest()).
                          downloadHandler.text;
        Profile profile = new Profile();
        //try-catch
        JsonUtility.FromJsonOverwrite(response, profile);
        return profile;
    }

    public async UniTaskVoid SaveAvatarAsync(string profileid)
    {
        saveButton.interactable = false;

        var profile = await LoadProfile(profileid);

        var updateObj = new
        {
            avatar = new Profile.Avatar()
            {
                male  = genderSelector.CurrentIndex == 0 ? true : false,
                hair  = hairSelector.CurrentIndex,
                faceA = faceASelector.CurrentIndex,
                faceB = faceBSelector.CurrentIndex,
                torso = torsoSelector.CurrentIndex
            },
            id = profile.id
        };

        var profileEndpoint = StrapiEndpoints.ProfileEndpoint(profileid);
        var response = (await UnityWebRequest.Put(profileEndpoint, JsonUtility.ToJson(updateObj)).SendWebRequest()).
                       downloadHandler.text;

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