using UnityEngine;
using Whoo;
using Whoo.Views;

public class UsersFinder : MonoBehaviour
{
    [SerializeField] private StrapiUserView[] strapiUserViews;
    [SerializeField] private TableView[] tableViews;

    private RoomView roomView; //roomView.WhooRoom.Tables

    private void Awake()
    {
        roomView = FindObjectOfType<RoomView>();
        roomView.userAdded += FindUsers;
    }

    private void Start()
    {
        FindUsers();
        tableViews = FindObjectsOfType<TableView>();
        for (int i = 0; i < tableViews.Length; i++)
        {
            tableViews[i].userChangedPosition += SortUsers;
        }
    }

    private void FindUsers()
    {
        strapiUserViews = FindObjectsOfType<StrapiUserView>();
    }

    private void SortUsers(string seatTableId)
    {
        for (int strapiUsers = 0; strapiUsers < strapiUserViews.Length; strapiUsers++)
        {
            for (int tables = 0; tables < roomView.WhooRoom.Tables.Count; tables++)
            {
            }
        }
    }
}
