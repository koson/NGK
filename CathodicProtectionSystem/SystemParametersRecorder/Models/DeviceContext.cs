using System;
using System.Collections.Generic;
using System.Text;

namespace SystemParametersRecorder.Models
{
    public class DeviceContext: IDevice
    {
        #region Constructors

        public DeviceContext()
        { 
        }

        #endregion


        #region IDevice Members

        public int CanNetworkId
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string CanNetworkName
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public byte NodeId
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string Location
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public float? PolarisationPotential
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public float? PolarisationCurrent
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public float? ProtectionPotential
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public float? ProtectionCurrent
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public uint? CorrosionDepth
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public uint? CorrosionSpeed
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion
    }
}
