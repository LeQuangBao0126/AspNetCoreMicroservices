using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Entities
{
    public class ShoppingCart
    {
        public string UserName { get; set; }
        public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
        public ShoppingCart()
        {

        }
        public ShoppingCart(string username)
        {
            UserName = username;
        }
        public decimal TotalPrice
        {
            get
            {
                if(Items.Count > 0)
                {
                    decimal total = 0;
                    foreach(var item in Items)
                    {
                        total = item.Quantity * item.Price;
                    }
                    return total;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
