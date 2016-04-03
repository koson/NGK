using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace NGK.CorrosionMonitoringSystem.Models
{
    public class SystemParametersPivotTable
    {
        #region Constructors
        #endregion

        #region Filds And Properties

        DataTable _ParametersPivotTable;
        public DataTable ParametersPivotTable 
        {
            get { return _ParametersPivotTable; }
        }
        #endregion
    }
}
