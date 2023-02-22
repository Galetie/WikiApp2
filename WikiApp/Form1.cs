using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WikiApp
{
    public partial class WikiApp : Form
    {
        const string fileName = "definitions.dat";
        const int rows = 12;
        const int columns = 4;
        string[,] wiki = new string[rows, columns];
        public WikiApp()
        {
            InitializeComponent();
            initializeWiki();
        }

        private void initializeWiki()
        {
            // Initialize the wiki array with empty strings
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    wiki[row, column] = "";
                }
            }
        }

        private void updateStatus(string status)
        {
            statusStripLabel.Text = status;
        }

        private void updateDisplay()
        {
            // Clear the display
            listViewDisplay.Items.Clear();

            // Add items to the display
            for (int row = 0; row < rows; row++)
            {
                // Populate the item
                ListViewItem item = new ListViewItem(wiki[row, 0]);
                item.SubItems.Add(wiki[row, 1]);

                // Add the item
                listViewDisplay.Items.Add(item);
            }
        }

        private void clearFields()
        {
            // Reset each field to an empty string
            textBoxName.Text = "";
            textBoxCategory.Text = "";
            textBoxStructure.Text = "";
            textBoxDefinition.Text = "";
        }

        private void displayFieldData(int index)
        {
            // Update input fields to reflect the data
            textBoxName.Text = wiki[index, 0];
            textBoxCategory.Text = wiki[index, 1];
            textBoxStructure.Text = wiki[index, 2];
            textBoxDefinition.Text = wiki[index, 3];
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Validate all form data
            string name = textBoxName.Text;
            string category = textBoxCategory.Text;
            string structure = textBoxStructure.Text;
            string definition = textBoxDefinition.Text;
            if (string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(category) ||
                string.IsNullOrEmpty(structure) ||
                string.IsNullOrEmpty(definition))
            {
                updateStatus("Invalid field input");
                return;
            }

            // If everything is valid, insert the new item
            // Find an empty spot to insert in to
            for (int row = 0; row < rows; row++)
            {
                if (string.IsNullOrEmpty(wiki[row, 0]))
                {
                    // Found a spot to insert to
                    wiki[row, 0] = name;
                    wiki[row, 1] = category;
                    wiki[row, 2] = structure;
                    wiki[row, 3] = definition;

                    // Inserted so exit
                    updateStatus("Wiki item inserted");
                    clearFields();
                    updateDisplay();
                    return;
                }
            }

            // If loop finishes, wiki is full
            updateStatus("Unable to insert wiki item. Wiki is full.");

        }

        private void listViewDisplay_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex;
            // Set the text box fields
            if (listViewDisplay.SelectedIndices.Count > 0)
            {
                // What index in the list has the user selected
                selectedIndex = listViewDisplay.SelectedIndices[0];

                // Display the data in the input field boxes
                displayFieldData(selectedIndex);
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            // Validate input
            string name = textBoxName.Text;
            string category = textBoxCategory.Text;
            string structure = textBoxStructure.Text;
            string definition = textBoxDefinition.Text;
            if (string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(category) ||
                string.IsNullOrEmpty(structure) ||
                string.IsNullOrEmpty(definition))
            {
                updateStatus("Invalid field input");
                return;
            }

            // Update item
            int selectedIndex;

            // Get the current selected index
            if (listViewDisplay.SelectedIndices.Count > 0)
            {
                // What index in the list has the user selected
                selectedIndex = listViewDisplay.SelectedIndices[0];

                // Update input fields to reflect the data
                wiki[selectedIndex, 0] = textBoxName.Text;
                wiki[selectedIndex, 1] = textBoxCategory.Text;
                wiki[selectedIndex, 2] = textBoxStructure.Text;
                wiki[selectedIndex, 3] = textBoxDefinition.Text;

                // Update the display
                updateStatus("Edited wiki item.");
                clearFields();
                updateDisplay();
                return;
            }

            // No item was selected
            updateStatus("Unable to update wiki item. No item selected.");
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            int selectedIndex;

            // Get the current selected index
            if (listViewDisplay.SelectedIndices.Count > 0)
            {
                // What index in the list has the user selected
                selectedIndex = listViewDisplay.SelectedIndices[0];

                // Confirm the user meant to hit delete
                DialogResult dialogResult = MessageBox.Show("Are you sure you wish to delete this item?",
                    "Warning", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    // Clear the fields
                    wiki[selectedIndex, 0] = "";
                    wiki[selectedIndex, 1] = "";
                    wiki[selectedIndex, 2] = "";
                    wiki[selectedIndex, 3] = "";

                    // Update the display
                    updateStatus("Deleted wiki item.");
                    clearFields();
                    updateDisplay();
                }

                return;
            }

            // No item was selected
            updateStatus("Unable to delete wiki item. No item selected.");
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Open file to save data to
            using (var stream = File.Open(fileName, FileMode.Create))
            {
                // Create a binary writer to write data to the file
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    // Write each wiki entry to the file
                    for (int row = 0; row < rows; row++)
                    {
                        writer.Write(wiki[row, 0]);
                        writer.Write(wiki[row, 1]);
                        writer.Write(wiki[row, 2]);
                        writer.Write(wiki[row, 3]);
                    }
                }
            }

            updateStatus("Saving to " + fileName + " complete.");
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            // Check if the file exists
            if (File.Exists(fileName))
            {
                // Open the file
                using (var stream = File.Open(fileName, FileMode.Open))
                {
                    // Create a reader to read from the file stream
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        // Read each item from the file
                        for (int row = 0; row < rows; row++)
                        {
                            wiki[row, 0] = reader.ReadString();
                            wiki[row, 1] = reader.ReadString();
                            wiki[row, 2] = reader.ReadString();
                            wiki[row, 3] = reader.ReadString();
                        }
                    }
                }

                // Update display
                updateStatus("Reading file '" + fileName + "' complete.");
                updateDisplay();
            }
            else
            {
                updateStatus("Unable to open file '" + fileName + "'. File does not exist.");
            }
        }

        private void swap(int index)
        {
            // Swap index and index + 1
            string name = wiki[index, 0];
            string category = wiki[index, 1];
            string structure = wiki[index, 2];
            string definition = wiki[index, 3];

            // Swap row at index with index + 1
            wiki[index, 0] = wiki[index + 1, 0];
            wiki[index, 1] = wiki[index + 1, 1];
            wiki[index, 2] = wiki[index + 1, 2];
            wiki[index, 3] = wiki[index + 1, 3];

            // Set row at index + 1 with the data from row at index
            wiki[index + 1, 0] = name;
            wiki[index + 1, 1] = category;
            wiki[index + 1, 2] = structure;
            wiki[index + 1, 3] = definition;
        }

        private void buttonSort_Click(object sender, EventArgs e)
        {
            // Bubble sort the array
            // Sorting by name means we target column index 0
            for (int row = 0; row < rows - 1; row++)
            {
                for (int swapIndex = 0; swapIndex < rows - 1 - row; swapIndex++)
                {
                    // Check if either entry is empty
                    if (string.IsNullOrEmpty(wiki[swapIndex, 0]) || string.IsNullOrEmpty(wiki[swapIndex + 1, 0]))
                    {
                        // Count empty strings as large so they end up on the bottom
                        continue;
                    }
                    else if (string.Compare(wiki[swapIndex, 0], wiki[swapIndex + 1, 0]) == 1)
                    {
                        swap(swapIndex);
                    }
                }
            }

            // Sorting is complete, update display
            updateStatus("Wiki sorted by name.");
            updateDisplay();
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            // validate the search term
            string target = textBoxName.Text;

            if (string.IsNullOrEmpty(target))
            {
                updateStatus("Input is invalid.");
            }

            // target is a valid choice, search for it with binary search
            int mid;
            int first = 0;
            int last = rows;

            int comparisonResult;

            while (first <= last)
            {
                mid = (first + last) / 2;
                
                // Account for possible empty spots in the sorted array
                if (string.IsNullOrEmpty(wiki[mid, 0]))
                {
                    last = mid - 1;
                    continue;
                }

                comparisonResult = string.Compare(wiki[mid, 0], target);

                // Check for match
                if (comparisonResult == 0)
                {
                    updateStatus("Found target value.");
                    listViewDisplay.Items[mid].Selected = true;
                    displayFieldData(mid);
                    return;
                }

                // Move the first and last if applicable
                if (comparisonResult == 1)
                {
                    last = mid - 1;
                }
                else
                {
                    first = mid + 1;
                }
            }

            updateStatus("Target not found in wiki.");
        }
    }
}
