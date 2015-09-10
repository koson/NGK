using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataLinkLayer.Message;

//========================================================================================
namespace NGK.CAN.DataLinkLayer.CanPort.IXXAT
{
    //====================================================================================
    /// <summary>
    /// Фильтр для ID сообщений
    /// </summary>
    public class FilterIds
    {
        //--------------------------------------------------------------------------------
        #region Fields And Properties
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Формат кадров для фильтрации
        /// </summary>
        private FrameFormat _Format;
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Формат кадров для фильтрации
        /// </summary>
        public FrameFormat Format
        {
            get { return _Format; }
            set { _Format = value; }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Message identifier (inclusive RTR) to add to the filter list.
        /// for exampel: 0101 0001 1000 1 ([0xxx xxxx xxxx]: 11-bit filter (standart frame) [x]: a bit RTR)
        /// </summary>
        private UInt32 _Code;
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Message identifier (inclusive RTR) to add to the filter list.
        /// For exampel: 0101 0001 1000 1 ([0xxx xxxx xxxx]: 11-bit filter (standart frame) [x]: a bit RTR)
        /// </summary>
        public UInt32 Code
        {
            get { return _Code; }
            set { _Code = value; }
        }
        //--------------------------------------------------------------------------------
        private UInt32 _Mask;
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Mask that specifies the relevant bits within code. 
        /// Relevant bits are specified by a 1 in the corresponding bit position, non relevant bits are 0.
        /// For exampel: 0111 1111 1100 1 ([0xxx xxxx xxxx]: 11-bit mask (standart frame) [x]: a bit mask RTR)
        /// </summary>
        public UInt32 Mask
        {
            get { return _Mask; }
            set { _Mask = value; }
        }
        //--------------------------------------------------------------------------------
        #endregion
        //--------------------------------------------------------------------------------
        #region Constructors
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        public FilterIds()
        {
            _Format = FrameFormat.StandardFrame;
        }
        //--------------------------------------------------------------------------------
        #endregion
        //--------------------------------------------------------------------------------
    }
    //====================================================================================
}
//========================================================================================
// End Of File