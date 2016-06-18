using System;
using System.Collections.Generic;
using System.Text;
using Test.DataTypes;
using Test.ControllerTest;
using Test.DeviceTest;
using Test.EnumTest;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //DataTypesTest.TestDataTypeInfo_GetTotalValue();
            //DataTypesTest.TestDataTypeInfo_SetValue();
            //DataTypesTest.TestNgkInt16Convertor();
            DataTypesTest.TestNgkFloatConvertor();
            //NetworkControllerTest.NetworkTest();
            //NetworkControllerTest.SerializeNetworkControllerCollectionTest();
            //DeviceClassTest.ToStingTest();
            //DeviceClassTest.CreateFromStringTest();
            //EnumerationTest.Test();
            Console.ReadLine();
        }
    }
}
