using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace OBJ
{
    public partial class MainWindow : Page
    {
        private DataTable todoList;
        private bool isEditing = false;
        private string dataFilePath = "todolist.json";

        public MainWindow()
        {
            InitializeComponent();
            InitializeDataTable();
            InitializeEventHandlers();
            LoadDataFromJson();
        }

        private void InitializeDataTable()
        {
            
            todoList = new DataTable("TodoList");

            
            todoList.Columns.Add("Title", typeof(string));
            todoList.Columns.Add("Description", typeof(string));

            
            ToDoListView.ItemsSource = todoList.DefaultView;
        }

        private void InitializeEventHandlers()
        {
            
            newButton.Click += newButton_Click;
            editButton.Click += editButton_Click;
            deleteButton.Click += deleteButton_Click;
            saveButton.Click += saveButton_Click;
            ToDoListView.SelectionChanged += ToDoListView_SelectionChanged;
        }

        
        public class TodoItem
        {
            public string Title { get; set; }
            public string Description { get; set; }
        }

        
        private void LoadDataFromJson()
        {
            try
            {
                if (File.Exists(dataFilePath))
                {
                    string json = File.ReadAllText(dataFilePath);
                    List<TodoItem> items = JsonConvert.DeserializeObject<List<TodoItem>>(json);

                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            DataRow newRow = todoList.NewRow();
                            newRow["Title"] = item.Title;
                            newRow["Description"] = item.Description;
                            todoList.Rows.Add(newRow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        private void SaveDataToJson()
        {
            try
            {
                List<TodoItem> items = new List<TodoItem>();

                foreach (DataRow row in todoList.Rows)
                {
                    
                    if (row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached)
                    {
                        items.Add(new TodoItem
                        {
                            Title = row["Title"].ToString(),
                            Description = row["Description"].ToString()
                        });
                    }
                }

                string json = JsonConvert.SerializeObject(items, Formatting.Indented);
                File.WriteAllText(dataFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            titleTextBox.Text = "";
            descriptionTextBox.Text = "";
            isEditing = false;
            ToDoListView.UnselectAll();
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            if (ToDoListView.SelectedItem == null)
            {
                MessageBox.Show("Please select an item to edit.", "No Selection",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            isEditing = true;
            
            DataRowView selectedRow = (DataRowView)ToDoListView.SelectedItem;
            titleTextBox.Text = selectedRow["Title"].ToString();
            descriptionTextBox.Text = selectedRow["Description"].ToString();
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ToDoListView.SelectedItem == null)
            {
                MessageBox.Show("Please select an item to delete.", "No Selection",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                DataRowView selectedRow = (DataRowView)ToDoListView.SelectedItem;
                selectedRow.Delete();
                todoList.AcceptChanges();

                
                titleTextBox.Text = "";
                descriptionTextBox.Text = "";
                isEditing = false;

                
                SaveDataToJson();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting item: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(titleTextBox.Text))
            {
                MessageBox.Show("Please enter a title.", "Validation Error",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (isEditing)
                {
                    if (ToDoListView.SelectedItem != null)
                    {
                        DataRowView selectedRow = (DataRowView)ToDoListView.SelectedItem;
                        selectedRow.BeginEdit();
                        selectedRow["Title"] = titleTextBox.Text;
                        selectedRow["Description"] = descriptionTextBox.Text;
                        selectedRow.EndEdit();
                    }
                }
                else
                {
                    DataRow newRow = todoList.NewRow();
                    newRow["Title"] = titleTextBox.Text;
                    newRow["Description"] = descriptionTextBox.Text;
                    todoList.Rows.Add(newRow);
                }

                
                todoList.AcceptChanges();
                SaveDataToJson();

                
                titleTextBox.Text = "";
                descriptionTextBox.Text = "";
                isEditing = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving item: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToDoListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ghfgf
            deleteButton.IsEnabled = ToDoListView.SelectedItem != null;
            editButton.IsEnabled = ToDoListView.SelectedItem != null;
        }
    }
}