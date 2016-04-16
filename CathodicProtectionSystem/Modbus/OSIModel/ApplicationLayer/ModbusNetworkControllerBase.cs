using System;
using System.Collections.Generic;
using System.Text;
using Common.Controlling;

namespace Modbus.OSIModel.ApplicationLayer
{
    public abstract class ModbusNetworkControllerBase : IManageable
    {
        #region Fields And Properties
        
        public abstract WorkMode Mode { get; }
        public abstract string NetworkName { get; set; }

        #endregion
        
        #region Methods

        public abstract void Start();

        public abstract void Stop();

        public abstract void Suspend();

        public abstract Status Status { get; set; }

        protected void OnStatusWasChanged()
        {
            EventArgs args = new EventArgs();
            EventHandler handler = StatusWasChanged;

            if (handler != null)
            {
                foreach (EventHandler singleCast in handler.GetInvocationList())
                {
                    System.ComponentModel.ISynchronizeInvoke syncInvoke =
                        singleCast.Target as System.ComponentModel.ISynchronizeInvoke;

                    if (syncInvoke != null)
                    {
                        if (syncInvoke.InvokeRequired)
                        {
                            syncInvoke.Invoke(singleCast, new Object[] { this, args });
                        }
                        else
                        {
                            singleCast(this, args);
                        }
                    }
                    else
                    {
                        singleCast(this, args);
                    }
                }
            }
        }

        #endregion

        #region Events

        public event EventHandler StatusWasChanged;

        #endregion
    }
}
