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

        // 6.2 Create a global List<T> of type Information called Wiki.
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

        // 6.6 Create two methods to highlight and return the values from the Radio button GroupBox. The first method must return a string value from the selected radio button (Linear or Non-Linear). The second method must send an integer index which will highlight an appropriate radio button.
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

        // 6.6 Create two methods to highlight and return the values from the Radio button GroupBox. The first method must return a string value from the selected radio button (Linear or Non-Linear). The second method must send an integer index which will highlight an appropriate radio button.
        private void setSelectedStructure(int index)
        {
            // 0 for linear
            // 1 for non linear
            switch (index)
            {
                case 0:
                    radioButtonLinear.Checked = true;
                    break;
                case 1:
                    radioButtonNonLinear.Checked = true;
                    break;
            }
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

        private void updateStatus(string status, Color colour)
        {
            statusStripLabel.Text = status;
            statusStrip.BackColor = colour;
            
            // Colour updated, start the fade effect
            statusStripUpdater.Start();
        }

        // 6.9 Create a single custom method that will sort and then display the Name and Category from the wiki information in the list.
        private void updateAndSortDisplay()
        {
            // Sort the list
            wiki.Sort();

            // Clear the display
            listViewDisplay.Items.Clear();

            // Add items to the display
            foreach (var info in wiki)
            {
                // Populate the item
                ListViewItem item = new ListViewItem(info.getName());
                item.SubItems.Add(info.getCategory());

                // Add the item
                listViewDisplay.Items.Add(item);
            }
        }

        // 6.12 Create a custom method that will clear and reset the TextBoxes, ComboBox and Radio button
        private void clearFields()
        {
            // Reset each field to an empty string
            textBoxName.Text = "";
            comboBoxCategory.SelectedIndex = -1;
            textBoxDefinition.Text = "";

            // Reset search box
            textBoxSearch.Text = "";

            // Reset the radio buttons
            foreach (var btn in groupBoxStructure.Controls.OfType<RadioButton>())
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

            // Set the structure
            switch (info.getStructure())
            {
                case "linear":
                    setSelectedStructure(0);
                    break;
                case "non-linear":
                    setSelectedStructure(1);
                    break;
                default:
                    updateStatus("Error parsing structure from selection", Color.Red);
                    break;
            }
        }

        // 6.5 Create a custom ValidName method which will take a parameter string value from the Textbox Name and returns a Boolean after checking for duplicates. Use the built in List<T> method “Exists” to answer this requirement.
        private bool validName(string name)
        {
            // Check for null empty
            if (string.IsNullOrEmpty(name))
            {
                return false;
            } else if (name.Any(ch => (!char.IsWhiteSpace(ch) && !char.IsLetter(ch))))
            {
                return false;
            }
            return true;
        }

        //6.3 Create a button method to ADD a new item to the list. Use a TextBox for the Name input, ComboBox for the Category, Radio group for the Structure and Multiline TextBox for the Definition.
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Validate all form data
            string name = getInputName();
            string category = getInputCategory();
            string structure = getInputStructure();
            string definition = getInputDefinition();
            if (!validName(name) ||
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
            updateAndSortDisplay();
        }

        // 6.11 Create a ListView event so a user can select a Data Structure Name from the list of Names and the associated information will be displayed in the related text boxes combo box and radio button.
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

        // 6.8 Create a button method that will save the edited record of the currently selected item in the ListView. All the changes in the input controls will be written back to the list. Display an updated version of the sorted list at the end of this process.
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            // Validate input
            string name = getInputName();
            string category = getInputCategory();
            string structure = getInputStructure();
            string definition = getInputDefinition();
            if (validName(name) ||
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
                updateAndSortDisplay();
                return;
            }

            // No item was selected
            updateStatus("Unable to update wiki item. No item selected.", Color.Red);
        }

        // 6.7 Create a button method that will delete the currently selected record in the ListView. Ensure the user has the option to backout of this action by using a dialog box. Display an updated version of the sorted list at the end of this process.
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
                    updateAndSortDisplay();
                }

                return;
            }

            // No item was selected
            updateStatus("Unable to delete wiki item. No item selected.", Color.Red);
        }

        private void saveWiki()
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
                    foreach (var info in wiki)
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

        // 6.14 Create two buttons for the manual open and save option; this must use a dialog box to select a file or rename a saved file. All Wiki data is stored/retrieved using a binary reader/writer file format.
        private void buttonSave_Click(object sender, EventArgs e)
        {
            saveWiki();
        }

        // 6.14 Create two buttons for the manual open and save option; this must use a dialog box to select a file or rename a saved file. All Wiki data is stored/retrieved using a binary reader/writer file format.
        private void buttonLoad_Click(object sender, EventArgs e)
        {
            // Create and configure OpenFileDialog
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Open dat file",
                Filter = "DAT files|*.dat"
            };
            ofd.InitialDirectory = Application.StartupPath;

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
                updateAndSortDisplay();
            }
            else
            {
                updateStatus("Unable to open file '" + ofd.SafeFileName + "'. File does not exist.", Color.Red);
            }
        }

        private void buttonSort_Click(object sender, EventArgs e)
        {
            updateAndSortDisplay();
        }

        // 6.10 Create a button method that will use the builtin binary search to find a Data Structure name. If the record is found the associated details will populate the appropriate input controls and highlight the name in the ListView. At the end of the search process the search input TextBox must be cleared.
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            // Sort the data first
            updateAndSortDisplay();

            // Get the target and binary search
            string target = getInputSearch();
            int index = wiki.BinarySearch(new Information(target, null, null, null));

            // Select if found
            if (index >= 0)
            {
                listViewDisplay.Items[index].Selected = true;
                updateStatus("Found target value.", Color.Green);
                clearFields();
                displayFieldData(wiki[index]);
            } else
            {
                updateStatus("Target not found in wiki.", Color.Red);
            }
        }

        // 6.13 Create a double click event on the Name TextBox to clear the TextBboxes, ComboBox and Radio button.
        private void textBoxName_DoubleClick(object sender, EventArgs e)
        {
            clearFields();
            textBoxName.Focus();
        }

        //6.4 Create a custom method to populate the ComboBox when the Form Load method is called. The six categories must be read from a simple text file.
        private void WikiApp_Load(object sender, EventArgs e)
        {
            try
            {
                foreach (var value in System.IO.File.ReadLines(Application.StartupPath + @"\cat.txt"))
                {
                    comboBoxCategory.Items.Add(value);
                }
            }
            catch
            {
                updateStatus("Unable to open category file: " + Application.StartupPath + @"\cat.txt", Color.Red);
            }
        }

        // 6.15 The Wiki application will save data when the form closes. 
        private void WikiApp_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if the user wants to save then save
            if (MessageBox.Show("Do you want to save before closing?", "Save?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                saveWiki();
            }
        }
    }
}
