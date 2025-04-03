using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

public class DatabasePlugin
{
    private readonly string _filePath;

    // Constructor accepts the file path as a parameter  
    public DatabasePlugin(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        _filePath = filePath;
    }

    /// <summary>  
    /// Reads all student records from the JSON file.  
    /// </summary>  
    /// <returns>A list of student records.</returns>  
    [KernelFunction]
    [Description("Read all student records from the JSON file.")]
    public async Task<List<Student>> GetAllStudentsAsync()
    {
        if (!File.Exists(_filePath))
            throw new FileNotFoundException("The specified JSON file does not exist.", _filePath);

        var fileContent = await File.ReadAllTextAsync(_filePath);
        var students = JsonSerializer.Deserialize<List<Student>>(fileContent);
        return students ?? new List<Student>();
    }

    /// <summary>  
    /// Finds a student record by name.  
    /// </summary>  
    /// <param name="name">The name of the student to find.</param>  
    /// <returns>The student record if found; otherwise, null.</returns>  
    [KernelFunction]
    [Description("Find a student record by name from the JSON file.")]
    public async Task<Student?> GetStudentByNameAsync(string name)
    {
        var students = await GetAllStudentsAsync();

        foreach (var student in students)
        {
            if (student.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return student;
            }
        }

        return null;
    }

    /// <summary>  
    /// Adds a new student record to the JSON file.  
    /// </summary>  
    /// <param name="student">The student record to add.</param>  
    [KernelFunction]
    [Description("Add a new student record to the JSON file.")]
    public async Task AddStudentAsync(Student student)
    {
        var students = await GetAllStudentsAsync();
        students.Add(student);

        var updatedContent = JsonSerializer.Serialize(students, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_filePath, updatedContent);
    }

    /// <summary>  
    /// Deletes a student record by name.  
    /// </summary>  
    /// <param name="name">The name of the student to delete.</param>  
    [KernelFunction]
    [Description("Delete a student record by name from the JSON file.")]
    public async Task DeleteStudentAsync(string name)
    {
        var students = await GetAllStudentsAsync();
        students.RemoveAll(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        var updatedContent = JsonSerializer.Serialize(students, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_filePath, updatedContent);
    }
}

/// <summary>  
/// Represents a student record.  
/// </summary>  
public class Student
{
    public string Name { get; set; }
    public string DOB { get; set; }
    public List<string> Interests { get; set; }

    public Student(string name, string dob, List<string> interests)
    {
        Name = name;
        DOB = dob;
        Interests = interests;
    }
}