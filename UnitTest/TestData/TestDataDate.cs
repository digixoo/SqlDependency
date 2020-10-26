using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace UnitTest.TestData
{
    public class TestDataDate : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new List<object[]>
        {
            new object[]{true, 2020, 01, 24},
            new object[]{true, 2020, 01, 25},
            new object[]{false, 2020, 01, 25},
            new object[]{true, 2020, 02, 10}
        };

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();
      

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
    }
}
