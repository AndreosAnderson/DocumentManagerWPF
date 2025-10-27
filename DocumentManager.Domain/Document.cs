using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DocumentManager.Domain
{
    public class Document
    {
        [Key]
        public int Id { get; set; }

        public string Type { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;

        public ICollection<DocumentItems> Items { get; set; } = new List<DocumentItems>();
    }
}
