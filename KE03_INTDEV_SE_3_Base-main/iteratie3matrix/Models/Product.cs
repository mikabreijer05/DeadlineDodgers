using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iteratie3matrix.Models;

public class Product
{
    public int ProductId { get; set; }

    public string ProdName { get; set; } = string.Empty;

    public decimal ProdPrice { get; set; }

    public string ProdDescription { get; set; } = string.Empty;

    public string ProdImage { get; set; } = string.Empty;

    public int ProdQuantity { get; set; }

    public string ProdDimensions { get; set; } = string.Empty;
}