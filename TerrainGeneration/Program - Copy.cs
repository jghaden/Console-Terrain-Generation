using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrainGeneration {
    class Program {
        public static void Main(string[] args) {
            //This sets the required variables to default values if the user does not provide arguments
            bool hasBorder = false;
            bool isGrass = false;
            bool isWater = false;
            bool chCoords = false;
            int landType;
            int sizeX = 40;
            int sizeY = 20;
            int seed = 0;
            int chX = 1;
            int chY = 1;
            int offX = 0;
            int offY = 0;
            int landRange = 40;
            int waterR = 8;
            int dGrassR = 25;
            String offXS = "";

            //This checks if the user has provide arguments
            for (int i = 0; i < args.Length; i++) {
                if(args[i].StartsWith("-")) {
                    String optionName = args[i].Trim(new char[] { '-' }).ToLower();
                    switch(optionName) {
                        case "?":
                        case "h":
                        case "help":
                            Console.WriteLine("\t-border,-b\tPlaces a border around the map");
                            Console.WriteLine("\t-coords\t\tChecks the type of terrain at the provided coordinates (starts at [0,0])");
                            Console.WriteLine("\t-help,-h,-?\tDisplays this help screen");
                            Console.WriteLine("\t-offset\t\tDisplaces the map by the provided amount on X and or Y (default 0x0)");
                            Console.WriteLine("\t-seed\t\tSets the seed of the map (default = 0)(range 0-2147483648)");
                            Console.WriteLine("\t-size\t\tSets the size of the map (default 40x20)(minimum 3x3)");
                            Console.WriteLine();
                            System.Environment.Exit(1);
                            break;

                        case "size":
                            String sizeText = args[i + 1];

                            int sizeDelim = sizeText.IndexOf('x');

                            sizeX = int.Parse(sizeText.Substring(0, sizeDelim));
                            sizeY = int.Parse(sizeText.Substring(sizeDelim + 1));

                            //Throws and error and exits if the provided map size is smaller than 3x3
                            if (sizeX < 3 || sizeY < 3) {
                                Console.WriteLine("Invalid size, minium size of 3x3");
                                System.Environment.Exit(1);
                            }

                            i++;
                            break;

                        case "seed":
                            //Changes the default seed of 0 to the provided seed, this is the backbone of procedural generation
                            seed = int.Parse(args[i + 1]);
                            break;

                        case "coords":
                            //If the user wants to know what sort of terrain is at the provided coordinates
                            String coordsText = args[i + 1];

                            int coordsDelim = coordsText.IndexOf('x');

                            chX = int.Parse(coordsText.Substring(0, coordsDelim));
                            chY = int.Parse(coordsText.Substring(coordsDelim + 1));

                            if(chX >= sizeX || chY >= sizeY) {
                                Console.WriteLine("Coordinates are outside of " + sizeX + "x" + sizeY + " map");
                                System.Environment.Exit(1);
                            }

                            chCoords = true;
                            i++;
                            break;

                        case "offset":
                            //Allows the user to offset the map on X and or Y
                            String offsetText = args[i + 1];

                            int offsetDelim = offsetText.IndexOf('x');

                            offX = int.Parse(offsetText.Substring(0, offsetDelim));
                            offY = int.Parse(offsetText.Substring(offsetDelim + 1));

                            if(offX < 0 || offY < 0) {
                                Console.WriteLine("The X and Y offset must both me equal or greater than zero (0)");
                                System.Environment.Exit(1);
                            }

                            i++;
                            break;

                        case "b":
                        case "border":
                            //Sets a border around the map if the user provides the argument
                            hasBorder = true;
                            i++;
                            break;

                        default:
                            //This handles for arguments that are unknown that the user entered
                            Console.WriteLine("Unknown argument '{0}'", optionName);
                            System.Environment.Exit(1);
                            break;
                    }
                }
            }
            
            if(offX > 0) {
                for (int i = 0; i < offX; i++)
                    offXS += " ";
            }
            if(offY > 0) {
                for (int i = 0; i < offY; i++)
                    Console.WriteLine();
            }
            Random rand = new Random(seed);
            String[,] coords = new String[sizeX, sizeY];
            for(int y = 0; y < sizeY; y++) {
                if (offX > 0) {
                    Console.ResetColor();
                    Console.Write(offXS);
                }
                for (int x = 0; x < sizeX; x++) {
                    if (hasBorder && (x == 0 || x == sizeX - 1 || y == 0 || y == sizeY - 1)) {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("\u2588");
                        coords[x, y] = "border";
                    } else {
                        if (isWater) {
                            DrawTerrain("water");
                            isWater = false;
                            coords[x, y] = "water";
                        } else if (isGrass) {
                            DrawTerrain("dark grass");
                            isGrass = false;
                            coords[x, y] = "dark grass";
                        } else {
                            landType = rand.Next(0, landRange);
                            if (landType < waterR) {
                                DrawTerrain("water");
                                isWater = true;
                                coords[x, y] = "water";
                            }
                            if (landType > waterR-1 && landType < dGrassR) {
                                DrawTerrain("dark grass");
                                isGrass = true;
                                coords[x, y] = "dark grass";
                            }
                            if (landType > dGrassR-1) {
                                DrawTerrain("light grass");
                                coords[x, y] = "light grass";
                            }
                        }
                    }
                }
                Console.WriteLine();
            }
            //Change the color of the console back to the default or user set color
            Console.ResetColor();
            //If the argument -coords NxN is passed, this will display the type at said coords
            if(chCoords)
                Console.WriteLine("(" + chX + "," + chY + ")" + ": " + coords[chX, chY]);
        }

        public static void DrawTerrain(String terrain) {
            if(terrain == "water") {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("\u2588");
            }
            if(terrain == "dark grass") {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.Write("\u2591");
            }
            if(terrain == "light grass") {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.Write("\u2591");
            }
        }
    }
}
