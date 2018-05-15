using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Entity;
using System.Threading.Tasks;
using System.Transactions;
using Memberships.Areas.Admin.Models;
using Memberships.Entities;
using Memberships.Models;

namespace Memberships.Extensions
{
    public static class ConversionExtensions
    {
        public static async Task<IEnumerable<ProductModel>> Convert(this IEnumerable<Product> products, ApplicationDbContext dbContext)
        {
            if(products.Count().Equals(0))
                return new List<ProductModel>();

            var texts = await dbContext.ProductLinkTexts.ToListAsync();
            var types = await dbContext.ProductTypes.ToListAsync();

            return from p in products
                select new ProductModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,
                    ProductLinkTextId = p.ProductLinkTextId,
                    ProductTypeId = p.ProductTypeId,
                    ProductLinkTexts = texts,
                    ProductTypes = types
                };
        }

        public static async Task<ProductModel> Convert(this Product product, ApplicationDbContext dbContext)
        {
            var text = await dbContext.ProductLinkTexts.FirstOrDefaultAsync(p => p.Id.Equals(product.ProductLinkTextId));
            var type = await dbContext.ProductTypes.FirstOrDefaultAsync(p => p.Id.Equals(product.ProductTypeId));

            var model = new ProductModel
                {
                    Id = product.Id,
                    Title = product.Title,
                    Description = product.Description,
                    ImageUrl = product.ImageUrl,
                    ProductLinkTextId = product.ProductLinkTextId,
                    ProductTypeId = product.ProductTypeId,
                    ProductLinkTexts = new List<ProductLinkText>(),
                    ProductTypes = new List<ProductType>()
                };

            model.ProductLinkTexts.Add(text);
            model.ProductTypes.Add(type);

            return model;
        }

        public static async Task<List<ProductItemModel>> Convert(this IQueryable<ProductItem> productItems,
            ApplicationDbContext dbContext)
        {
            if (productItems.Count().Equals(0))
                return new List<ProductItemModel>();

            var model = await (from pi in productItems
                               select new ProductItemModel
                        {
                            ItemId = pi.ItemId,
                            ProductId = pi.ProductId,
                            ItemTitle = dbContext.Items.FirstOrDefault(i => i.Id.Equals(pi.ItemId)).Title,
                            ProductTitle = dbContext.Products.FirstOrDefault(p => p.Id.Equals(pi.ProductId)).Title
                        }).ToListAsync();
            return model;
        }

        public static async Task<ProductItemModel> Convert(this ProductItem productItem, ApplicationDbContext dbContext, bool addListData = true)
        {
            var model = new ProductItemModel
            {
                ItemId = productItem.ItemId,
                ProductId = productItem.ProductId,
                Items = await dbContext.Items.ToListAsync(),
                Products = await dbContext.Products.ToListAsync(),
                ItemTitle = (await dbContext.Items.FirstOrDefaultAsync(i => i.Id.Equals(productItem.ItemId)))?.Title,
                ProductTitle = (await dbContext.Products.FirstOrDefaultAsync(p => p.Id.Equals(productItem.ProductId)))?.Title
            };
            return model;
        }

        public static async Task<bool> CanChange(this ProductItem productItem, ApplicationDbContext dbContext)
        {
            var oldPoroductItem = await dbContext.ProductItems.CountAsync(pi => pi.ProductId.Equals(productItem.OldProductId) &&
                                                                      pi.ItemId.Equals(productItem.OldItemId));
            var newProductItem = await dbContext.ProductItems.CountAsync(pi => pi.ProductId.Equals(productItem.ProductId) &&
                                                                      pi.ItemId.Equals(productItem.ItemId));
            return oldPoroductItem.Equals(1) && newProductItem.Equals(0);
        }

