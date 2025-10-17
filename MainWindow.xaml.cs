using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace OBJ
{
    public partial class MainWindow : Page
    {
        private DataTable todoList;
        private bool isEditing = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeDataTable();
            InitializeEventHandlers();
        }

        private void InitializeDataTable()
        {
            // Create DataTable
            todoList = new DataTable("TodoList");

            // Create columns
            todoList.Columns.Add("Title", typeof(string));
            todoList.Columns.Add("Description", typeof(string));

            // Point our datagrid to our datasource
            ToDoListView.ItemsSource = todoList.DefaultView;
        }

        private void InitializeEventHandlers()
        {
            // Подписываемся на события кнопок
            newButton.Click += newButton_Click;
            editButton.Click += editButton_Click;
            deleteButton.Click += deleteButton_Click;
            saveButton.Click += saveButton_Click;
            ToDoListView.SelectionChanged += ToDoListView_SelectionChanged;
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
            // Fill text fields with data from selected row
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

                // Clear fields after deletion
                titleTextBox.Text = "";
                descriptionTextBox.Text = "";
                isEditing = false;

                // Обновляем DataTable
                todoList.AcceptChanges();
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

                // Clear fields and reset editing state
                titleTextBox.Text = "";
                descriptionTextBox.Text = "";
                isEditing = false;

                // Refresh the DataGrid view
                todoList.AcceptChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving item: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToDoListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable/disable buttons based on selection
            deleteButton.IsEnabled = ToDoListView.SelectedItem != null;
            editButton.IsEnabled = ToDoListView.SelectedItem != null;
        }
    }
}