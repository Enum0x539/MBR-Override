using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MBR_Override
{
    internal static class Program
    {
        [DllImport("kernel32")]
        private static extern IntPtr CreateFile(
        string lpFileName,
        uint dwDesiredAccess,
        uint dwShareMode,
        IntPtr lpSecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes,
        IntPtr hTemplateFile);

        [DllImport("kernel32")]
        private static extern bool WriteFile(
            IntPtr hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped);

        [DllImport("kernel32")]
        private static extern bool ReadFile(
            IntPtr hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped);

        private const uint MbrSize = 512u;

        private static void Main()
        {
            bool read = true;

            //----------- MODE ------------//

            string mode = read ? "Read" : "Write";
            DialogResult result = MessageBox.Show($"Mode: {mode}, Are you sure you would like to continue?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes)
                return;

            byte[] mbrData = new byte[MbrSize];
            IntPtr mbr = CreateFile("\\\\.\\PhysicalDrive0", 0x10000000, 0x1 | 0x2, IntPtr.Zero, 0x3, 0, IntPtr.Zero);

            if (mbr == (IntPtr)(-0x1))
                return;

            if (read)
            {
                ReadFile(mbr, mbrData, MbrSize, out uint lpNumberOfBytesWritten, IntPtr.Zero);
                for (int i = 0; i < 512; ++i)
                {
                    if (i % 128 == 0 && i != 0)
                        Console.WriteLine();

                    if (mbrData[i] >= 0x20 && mbrData[i] <= 0x7E)
                        Console.Write((char)mbrData[i]);
                    else
                        Console.Write(".");
                }
            }
            else
                WriteFile(mbr, mbrData, MbrSize, out uint lpNumberOfBytesWritten, IntPtr.Zero);

            Console.ReadLine();
        }
    }
}