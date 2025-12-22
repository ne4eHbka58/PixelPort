using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BetterLogs
{
    public class BetterLog
    {
        string _logDirectory;
        string _baseFileName;
        long _maxFileSizeBytes;
        int _currentFileIndex;
        public BetterLog(string LogDirectory, string BaseFileName, long MaxFileSizeMB) 
        {
            _logDirectory = LogDirectory;
            _baseFileName = BaseFileName;
            _maxFileSizeBytes = MaxFileSizeMB * 1024 * 1024;

            EnsureDirectoryExists();
            InitializeFileIndex();
        }
        private void EnsureDirectoryExists()
        {
            try
            {
                if (!Directory.Exists(_logDirectory))
                {
                    Directory.CreateDirectory(_logDirectory);
                    Console.WriteLine($"Created log directory: {_logDirectory}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating directory {_logDirectory}: {ex.Message}");
            }
        }
        private void InitializeFileIndex()
        {
            // Находим последний существующий файл
            var files = Directory.GetFiles(_logDirectory, $"{_baseFileName}*.log")
                .OrderBy(f => f)
                .ToArray();

            if (files.Length > 0)
            {
                var lastFile = files.Last();
                // Извлекаем номер из имени файла
                var match = Regex.Match(lastFile, $@"{_baseFileName}(\d+)\.log$");
                if (match.Success)
                {
                    _currentFileIndex = int.Parse(match.Groups[1].Value);
                }
            }
        }
        private string GetCurrentPath()
        {
            return Path.Combine(_logDirectory, $"{_baseFileName}{_currentFileIndex}.log");
        }
        public void WriteLog(string message, string type = "info", bool logToConsole = true, bool logToFile = true)
        {
            if (logToConsole)
            {
                Console.Write($"{DateTime.Now}: ");
                switch(type)
                {
                    case "error":
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("ОШИБКА");
                        break;
                    case "warning":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("ПРЕДУПРЕЖДЕНИЕ");
                        break;
                    case "info":
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("ИНФО");
                        break;
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($" - {message}\n");
            }
            if (logToFile)
            {
                string filePath = GetCurrentPath();

                // Проверяем размер текущего файла
                if (File.Exists(filePath))
                {
                    var fileInfo = new FileInfo(filePath);
                    if (fileInfo.Length > _maxFileSizeBytes)
                    {
                        _currentFileIndex++;
                        filePath = GetCurrentPath();
                    }
                }

                File.AppendAllText(filePath, $"{DateTime.Now}: {type.ToUpper()} - {message}\n");
            }
        }
    }
}
