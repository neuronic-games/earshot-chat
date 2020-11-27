using UI;
using UnityEngine.EventSystems;

namespace Whoo.Views
{
    public class RoomView : GroupView, IClickDownHandler
    {
        public Room Room { get; private set; }
        
        public void Setup(Room room)
        {
            base.Setup(room.RoomGroup);
            Room = room;
        }

        #region IPointerDownHandler

        public void MiddleClick(PointerEventData ptrData)
        {
            //ignore
        }

        public void RightClick(PointerEventData ptrData)
        {
            //ignore
        }

        public void LeftClick(PointerEventData ptrData)
        {
            if (Room.RoomGroup.LocalUser.IsSitting()) return;
            Room.SeatLocalUserAtGroup(Room.RoomGroup);
        }

        #endregion
    }
}