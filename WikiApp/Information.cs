// 6.1 Create a separate class file to hold the four data items of the Data Structure
// (use the Data Structure Matrix as a guide). Use private properties for the fields
// which must be of type “string”. The class file must have separate setters and getters,
// add an appropriate IComparable for the Name attribute. Save the class as “Information.cs”.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiApp
{
    [Serializable]
    internal class Information : IComparable<Information>
    {
        // Information stored here
        private string name;
        private string category;
        private string structure;
        private string description;

        // Constructor
        public Information(string newName, string newCategory, string newStructure, string newDescription) {
            name = newName;
            category = newCategory;
            structure = newStructure; 
            description = newDescription;
        }

        // Implement the CompareTo function
        public int CompareTo(Information other)
        {
            // Check other exists
            if (other == null)
            {
                return 1;
            }

            // Cast to Information and check success
            Information otherRecord = other as Information;
            if (otherRecord != null)
            {
                return this.name.CompareTo(otherRecord.name);
            }

            // Other object isn't a record type?
            throw new ArgumentException("Other is not of type Record");
        }

        // Getter for each property
        public string getName() { return name; }
        public string getCategory() { return category; }
        public string getStructure() { return structure; }
        public string getDefinition() { return description; }

        // Setter for each property
        public void setName(string newName) { name = newName; }
        public void setCategory(string newCategory) { category = newCategory; }
        public void setStructure(string newStructure) { structure = newStructure; }
        public void setDefinition(string newDefinition) { description = newDefinition; }

    }
}