        public static async Task Change(this ProductItem productItem, ApplicationDbContext dbContext)
        {
            var oldProductItem =
                await dbContext.ProductItems.FirstOrDefaultAsync(pi => pi.ProductId.Equals(productItem.OldProductId) &&
                                                                       pi.ItemId.Equals(productItem.OldItemId));
            var newProductItem =
                await dbContext.ProductItems.FirstOrDefaultAsync(pi => pi.ProductId.Equals(productItem.ProductId) &&
                                                                       pi.ItemId.Equals(productItem.ItemId));
            if (oldProductItem != null && newProductItem == null)
            {
                newProductItem = new ProductItem
                {
                    ProductId = productItem.ProductId,
                    ItemId = productItem.ItemId
                };

                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        dbContext.ProductItems.Remove(oldProductItem);
                        dbContext.ProductItems.Add(newProductItem);
                        await dbContext.SaveChangesAsync();
                        transaction.Complete();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        transaction.Dispose();
                    }
                }
            }
        }

        public static async Task<List<SubscriptionProductModel>> Convert(this IQueryable<SubscriptionProduct> subscriptionProducts,
            ApplicationDbContext dbContext)
        {
            if (subscriptionProducts.Count().Equals(0)) 
                return new List<SubscriptionProductModel>();

            var model = await (from sp in subscriptionProducts
                select new SubscriptionProductModel
                {
                    SubscriptionId = sp.SubscriptionId,
                    ProductId = sp.ProductId,
                    SubscriptionTitle = dbContext.Subscriptions.FirstOrDefault(s => s.Id.Equals(sp.SubscriptionId)).Title,
                    ProductTitle = dbContext.Products.FirstOrDefault(p => p.Id.Equals(sp.ProductId)).Title
                }).ToListAsync();

            return model;
        }

        public static async Task<SubscriptionProductModel> Convert(this SubscriptionProduct subscriptionProduct,
            ApplicationDbContext dbContext, bool addListData = true)
        {
            var model = new SubscriptionProductModel
            {
                SubscriptionId = subscriptionProduct.SubscriptionId,
                ProductId = subscriptionProduct.ProductId,
                Subscriptions = addListData ? await dbContext.Subscriptions.ToListAsync():null,
                Products = addListData ? await dbContext.Products.ToListAsync():null,
                SubscriptionTitle = (await dbContext.Subscriptions.FirstOrDefaultAsync(s => s.Id.Equals(subscriptionProduct.SubscriptionId)))?.Title,
                ProductTitle = (await dbContext.Products.FirstOrDefaultAsync(p => p.Id.Equals(subscriptionProduct.ProductId)))?.Title
            };
            return model;
        }

        public static async Task<bool> CanChange(this SubscriptionProduct subscriptionProduct,
            ApplicationDbContext dbContext)
        {
            var oldSubscriptionProduct = await dbContext.SubscriptionProducts.CountAsync(
                sp => sp.ProductId.Equals(subscriptionProduct.OldProductId) &&
                      sp.ProductId.Equals(subscriptionProduct.OldSubscriptionId));

            var newSubscriptionProduct = await dbContext.SubscriptionProducts.CountAsync(
                sp => sp.ProductId.Equals(subscriptionProduct.ProductId) &&
                      sp.SubscriptionId.Equals(subscriptionProduct.SubscriptionId));

            return oldSubscriptionProduct.Equals(1) && newSubscriptionProduct.Equals(0);
        }

        public static async Task Change(this SubscriptionProduct subscriptionProduct, ApplicationDbContext dbContext)
        {
            var oldSubscriptionProduct =
                await dbContext.SubscriptionProducts.FirstOrDefaultAsync(
                    sp => sp.ProductId.Equals(subscriptionProduct.OldProductId) &&
                          sp.SubscriptionId.Equals(subscriptionProduct.OldSubscriptionId));
            var newSubscriptionProduct =
                await dbContext.SubscriptionProducts.FirstOrDefaultAsync(
                    sp => sp.ProductId.Equals(subscriptionProduct.ProductId) &&
                          sp.SubscriptionId.Equals(subscriptionProduct.SubscriptionId));

            if (oldSubscriptionProduct != null && newSubscriptionProduct == null)
            {
                newSubscriptionProduct = new SubscriptionProduct
                {
                    ProductId = subscriptionProduct.ProductId,
                    SubscriptionId = subscriptionProduct.SubscriptionId
                };

                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        dbContext.SubscriptionProducts.Remove(oldSubscriptionProduct);
                        dbContext.SubscriptionProducts.Add(newSubscriptionProduct);
                        await dbContext.SaveChangesAsync();
                        transaction.Complete();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        transaction.Dispose();
                    }
                }
            }
        }


    }
}