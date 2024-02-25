using ComputerNetworksProject.Data;
using ComputerNetworksProject.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

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
            ShowPages = new List<int>();
            try
            {
                InitPage(1);
            } catch(Exception ex) {
                ProductsInPage = new List<Product>();
                ShowPages.Add(1);
                ActivePage = 1;
            }
        }

        public HomeModel(List<Product> products,int page)
        {
            Products = products;
            FilterdProducts = products;
            ShowPages = new List<int>();
            InitPage(page);
        }

        public void InitPage(int page)
        {

            var totalCount = FilterdProducts.Count;
            var totalPages = (int)Math.Ceiling((decimal)totalCount/Constant.PageSize);
            if(page<1 || page> totalPages)
            {
                throw new ArgumentException("not enough pages");
            }
            ProductsInPage = FilterdProducts.Skip((page - 1) * Constant.PageSize).Take(Constant.PageSize).ToList();
            if(ShowPages is null)
            {
                ShowPages=new List<int>();  
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

        public class Filter
        {
            public string? Search { get; set; }
            [Range(1,5)]
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
