using UnityEngine;
using Whoo.Views;
using System.Collections.Generic;

public class UsersFinder : MonoBehaviour
{
    [SerializeField] private StrapiUserView[] strapiUserViews;

    private RoomView roomView; //roomView.WhooRoom.Tables      

    private List<UserView> usersWithoutOwner;

    private void Awake()
    {
        roomView = FindObjectOfType<RoomView>();
        roomView.userAdded += FindAndSortUsers;
    }

    private void FindAndSortUsers()
    {
        strapiUserViews = FindObjectsOfType<StrapiUserView>();

        for (int i = 0; i < strapiUserViews.Length; i++)
        {
            usersWithoutOwner.Add(strapiUserViews[i]);

            foreach (char id in roomView.Group.LocalUser.UniqueId)
            {

            }
        }
    }
}
