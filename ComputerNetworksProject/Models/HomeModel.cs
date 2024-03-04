using ComputerNetworksProject.Data;
using ComputerNetworksProject.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;
using Microsoft.Data.SqlClient;
using System.Globalization;

namespace ComputerNetworksProject.Models
{
    public class HomeModel
    {
        
        public List<Product>? Products { get; set; }
        public List<Product>? ProductsInPage { get; set; }
        public List<Product>? FilterdProducts { get; set; }
        public List<int>? ShowPages { get; set; }
        public int ActivePage { get; set; }
        public Filter? FilterInput { get; set; }

        public bool? ShowTable { get; set; }

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
            int pageSize= Constant.PageSizeCards;
            if (ShowTable is not null && (bool)ShowTable)
            {
                pageSize = Constant.PageSizeTable;
            }
            var totalCount = FilterdProducts.Count;
            var totalPages = (int)Math.Ceiling((decimal)totalCount/ pageSize);
            if (totalPages == 0)
            {
                totalPages = 1;
            }
            if(page<1 || page > totalPages)
            {
                throw new ArgumentException("not enough pages");
            }
            ProductsInPage = FilterdProducts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
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
            if(FilterInput.CategoryName is not null && FilterInput.CategoryName != "ALL")
            {
                FilterdProducts = FilterdProducts.Where(p =>
                p.CategoryName == FilterInput.CategoryName).ToList();
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
        public void ApplySort(string sort)
        {
            var res = sort.Split('_');
            string sortBy = res[0];
            string sortOrder = res[1];

            switch (sortBy)
            {
                case "price":
                    FilterdProducts.Sort((p1, p2) => CompareByPrice(p1, p2, sortOrder));
                    break;
                case "rate":
                    FilterdProducts.Sort((p1, p2) => sortOrder == "asc" ? p1.Rate.CompareTo(p2.Rate) : p2.Rate.CompareTo(p1.Rate));
                    break;
                case "cat":
                    FilterdProducts.Sort((p1, p2) => sortOrder == "asc" ? p1.Category.Name.CompareTo(p2.Category.Name) : p2.Category.Name.CompareTo(p1.Category.Name));
                    break;
                default:
                    throw new ArgumentException("Invalid sortBy argument");
            }
        }
        static int CompareByPrice(Product p1, Product p2, string sortOrder)
        {
            float price1 = p1.PriceDiscount ?? p1.Price;
            float price2 = p2.PriceDiscount ?? p2.Price;

            return sortOrder == "asc" ? price1.CompareTo(price2) : price2.CompareTo(price1);
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

            public string? CategoryName { get; set; }

        }
    }
}
