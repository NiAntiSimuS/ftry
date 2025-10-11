using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OBJ
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        DataTable todoList = new DataTable();
        bool isEditing = false;
        private void ToDoList_Load(object sender, EventArgs e)
        {
            // Create columns
            todoList.Columns.Add("Title");
            todoList.Columns.Add("Description");

            // Point our datagridview to our datasource
            toDoListView.DataSource = todoList;
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            titleTextBox.Text = "";
            descriptionTextbox.Text = "";
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            isEditing = true;
            // Fill text fields with data from table
            titleTextBox.Text = todoList.Rows[toDoListView.CurrentCell.RowIndex].ItemArray[0].ToString();
            descriptionTextbox.Text = todoList.Rows[toDoListView.CurrentCell.RowIndex].ItemArray[1].ToString();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                todoList.Rows[toDoListView.CurrentCell.RowIndex].Delete();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (isEditing)
            {
                todoList.Rows[toDoListView.CurrentCell.RowIndex]["Title"] = titleTextBox.Text;
                todoList.Rows[toDoListView.CurrentCell.RowIndex]["Description"] = descriptionTextbox.Text;
            }
            else
            {
                todoList.Rows.Add(titleTextBox.Text, descriptionTextbox.Text);
            }
            // Clear fields
            titleTextBox.Text = "";
            descriptionTextbox.Text = "";
            isEditing = false;
        }
    }
}