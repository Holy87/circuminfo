using CircumInfo.Common;
//using CircumInfo.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Windows.ApplicationModel.Store;

namespace CircumInfo.Common
{
    public static class Shop
    {
#if DEBUG
        public static readonly string prodotto = "product";
#else
        public static readonly string prodotto = "NoAds";
#endif
        public static async Task<ProductPurchaseStatus> buyNoAds()
        {
            //ListingInformation information = await CurrentApp.LoadListingInformationAsync();
            //List<ProductListing> products = information.ProductListings.Values.ToList();
            //System.Diagnostics.Debug.WriteLine(information.ProductListings.Count);
            //foreach (ProductListing prod in products)
            //{
            //    System.Diagnostics.Debug.WriteLine("Prodotto: " + prod.ProductId + " Nome: " + prod.Name);
            //}
            //ProductListing product = products.Single(x => x.ProductId == prodotto);
            //System.Diagnostics.Debug.WriteLine(product.Name);
#if DEBUG
            PurchaseResults result = await CurrentAppSimulator.RequestProductPurchaseAsync(prodotto);
#else
            PurchaseResults result = await CurrentApp.RequestProductPurchaseAsync(prodotto);
#endif
            return result.Status;
            //return ProductPurchaseStatus.NotPurchased;
        }

        public static async Task<string> getProductPrice()
        {
#if DEBUG
            ListingInformation information = await CurrentAppSimulator.LoadListingInformationAsync();
#else
            ListingInformation information = await CurrentApp.LoadListingInformationAsync();
#endif
            List<ProductListing> products = information.ProductListings.Values.ToList();
            ProductListing product = products.Single(x => x.ProductId == prodotto);
            return product.FormattedPrice;
        }

        public static bool getProductStatus()
        {
            LicenseInformation license = CurrentApp.LicenseInformation;
            ProductLicense product = license.ProductLicenses[prodotto];
            return product.IsActive;
        }
    }
}
