using System;
using System.IO;

namespace FolderCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите путь до папки для очистки:");
            string folderPath = Console.ReadLine();

            try
            {
                DirectoryInfo directory = new DirectoryInfo(folderPath);

                if (directory.Exists)
                {
                    Console.WriteLine($"Размер папки {directory.FullName} до очистки: {GetFolderSize(directory)} байт");

                    ClearFolder(directory);

                    Console.WriteLine("Очистка завершена");

                    Console.WriteLine($"Удалено {countFilesDeleted} файлов, освобождено {sizeDeleted} байт");

                    Console.WriteLine($"Размер папки {directory.FullName} после очистки: {GetFolderSize(directory)} байт");
                }
                else
                {
                    Console.WriteLine("Папки по заданному пути не существует.");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Ошибка доступа: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.ReadKey();
        }

        static long sizeDeleted = 0;
        static int countFilesDeleted = 0;

        static void ClearFolder(DirectoryInfo directory)
        {
            foreach (FileInfo file in directory.GetFiles())
            {
                if (DateTime.Now - file.LastAccessTime > TimeSpan.FromMinutes(30))
                {
                    sizeDeleted += file.Length;
                    countFilesDeleted++;
                    file.Delete();
                }
            }

            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                if (subDirectory.Attributes.HasFlag(FileAttributes.ReparsePoint))
                {
                    continue;
                }

                ClearFolder(subDirectory);

                if (subDirectory.GetFiles().Length == 0 && subDirectory.GetDirectories().Length == 0 &&
                    DateTime.Now - subDirectory.LastAccessTime > TimeSpan.FromMinutes(30))
                {
                    subDirectory.Delete();
                }
            }
        }

        static long GetFolderSize(DirectoryInfo dir)
        {
            long size = 0;

            foreach (FileInfo file in dir.GetFiles())
            {
                size += file.Length;
            }

            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                
                size += GetFolderSize(subDir);
            }

            return size;
        }
    }
}
