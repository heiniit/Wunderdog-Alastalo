using System;
using System.Collections.Generic;
using System.Linq;

namespace Alastalo
{
    class Program
    {
        static void Main(string[] args)
        {
            const int MAX_LINE_LENGTH = 80;

            // Word structure: list all words so that they are grouped by the length of the word
            // The longest word is 29 characters: "Parkki-parkki-männynparkki!" so that's all we need.
            const int MAX_CHARS = 30;
            List<string>[] words_by_length = new List<string>[MAX_CHARS];
            for (int i = 0; i < MAX_CHARS; i++)
                words_by_length[i] = new List<string>();

            // Read words from file to the structure
            string file_line;
            using (System.IO.StreamReader file_read = new System.IO.StreamReader(@"C:\Temp\alastalon_salissa.txt"))
            {
                while ((file_line = file_read.ReadLine()) != null)
                {
                    string[] file_line_words = file_line.Split();
                    foreach (string file_word in file_line_words)
                        words_by_length[file_word.Length].Add(file_word);
                }
            }

            // This structure contains output lines according to how much free space there are in the string (max line length is 80 chars)
            // The new line is added to the struct when there is less than 30 characters left (i.e. it may be full after adding one word)
            List<string>[] lines_by_free_length = new List<string>[MAX_CHARS];
            for (int i = 0; i < MAX_CHARS; i++)
                lines_by_free_length[i] = new List<string>();

            // Handle words from long to short and put them to the line they fit
            bool line_found = false;
            string line = "";
            string new_line = "";
            // First words that have 29 letters, then 28 etc.
            for (int i=MAX_CHARS-1; i>0; i--)
            {
                foreach (string word in words_by_length[i])
                {
                    line_found = false;
                    // Start with lines that have exactly room for this word
                    for (int j=word.Length + 1; j<MAX_CHARS; j++) // +1: We need to add space before the word
                    {
                        if (word.Length > 2)
                        {
                            // Simple optimization: we know that there are plenty of words with 2 letters
                            // so it's not optimal to leave 1-2 empty spaces at the end of the line.
                            if (j == word.Length + 2 || j == word.Length + 3)
                                continue;
                        }
                        if (lines_by_free_length[j].Count > 0)
                        {
                            // Found a line where the word fits, add the word and move the line to the correct position
                            line = lines_by_free_length[j].ElementAt(0);
                            lines_by_free_length[j].RemoveAt(0);
                            line = line + " " + word;
                            lines_by_free_length[MAX_LINE_LENGTH - line.Length].Add(line);
                            line_found = true;
                            break;
                        }
                    }
                    // In case there was no line to add the word, we need to create a new line
                    if (!line_found)
                    {
                        if (new_line.Length > 0)
                            new_line += " ";
                        new_line += word;
                        if (new_line.Length > MAX_LINE_LENGTH-MAX_CHARS)
                        {
                            lines_by_free_length[MAX_LINE_LENGTH - new_line.Length].Add(new_line);
                            new_line = "";
                        }
                    }
                }
            }

            // Finally write the output file
            int line_count = 0;
            using (System.IO.StreamWriter file_write = new System.IO.StreamWriter(@"C:\Temp\alastalon_salissa_result.txt"))
            {
                // Loop lines from the structure
                for (int i = 0; i < MAX_CHARS; i++)
                {
                    foreach (string line_write in lines_by_free_length[i])
                        file_write.WriteLine(line_write);
                    line_count += lines_by_free_length[i].Count;
                }
                // ... and in case there is some new line that was not yet added to the struct, add it also
                if (new_line.Length > 0)
                {
                    file_write.Write(new_line);
                    line_count++;
                }
            }
            Console.WriteLine("Wrote {0} lines to the file.", line_count);

            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }
    }
}
