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

            Object obj = Enum.ToObject(typeof(SomeThingEnum), 4); // Возвращается 4
            string nm = Enum.GetName(typeof(SomeThingEnum), 4); // Возвращается null
            
            nm = Enum.GetName(typeof(SomeThingEnum), 2);
            enm = (SomeThingEnum)Enum.Parse(typeof(SomeThingEnum), nm); //Возвращается ItemTwo

            enm = (SomeThingEnum)Enum.Parse(typeof(SomeThingEnum), "4"); //Возвращается 4

            bool result = Enum.IsDefined(typeof(SomeThingEnum), 4);

            Console.WriteLine("Имена членов перечисления:");
            foreach(string str in Enum.GetNames(typeof(SomeThingEnum)))
            {
                Console.WriteLine(str);
            }
            
            Console.WriteLine("Значения членов перечисления:");
            foreach(Object item in Enum.GetValues(typeof(SomeThingEnum)))
            {
                Console.WriteLine(item);
            }
            
            Console.WriteLine("Приведение к int: {0}", (int)enm);

            try
            {
                enm = (SomeThingEnum)3;
                Console.WriteLine("Удалось привести число, которое не содержится в enum");
                Console.WriteLine("Приведение к int: {0}", (int)enm);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
