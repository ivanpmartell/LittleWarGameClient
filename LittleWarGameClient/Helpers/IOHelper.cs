
using SharpCompress.Readers.Tar;
using System.Collections;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace LittleWarGameClient.Helpers
{
    internal class IOHelper
    {
        internal static string[] ReadFirstNLines(string filePath, int numberOfLines)
        {
            string[] lines = new string[numberOfLines];

            using (StreamReader reader = new(filePath))
            {
                string? line;
                int counter = 0;
                while (counter < numberOfLines && (line = reader.ReadLine()) != null)
                {
                    lines[counter] = line;
                    counter++;
                }
            }
            return lines;
        }

        internal static string ReadFirstLine(string filePath)
        {
            return ReadFirstNLines(filePath, 1)[0];
        }

        internal static bool IsTarFile(string filePath)
        {
            try
            {
                using (Stream stream = File.OpenRead(filePath))
                {
                    var reader = TarReader.Open(stream);
                    reader.MoveToNextEntry();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}