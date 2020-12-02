using System;
using System.ComponentModel;
using AppLayer.NetworkGroups;
using Cysharp.Threading.Tasks;
using UI;
using Whoo.Data;

namespace Whoo
{
    public class WhooTable : NotifyPropertyChanged, IEquatable<WhooTable>, IDisposable
    {
        public readonly WhooRoom WhooRoom;
        public          bool     IsConnected => Group != null && Group.IsAlive;

        public ZoneInstance ZoneInstance
        {
            get => _zoneInstance;
            set => SetPropertyField(ref _zoneInstance, value);
        }

        public INetworkGroup Group
        {
            get => _group;
            set => SetPropertyField(ref _group, value);
        }

        #region INotifyPropertyChanged

        #endregion

        public WhooTable(WhooRoom whooRoom, ZoneInstance instance)
        {
            WhooRoom      = whooRoom;
            _zoneInstance = instance;
        }

        #region IEquatable

        public bool Equals(WhooTable other)
        {
            if (object.ReferenceEquals(other, this)) return true;
            if (other == null) return false;
            return this.Group.Equals(other.Group);
        }

        #endregion

        #region IDisposable

        private bool          _disposed = false;
        private ZoneInstance  _zoneInstance;
        private INetworkGroup _group;


        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Group?.LeaveOrDestroy(null);
            ZoneInstance = null;
            Group        = null;
        }

        #endregion
        
        public void SeatUserHere()
        {
            WhooRoom.SeatLocalUserAtTableAsync(this).Forget();
        }

        public bool IsLocalUserSitting()
        {
            return Group != null && WhooRoom.CurrentSitting == Group;
        }
    }
}