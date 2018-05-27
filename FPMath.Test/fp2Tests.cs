using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FPMath.Test
{
    public class fp2Tests
    {
        [Fact]
        public void ItPerformsAddition()
        {
            fp2 one = new fp2(1, 1);
            fp2 two = new fp2(2, 2);

            fp2 result = one + two;

            Assert.Equal(result.x, (fp)3);
            Assert.Equal(result.y, (fp)3);
        }

        [Fact]
        public void ItPerformsSubtraction()
        {
            fp2 one = new fp2(1, 1);
            fp2 two = new fp2(2, 2);

            fp2 result = two - one;

            Assert.Equal(result.x, (fp)1);
            Assert.Equal(result.y, (fp)1);
        }

        [Fact]
        public void ItPerformsMultiplication()
        {
            fp2 seven = new fp2(7, 7);
            fp2 three = new fp2(3, 3);

            fp2 result = three * seven;

            Assert.Equal(result.x, (fp)21);
            Assert.Equal(result.y, (fp)21);
        }

        [Fact]
        public void ItPerformsDivision()
        {
            fp2 ten = new fp2(10, 10);
            fp2 five = new fp2(5, 5);

            fp2 result = ten / five;

            Assert.Equal(result.x, (fp)2);
            Assert.Equal(result.y, (fp)2);
        }
    }
}
