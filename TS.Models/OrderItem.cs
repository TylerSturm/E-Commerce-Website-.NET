using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TS.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        public int OrderHeaderId { get; set; }
        public int ProductId { get; set; }    
        public double Price { get; set; }      
        public int Quantity { get; set; } 

        [ForeignKey("OrderHeaderId")]
        public OrderHeader OrderHeader { get; set; } 

        [ForeignKey("ProductId")]
        public Product Product { get; set; } 
    }
}