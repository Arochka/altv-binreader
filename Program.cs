using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace BinReader
{
    class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                Console.Clear();
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1) Build vehmods.bin");
                Console.WriteLine("2) Read vehmods.bin");
                Console.WriteLine("3) Exit");
                Console.Write("\r\nSelect an option: ");

                try
                {
                    switch (Console.ReadLine())
                    {
                        case "1":
                            BuildVehMods();
                            break;
                        case "2":
                            ReadVehMods();
                            break;
                        case "3":
                            return;
                        default:
                            continue;
                    }
                }
                catch(Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }

                Console.WriteLine("Press a key to continue...");
                Console.ReadKey();
            }
        }

        static void BuildVehMods()
        {
            Console.WriteLine("Carcols Directory (default: ./data/carcols):");
            var carcolsDir = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(carcolsDir))
            {
                Console.WriteLine("You should specified a directory with exported carcols");
                return;
            }

            Console.WriteLine("Output filename (default: ./data/vehmods.bin):");
            var outputFile = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(outputFile))
            {
                Console.WriteLine("You should specified an output filename");
                return;
            }

            var carcolsReader = new CarcolsReader();
            var modKit = carcolsReader.Read(carcolsDir);

            var vehModsReader = new VehModsReader();
            vehModsReader.Write(outputFile, modKit.ToArray());

            Console.WriteLine($"Build {outputFile} using Carcols Directory {carcolsDir} successfully.");
        }

        static void ReadVehMods()
        {
            Console.WriteLine("Input filename (default: ./data/vehmods.bin):");
            var inputFile = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(inputFile))
            {
                Console.WriteLine("You should specified an input filename");
                return;
            }

            Console.WriteLine("Output JSON result (default: ./data/vehmods.json):");
            var outputFile = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(outputFile))
            {
                Console.WriteLine("You should specified an output filename");
                return;
            }

            var vehModsReader = new VehModsReader();
            var modKits = vehModsReader.Read(inputFile);

            var result = JsonSerializer.Serialize(modKits);

            File.WriteAllText(outputFile, result);

            Console.WriteLine($"Read {inputFile} successfully. Write result to {outputFile}");
        }
    }
}
