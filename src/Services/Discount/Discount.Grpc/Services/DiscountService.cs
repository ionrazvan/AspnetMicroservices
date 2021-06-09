using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc.Services
{
    public class DiscountService: DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository repository;
        private readonly IMapper mapper;
        private readonly ILogger<DiscountService> logger;

        public DiscountService(IDiscountRepository repository, IMapper mapper, ILogger<DiscountService> logger)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await repository.GetDiscount(request.ProductName);
            if (coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={request.ProductName} is not found."));
            }
            logger.LogInformation($"Discount is retrieved for ProductName: {coupon.ProductName}, Amount: {coupon.Amount}");

            var couponModel = mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = mapper.Map<Coupon>(request.Coupon);
            var result = await repository.CreateDiscount(coupon);
            if (!result)
                throw new RpcException(new Status(StatusCode.Unknown, $"Error creating the discount for ProductName = {request.Coupon.ProductName}."));
            logger.LogInformation($"Discount is successfully created. ProductName: {coupon.ProductName}");

            var couponModel = mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = mapper.Map<Coupon>(request.Coupon);
            var result = await repository.UpdateDiscount(coupon);
            if (!result)
                throw new RpcException(new Status(StatusCode.Unknown, $"Error updating the discount for ProductName = {request.Coupon.ProductName}."));
            logger.LogInformation($"Discount is successfully created. ProductName: {coupon.ProductName}");

            var couponModel = mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var deleted = await repository.DeleteDiscount(request.ProductName);
            var response = new DeleteDiscountResponse
            {
                Success = deleted
            };
            return response;
        }
    }
}
