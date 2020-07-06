using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace osu_resize
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Made with love by minisbett, 2020.");
      Console.WriteLine("https://github.com/minisbett");
      List<string> files = args.ToList();

      List<string> validFiles = new List<string>();
      // Validate the files
      for (int i = 0; i < files.Count; i++)
      {
        // Check if the file even exists
        if (!File.Exists(files[i]))
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine($"The file {files[i]} was not found.");
          Console.ForegroundColor = ConsoleColor.Gray;
          continue;
        }

        // Check if the file ends with @2x.png
        if (!Path.GetFileName(files[i]).EndsWith("@2x.png"))
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine($"The file {files[i]} is not a PNG file or does not end with @2x.");
          Console.ForegroundColor = ConsoleColor.Gray;
          continue;
        }

        validFiles.Add(files[i]);
      }

      // Load the textures
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("Loading textures");
      Console.ForegroundColor = ConsoleColor.Gray;
      Dictionary<string, Bitmap> originalTextures = new Dictionary<string, Bitmap>();
      foreach (string file in validFiles)
      {
        try
        {
          originalTextures.Add(file, (Bitmap)Image.FromFile(file));
          Console.WriteLine($"Loaded {file}");
        }
        catch (Exception ex)
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine($"{file} could not be loaded.");
          Console.WriteLine($"{ex.Message}");
          Console.ForegroundColor = ConsoleColor.Gray;
        }
      }

      Dictionary<string, Bitmap> resizedTextures = new Dictionary<string, Bitmap>();
      // Resize the textures to their smaller size
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("Resizing textures");
      Console.ForegroundColor = ConsoleColor.Gray;
      foreach (var texture in originalTextures)
      {
        Bitmap bmp = texture.Value;
        Bitmap resized = new Bitmap(bmp.Width / 2, bmp.Height / 2);
        Graphics g = Graphics.FromImage(resized);
        g.DrawImage(bmp, new Rectangle(0, 0, resized.Width, resized.Height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel);

        resizedTextures.Add(texture.Key, resized);
        g.Dispose();
        bmp.Dispose();
        Console.WriteLine($"Resized {texture.Key}");
      }

      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("Saving textures");
      Console.ForegroundColor = ConsoleColor.Gray;
      int saved = 0;
      foreach (var texture in resizedTextures)
      {
        string name = Path.GetFileName(texture.Key).Replace("@2x", "");
        string newfilepath = Path.Combine(new FileInfo(texture.Key).DirectoryName, name);
        if (File.Exists(newfilepath))
        {
          Console.ForegroundColor = ConsoleColor.Yellow;
          Console.WriteLine($"The file {newfilepath} already exists. Overwrite? (Y/N)");
          Console.ForegroundColor = ConsoleColor.Gray;
          bool overwrite = false;
          while (true)
          {
            var cki = Console.ReadKey(true);
            if (cki.Key == ConsoleKey.N || cki.Key == ConsoleKey.Y)
            {
              overwrite = cki.Key == ConsoleKey.Y;
              break;
            }
          }

          if (!overwrite)
            continue;
        }

        texture.Value.Save(newfilepath, ImageFormat.Png);
        Console.WriteLine($"Saved {texture.Key}");
        saved++;
      }

      Console.WriteLine($"Done. Saved {saved} out of {args.Length} files");
      Console.ReadLine();
    }
  }
}