/**
 * Name:    Jarryd Hassall
 * SID:     30063186
 * Date:    01/03/2023
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WikiApp
{
    public partial class WikiApp : Form
    {
        // Time will gradually update the status strip colour until default
        static Timer statusStripUpdater = new Timer();

        List<Information> wiki = new List<Information>();

        public WikiApp()
        {
            InitializeComponent();
            initializeWiki();

            // Configure timer
            statusStripUpdater.Tick += statusStripColourFade;
            statusStripUpdater.Interval = 5;
        }

        #region Input Getters
        private string getInputName()
        {
            return textBoxName.Text.ToLower();
        }

        private string getInputCategory()
        {
            return comboBoxCategory.Text.ToLower();
        }

        private string getInputStructure()
        {
            if (radioButtonLinear.Checked) {
                return "linear";
            }
            if (radioButtonNonLinear.Checked)
            {
                return "non-linear";
            }

            return null;
        }

        private string getInputDefinition()
        {
            return textBoxDefinition.Text.ToLower();
        }
        #endregion

        private void statusStripColourFade(object sender, EventArgs e)
        {
            // By how much to fade the current colour
            const int reductionFactor = 3;

            // Get the current colour of the status strip
            Color current = statusStrip.BackColor;
            
            // If the alpha is not 0, update it
            if (current.A > reductionFactor)
            {
                // Casually fade out the colour
                statusStrip.BackColor = Color.FromArgb(current.A - reductionFactor, current.R, current.G, current.B);
            } else
            {
                // Once it is 0, stop the timer and set the colour to completely transparent
                statusStrip.BackColor = Color.FromArgb(0, 0, 0, 0);
                statusStripUpdater.Stop();
            }
        }

        private int compareWikiString(string wikiStringA, string wikiStringB)
        {
            // Consider empty strings to be greater when compared against anything
            if (string.IsNullOrEmpty(wikiStringA))
            {
                return 1;
            } 
            else if (string.IsNullOrEmpty(wikiStringB))
            {
                return -1;
            }

            // Otherwise return the result of a regular string comparison
            return string.Compare(wikiStringA, wikiStringB);
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

        private void updateStatus(string status, Color colour)
        {
            statusStripLabel.Text = status;
            statusStrip.BackColor = colour;
            
            // Colour updated, start the fade effect
            statusStripUpdater.Start();
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

        // 9.5 Create a CLEAR method to clear the four text boxes so a new definition can be added,
        private void clearFields()
        {
            // Reset each field to an empty string
            textBoxName.Text = "";
            textBoxCategory.Text = "";
            textBoxStructure.Text = "";
            textBoxDefinition.Text = "";

            // Reset search box
            textBoxSearch.Text = "";
        }

        // 9.8 Create a display method that will show the following information in a ListView: Name and Category,
        private void displayFieldData(int index)
        {
            // Update input fields to reflect the data
            textBoxName.Text = wiki[index, 0];
            textBoxCategory.Text = wiki[index, 1];
            textBoxStructure.Text = wiki[index, 2];
            textBoxDefinition.Text = wiki[index, 3];
        }

        //9.2 Create an ADD button that will store the information from the 4 text boxes into the 2D array,
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Validate all form data
            string name = getInputName();
            string category = getInputCategory();
            string structure = getInputStructure();
            string definition = getInputDefinition();
            if (string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(category) ||
                string.IsNullOrEmpty(structure) ||
                string.IsNullOrEmpty(definition))
            {
                updateStatus("Invalid field input", Color.Red);
                return;
            }

            // Check for unique values
            if (wiki.FindIndex(i => i.getName().CompareTo(name) == 0) > -1)
            {
                updateStatus("Only unique name values may be inserted", Color.Red);
                return;
            }

            // Add the new item
            wiki.Add(new Information(name, category, structure, definition));
            updateStatus("Wiki item inserted", Color.Green);
            clearFields();
            updateDisplay();
        }

        // 9.9 Create a method so the user can select a definition (Name) from the ListView and all the information is displayed in the appropriate Textboxes,
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

        // 9.3 Create an EDIT button that will allow the user to modify any information from the 4 text boxes into the 2D array,
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
                updateStatus("Invalid field input", Color.Red);
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
                updateStatus("Edited wiki item.", Color.Green);
                clearFields();
                updateDisplay();
                return;
            }

            // No item was selected
            updateStatus("Unable to update wiki item. No item selected.", Color.Red);
        }

        //9.4	Create a DELETE button that removes all the information from a single entry of the array; the user must be prompted before the final deletion occurs, 
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
                    updateStatus("Deleted wiki item.", Color.Green);
                    clearFields();
                    updateDisplay();
                }

                return;
            }

            // No item was selected
            updateStatus("Unable to delete wiki item. No item selected.", Color.Red);
        }

        // 9.10	Create a SAVE button so the information from the 2D array can be written into a binary file called definitions.dat which is sorted by Name, ensure the user has the option to select an alternative file. Use a file stream and BinaryWriter to create the file.
        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Create and configure SaveFielDialog
            SaveFileDialog sfd = new SaveFileDialog
            {
                Title = "Select dat file to save to",
                Filter = "DAT files|*.dat"
            };

            // Open the save file dialog and prompt the user to select a file to save to
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                // Nothing was selected, lets get out of here
                return;
            }

            // Open file to save data to
            using (var stream = File.Open(sfd.FileName, FileMode.Create))
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

            updateStatus("Saving to '" + Path.GetFileName(sfd.FileName) + "' complete.", Color.Green);
        }

        // 9.11	Create a LOAD button that will read the information from a binary file called definitions.dat into the 2D array, ensure the user has the option to select an alternative file. Use a file stream and BinaryReader to complete this task.
        private void buttonLoad_Click(object sender, EventArgs e)
        {
            // Create and configure OpenFileDialog
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Open dat file",
                Filter = "DAT files|*.dat"
            };

            // Open a file dialog box and prompt the user to select a file to open
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                // No file was selected, lets get out of here
                return;
            }

            // Check if the file exists
            if (File.Exists(ofd.FileName))
            {
                // Open the file
                using (var stream = File.Open(ofd.FileName, FileMode.Open))
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
                updateStatus("Reading file '" + ofd.SafeFileName + "' complete.", Color.Green);
                updateDisplay();
            }
            else
            {
                updateStatus("Unable to open file '" + ofd.SafeFileName + "'. File does not exist.", Color.Red);
            }
        }

        private void swap(int index)
        {
            // Store temp values
            string temp;

            // Swap items
            for (int i = 0; i < 4; i++)
            {
                temp = wiki[index, i];
                wiki[index, i] = wiki[index + 1, i];
                wiki[index + 1, i] = temp;
            }
        }

        //9.6	Write the code for a Bubble Sort method to sort the 2D array by Name ascending, ensure you use a separate swap method that passes the array element to be swapped (do not use any built-in array methods),
        private void sortData()
        {
            // Bubble sort the array
            // Sorting by name means we target column index 0
            for (int row = 0; row < rows - 1; row++)
            {
                for (int swapIndex = 0; swapIndex < rows - 1 - row; swapIndex++)
                {
                    if (compareWikiString(wiki[swapIndex, 0], wiki[swapIndex + 1, 0]) > 0)
                    {
                        // Swap swapIndex with swapIndex + 1
                        swap(swapIndex);
                    }
                }
            }

            // Sorting is complete, update display
            updateStatus("Wiki sorted by name.", Color.Green);
            updateDisplay();
        }

        private void buttonSort_Click(object sender, EventArgs e)
        {
            sortData();
        }

        //9.7	Write the code for a Binary Search for the Name in the 2D array and display the information in the other textboxes when found, add suitable feedback if the search in not successful and clear the search textbox (do not use any built-in array methods),
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            // Sort the data first
            sortData();

            // validate the search term
            string target = textBoxSearch.Text;

            if (string.IsNullOrEmpty(target))
            {
                updateStatus("Input is invalid.", Color.Red);
                return;
            }

            // target is a valid choice, search for it with binary search
            int mid;
            int first = 0;
            int last = rows - 1;

            int comparisonResult;

            while (first <= last)
            {
                mid = (first + last) / 2;

                // Compare the entry and the target
                comparisonResult = compareWikiString(wiki[mid, 0], target);

                // Check for match
                if (comparisonResult == 0)
                {
                    listViewDisplay.Items[mid].Selected = true;
                    updateStatus("Found target value.", Color.Green);
                    clearFields();
                    displayFieldData(mid);
                    return;
                }

                // Move the first and last if applicable
                if (comparisonResult > 0)
                {
                    last = mid - 1;
                }
                else
                {
                    first = mid + 1;
                }
            }

            updateStatus("Target not found in wiki.", Color.Red);
        }

        private void textBoxName_DoubleClick(object sender, EventArgs e)
        {
            clearFields();
            textBoxName.Focus();
        }
    }
}
