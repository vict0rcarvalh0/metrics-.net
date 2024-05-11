using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Threading;

class Program
{
    static Meter s_meter = new Meter("HatCo.Store");
    static Counter<int> s_hatsSold = s_meter.CreateCounter<int>("hatco.store.hats_sold");

    static void Main(string[] args)
    {
        Console.WriteLine("Press any key to exit");
        while(!Console.KeyAvailable)
        {
            // Pretend our store has a transaction, every 100ms, that sells two size 12 red hats, and one size 19 blue hat.
            Thread.Sleep(100);
            s_hatsSold.Add(2,
                new KeyValuePair<string,object>("product.color", "red"),
                new KeyValuePair<string,object>("product.size", 12));
            s_hatsSold.Add(1,
                new KeyValuePair<string,object>("product.color", "blue"),
                new KeyValuePair<string,object>("product.size", 19));
        }
    }
}