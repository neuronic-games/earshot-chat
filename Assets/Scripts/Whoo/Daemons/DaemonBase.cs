using System;
using Cysharp.Threading.Tasks;

namespace Whoo.Daemons
{
    /// <summary>
    /// Daemons are composition of processes for the application.
    /// They handle their own tasks partially isolated from the system.
    /// If the application runtime configuration satisfies certain rules,
    /// the daemon is run.
    /// </summary>
    public abstract class DaemonBase : IDisposable
    {
        protected WhooRoom Room;

        public DaemonBase(WhooRoom room) => Room = room;

        /// <summary>
        /// Lets the Daemon determine if runtime config allows it to run.
        /// </summary>
        /// <returns></returns>
        public abstract UniTask<bool> CanAttach();

        /// <summary>
        /// Called initially to let the daemon know it is being run.
        /// A daemon should try not to allocate any resources before this call.
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// Called repeatedly on the Daemon to move any asyncronous operations forward.
        /// Daemon may have other async operations, or be entirely reactive.
        /// </summary>
        public virtual void Update()
        {
        }

        public virtual void Dispose()
        {
            Room = null;
        }
    }
}