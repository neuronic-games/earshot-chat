using UnityEngine;
using Whoo;
using Whoo.Views;
using DiscordAppLayer;
using System;

public class UsersFinder : MonoBehaviour
{
    [SerializeField] private StrapiUserView[] userViews;
    [SerializeField] private TableView[] tableViews;
    [SerializeField] private SeatArrangement[] seatArrangement;

    private void Start()
    {
        FindTables();

        for (int tableIndex = 0; tableIndex < tableViews.Length; tableIndex++)
        {
            tableViews[tableIndex].userChangedPosition += SortUsers;
        }
    }

    private void FindTables() => tableViews = FindObjectsOfType<TableView>();

    private StrapiUserView[] FindUsersFromTable(SeatArrangement seatArrangement) => seatArrangement.GetComponentsInChildren<StrapiUserView>();

    private SeatArrangement[] FindSeatArrangements() => seatArrangement = FindObjectsOfType<SeatArrangement>();

    private void SortUsers(WhooTable seatTable, GameObject currentTableGO)
    {
        FindSeatArrangements();

        for (int seat = 0; seat < seatArrangement.Length; seat++)
        {
            userViews = FindUsersFromTable(seatArrangement[seat]);

            if (currentTableGO.gameObject.GetComponentInChildren<SeatArrangement>() == seatArrangement[seat])
            {
                if (userViews != null)
                {
                    for (int i = 0; i < userViews.Length; i++)
                    {
                        DiscordApp.GetDiscordApp(out DiscordApp app);
                        app.VoiceManager.SetLocalMute(Int64.Parse(userViews[i].User.UniqueId), true);
                        Debug.Log(userViews[i].User.Name);
                    }
                }
            }
        }
    }
}
