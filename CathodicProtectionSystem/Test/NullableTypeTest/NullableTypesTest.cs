using System;
using System.Collections.Generic;
using System.Text;

namespace Test.NullableTypeTest
{
    public static class NullableTypesTest
    {
        public static void Test()
        {
            DateTime? nullDateTime;

            nullDateTime = new DateTime();

            nullDateTime = null;

            nullDateTime = DateTime.Now;

            string s = nullDateTime.ToString();
        }
    }
}
