using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiApp
{
    internal class Information : IComparable
    {
        // Information stored here
        private string name;
        private string category;
        private string structure;
        private string description;

        // Constructor
        public Information(string name, string category, string structure, string description) {
            this.name = name;
            this.category = category;
            this.structure = structure; 
            this.description = description;
        }

        // Implement the CompareTo function
        public int CompareTo(Object other)
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
        public string getName() { return this.name; }
        public string getCategory() { return this.category; }
        public string getStructure() { return this.structure; }
        public string getDescription() { return this.description; }

        // Setter for each property
        public void setName(string change) { this.name = change; }
        public void setCategory(string change) { this.category = change; }
        public void setStructure(string change) { this.structure = change; }
        public void setDescription(string change) { this.description = change; }

    }
}
