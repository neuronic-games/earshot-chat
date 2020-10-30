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

        protected override void MiddleClicked(PointerEventData eventData)
        {
            //ignore
        }

        protected override void RightClicked(PointerEventData eventData)
        {
            //ignore
        }

        protected override void LeftClicked(PointerEventData eventData)
        {
            if (Room.RoomGroup.LocalUser.IsSitting()) return;
            Room.SeatLocalUserAtGroup(Room.RoomGroup);
        }

        #endregion
    }
}