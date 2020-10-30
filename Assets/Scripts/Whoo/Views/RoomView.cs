using UnityEngine.EventSystems;

namespace Whoo.Views
{
    public class RoomView : GroupView
    {
        public Room Room { get; private set; }
        
        public void Setup(Room room)
        {
            base.Setup(room.RoomGroup);
            Room = room;
        }

        #region IPointerDownHandler

        public override void MiddleClick()
        {
            //ignore
        }

        public override void RightClick()
        {
            //ignore
        }

        public override void LeftClick()
        {
            if (Room.RoomGroup.LocalUser.IsSitting()) return;
            Room.SeatLocalUserAtGroup(Room.RoomGroup);
        }

        #endregion
    }
}