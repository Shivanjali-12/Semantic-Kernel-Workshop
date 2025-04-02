using Microsoft.SemanticKernel;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

public class FilePlugin
{
    private readonly string _filePath;

    // Constructor accepts the file path as a parameter
    public FilePlugin(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        _filePath = filePath;
    }

    /// <summary>
    /// Reads the entire content of the file.
    /// </summary>
    /// <returns>Content of the file.</returns>
    [KernelFunction]
    [Description("Read the contents of the file.")]
    public Task<string> ReadAsync()
    {
        if (!File.Exists(_filePath))
            throw new FileNotFoundException("The specified file does not exist.", _filePath);

        return File.ReadAllTextAsync(_filePath);
    }

    /// <summary>
    /// Writes content to the file, overwriting any existing content.
    /// </summary>
    /// <param name="content">The content to write to the file.</param>
    [KernelFunction]
    [Description("Write content to the file.")]
    public async Task WriteAsync(string content)
    {
        await File.WriteAllTextAsync(_filePath, content);
    }

    /// <summary>
    /// Appends content to the file without overwriting the existing content.
    /// </summary>
    /// <param name="content">The content to append to the file.</param>
    [KernelFunction]
    [Description("Append content to the file.")]
    public async Task AppendAsync(string content)
    {
        await File.AppendAllTextAsync(_filePath, content);
    }

    /// <summary>
    /// Deletes the file.
    /// </summary>
    [KernelFunction]
    [Description("Delete the file.")]
    public Task DeleteAsync()
    {
        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
        }
        return Task.CompletedTask;
    }
}
