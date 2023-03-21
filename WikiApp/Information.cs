﻿// 6.1 Create a separate class file to hold the four data items of the Data Structure
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
        public string getDefinition() { return this.description; }

        // Setter for each property
        public void setName(string change) { this.name = change; }
        public void setCategory(string change) { this.category = change; }
        public void setStructure(string change) { this.structure = change; }
        public void setDefinition(string change) { this.description = change; }

    }
}
