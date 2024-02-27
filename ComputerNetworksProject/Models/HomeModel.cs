using ComputerNetworksProject.Data;
using ComputerNetworksProject.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;

namespace ComputerNetworksProject.Models
{
    public class HomeModel:LayoutModel
    {
        
        public List<Product>? Products { get; set; }
        public List<Product>? ProductsInPage { get; set; }
        public List<Product>? FilterdProducts { get; set; }
        public List<int>? ShowPages { get; set; }
        public int ActivePage { get; set; }
        public Filter? FilterInput { get; set; }

        public HomeModel() { 

        }
        public HomeModel(List<Product> products) { 
            Products= products;
            FilterdProducts= products;
        }

        public HomeModel(List<Product> products,int page)
        {
            Products = products;
            FilterdProducts = products;
            ShowPages = [];
            InitPage(page);
        }

        public void InitPage(int page)
        {

            var totalCount = FilterdProducts.Count;
            var totalPages = (int)Math.Ceiling((decimal)totalCount/Constant.PageSize);
            if (totalPages == 0)
            {
                totalPages = 1;
            }
            if(page<1 || page > totalPages)
            {
                throw new ArgumentException("not enough pages");
            }
            ProductsInPage = FilterdProducts.Skip((page - 1) * Constant.PageSize).Take(Constant.PageSize).ToList();
            if(ShowPages is null)
            {
                ShowPages = [];
            }
            else
            {
                ShowPages.Clear();
            }
           
            int count = 1;
            for (int i=-1;count<=3&&page+i<=totalPages;i++)
            {
                if (page + i <= 0)
                    continue;
                ShowPages.Add(page+i);
                count++;
            }
            ActivePage= page;

        }
        public void ApplyFilters()
        {
            if (FilterInput is null)
            {
                return;
            }
            if (FilterInput.Search is not null)
            {
                FilterdProducts = FilterdProducts.Where(p =>
                p.Name.Contains(FilterInput.Search, StringComparison.InvariantCultureIgnoreCase) 
                || FilterInput.Search.Contains(p.Name, StringComparison.InvariantCultureIgnoreCase) 
                || p.Category.Name.Contains(FilterInput.Search, StringComparison.InvariantCultureIgnoreCase)
                || FilterInput.Search.Contains(p.Category.Name, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
            if(FilterInput.CategoryId is not null && FilterInput.CategoryId!=0)
            {
                FilterdProducts = FilterdProducts.Where(p =>
                p.CategoryId== FilterInput.CategoryId).ToList();
            }
            if(FilterInput.Rate is not null && FilterInput.Rate > 0)
            {
                FilterdProducts = FilterdProducts.Where(p =>
                p.Rate >= FilterInput.Rate).ToList();
            }
            if(FilterInput.OnlySale)
            {
                FilterdProducts = FilterdProducts.Where(p =>
                p.PriceDiscount is not null).ToList();
            }
            if (FilterInput.StartPrice is not null)
            {
                FilterdProducts = FilterdProducts.Where(p => p.PriceDiscount is not null ? p.PriceDiscount >= FilterInput.StartPrice :
                p.Price >= FilterInput.StartPrice).ToList();
            }
            if (FilterInput.EndPrice is not null)
            {
                FilterdProducts = FilterdProducts.Where(p => p.PriceDiscount is not null ? p.PriceDiscount <= FilterInput.EndPrice :
                p.Price <= FilterInput.EndPrice).ToList();
            }
            if(FilterInput.StartDate is not null)
            {
                FilterdProducts = FilterdProducts.Where(p => p.Created >= FilterInput.StartDate).ToList();
            }
            if (FilterInput.EndDate is not null)
            {
                FilterdProducts = FilterdProducts.Where(p => p.Created <= FilterInput.EndDate).ToList();
            }

        }

        public class Filter
        {

            public string? Search { get; set; }
            [Range(0,5)]
            public float? Rate { get; set; }

            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }

            [Range(0, float.MaxValue,ErrorMessage ="Price greater than 0")]
            [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "Must be a valid number.")]
            public float? StartPrice { get; set; }

            [Range(0, float.MaxValue, ErrorMessage = "Price greater than 0")]
            [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "Must be a valid number.")]
            public float? EndPrice { get; set;}

            [DefaultValue(false)]
            public bool OnlySale { get; set; }

            public int? CategoryId { get; set; }

        }
    }
}
