//This code is copied from:
//http://code.google.com/p/cfstox/source/browse/trunk/+cfstox/CFStox/TALib+Example+of+Moving+Average.txt?r=6
//and adapted to compile under C#.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TicTacTec.TA.Library;

namespace TALibTester
{
    public class TALibDemo
    {
        //you need to instantiate some basic variables
        private double[] input;
        private int[] inputInt;
        private double[] output;
        private int[] outputInt;
        private int outBegIdx;
        private int outNbElement;
        private Core.RetCode retCode;
        private int lookback;

        //Here's some data over which we will calculate a moving average. I snagged it from this forum.
        public static double[] close = new double[]{91.500000, 94.815000, 94.375000, 95.095000, 93.780000, 94.625000, 92.530000, 92.750000, 90.315000, 92.470000, 96.125000,
97.250000, 98.500000, 89.875000, 91.000000, 92.815000, 89.155000, 89.345000, 91.625000, 89.875000, 88.375000,
87.625000, 84.780000, 83.000000, 83.500000, 81.375000, 84.440000, 89.250000, 86.375000, 86.250000, 85.250000,
87.125000, 85.815000, 88.970000, 88.470000, 86.875000, 86.815000, 84.875000, 84.190000, 83.875000, 83.375000,
85.500000, 89.190000, 89.440000, 91.095000, 90.750000, 91.440000, 89.000000, 91.000000, 90.500000, 89.030000,
88.815000, 84.280000, 83.500000, 82.690000, 84.750000, 85.655000, 86.190000, 88.940000, 89.280000, 88.625000,
88.500000, 91.970000, 91.500000, 93.250000, 93.500000, 93.155000, 91.720000, 90.000000, 89.690000, 88.875000,
85.190000, 83.375000, 84.875000, 85.940000, 97.250000, 99.875000, 104.940000, 106.000000, 102.500000, 102.405000,
104.595000, 106.125000, 106.000000, 106.065000, 104.625000, 108.625000, 109.315000, 110.500000, 112.750000, 123.000000,
119.625000, 118.750000, 119.250000, 117.940000, 116.440000, 115.190000, 111.875000, 110.595000, 118.125000, 116.000000,
116.000000, 112.000000, 113.750000, 112.940000, 116.000000, 120.500000, 116.620000, 117.000000, 115.250000, 114.310000,
115.500000, 115.870000, 120.690000, 120.190000, 120.750000, 124.750000, 123.370000, 122.940000, 122.560000, 123.120000,
122.560000, 124.620000, 129.250000, 131.000000, 132.250000, 131.000000, 132.810000, 134.000000, 137.380000, 137.810000,
137.880000, 137.250000, 136.310000, 136.250000, 134.630000, 128.250000, 129.000000, 123.870000, 124.810000, 123.000000,
126.250000, 128.380000, 125.370000, 125.690000, 122.250000, 119.370000, 118.500000, 123.190000, 123.500000, 122.190000,
119.310000, 123.310000, 121.120000, 123.370000, 127.370000, 128.500000, 123.870000, 122.940000, 121.750000, 124.440000,
122.000000, 122.370000, 122.940000, 124.000000, 123.190000, 124.560000, 127.250000, 125.870000, 128.860000, 132.000000,
130.750000, 134.750000, 135.000000, 132.380000, 133.310000, 131.940000, 130.000000, 125.370000, 130.130000, 127.120000,
125.190000, 122.000000, 125.000000, 123.000000, 123.500000, 120.060000, 121.000000, 117.750000, 119.870000, 122.000000,
119.190000, 116.370000, 113.500000, 114.250000, 110.000000, 105.060000, 107.000000, 107.870000, 107.000000, 107.120000,
107.000000, 91.000000, 93.940000, 93.870000, 95.500000, 93.000000, 94.940000, 98.250000, 96.750000, 94.810000,
94.370000, 91.560000, 90.250000, 93.940000, 93.620000, 97.000000, 95.000000, 95.870000, 94.060000, 94.620000,
93.750000, 98.000000, 103.940000, 107.870000, 106.060000, 104.500000, 105.000000, 104.190000, 103.060000, 103.420000,
105.270000, 111.870000, 116.000000, 116.620000, 118.280000, 113.370000, 109.000000, 109.700000, 109.250000, 107.000000,
109.190000, 110.000000, 109.200000, 110.120000, 108.000000, 108.620000, 109.750000, 109.810000, 109.000000, 108.750000,
107.870000};

        public TALibDemo()
        {
            //initialize everything required for holding data
            int closeLength = close.Length;
            input = new double[closeLength];
            inputInt = new int[closeLength];
            output = new double[closeLength];
            outputInt = new int[closeLength];

            //keeping it simple here...
            simpleMovingAverageCall();
        }

