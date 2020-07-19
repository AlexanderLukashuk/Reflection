using System;

namespace Library
{
    public class Car
    {
        public string name;
        public string model;
        public string color;
        public string Manufacturer { get; set; }
        public int Price { get; set; }

        public void Drive()
        {
            Console.WriteLine("Driving");
        }
    }
}