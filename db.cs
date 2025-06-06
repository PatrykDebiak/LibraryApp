using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using LibraryApp.Models;
namespace LibraryApp;

public class db
{
      private static string connectionString = "Host=localhost;Username=postgres;Password=1833;Database=DBLibrary";

        public static List<Author> GetAuthors()
        {
            var authors = new List<Author>();
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            var cmd = new NpgsqlCommand("SELECT * FROM Authors", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                authors.Add(new Author
                {
                    AuthorID = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    BirthDate = reader.GetDateTime(2)
                });
            }

            return authors;
        }

        public static List<BookDisplay> GetBooks()
        {
            var books = new List<BookDisplay>();
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            
            var cmd = new NpgsqlCommand(@"
                   SELECT b.BookID, b.Title, a.Name AS AuthorName, b.PublishedYear, b.Available
                FROM Books b
                JOIN Authors a ON b.AuthorID = a.AuthorID", conn);


            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                books.Add(new BookDisplay
                {
                    
                    BookID = reader.GetInt32(reader.GetOrdinal("BookID")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    AuthorName = reader.GetString(reader.GetOrdinal("AuthorName")),
                    PublishedYear = reader.GetInt32(reader.GetOrdinal("PublishedYear")),
                    Available = reader.GetBoolean(reader.GetOrdinal("Available"))

                });
            }

            return books;
        }

        public static void AddBook(Book book)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            var cmd = new NpgsqlCommand(@"
                INSERT INTO Books (Title, AuthorID, PublishedYear, Available)
                VALUES (@title, @auth, @year, @avail)", conn);

            cmd.Parameters.AddWithValue("title", book.Title);
            cmd.Parameters.AddWithValue("auth", book.AuthorID);
            cmd.Parameters.AddWithValue("year", book.PublishedYear);
            cmd.Parameters.AddWithValue("avail", book.Available);

            cmd.ExecuteNonQuery();
        }
}