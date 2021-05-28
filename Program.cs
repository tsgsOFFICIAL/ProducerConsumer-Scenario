using System;
using System.Collections.Generic;
using System.Threading;

namespace ProducerConsumer_Scenario
{
    class Program
    {
        static Queue<Item> _buffer = new Queue<Item>(10); // Opretter en Queue med en kapacitet på 10 elementer
        static string[] _itemTypes = new string[]
        {
            "ToothBrush",
            "Cat",
            "Door",
            "Dog",
            "Fridge",
            "House",
            "Computer",
            "Cocaine",
            "Cookie",
            "Deodorant",
            "iPhone 4s",
            "iPhone 5s",
            "iPhone 6s",
            "iPhone 7",
            "iPhone 8",
            "iPhone X",
            "Huawei",
            "OnePlus 5",
            "OnePlus 6",
            "OnePlus 7",
            "OnePlus 8",
            "OnePlus 9"
        };
        static void Main()
        {
            Thread producerThread = new Thread(Produce); // Skab
            Thread consumerThread = new Thread(Consume); // Tråde

            producerThread.Start(); // Start
            consumerThread.Start(); // Tråde

            producerThread.Join(); // Join / Vent på dem, betyder ikke noget
            consumerThread.Join();
        }
        static void Produce()
        {
            while (true) // Kør forevigt
            {
                while (_buffer.Count < 10) // Kør så længe der er mindre end 10 elementer i kø / bufferen
                {
                    Thread.Sleep(new Random().Next(50, 300 + 1)); // Skab et random delay mellem 50 & 1000 ms (+1 fordi den går fra OG med, til IKKE MED)
                    Monitor.Enter(_buffer); // Lås bufferen / Brug den som låse objekt
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.Green; // Skift konsol farve
                        Item newItem = new Item(_itemTypes[new Random().Next(0, _itemTypes.Length)]); // Producer et nyt objekt af typen Item, med et random navn fra mit statiske array
                        _buffer.Enqueue(newItem); // Tilføj det nye Item til bufferen
                        Console.WriteLine($"Producer => Buffer({_buffer.Count}/10) [{newItem.Name}]"); // Udskriv til konsollen
                    }
                    catch (Exception)
                    { }
                    finally
                    {
                        Monitor.Pulse(_buffer); // Send en pulse til en tråd der venter
                    }
                    if (_buffer.Count >= 10) // Hvis buffer count, dvs. elementer er 10 eller mere
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow; // Skift farve til gul
                        Console.WriteLine($"Producer waits, Buffer({_buffer.Count}/10) is full"); // Produceren vil nu vente
                        Monitor.Wait(_buffer); // Vent & frigiv buffer låsen
                    }
                    Monitor.Exit(_buffer); // Frigiv låsen
                }
            }
        }
        static void Consume()
        {
            while (true) // Kør forevigt
            {
                Thread.Sleep(new Random().Next(50, 300 + 1)); // Sov tråden i x antal mx (50=>1000)

                try
                {
                    Monitor.Enter(_buffer); // Forhindre andre tråde i at bruge _buffer som lås
                    Console.ForegroundColor = ConsoleColor.Cyan; // Skift farve
                    Item retrievedItem = _buffer.Dequeue(); // Fjern det første element fra køen
                    Console.WriteLine($"Consumer <= Buffer({_buffer.Count}/10) [{retrievedItem.Name}]"); // Udskriv til konsollen
                }
                catch (Exception)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow; // Skift farve
                    Console.WriteLine($"Consumer waits, Buffer({_buffer.Count}/10) is empty"); // Forbrugeren må lige køle af, vi skal have produceret noget nyt
                    Monitor.Wait(_buffer); // Forbrugeren venter & frigiver objektet
                }
                finally
                {
                    Monitor.Pulse(_buffer); // Forbrugeren meddeler til en anden tråd at den (_buffer) er ledig igen
                    Monitor.Exit(_buffer); // Frigiv _buffer
                }
            }
        }
    }
    /// <summary>
    /// Creates and manages all produces Items
    /// </summary>
    class Item
    {
        public string Name { get; private set; } // Et navn, f.eks. Coca Cola eller Cocaine
        public static int Count { get; private set; } // Total count på alle objekter
        public Item(string name)
        {
            Name = name; // Set navnet
            Count++; // ++
        }
    }
}
