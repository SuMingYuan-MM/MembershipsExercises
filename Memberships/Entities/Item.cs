﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Memberships.Entities
{
    [Table("Item")]
    public class Item
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(255)]
        [Required]
        public string Title { get; set; }

        [MaxLength(2048)]
        public string Description { get; set; }

        [MaxLength(1024)]
        public string Url { get; set; }

        [MaxLength(1024)]
        [DisplayName("Image Url")]
        public string ImageUrl { get; set; }

        [AllowHtml]
        public string Html { get; set; }
        
        public int ItemTypeId { get; set; }
        public int PartId { get; set; }
        public int SectionId { get; set; }

        public virtual ICollection<Section> Sections { get; set; }
        public virtual ICollection<Part> Parts { get; set; }

        [DisplayName("Item Types")]
        public virtual ICollection<ItemType> ItemTypes { get; set; }

        [DisplayName("Wait Days")]
        [DefaultValue(0)]
        public int WaitDays { get; set; }

        [DisplayName("Is Free")]
        public bool IsFree { get; set; }


        [NotMapped]
        public string HtmlShort
        {
            get
            {
                return Html == null || Html.Length < 50 ? Html : Html.Substring(0, 50);
            }
        }
    }
}