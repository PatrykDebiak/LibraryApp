using LibraryApp.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using Npgsql;

namespace LibraryApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadAuthors();
            LoadBooks();
            AddBookButton.Click += AddBookButton_Click;
            AddAuthorButton.Click += AddAuthorButton_Click;
            DeleteBookButton.Click += DeleteBookButton_Click;
        }

        private void LoadAuthors()
        {
            try
            {
                var authors = db.GetAuthors();
                AuthorComboBox.ItemsSource = authors;
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error loading authors: {ex.Message}";
            }
        }

        private void LoadBooks()
        {
            try
            {
                BooksDataGrid.ItemsSource = db.GetBooks();
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error loading books: {ex.Message}";
            }
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TitleTextBox.Text) || AuthorComboBox.SelectedValue == null ||
                    !int.TryParse(YearTextBox.Text, out int year))
                {
                    StatusTextBlock.Text = "Please enter valid book details.";
                    return;
                }

                var book = new Book
                {
                    Title = TitleTextBox.Text,
                    AuthorID = (int)AuthorComboBox.SelectedValue,
                    PublishedYear = year,
                    Available = AvailableCheckBox.IsChecked ?? true
                };

                db.AddBook(book);
                StatusTextBlock.Text = "Book added successfully.";
                ClearForm();
                LoadBooks();
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error adding book: {ex.Message}";
            }
        }

        private void ClearForm()
        {
            TitleTextBox.Clear();
            AuthorComboBox.SelectedIndex = -1;
            YearTextBox.Clear();
            AvailableCheckBox.IsChecked = true;
        }

        private bool isFormVisible = true;
        private static string _connectionString = "Host=localhost;Username=postgres;Password=1833;Database=DBLibrary";

        private void ToggleFormButton_Click(object sender, RoutedEventArgs e)
        {
            isFormVisible = !isFormVisible;
            FormPanel.Visibility = isFormVisible ? Visibility.Visible : Visibility.Collapsed;
            ToggleFormButton.Content = isFormVisible ? "Hide Form" : "Show Form";
        }

        private void AddAuthorButton_Click(object sender, RoutedEventArgs e)
        {
            string name = AuthorNameTextBox.Text.Trim();
            string birthDateText = AuthorBirthDateTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(birthDateText))
            {
                StatusTextBlock.Text = "Please fill in all author fields.";
                return;
            }

            if (!DateTime.TryParse(birthDateText, out DateTime birthDate))
            {
                StatusTextBlock.Text = "Invalid date format. Use YYYY-MM-DD.";
                return;
            }

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Authors (Name, BirthDate) VALUES (@name, @birthDate)";
                
                string checkSql = "SELECT COUNT(*) FROM Authors WHERE Name = @name AND BirthDate = @birthDate";
                using (var checkCmd = new NpgsqlCommand(checkSql, conn))
                {
                    checkCmd.Parameters.AddWithValue("name", name);
                    checkCmd.Parameters.AddWithValue("birthDate", birthDate);

                    long count = (long)checkCmd.ExecuteScalar();
                    if (count > 0)
                    {
                        StatusTextBlock.Text = "Autor już istnieje w bazie danych.";
                        return;
                    }
                }

              
                string insertSql = "INSERT INTO Authors (Name, BirthDate) VALUES (@name, @birthDate)";
                using (var insertCmd = new NpgsqlCommand(insertSql, conn))
                {
                    insertCmd.Parameters.AddWithValue("name", name);
                    insertCmd.Parameters.AddWithValue("birthDate", birthDate);
                    insertCmd.ExecuteNonQuery();
                }

            }

            StatusTextBlock.Text = "Author added successfully.";
            LoadAuthors(); 
        }

        private void DeleteBookButton_Click(object sender, RoutedEventArgs e)
        {
           
            if (BooksDataGrid.SelectedItem is BookDisplay selectedBook)
            {
                
                var result = MessageBox.Show($"Czy na pewno chcesz usunąć książkę '{selectedBook.Title}'?",
                    "Potwierdzenie usunięcia",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var conn = new NpgsqlConnection(_connectionString))
                        {
                            conn.Open();
                            string sql = "DELETE FROM Books WHERE BookID = @id";
                            using (var cmd = new NpgsqlCommand(sql, conn))
                            {
                                cmd.Parameters.AddWithValue("id", selectedBook.BookID);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        StatusTextBlock.Text = $"Książka '{selectedBook.Title}' została usunięta.";
                        LoadBooks(); 
                    }
                    catch (Exception ex)
                    {
                        StatusTextBlock.Text = $"Błąd podczas usuwania książki: {ex.Message}";
                    }
                }
            }
            else
            {
                StatusTextBlock.Text = "Nie wybrano książki do usunięcia.";
            }
        }
    }
}
