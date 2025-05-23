﻿using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.Domain.Entities
{
    /// <summary>
    /// Domain model for Country
    /// </summary>
    public class Country
    {
        [Key]
        public Guid CountryId { get; set; }
        [StringLength(100)]
        public string? CountryName { get; set; }
        public virtual ICollection<Person>? Persons { get; set; }
    }
}
