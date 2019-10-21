using System;

namespace ShoppingSamples.Content
{
    public class ContentSamplesRunner
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            new ShoppingContentSample().startSamples(args , "shopping-samples");

            // run the logic for TMLewin
            new ShoppingContentSample().startSamples(args , "shopping-samples-tmlewin");

        }
    }
}

