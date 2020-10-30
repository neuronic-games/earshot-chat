using System;
using AppLayer.NetworkGroups;
using UnityEngine;

namespace Whoo
{
    public class Table : IEquatable<Table>, IDisposable
    {
        public readonly Room                   Room;
        public readonly INetworkGroup          Group;
        public          TableDisplayProperties Properties { get; private set; }

        public Table(Room room, INetworkGroup @group, TableDisplayProperties properties)
        {
            Room  = room;
            Group = @group;
            SetDisplayProperties(properties);
        }

        public void SeatUserHere()
        {
            Room.SeatLocalUserAtGroup(Group);
        }

        #region Display Properties

        public void SetDisplayProperties(TableDisplayProperties properties)
        {
            Properties = properties;
            if (properties != null)
            {
                UpdateGroupWithDisplayProperties();
            }
            else
            {
                LoadDisplayPropertiesFromGroup();
            }
        }

        private void UpdateGroupWithDisplayProperties()
        {
            Rect   rect      = Properties.rect;
            int    numSeats  = Properties.seats;
            string propValue = $"{rect.x},{rect.y},{rect.width},{rect.height},{numSeats}";
            Group.SetOrDeleteCustomProperty(Constants.TableDisplay, propValue);
        }

        public bool LoadDisplayPropertiesFromGroup()
        {
            string fullString = null;
            try
            {
                if (Group.CustomProperties.TryGetValue(Constants.TableDisplay, out fullString))
                {
                    string[] props    = fullString.Split(',');
                    float    posX     = float.Parse(props[0]);
                    float    posY     = float.Parse(props[1]);
                    float    width    = float.Parse(props[2]);
                    float    height   = float.Parse(props[3]);
                    int      numSeats = int.Parse(props[4]);

                    Properties = new TableDisplayProperties()
                    {
                        rect  = new Rect(posX, posY, width, height),
                        seats = numSeats
                    };

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.Log(
                    $"{GetType().Name} : {nameof(LoadDisplayPropertiesFromGroup)} Exception caught. Value of property {fullString}.\n{ex}");
            }

            return false;
        }

        #endregion

        #region IEquatable

        public bool Equals(Table other)
        {
            if (object.ReferenceEquals(other, this)) return true;
            if (other == null) return false;
            return this.Group.Equals(other.Group);
        }

        #endregion

        #region IDisposable

        private bool _disposed = false;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            //DetachListeners(Group);
            Group.LeaveOrDestroy(null);
        }

        #endregion
    }
}