using Dapper;
using Discount.API.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.API.Repositories
{
    public class DiscountRepository: IDiscountRepository
    {
        private readonly IConfiguration configuration;
        private readonly Coupon DefaultCoupon = new() { ProductName = "No Discount", Amount = 0, Description = "No Discount Description" };
        private readonly string DbConnConfigString = "DatabaseSettings:ConnectionString";
        public DiscountRepository(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public async Task<Coupon> GetDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(configuration.GetValue<string>(DbConnConfigString));

            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>("SELECT * FROM Coupon WHERE ProductName = @ProductName", 
                new { ProductName = productName });

            if (coupon == null)
                return DefaultCoupon;

            return coupon;
        }
        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(configuration.GetValue<string>(DbConnConfigString));

            var rowCount =
                await connection.ExecuteAsync("INSERT INTO Coupon(ProductName, Description, Amount) VALUES(@ProductName, @Description, @Amount)",
                    new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

            if (rowCount == 0)
                return false;

            return true;
        }
        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(configuration.GetValue<string>(DbConnConfigString));

            var rowCount =
                await connection.ExecuteAsync("UPDATE Coupon SET ProductName=@ProductName, Description=@Description, Amount=@Amount WHERE Id=@Id",
                    new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, Id = coupon.Id });

            if (rowCount == 0)
                return false;

            return true;
        }
        public async Task<bool> DeleteDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(configuration.GetValue<string>(DbConnConfigString));

            var rowCount =
                await connection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName=@ProductName",
                    new { ProductName = productName });

            if (rowCount == 0)
                return false;

            return true;
        }
    }
}