        /**
        * resets the arrays used in this application since they are only
        * initialized once
        */
        private void resetArrayValues()
        {
            //provide default "fill" values to avoid nulls.

            for (int i = 0; i < input.Length; i++)
            {
                input[i] = (double)i;
                inputInt[i] = i;
            }
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = (double)-999999.0;
                outputInt[i] = -999999;
            }

            //provide some "fail" values up front to ensure completion if correct.
            outBegIdx = -1;
            outNbElement = -1;
            retCode = Core.RetCode.InternalError;
            lookback = -1;

        }

        private void showDefaultTALIBOutput()
        {
            Console.WriteLine("Printing the default TALIB output to the screen");
            Console.WriteLine("lookback = " + lookback + ", outBegIdx=" + outBegIdx
            + ", outNbElement=" + outNbElement + "\nretCode=" + retCode.ToString());
            Console.WriteLine("\nClose \t" + "Indicator");
            for (int i = 0; i < output.Length; i++)
            {
                Console.WriteLine(close[i] + ",\t " + output[i]);
            }
            Console.WriteLine("Notice the default values are at the end of the list when they");
            Console.WriteLine("should be correlating with their proper data values. This is TA-lib's way");
            Console.WriteLine("of doing business.");
            Console.WriteLine("There's issues with doing it this way. The biggest is that");
            Console.WriteLine("every developer is expecting the calculated outputs to be lined");
            Console.WriteLine("up nice and neat with the value to which it actually correlates");
            Console.WriteLine("TA-Lib does NOT do this for us");
            Console.WriteLine("So here's my recommendation if you are using TA-Lib in its raw format:");
            Console.WriteLine("Look at the next set of outputs from this example...");

        }

        private void showFinalOutput()
        {
            Console.WriteLine("Printing the output you'd prefer to see to the screen");
            Console.WriteLine("lookback = " + lookback + ", outBegIdx=" + Convert.ToString(outBegIdx)
            + ", outNbElement=" + Convert.ToString(outNbElement) + "\nretCode=" + Convert.ToString(retCode));
            Console.WriteLine("\nClose \t" + "Indicator");
            int j = 0;
            for (int i = 0; i < output.Length; i++)
            {
                if (i >= lookback)
                    Console.WriteLine(close[i] + ",\t " + output[j++]);
                else
                    Console.WriteLine(close[i] + ",\t " + "-999999");
            }
            Console.WriteLine("Notice how the values are actually lining up now! Well, it's forced!");

            Console.WriteLine("Just to be sure, the next value should be the default junk value and it's = " + output[j++]);
            Console.WriteLine("There's issues with doing it this way as well. The biggest is that");
            Console.WriteLine("every developer must always keep track of where his output");
            Console.WriteLine("array's index is in relation to the array of original data");
            Console.WriteLine("This method contains the best way to handle this, so here's my recommendation:");
            Console.WriteLine("if you are painting your calculated values to a graph or placing them in a table,");
            Console.WriteLine("use the for loop approach in this method. Don't waste time");
            Console.WriteLine("reinstantiating an array that is cleanly structured like we would");
            Console.WriteLine("prefer the code did for us naturally.");
            Console.WriteLine("This is the only way to do this in the Java version. When the");
            Console.WriteLine("code was ported over from C, there were several coding conventions");
            Console.WriteLine("that ported over with it that are painful nuisances in the Java world");
            Console.WriteLine("Basically, my assessment is that the entire library needs to be");
            Console.WriteLine("re-written in Java for Java without the porting taking place");
            Console.WriteLine("Looks like I'll just use it as is.");

            Console.ReadLine();
        }

        public void simpleMovingAverageCall()
        {
            //you'll probably have to do this next call every time you add a value to your data array
            //I haven't checked this in a live app yet
            resetArrayValues();

            //The "lookback" is really your indicator's period minus one because it's expressed as an array index
            //At least that's true for movingAverage(...)
            lookback = Core.MovingAverageLookback(10, Core.MAType.Sma);

            Console.WriteLine("Starting everthing off...");
            Console.WriteLine("Lookback=" + Convert.ToString(lookback));
            Console.WriteLine("outBegIdx.value=" + Convert.ToString(outBegIdx));
            Console.WriteLine("outNbElement.value=" + Convert.ToString(outNbElement));
            retCode = Core.MovingAverage(0, close.Length - 1, close, lookback + 1, Core.MAType.Sma, out outBegIdx, out outNbElement, output);

            showDefaultTALIBOutput();
            showFinalOutput();
        }

        static void Main(string[] args)
        {
            //keeping this really simple...
            TALibDemo demo = new TALibDemo();
        }

    }

}
