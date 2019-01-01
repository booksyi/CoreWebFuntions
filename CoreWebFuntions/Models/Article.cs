using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreWebFuntions.Models
{
    [Table("Article")]
    public class Article
    {
        [Column("Art_PriKey", TypeName = "int")]
        public int Art_PriKey { get; set; }
        
        [Column("Art_Class11", TypeName = "int")]
        public int? Art_Class11 { get; set; }
        
        [Column("Art_Class12", TypeName = "int")]
        public int? Art_Class12 { get; set; }
        
        [Column("Art_Class21", TypeName = "int")]
        public int? Art_Class21 { get; set; }
        
        [Column("Art_Class22", TypeName = "int")]
        public int? Art_Class22 { get; set; }
        
        [Column("Art_Class31", TypeName = "int")]
        public int? Art_Class31 { get; set; }
        
        [Column("Art_Class32", TypeName = "int")]
        public int? Art_Class32 { get; set; }
        
        [Column("Art_Title")]
        public string Art_Title { get; set; }
        
        [Column("Art_Member", TypeName = "int")]
        public int? Art_Member { get; set; }
        
        [Column("Art_Author")]
        public string Art_Author { get; set; }
        
        [Column("Art_Date", TypeName = "datetime")]
        public DateTime? Art_Date { get; set; }
        
        [Column("Art_Books")]
        [StringLength(50)]
        public string Art_Books { get; set; }
        
        [Column("Art_Contents1")]
        public string Art_Contents1 { get; set; }
        
        [Column("Art_Contents2")]
        public string Art_Contents2 { get; set; }
        
        [Column("Art_Contents3")]
        public string Art_Contents3 { get; set; }
        
        [Column("Art_Type1", TypeName = "int")]
        public int? Art_Type1 { get; set; }
        
        [Column("Art_Type2", TypeName = "int")]
        public int? Art_Type2 { get; set; }
        
        [Column("Art_Type3", TypeName = "int")]
        public int? Art_Type3 { get; set; }
        
        [Column("Art_Type4", TypeName = "int")]
        public int? Art_Type4 { get; set; }
        
        [Column("Art_Type5", TypeName = "int")]
        public int? Art_Type5 { get; set; }
        
        [Column("Art_Type6", TypeName = "int")]
        public int? Art_Type6 { get; set; }
        
        [Column("Art_Modify", TypeName = "datetime")]
        public DateTime? Art_Modify { get; set; }
        
        [Column("Art_Count", TypeName = "int")]
        public int? Art_Count { get; set; }
        
        [Column("Art_Top", TypeName = "int")]
        public int Art_Top { get; set; }
        
        [Column("Art_Free", TypeName = "int")]
        public int Art_Free { get; set; }
        
        [Column("Art_All", TypeName = "int")]
        public int Art_All { get; set; }
        
        [Column("Art_Url")]
        public string Art_Url { get; set; }
        
        [Column("Art_UnDate", TypeName = "datetime")]
        public DateTime? Art_UnDate { get; set; }
        
        [Column("Art_Sort", TypeName = "int")]
        public int Art_Sort { get; set; }
    }
}