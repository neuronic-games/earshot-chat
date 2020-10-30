using UI.ContextMenu;

namespace Whoo.Views
{
    public class TableContextMenu : ContextMenuArea
    {
        public TableView view;

        public TableStopwatch watch;

        public override void Start()
        {
            base.Start();
            if (view == null)
            {
                view = GetComponentInParent<TableView>();
            }

            if (watch == null)
            {
                watch = GetComponentInChildren<TableStopwatch>();
            }
        }

        protected override ContextMenu GetContextMenu()
        {
            ContextMenu menu = new ContextMenu();
            menu.SetTitle("Table");

            if (view.Group.LocalUser.IsSitting())
            {
                menu.RegisterAction("To Room", ToRoom);
            }
            else
            {
                menu.RegisterAction("Sit at Table", Sit);
            }

            if (watch != null && view.Table.Group.IsOwner)
            {
                if (watch.IsRunning)
                {
                    if(watch.CanStop)
                        menu.RegisterAction("Stop Stopwatch.", watch.StopStopwatch);
                }
                else
                {
                    menu.RegisterAction("Start Stopwatch.", watch.StartStopwatch);
                    if (!watch.IsCleared)
                    {
                        menu.RegisterAction("Clear Timer", watch.ClearTimer);
                    }
                }
            }

            return menu;
        }

        private void Sit()
        {
            view.Table.SeatUserHere();
        }

        private void ToRoom()
        {
            view.Table.Room.SeatLocalUserAtGroup(view.Table.Room.RoomGroup);
        }
    }
}