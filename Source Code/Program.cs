using System.IO;
using System;
using MemoryLoader;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.InteropServices;

Main main = new Main();
main.Execute();
namespace MemoryLoader {

    class Main
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static void ShowConsoleWindow()
        {
            var handle = GetConsoleWindow();

            if (handle == IntPtr.Zero)
            {
                AllocConsole();
            }
            else
            {
                ShowWindow(handle, SW_SHOW);
            }
        }

        public static void HideConsoleWindow()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
        }



        string HeaderStart = "f12a4c55410000000105";
        string HeaderEnd = "80000000020000000780";
        string BridgeStart = "040000000f";
        string BridgeMiddle = "1400f100";
        string BridgeEnd = "1400f000";

        string Example = "f1 2a 4c 55 41 00 00 00 01 05 00 00 00 07 80 00 00 00 02 00 00 00 07 80 37 36 35 34 33 32 31 2e 32 33 34 35 36 37 38 04 00 00 00 0f 37 34 35 36 33 38 31 2e 33 32 34 31 36 33 39 14 00 f1 00 35 34 32 39 34 39 31 2e 31 37 34 32 36 37 37 14 00 f1 00 32 31 38 34 34 36 38 2e 37 34 33 36 35 31 31 14 00 f1 00 33 31 39 34 35 36 39 2e 38 34 33 37 35 31 32 14 00 f1 00 34 31 32 34 36 36 31 2e 39 34 34 37 36 31 33 14 00 f0 00 34 31 32 34 36 36 31 2e 39 34 34 36 39 36 39";

        Dictionary<char, string> Replace = new Dictionary<char, string>();
        public void SetUpReplaceDictionary()
        {
            for (int i = 0; i < 10; i++)
            {
                Replace.Add(i.ToString()[0], "3" + i.ToString());
            }
            Replace.Add('.', "2e");
        }
        string InputFilePath;
        string OutputFilePath;
        public void Execute()
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Exit);
            ShowConsoleWindow();
            Console.WriteLine("Started");
            InputFilePath = System.IO.Directory.GetParent(Path.GetFullPath(this.GetType().Assembly.Location)).FullName + "\\Input";
            OutputFilePath = System.IO.Directory.GetParent(Path.GetFullPath(this.GetType().Assembly.Location)).FullName + "\\Output";
            string[] FilesToProcess = System.IO.Directory.GetFiles(InputFilePath);
            if (FilesToProcess.Length == 0)
            {
                Console.WriteLine("Please Put Input File(s) In This Directory: ");
                Console.WriteLine(InputFilePath);
                Exit();
            }
            Console.WriteLine("Running...");
            SetUpReplaceDictionary();
            for (int i = 0; i < FilesToProcess.Length; i++) {
                ProcessFile(FilesToProcess[i]);
            }
        }
        public void ProcessFile(string Path)
        {
            if (!Path.EndsWith(".txt"))
            {
                Console.WriteLine($"[ERROR]: File Not .txt, Cant Process File At: {Path}");
                return;
            }
            Console.WriteLine($"Working On File At {Path}");

            string[] InputLines = File.ReadAllLines(Path);
            string[] OutputLines = new string[InputLines.Length];
            for (int i = 0; i < InputLines.Length; i++)
            {
                OutputLines[i] = ProcessLine(InputLines[i], i, Path);
                
            }
            string FileName = System.IO.Path.GetFileNameWithoutExtension(Path);
            using (System.IO.StreamWriter sw = System.IO.File.CreateText(OutputFilePath + $"\\{FileName}_OUTPUT.txt"))
            {
                sw.WriteLine(ConvertCombinedToBase64(CombineLines(OutputLines)));
                
                
            }
            Console.WriteLine("Finished! Have a good day! :D");
            Exit();
        }
        public string ConvertCombinedToBase64(string Combined)
        {
            string Output = Convert.ToBase64String(HexStringToHex(Combined));
            // Need to prune all the "=" at the end of the base64 so SM can understand it
            Output = Output.Replace("=", "");
            Output = Output.Replace("=", "");
            Output = Output.Replace("=", "");
            return Output;
        }
        public byte[] HexStringToHex(string inputHex)
        {
            var resultantArray = new byte[inputHex.Length / 2];
            for (var i = 0; i < resultantArray.Length; i++)
            {
                resultantArray[i] = Convert.ToByte(inputHex.Substring(i * 2, 2), 16);
            }
            return resultantArray;
        }
        public string CombineLines(string[] Lines)
        {
            int Length = Lines.Length > 3 ? Lines.Length : 4;
            string Output = "";
            Output += HeaderStart;
            string LengthHex = Convert.ToString(Length, 16);
            for (int i = 0; i < 8; i++)
            {
                if (LengthHex.Length < 8)
                {
                    LengthHex = "0" + LengthHex;
                }
            }
            Output += LengthHex;
            Output += HeaderEnd;
            for (int i = 0; i < Length - 1; i++)
            {
                if (Lines.Length - 1 < i) {
                    Output += "303030303030302e30303030303030";
                    
                }
                else
                {
                    Output += Lines[i];
                }
                if (i == 0)
                {
                    Output += BridgeStart;
                    continue;
                }
                if ( i == Length - 2)
                {
                    Output += BridgeEnd;
                    continue;
                }
                Output += BridgeMiddle;
            }
            if (Lines.Length < 4)
            {
                Output += "303030303030302e30303030303030";
            }
            else
            {
                Output += Lines[Length - 1];
            }
            return Output;
        }
        public string ProcessLine(string Line, int Index, string FilePath)
        {
            string OutPut = "";
            int DecimalPoint = 9;
            string Digits = Line.Replace(".", "");
            if (Digits.Length > 14)
            {
                Console.WriteLine($"[ERROR] Can Only Accept Up To 14 Digits, Line {Index} of file {FilePath}");
                
                Exit();
            }
            if (Line.Contains('.'))
            {
                DecimalPoint = Line.IndexOf('.');
                if (DecimalPoint > 9 || Digits.Length - DecimalPoint > 9)
                {
                    Console.WriteLine($"[ERROR] Can Only Have Up To 9 Digits On One Side Of A Decimal Point, Line {Index} of file {FilePath}");
                    Exit();
                }
            }
            
            else
            {
                Line = Line + ".";
            }
            for (int i = 0; i < 14 - Digits.Length; i++)
            {
                DecimalPoint = Line.IndexOf('.');
                if (DecimalPoint < 7)
                {
                    Line = "0" + Line;
                    continue;
                }
                Line += "0";
            }
            if (Line.Count(c => c == '.') > 1)
            {
                Console.WriteLine($"[ERROR] Can Only Have 1 Decimal Point Per Line, Line {Index} of file {FilePath}");
                Exit();
            }
            for (int i = 0; i < Line.Length; i++)
            {
                if (!Replace.ContainsKey(Line[i]))
                {
                    Console.WriteLine($"[ERROR] Can Only Have Numbers And A Decimal Point, Line {Index} of file {FilePath}");
                    Exit();
                }
                
                OutPut += Replace[Line[i]];
            }
            return OutPut;
        }
        public void Exit()
        {
            Console.WriteLine("Exiting...");
            Console.Read();
            Environment.Exit(0);
        }
        public void Exit(object e, ConsoleCancelEventArgs a)
        {
            Exit();
        }
    }

}