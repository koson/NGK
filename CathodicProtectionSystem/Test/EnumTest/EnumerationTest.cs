using System;
using System.Collections.Generic;
using System.Text;

namespace Test.EnumTest
{
    public static class EnumerationTest
    {
        public enum SomeThingEnum
        {
            ItemOne = 1,
            ItemTwo = 2
        }

        public static void Test()
        {
            SomeThingEnum enm = SomeThingEnum.ItemOne;

            Object obj = Enum.ToObject(typeof(SomeThingEnum), 4); // ������������ 4
            string nm = Enum.GetName(typeof(SomeThingEnum), 4); // ������������ null
            
            nm = Enum.GetName(typeof(SomeThingEnum), 2);
            enm = (SomeThingEnum)Enum.Parse(typeof(SomeThingEnum), nm); //������������ ItemTwo

            enm = (SomeThingEnum)Enum.Parse(typeof(SomeThingEnum), "4"); //������������ 4

            bool result = Enum.IsDefined(typeof(SomeThingEnum), 4);

            Console.WriteLine("����� ������ ������������:");
            foreach(string str in Enum.GetNames(typeof(SomeThingEnum)))
            {
                Console.WriteLine(str);
            }
            
            Console.WriteLine("�������� ������ ������������:");
            foreach(Object item in Enum.GetValues(typeof(SomeThingEnum)))
            {
                Console.WriteLine(item);
            }
            
            Console.WriteLine("���������� � int: {0}", (int)enm);

            try
            {
                enm = (SomeThingEnum)3;
                Console.WriteLine("������� �������� �����, ������� �� ���������� � enum");
                Console.WriteLine("���������� � int: {0}", (int)enm);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
