using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConsoleMatrixEffect
{

    // press Alt+Enter to stretch the console to full screen.
    static class Program
    {

        private const int delay = 40;

        static void Main(string[] args)
        {
            SetConsoleParams();

            // I set the Matrix to the size of the console window
            var consoleMatrix = new ConsoleMatrix(Console.WindowWidth, Console.WindowHeight);

            var cycle = 1; // Cycle counter. Needed to adjust the speed of different columns
            do
            {
                consoleMatrix.ShowMatrix();
                Thread.Sleep(delay);
                consoleMatrix.ShiftMatrixElements(cycle);
                cycle = ((cycle + 1) % 1000); // limit the variable so it won't overflow int
            } while (!Console.KeyAvailable);
        }

        private static void SetConsoleParams()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.CursorVisible = false;
        }
    }

    public class ConsoleMatrix
    {
        //characters to be used for the matrix
        //private static readonly char[] _symbols = "¢£¥¥§gYhjwHwWÞßĦŠŊŁþøðÖ ".ToCharArray(); //if the console font supports Unicode
        private static readonly char[] _symbols = "&%#$@@*(@YyyuUjJhHFfsqVNnv ".ToCharArray();
        private readonly int _symbolsLen = _symbols.Length;

        // matrix
        private char[][] _matrix;
        private readonly int _width;
        private readonly int _height;

        private readonly Random _rand = new Random(); // define a global randomizer for the symbols.
        private const int period = 6; // for column rate computation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width">Width of the console window</param>
        /// <param name="height">height of the console window</param>
        public ConsoleMatrix(int width, int height)
        {
            _width = width;
            _height = height;
            CreateMatrix();
        }

        /// <summary>
        /// Fill in the original matrix
        /// </summary>
        private void CreateMatrix()
        {
            // the matrix is an array of arrays of characters
            _matrix = new char[_height][];

            for (var h = 0; h < _height; h++)
            {
                // I fill lines with characters. Every second character is a space, forming empty columns
                _matrix[h] =
                    // Creation of a sequence of 1...N. This can be used instead of standard loops
                    Enumerable.Range(0, _width)
                        .Select(position => (position + 1) % 2 == 0
                                ? ' ' // blank space is needed to form empty columns
                                : _symbols[_rand.Next(0, _symbolsLen)])
                        .ToArray();
            }
        }

        /// <summary>
        /// Print the matrix in the console
        /// </summary>
        public void ShowMatrix()
        {
            // reset the cursor position to its original position
            Console.SetCursorPosition(0, 0);

            // print the characters line by line
            for (var h = 0; h < _height; h++)
            {
                var s = new string(_matrix[h]); // make a single line from the array of symbols, because printing one line is much faster than printing symbols one by one
                if (h < _height - 1) Console.WriteLine(s);
                else Console.Write(s); // print the last line without hyphenation, to avoid an empty line at the bottom
            }
        }

        /// <summary>
        /// The method for shifting the columns
        /// </summary>
        /// <param name="cycle"></param>
        public void ShiftMatrixElements(int cycle)
        {
            for (var w = 0; w < _width; w += 2) // every other column is blank
            {
                if (w % period > cycle % period) continue; // adjust speed for different columns

                // move all characters in a column down by one character
                for (var h = _height - 1; h >= 0; h--)
                {
                    // for the first line, I add a random character
                    if (h == 0)
                    {
                        _matrix[h][w] = _symbols[_rand.Next(0, _symbolsLen)];
                        continue;
                    }
                    //moving the superior symbol into the current line
                    _matrix[h][w] = _matrix[h - 1][w];
                }
            }
        }
    }
}
