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
using System.Xml.Linq;

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

            // Configure timer
            statusStripUpdater.Tick += statusStripColourFade;
            statusStripUpdater.Interval = 5;
        }

        #region Input Getters | Setters
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
            // Get if any the selected radio button
            RadioButton selected = groupBoxStructure.Controls.OfType<RadioButton>().FirstOrDefault(rbtn => rbtn.Checked);
            
            // If a button is selected, return its text
            if (selected != null)
            {
                return selected.Text.ToLower();
            }

            return null;
        }

        private string getInputSearch()
        {
            return textBoxSearch.Text.ToLower();
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
            foreach (Information info in wiki)
            {
                // Populate the item
                ListViewItem item = new ListViewItem(info.getName());
                item.SubItems.Add(info.getCategory());

                // Add the item
                listViewDisplay.Items.Add(item);
            }
        }

        private void clearFields()
        {
            // Reset each field to an empty string
            textBoxName.Text = "";
            comboBoxCategory.SelectedIndex = -1;
            textBoxDefinition.Text = "";

            // Reset search box
            textBoxSearch.Text = "";

            // Reset the radio buttons
            foreach (RadioButton btn in groupBoxStructure.Controls.OfType<RadioButton>())
            {
                btn.Checked = false;
            }
        }

        private void displayFieldData(Information info)
        {
            // Update input fields to reflect the data
            // Set the easy values
            textBoxName.Text = info.getName();
            textBoxDefinition.Text = info.getDefinition(); ;

            // Set the combobox
            comboBoxCategory.SelectedIndex = comboBoxCategory.FindStringExact(info.getCategory());

            // Try to find a radio button with the right text
            RadioButton target = groupBoxStructure.Controls.OfType<RadioButton>().FirstOrDefault(rbtn => rbtn.Text.ToLower().CompareTo(info.getStructure()) == 0);

            // If a radio button was found great!
            // If not tell the user of the error
            if (target != null)
            {
                target.Checked = true;
            }
            else
            {
                updateStatus("Error parsing structure from selection?", Color.Red);
            }
        }

        private bool containsNumOrSpecial(string check)
        {
            return check.Any(ch => !(char.IsWhiteSpace(ch) || char.IsLetter(ch)));   
        }
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Validate all form data
            string name = getInputName();
            string category = getInputCategory();
            string structure = getInputStructure();
            string definition = getInputDefinition();
            if (string.IsNullOrEmpty(name) ||
                containsNumOrSpecial(name) ||
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

        private void listViewDisplay_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex;
            // Set the text box fields
            if (listViewDisplay.SelectedIndices.Count > 0)
            {
                // What index in the list has the user selected
                selectedIndex = listViewDisplay.SelectedIndices[0];

                // Display the data in the input field boxes
                displayFieldData(wiki[selectedIndex]);
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            // Validate input
            string name = getInputName();
            string category = getInputCategory();
            string structure = getInputStructure();
            string definition = getInputDefinition();
            if (string.IsNullOrEmpty(name) ||
                containsNumOrSpecial(name) ||
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
                wiki[selectedIndex] = new Information(name, category, structure, definition);

                // Update the display
                updateStatus("Edited wiki item.", Color.Green);
                clearFields();
                updateDisplay();
                return;
            }

            // No item was selected
            updateStatus("Unable to update wiki item. No item selected.", Color.Red);
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
                    // Clear the information
                    wiki.RemoveAt(selectedIndex);

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
                    // Write the number of records being saved
                    writer.Write(wiki.Count);

                    // Write each wiki entry to the file
                    foreach (Information info in wiki)
                    {
                        writer.Write(info.getName());
                        writer.Write(info.getCategory());
                        writer.Write(info.getStructure());
                        writer.Write(info.getDefinition());
                    }
                }
            }

            updateStatus("Saving to '" + Path.GetFileName(sfd.FileName) + "' complete.", Color.Green);
        }

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
                        int numRecords = reader.ReadInt32();

                        // Read each item from the file
                        for (int i = 0; i < numRecords; i++)
                        {
                            wiki.Add(new Information(
                                reader.ReadString(),
                                reader.ReadString(),
                                reader.ReadString(),
                                reader.ReadString())
                                );
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
            Information temp = wiki[index + 1];

            // Swap items
            wiki[index + 1] = wiki[index];
            wiki[index] = temp;
        }

        private void sortData()
        {
            int count = wiki.Count;

            // Bubble sort the list
            for (int limit = 0; limit < count; limit++)
            {
                for (int swapIndex = 0; swapIndex < count - limit - 1; swapIndex++)
                {
                    if (wiki[swapIndex].CompareTo(wiki[swapIndex + 1]) > 0)
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

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            // Sort the data first
            sortData();

            // validate the search term
            string target = getInputSearch();

            if (string.IsNullOrEmpty(target))
            {
                updateStatus("Input is invalid.", Color.Red);
                return;
            }

            // target is a valid choice, search for it with binary search
            int mid;
            int first = 0;
            int last = wiki.Count - 1;

            int comparisonResult;

            while (first <= last)
            {
                mid = (first + last) / 2;

                // Compare the entry and the target
                comparisonResult = wiki[mid].getName().CompareTo(target);

                // Check for match
                if (comparisonResult == 0)
                {
                    listViewDisplay.Items[mid].Selected = true;
                    updateStatus("Found target value.", Color.Green);
                    clearFields();
                    displayFieldData(wiki[mid]);
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
