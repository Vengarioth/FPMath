using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace FPMath
{
    public class fpTests
    {
        long[] m_testCases = new[] {
            // Small numbers
            0L, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
            -1, -2, -3, -4, -5, -6, -7, -8, -9, -10,
  
            // Integer numbers
            0x100000000, -0x100000000, 0x200000000, -0x200000000, 0x300000000, -0x300000000,
            0x400000000, -0x400000000, 0x500000000, -0x500000000, 0x600000000, -0x600000000,
  
            // Fractions (1/2, 1/4, 1/8)
            0x80000000, -0x80000000, 0x40000000, -0x40000000, 0x20000000, -0x20000000,
  
            // Problematic carry
            0xFFFFFFFF, -0xFFFFFFFF, 0x1FFFFFFFF, -0x1FFFFFFFF, 0x3FFFFFFFF, -0x3FFFFFFFF,
  
            // Smallest and largest values
            long.MaxValue, long.MinValue,
  
            // Large random numbers
            6791302811978701836, -8192141831180282065, 6222617001063736300, -7871200276881732034,
            8249382838880205112, -7679310892959748444, 7708113189940799513, -5281862979887936768,
            8220231180772321456, -5204203381295869580, 6860614387764479339, -9080626825133349457,
            6658610233456189347, -6558014273345705245, 6700571222183426493,
  
            // Small random numbers
            -436730658, -2259913246, 329347474, 2565801981, 3398143698, 137497017, 1060347500,
            -3457686027, 1923669753, 2891618613, 2418874813, 2899594950, 2265950765, -1962365447,
            3077934393

            // Tiny random numbers
            - 171,
            -359, 491, 844, 158, -413, -422, -737, -575, -330,
            -376, 435, -311, 116, 715, -1024, -487, 59, 724, 993
        };

        [Fact]
        public void Precision()
        {
            Assert.Equal(0.00000000023283064365386962890625m, fp.Precision);
        }

        [Fact]
        public void LongTofpAndBack()
        {
            var sources = new[] { long.MinValue, int.MinValue - 1L, int.MinValue, -1L, 0L, 1L, int.MaxValue, int.MaxValue + 1L, long.MaxValue };
            var expecteds = new[] { 0L, int.MaxValue, int.MinValue, -1L, 0L, 1L, int.MaxValue, int.MinValue, -1L };
            for (int i = 0; i < sources.Length; ++i)
            {
                var expected = expecteds[i];
                var f = (fp)sources[i];
                var actual = (long)f;
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void DoubleTofpAndBack()
        {
            var sources = new[] {
                (double)int.MinValue,
                -(double)Math.PI,
                -(double)Math.E,
                -1.0,
                -0.0,
                0.0,
                1.0,
                (double)Math.PI,
                (double)Math.E,
                (double)int.MaxValue
            };

            foreach (var value in sources)
            {
                AreEqualWithinPrecision(value, (double)(fp)value);
            }
        }

        static void AreEqualWithinPrecision(decimal value1, decimal value2)
        {
            Assert.True(Math.Abs(value2 - value1) < fp.Precision);
        }

        static void AreEqualWithinPrecision(double value1, double value2)
        {
            Assert.True(Math.Abs(value2 - value1) < (double)fp.Precision);
        }

        [Fact]
        public void DecimalTofpAndBack()
        {

            Assert.Equal(fp.MaxValue, (fp)(decimal)fp.MaxValue);
            Assert.Equal(fp.MinValue, (fp)(decimal)fp.MinValue);

            var sources = new[] {
                int.MinValue,
                -(decimal)Math.PI,
                -(decimal)Math.E,
                -1.0m,
                -0.0m,
                0.0m,
                1.0m,
                (decimal)Math.PI,
                (decimal)Math.E,
                int.MaxValue
            };

            foreach (var value in sources)
            {
                AreEqualWithinPrecision(value, (decimal)(fp)value);
            }
        }

        [Fact]
        public void Addition()
        {
            var terms1 = new[] { fp.MinValue, (fp)(-1), fp.Zero, fp.One, fp.MaxValue };
            var terms2 = new[] { (fp)(-1), (fp)2, (fp)(-1.5m), (fp)(-2), fp.One };
            var expecteds = new[] { fp.MinValue, fp.One, (fp)(-1.5m), (fp)(-1), fp.MaxValue };
            for (int i = 0; i < terms1.Length; ++i)
            {
                var actual = terms1[i] + terms2[i];
                var expected = expecteds[i];
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void Substraction()
        {
            var terms1 = new[] { fp.MinValue, (fp)(-1), fp.Zero, fp.One, fp.MaxValue };
            var terms2 = new[] { fp.One, (fp)(-2), (fp)(1.5m), (fp)(2), (fp)(-1) };
            var expecteds = new[] { fp.MinValue, fp.One, (fp)(-1.5m), (fp)(-1), fp.MaxValue };
            for (int i = 0; i < terms1.Length; ++i)
            {
                var actual = terms1[i] - terms2[i];
                var expected = expecteds[i];
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void BasicMultiplication()
        {
            var term1s = new[] { 0m, 1m, -1m, 5m, -5m, 0.5m, -0.5m, -1.0m };
            var term2s = new[] { 16m, 16m, 16m, 16m, 16m, 16m, 16m, -1.0m };
            var expecteds = new[] { 0L, 16, -16, 80, -80, 8, -8, 1 };
            for (int i = 0; i < term1s.Length; ++i)
            {
                var expected = expecteds[i];
                var actual = (long)((fp)term1s[i] * (fp)term2s[i]);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void MultiplicationTestCases()
        {
            var sw = new Stopwatch();
            int failures = 0;
            for (int i = 0; i < m_testCases.Length; ++i)
            {
                for (int j = 0; j < m_testCases.Length; ++j)
                {
                    var x = fp.FromRaw(m_testCases[i]);
                    var y = fp.FromRaw(m_testCases[j]);
                    var xM = (decimal)x;
                    var yM = (decimal)y;
                    var expected = xM * yM;
                    expected =
                        expected > (decimal)fp.MaxValue
                            ? (decimal)fp.MaxValue
                            : expected < (decimal)fp.MinValue
                                  ? (decimal)fp.MinValue
                                  : expected;
                    sw.Start();
                    var actual = x * y;
                    sw.Stop();
                    var actualM = (decimal)actual;
                    var maxDelta = (decimal)fp.FromRaw(1);
                    if (Math.Abs(actualM - expected) > maxDelta)
                    {
                        Console.WriteLine("Failed for FromRaw({0}) * FromRaw({1}): expected {2} but got {3}",
                                          m_testCases[i],
                                          m_testCases[j],
                                          (fp)expected,
                                          actualM);
                        ++failures;
                    }
                }
            }
            Console.WriteLine("{0} total, {1} per multiplication", sw.ElapsedMilliseconds, (double)sw.Elapsed.Milliseconds / (m_testCases.Length * m_testCases.Length));
            Assert.True(failures < 1);
        }


        static void Ignore<T>(T value) { }

        [Fact]
        public void DivisionTestCases()
        {
            var sw = new Stopwatch();
            int failures = 0;
            for (int i = 0; i < m_testCases.Length; ++i)
            {
                for (int j = 0; j < m_testCases.Length; ++j)
                {
                    var x = fp.FromRaw(m_testCases[i]);
                    var y = fp.FromRaw(m_testCases[j]);
                    var xM = (decimal)x;
                    var yM = (decimal)y;

                    if (m_testCases[j] == 0)
                    {
                        Assert.Throws<DivideByZeroException>(() => Ignore(x / y));
                    }
                    else
                    {
                        var expected = xM / yM;
                        expected =
                            expected > (decimal)fp.MaxValue
                                ? (decimal)fp.MaxValue
                                : expected < (decimal)fp.MinValue
                                      ? (decimal)fp.MinValue
                                      : expected;
                        sw.Start();
                        var actual = x / y;
                        sw.Stop();
                        var actualM = (decimal)actual;
                        var maxDelta = (decimal)fp.FromRaw(1);
                        if (Math.Abs(actualM - expected) > maxDelta)
                        {
                            Console.WriteLine("Failed for FromRaw({0}) / FromRaw({1}): expected {2} but got {3}",
                                              m_testCases[i],
                                              m_testCases[j],
                                              (fp)expected,
                                              actualM);
                            ++failures;
                        }
                    }
                }
            }
            Console.WriteLine("{0} total, {1} per division", sw.ElapsedMilliseconds, (double)sw.Elapsed.Milliseconds / (m_testCases.Length * m_testCases.Length));
            Assert.True(failures < 1);
        }



        [Fact]
        public void Sign()
        {
            var sources = new[] { fp.MinValue, (fp)(-1), fp.Zero, fp.One, fp.MaxValue };
            var expecteds = new[] { -1, -1, 0, 1, 1 };
            for (int i = 0; i < sources.Length; ++i)
            {
                var actual = fp.Sign(sources[i]);
                var expected = expecteds[i];
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void Abs()
        {
            Assert.Equal(fp.MaxValue, fp.Abs(fp.MinValue));
            var sources = new[] { -1, 0, 1, int.MaxValue };
            var expecteds = new[] { 1, 0, 1, int.MaxValue };
            for (int i = 0; i < sources.Length; ++i)
            {
                var actual = fp.Abs((fp)sources[i]);
                var expected = (fp)expecteds[i];
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void FastAbs()
        {
            Assert.Equal(fp.MinValue, fp.FastAbs(fp.MinValue));
            var sources = new[] { -1, 0, 1, int.MaxValue };
            var expecteds = new[] { 1, 0, 1, int.MaxValue };
            for (int i = 0; i < sources.Length; ++i)
            {
                var actual = fp.FastAbs((fp)sources[i]);
                var expected = (fp)expecteds[i];
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void Floor()
        {
            var sources = new[] { -5.1m, -1, 0, 1, 5.1m };
            var expecteds = new[] { -6m, -1, 0, 1, 5m };
            for (int i = 0; i < sources.Length; ++i)
            {
                var actual = (decimal)fp.Floor((fp)sources[i]);
                var expected = expecteds[i];
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void Ceiling()
        {
            var sources = new[] { -5.1m, -1, 0, 1, 5.1m };
            var expecteds = new[] { -5m, -1, 0, 1, 6m };
            for (int i = 0; i < sources.Length; ++i)
            {
                var actual = (decimal)fp.Ceiling((fp)sources[i]);
                var expected = expecteds[i];
                Assert.Equal(expected, actual);
            }

            Assert.Equal(fp.MaxValue, fp.Ceiling(fp.MaxValue));
        }

        [Fact]
        public void Round()
        {
            var sources = new[] { -5.5m, -5.1m, -4.5m, -4.4m, -1, 0, 1, 4.5m, 4.6m, 5.4m, 5.5m };
            var expecteds = new[] { -6m, -5m, -4m, -4m, -1, 0, 1, 4m, 5m, 5m, 6m };
            for (int i = 0; i < sources.Length; ++i)
            {
                var actual = (decimal)fp.Round((fp)sources[i]);
                var expected = expecteds[i];
                Assert.Equal(expected, actual);
            }
            Assert.Equal(fp.MaxValue, fp.Round(fp.MaxValue));
        }


        [Fact]
        public void Sqrt()
        {
            for (int i = 0; i < m_testCases.Length; ++i)
            {
                var f = fp.FromRaw(m_testCases[i]);
                if (fp.Sign(f) < 0)
                {
                    Assert.Throws<ArgumentOutOfRangeException>(() => fp.Sqrt(f));
                }
                else
                {
                    var expected = Math.Sqrt((double)f);
                    var actual = (double)fp.Sqrt(f);
                    var delta = (decimal)Math.Abs(expected - actual);
                    Assert.True(delta <= fp.Precision);
                }
            }
        }

        [Fact]
        public void Log2()
        {
            double maxDelta = (double)(fp.Precision * 4);

            for (int j = 0; j < m_testCases.Length; ++j)
            {
                var b = fp.FromRaw(m_testCases[j]);

                if (b <= fp.Zero)
                {
                    Assert.Throws<ArgumentOutOfRangeException>(() => fp.Log2(b));
                }
                else
                {
                    var expected = Math.Log((double)b) / Math.Log(2);
                    var actual = (double)fp.Log2(b);
                    var delta = Math.Abs(expected - actual);

                    Assert.True(delta <= maxDelta, string.Format("Ln({0}) = expected {1} but got {2}", b, expected, actual));
                }
            }
        }

        [Fact]
        public void Ln()
        {
            double maxDelta = 0.00000001;

            for (int j = 0; j < m_testCases.Length; ++j)
            {
                var b = fp.FromRaw(m_testCases[j]);

                if (b <= fp.Zero)
                {
                    Assert.Throws<ArgumentOutOfRangeException>(() => fp.Ln(b));
                }
                else
                {
                    var expected = Math.Log((double)b);
                    var actual = (double)fp.Ln(b);
                    var delta = Math.Abs(expected - actual);

                    Assert.True(delta <= maxDelta, string.Format("Ln({0}) = expected {1} but got {2}", b, expected, actual));
                }
            }
        }

        [Fact]
        public void Pow2()
        {
            double maxDelta = 0.0000001;
            for (int i = 0; i < m_testCases.Length; ++i)
            {
                var e = fp.FromRaw(m_testCases[i]);

                var expected = Math.Min(Math.Pow(2, (double)e), (double)fp.MaxValue);
                var actual = (double)fp.Pow2(e);
                var delta = Math.Abs(expected - actual);

                Assert.True(delta <= maxDelta, string.Format("Pow2({0}) = expected {1} but got {2}", e, expected, actual));
            }
        }

        [Fact]
        public void Pow()
        {
            for (int i = 0; i < m_testCases.Length; ++i)
            {
                var b = fp.FromRaw(m_testCases[i]);

                for (int j = 0; j < m_testCases.Length; ++j)
                {
                    var e = fp.FromRaw(m_testCases[j]);

                    if (b == fp.Zero && e < fp.Zero)
                    {
                        Assert.Throws<DivideByZeroException>(() => fp.Pow(b, e));
                    }
                    else if (b < fp.Zero && e != fp.Zero)
                    {
                        Assert.Throws<ArgumentOutOfRangeException>(() => fp.Pow(b, e));
                    }
                    else
                    {
                        var expected = e == fp.Zero ? 1 : b == fp.Zero ? 0 : Math.Min(Math.Pow((double)b, (double)e), (double)fp.MaxValue);

                        // Absolute precision deteriorates with large result values, take this into account
                        // Similarly, large exponents reduce precision, even if result is small.
                        double maxDelta = Math.Abs((double)e) > 100000000 ? 0.5 : expected > 100000000 ? 10 : expected > 1000 ? 0.5 : 0.00001;

                        var actual = (double)fp.Pow(b, e);
                        var delta = Math.Abs(expected - actual);

                        Assert.True(delta <= maxDelta, string.Format("Pow({0}, {1}) = expected {2} but got {3}", b, e, expected, actual));
                    }
                }
            }
        }

        [Fact]
        public void Modulus()
        {
            var deltas = new List<decimal>();
            foreach (var operand1 in m_testCases)
            {
                foreach (var operand2 in m_testCases)
                {
                    var f1 = fp.FromRaw(operand1);
                    var f2 = fp.FromRaw(operand2);

                    if (operand2 == 0)
                    {
                        Assert.Throws<DivideByZeroException>(() => Ignore(f1 / f2));
                    }
                    else
                    {
                        var d1 = (decimal)f1;
                        var d2 = (decimal)f2;
                        var actual = (decimal)(f1 % f2);
                        var expected = d1 % d2;
                        var delta = Math.Abs(expected - actual);
                        deltas.Add(delta);
                        Assert.True(delta <= 60 * fp.Precision, string.Format("{0} % {1} = expected {2} but got {3}", f1, f2, expected, actual));
                    }
                }
            }
            Console.WriteLine("Max error: {0} ({1} times precision)", deltas.Max(), deltas.Max() / fp.Precision);
            Console.WriteLine("Average precision: {0} ({1} times precision)", deltas.Average(), deltas.Average() / fp.Precision);
            Console.WriteLine("failed: {0}%", deltas.Count(d => d > fp.Precision) * 100.0 / deltas.Count);
        }

        //[Fact]
        //public void SinBenchmark()
        //{
        //    var deltas = new List<double>();

        //    var swf = new Stopwatch();
        //    var swd = new Stopwatch();

        //    // Restricting the range to from 0 to Pi/2
        //    for (var angle = 0.0; angle <= 2 * Math.PI; angle += 0.000004)
        //    {
        //        var f = (fp)angle;
        //        swf.Start();
        //        var actualF = fp.Sin(f);
        //        swf.Stop();
        //        var actual = (double)actualF;
        //        swd.Start();
        //        var expectedD = Math.Sin(angle);
        //        swd.Stop();
        //        var expected = (double)expectedD;
        //        var delta = Math.Abs(expected - actual);
        //        deltas.Add(delta);
        //    }
        //    Console.WriteLine("Max error: {0} ({1} times precision)", deltas.Max(), deltas.Max() / (double)fp.Precision);
        //    Console.WriteLine("Average precision: {0} ({1} times precision)", deltas.Average(), deltas.Average() / (double)fp.Precision);
        //    Console.WriteLine("fp.Sin time = {0}ms, Math.Sin time = {1}ms", swf.ElapsedMilliseconds, swd.ElapsedMilliseconds);
        //}

        [Fact]
        public void Sin()
        {
            Assert.True(fp.Sin(fp.Zero) == fp.Zero);

            Assert.True(fp.Sin(fp.PiOver2) == fp.One);
            Assert.True(fp.Sin(fp.Pi) == fp.Zero);
            Assert.True(fp.Sin(fp.Pi + fp.PiOver2) == -fp.One);
            Assert.True(fp.Sin(fp.PiTimes2) == fp.Zero);

            Assert.True(fp.Sin(-fp.PiOver2) == -fp.One);
            Assert.True(fp.Sin(-fp.Pi) == fp.Zero);
            Assert.True(fp.Sin(-fp.Pi - fp.PiOver2) == fp.One);
            Assert.True(fp.Sin(-fp.PiTimes2) == fp.Zero);


            for (double angle = -2 * Math.PI; angle <= 2 * Math.PI; angle += 0.0001)
            {
                var f = (fp)angle;
                var actualF = fp.Sin(f);
                var expected = (decimal)Math.Sin(angle);
                var delta = Math.Abs(expected - (decimal)actualF);
                Assert.True(delta <= 3 * fp.Precision, string.Format("Sin({0}): expected {1} but got {2}", angle, expected, actualF));
            }

            var deltas = new List<decimal>();
            foreach (var val in m_testCases)
            {
                var f = fp.FromRaw(val);
                var actualF = fp.Sin(f);
                var expected = (decimal)Math.Sin((double)f);
                var delta = Math.Abs(expected - (decimal)actualF);
                Assert.True(delta <= 0.0000001M, string.Format("Sin({0}): expected {1} but got {2}", f, expected, actualF));
            }
        }

        [Fact]
        public void FastSin()
        {
            for (double angle = -2 * Math.PI; angle <= 2 * Math.PI; angle += 0.0001)
            {
                var f = (fp)angle;
                var actualF = fp.FastSin(f);
                var expected = (decimal)Math.Sin(angle);
                var delta = Math.Abs(expected - (decimal)actualF);
                Assert.True(delta <= 50000 * fp.Precision, string.Format("Sin({0}): expected {1} but got {2}", angle, expected, actualF));
            }

            foreach (var val in m_testCases)
            {
                var f = fp.FromRaw(val);
                var actualF = fp.FastSin(f);
                var expected = (decimal)Math.Sin((double)f);
                var delta = Math.Abs(expected - (decimal)actualF);
                Assert.True(delta <= 0.01M, string.Format("Sin({0}): expected {1} but got {2}", f, expected, actualF));
            }
        }

        [Fact]
        public void Acos()
        {
            var maxDelta = 0.00000001m;
            var deltas = new List<decimal>();

            Assert.Equal(fp.Zero, fp.Acos(fp.One));
            Assert.Equal(fp.PiOver2, fp.Acos(fp.Zero));
            Assert.Equal(fp.Pi, fp.Acos(-fp.One));

            // Precision
            for (var x = -1.0; x < 1.0; x += 0.001)
            {
                var xf = (fp)x;
                var actual = (decimal)fp.Acos(xf);
                var expected = (decimal)Math.Acos((double)xf);
                var delta = Math.Abs(actual - expected);
                deltas.Add(delta);
                Assert.True(delta <= maxDelta, string.Format("Precision: Acos({0}): expected {1} but got {2}", xf, expected, actual));
            }

            for (int i = 0; i < m_testCases.Length; ++i)
            {
                var b = fp.FromRaw(m_testCases[i]);

                if (b < -fp.One || b > fp.One)
                {
                    Assert.Throws<ArgumentOutOfRangeException>(() => fp.Acos(b));
                }
                else
                {
                    var expected = (decimal)Math.Acos((double)b);
                    var actual = (decimal)fp.Acos(b);
                    var delta = Math.Abs(expected - actual);
                    deltas.Add(delta);
                    Assert.True(delta <= maxDelta, string.Format("Acos({0}) = expected {1} but got {2}", b, expected, actual));
                }
            }
            Console.WriteLine("Max error: {0} ({1} times precision)", deltas.Max(), deltas.Max() / fp.Precision);
            Console.WriteLine("Average precision: {0} ({1} times precision)", deltas.Average(), deltas.Average() / fp.Precision);
        }

        [Fact]
        public void Cos()
        {
            Assert.True(fp.Cos(fp.Zero) == fp.One);

            Assert.True(fp.Cos(fp.PiOver2) == fp.Zero);
            Assert.True(fp.Cos(fp.Pi) == -fp.One);
            Assert.True(fp.Cos(fp.Pi + fp.PiOver2) == fp.Zero);
            Assert.True(fp.Cos(fp.PiTimes2) == fp.One);

            Assert.True(fp.Cos(-fp.PiOver2) == -fp.Zero);
            Assert.True(fp.Cos(-fp.Pi) == -fp.One);
            Assert.True(fp.Cos(-fp.Pi - fp.PiOver2) == fp.Zero);
            Assert.True(fp.Cos(-fp.PiTimes2) == fp.One);


            for (double angle = -2 * Math.PI; angle <= 2 * Math.PI; angle += 0.0001)
            {
                var f = (fp)angle;
                var actualF = fp.Cos(f);
                var expected = (decimal)Math.Cos(angle);
                var delta = Math.Abs(expected - (decimal)actualF);
                Assert.True(delta <= 3 * fp.Precision, string.Format("Cos({0}): expected {1} but got {2}", angle, expected, actualF));
            }

            foreach (var val in m_testCases)
            {
                var f = fp.FromRaw(val);
                var actualF = fp.Cos(f);
                var expected = (decimal)Math.Cos((double)f);
                var delta = Math.Abs(expected - (decimal)actualF);
                Assert.True(delta <= 0.0000001M, string.Format("Cos({0}): expected {1} but got {2}", f, expected, actualF));
            }
        }

        [Fact]
        public void FastCos()
        {
            for (double angle = -2 * Math.PI; angle <= 2 * Math.PI; angle += 0.0001)
            {
                var f = (fp)angle;
                var actualF = fp.FastCos(f);
                var expected = (decimal)Math.Cos(angle);
                var delta = Math.Abs(expected - (decimal)actualF);
                Assert.True(delta <= 50000 * fp.Precision, string.Format("Cos({0}): expected {1} but got {2}", angle, expected, actualF));
            }

            foreach (var val in m_testCases)
            {
                var f = fp.FromRaw(val);
                var actualF = fp.FastCos(f);
                var expected = (decimal)Math.Cos((double)f);
                var delta = Math.Abs(expected - (decimal)actualF);
                Assert.True(delta <= 0.01M, string.Format("Cos({0}): expected {1} but got {2}", f, expected, actualF));
            }
        }

        [Fact]
        public void Tan()
        {
            Assert.True(fp.Tan(fp.Zero) == fp.Zero);
            Assert.True(fp.Tan(fp.Pi) == fp.Zero);
            Assert.True(fp.Tan(-fp.Pi) == fp.Zero);

            Assert.True(fp.Tan(fp.PiOver2 - (fp)0.001) > fp.Zero);
            Assert.True(fp.Tan(fp.PiOver2 + (fp)0.001) < fp.Zero);
            Assert.True(fp.Tan(-fp.PiOver2 - (fp)0.001) > fp.Zero);
            Assert.True(fp.Tan(-fp.PiOver2 + (fp)0.001) < fp.Zero);

            for (double angle = 0;/*-2 * Math.PI;*/ angle <= 2 * Math.PI; angle += 0.0001)
            {
                var f = (fp)angle;
                var actualF = fp.Tan(f);
                var expected = (decimal)Math.Tan(angle);
                Assert.Equal(actualF > fp.Zero, expected > 0);
                //TODO figure out a real way to test this function
            }

            //foreach (var val in m_testCases) {
            //    var f = (fp)val;
            //    var actualF = fp.Tan(f);
            //    var expected = (decimal)Math.Tan((double)f);
            //    var delta = Math.Abs(expected - (decimal)actualF);
            //    Assert.True(delta <= 0.01, string.Format("Tan({0}): expected {1} but got {2}", f, expected, actualF));
            //}
        }

        [Fact]
        public void Atan()
        {
            var maxDelta = 0.00000001m;
            var deltas = new List<decimal>();

            Assert.Equal(fp.Zero, fp.Atan(fp.Zero));

            // Precision
            for (var x = -1.0; x < 1.0; x += 0.0001)
            {
                var xf = (fp)x;
                var actual = (decimal)fp.Atan(xf);
                var expected = (decimal)Math.Atan((double)xf);
                var delta = Math.Abs(actual - expected);
                deltas.Add(delta);
                Assert.True(delta <= maxDelta, string.Format("Precision: Atan({0}): expected {1} but got {2}", xf, expected, actual));
            }

            // Scalability and edge cases
            foreach (var x in m_testCases)
            {
                var xf = (fp)x;
                var actual = (decimal)fp.Atan(xf);
                var expected = (decimal)Math.Atan((double)xf);
                var delta = Math.Abs(actual - expected);
                deltas.Add(delta);
                Assert.True(delta <= maxDelta, string.Format("Scalability: Atan({0}): expected {1} but got {2}", xf, expected, actual));
            }
            Console.WriteLine("Max error: {0} ({1} times precision)", deltas.Max(), deltas.Max() / fp.Precision);
            Console.WriteLine("Average precision: {0} ({1} times precision)", deltas.Average(), deltas.Average() / fp.Precision);
        }
        //[Fact]
        public void AtanBenchmark()
        {
            var deltas = new List<decimal>();

            var swf = new Stopwatch();
            var swd = new Stopwatch();

            for (var x = -1.0; x < 1.0; x += 0.001)
            {
                for (int k = 0; k < 1000; ++k)
                {
                    var xf = (fp)x;
                    swf.Start();
                    var actualF = fp.Atan(xf);
                    swf.Stop();
                    swd.Start();
                    var expected = Math.Atan((double)xf);
                    swd.Stop();
                    deltas.Add(Math.Abs((decimal)actualF - (decimal)expected));
                }
            }
            Console.WriteLine("Max error: {0} ({1} times precision)", deltas.Max(), deltas.Max() / fp.Precision);
            Console.WriteLine("Average precision: {0} ({1} times precision)", deltas.Average(), deltas.Average() / fp.Precision);
            Console.WriteLine("fp.Atan time = {0}ms, Math.Atan time = {1}ms", swf.ElapsedMilliseconds, swd.ElapsedMilliseconds);
        }

        [Fact]
        public void Atan2()
        {
            var deltas = new List<decimal>();
            // Identities
            Assert.Equal(fp.Atan2(fp.Zero, -fp.One), fp.Pi);
            Assert.Equal(fp.Atan2(fp.Zero, fp.Zero), fp.Zero);
            Assert.Equal(fp.Atan2(fp.Zero, fp.One), fp.Zero);
            Assert.Equal(fp.Atan2(fp.One, fp.Zero), fp.PiOver2);
            Assert.Equal(fp.Atan2(-fp.One, fp.Zero), -fp.PiOver2);

            // Precision
            for (var y = -1.0; y < 1.0; y += 0.01)
            {
                for (var x = -1.0; x < 1.0; x += 0.01)
                {
                    var yf = (fp)y;
                    var xf = (fp)x;
                    var actual = fp.Atan2(yf, xf);
                    var expected = (decimal)Math.Atan2((double)yf, (double)xf);
                    var delta = Math.Abs((decimal)actual - expected);
                    deltas.Add(delta);
                    Assert.True(delta <= 0.005M, string.Format("Precision: Atan2({0}, {1}): expected {2} but got {3}", yf, xf, expected, actual));
                }
            }

            // Scalability and edge cases
            foreach (var y in m_testCases)
            {
                foreach (var x in m_testCases)
                {
                    var yf = (fp)y;
                    var xf = (fp)x;
                    var actual = (decimal)fp.Atan2(yf, xf);
                    var expected = (decimal)Math.Atan2((double)yf, (double)xf);
                    var delta = Math.Abs(actual - expected);
                    deltas.Add(delta);
                    Assert.True(delta <= 0.005M, string.Format("Scalability: Atan2({0}, {1}): expected {2} but got {3}", yf, xf, expected, actual));
                }
            }
            Console.WriteLine("Max error: {0} ({1} times precision)", deltas.Max(), deltas.Max() / fp.Precision);
            Console.WriteLine("Average precision: {0} ({1} times precision)", deltas.Average(), deltas.Average() / fp.Precision);
        }


        //[Fact]
        public void Atan2Benchmark()
        {
            var deltas = new List<decimal>();

            var swf = new Stopwatch();
            var swd = new Stopwatch();

            foreach (var y in m_testCases)
            {
                foreach (var x in m_testCases)
                {
                    for (int k = 0; k < 1000; ++k)
                    {
                        var yf = (fp)y;
                        var xf = (fp)x;
                        swf.Start();
                        var actualF = fp.Atan2(yf, xf);
                        swf.Stop();
                        swd.Start();
                        var expected = Math.Atan2((double)yf, (double)xf);
                        swd.Stop();
                        deltas.Add(Math.Abs((decimal)actualF - (decimal)expected));
                    }
                }
            }
            Console.WriteLine("Max error: {0} ({1} times precision)", deltas.Max(), deltas.Max() / fp.Precision);
            Console.WriteLine("Average precision: {0} ({1} times precision)", deltas.Average(), deltas.Average() / fp.Precision);
            Console.WriteLine("fp.Atan2 time = {0}ms, Math.Atan2 time = {1}ms", swf.ElapsedMilliseconds, swd.ElapsedMilliseconds);
        }

        [Fact]
        public void Negation()
        {
            foreach (var operand1 in m_testCases)
            {
                var f = fp.FromRaw(operand1);
                if (f == fp.MinValue)
                {
                    Assert.Equal(-f, fp.MaxValue);
                }
                else
                {
                    var expected = -((decimal)f);
                    var actual = (decimal)(-f);
                    Assert.Equal(expected, actual);
                }
            }
        }

        [Fact]
        public void EqualsTests()
        {
            foreach (var op1 in m_testCases)
            {
                foreach (var op2 in m_testCases)
                {
                    var d1 = (decimal)op1;
                    var d2 = (decimal)op2;
                    Assert.True(op1.Equals(op2) == d1.Equals(d2));
                }
            }
        }

        [Fact]
        public void EqualityAndInequalityOperators()
        {
            var sources = m_testCases.Select(fp.FromRaw).ToList();
            foreach (var op1 in sources)
            {
                foreach (var op2 in sources)
                {
                    var d1 = (double)op1;
                    var d2 = (double)op2;
                    Assert.True((op1 == op2) == (d1 == d2));
                    Assert.True((op1 != op2) == (d1 != d2));
                    Assert.False((op1 == op2) && (op1 != op2));
                }
            }
        }

        [Fact]
        public void CompareTo()
        {
            var nums = m_testCases.Select(fp.FromRaw).ToArray();
            var numsDecimal = nums.Select(t => (decimal)t).ToArray();
            Array.Sort(nums);
            Array.Sort(numsDecimal);
            Assert.True(nums.Select(t => (decimal)t).SequenceEqual(numsDecimal));
        }
    }
}