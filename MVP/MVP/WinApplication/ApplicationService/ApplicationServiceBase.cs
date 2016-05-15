using System;
using System.Collections.Generic;
using System.Text;
using Common.Controlling;

namespace Mvp.WinApplication
{
    public abstract class ApplicationServiceBase: IApplicationService
    {
        #region Constructors

        public ApplicationServiceBase(string serviceName)
        {
            _ServiceName = serviceName;
            _Application = null;
        }

        #endregion

        #region Fields And Properties

        string _ServiceName;

        public string ServiceName
        {
            get { return _ServiceName; }
        }

        IApplicationController _Application;

        public IApplicationController Application { get { return _Application; } }

        bool IsRegistred { get { return _Application != null; } }

        bool _IsInitialized = false;

        public bool IsInitialized
        {
            get { return _IsInitialized; }
        }

        Status _Status = Status.Stopped;

        public Status Status
        {
            get { return _Status; }
            private set
            {
                if (_Status != value)
                {
                    _Status = value;
                    OnStatusWasChanged();
                }
            } 
        }

        #endregion

        #region Methods

        public virtual void Start()
        {
            if (IsInitialized)
            {
                if (Status != Status.Running)
                {
                    OnStarting();
                    Status = Status.Running;
                    OnStatusWasChanged();
                }
            }
            else
                throw new InvalidOperationException("Попытка запустить не инициализированный сервис");
        }

        public virtual void Stop()
        {
            if (IsInitialized)
            {
                if (Status != Status.Stopped)
                {
                    OnStopping();
                    Status = Status.Stopped;
                    OnStatusWasChanged();
                }
            }
            else
                throw new InvalidOperationException("Попытка остановить не инициализированный сервис");
        }

        public virtual void Suspend()
        {
            if (IsInitialized)
            {
                if (Status == Status.Running)
                {
                    OnSuspending();
                    Status = Status.Paused;
                    OnStatusWasChanged();
                }
            }
            else
                throw new InvalidOperationException("Попытка приостановить не инициализированный сервис");
        }

        public virtual void OnStarting() { }
        public virtual void OnStopping() { }
        public virtual void OnSuspending() { }

        public virtual void Initialize(object context)
        {
            _IsInitialized = true;
        }

        public virtual void Dispose() 
        {
            _IsInitialized = false;
        }

        internal void Register(IApplicationController application)
        {
            if (application == null)
                throw new NullReferenceException();

            if (IsRegistred)
                throw new InvalidOperationException();

            _Application = application;
        }

        protected void OnStatusWasChanged()
        {
            if (StatusWasChanged != null)
            {
                StatusWasChanged(this, new EventArgs());
            }
        }

        #endregion

        #region Events

        public event EventHandler StatusWasChanged;

        #endregion

    }
}
