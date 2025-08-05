using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace LR2
{
    public class DatabaseHelper
    {
        private string _databaseFile = "StudentsDB.sqlite";
        private string _connectionString;

        public DatabaseHelper() 
        { 
        _connectionString = $"Data Source={_databaseFile};Version=3;";
        InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(_databaseFile))
            {
                SQLiteConnection.CreateFile(_databaseFile);

                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string createStudentsTable = @"CREATE TABLE IF NOT EXISTS Students(
                                                    Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                                                    StudentId INTEGER UNIQUE, 
                                                    FullName TEXT NOT NULL,     
                                                    BirthDate TEXT);";
                    string createGradesTable = @"CREATE TABLE IF NOT EXISTS Grades (
                                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                    StudentId INTEGER,
                                                    PhysicsGrade INTEGER,
                                                    MathGrade INTEGER,
                                                    FOREIGN KEY (StudentId) REFERENCES Students(StudentId)
                                                    );";
                    using (var command = new SQLiteCommand(createStudentsTable, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    using (var command = new SQLiteCommand(createGradesTable, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public void AddStudent(int studentId, string fullName, DateTime? birthDate, int physicsGrade, int mathGrade)
        {
            using(var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string insertStudent = @"
                        INSERT INTO Students (StudentID, FullName, BirthDate)
                        VALUES (@studentId, @fullName, @birthDate);";

                        using (var command = new SQLiteCommand(insertStudent, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@studentId", studentId);
                            command.Parameters.AddWithValue("@fullName", fullName);
                            command.Parameters.AddWithValue("@birthDate", birthDate?.ToString("yyyy-MM-dd"));

                            command.ExecuteNonQuery();
                        }

                        string InsertGrades = @"    
                        INSERT INTO Grades (StudentId, PhysicsGrade, MathGrade)
                        VALUES (@StudentId, @PhysicsGrade, @MathGrade);";

                        using (var command = new SQLiteCommand(InsertGrades, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@StudentId", studentId);
                            command.Parameters.AddWithValue("@PhysicsGrade", physicsGrade);
                            command.Parameters.AddWithValue("@MathGrade", mathGrade);

                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public List<Student> GetAllStudents()
        {
            var students = new List<Student>();

            string query = @"SELECT s.StudentId, s.FullName, s.BirthDate, g.PhysicsGrade, g.MathGrade
                            FROM Students s
                            JOIN Grades g ON s.StudentId = g.StudentID;";

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        {
                        students.Add(new Student
                        {
                            StudentId = Convert.ToInt32(reader["StudentId"]),
                            FullName = reader["FullName"].ToString(),
                            BirthDate = reader["BirthDate"] != DBNull.Value ?
                            DateTime.Parse(reader["BirthDate"].ToString()) : (DateTime?)null,
                            PhysicsGrade = Convert.ToInt32(reader["PhysicsGrade"]),
                            MathGrade = Convert.ToInt32(reader["MathGrade"])
                        });
                    }
                }
            }
            return students;
        }
        public void UpdateStudent(int studentId, string fullName, DateTime? birthDate, int physicsGrade, int mathGrade)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string updateStudent = @"UPDATE Students 
                                                SET FullName = @fullName, BirthDate = @birthDate
                                                WHERE StudentId = @studentId;";
                        using (var command = new SQLiteCommand(updateStudent, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@fullName", fullName);
                            command.Parameters.AddWithValue("@birthDate", birthDate?.ToString("yyyy-MM-dd"));
                            command.Parameters.AddWithValue("@studentId", studentId);
                            command.ExecuteNonQuery();
                        }
                        string updateGrades = @"UPDATE Grades 
                                              SET PhysicsGrade = @physicsGrade, MathGrade = @mathGrade 
                                              WHERE StudentId = @studentId;";

                        using (var command = new SQLiteCommand(updateGrades, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@physicsGrade", physicsGrade);
                            command.Parameters.AddWithValue("@mathGrade", mathGrade);
                            command.Parameters.AddWithValue("@studentId", studentId);
                            command.ExecuteNonQuery();
                        }    
                            transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public void DeleteStudent(int studentId)
        {
            using( var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string deleteGrades = "DELETE FROM Grades WHERE StudentId = @studentId;";
                        using (var command = new SQLiteCommand(deleteGrades, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@studentId", studentId);
                            command.ExecuteNonQuery();
                        }
                        string deleteStudent = "DELETE FROM Students WHERE StudentId = @studentId;";
                        using (var command = new SQLiteCommand(deleteStudent, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@studentId", studentId);
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
