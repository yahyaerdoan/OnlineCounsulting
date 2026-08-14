using Microsoft.EntityFrameworkCore;
using OnlineConsulting.DataAccess.Abstractions.IRepositories;
using OnlineConsulting.DataAccess.Concretions.Contexts;
using OnlineConsulting.DataAccess.Concretions.GenericRepositories;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataAccess.Concretions.Repositories;

public class OrderRepository(OnlineConsultingDbContext context) : GenericRepository<Order>(context), IOrderRepository
{
    public async Task<Order?> GetOrderAndOrderItemDetailByIdAsync(Guid orderId, Guid userId, bool tracking = true, bool? status = true)
    {
        IQueryable<Order> query = Entity;

        // Apply filters early
        query = query.Where(o => o.Id == orderId && o.UserId == userId);

        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        // Include necessary navigation properties
        query = query
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Service)
                    .ThenInclude(s => s.ServiceImages)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Service)
                    .ThenInclude(s => s.Category)
            .Include(o => o.ShippingAddress)
            .Include(o => o.InvoiceAddress);

        if (!tracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }

    public Task<int> GetOrderCountByOrderStatusAsync(Guid userId, string orderStatus, bool tracking = true, bool? status = true)
    {
        var query = Entity.Where(o => o.UserId == userId && o.OrderStatus == orderStatus);
        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        if (!tracking)
            query = query.AsNoTracking();

        return query.CountAsync();
    }

    public Task<int> GetOrderCountByPaymentStatusAsync(Guid userId, string paymentStatus, bool tracking = true, bool? status = true)
    {
        var query = Entity.Where(o => o.UserId == userId && o.PaymentStatus == paymentStatus);
        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        if (!tracking)
            query = query.AsNoTracking();

        return query.CountAsync();
    }

    public Task<int> GetTotalOrderCountAsync(Guid userId, bool tracking = true, bool? status = true)
    {
        var query = Entity.Where(o => o.UserId == userId);
        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        if (!tracking)
            query = query.AsNoTracking();

        return query.CountAsync();
    }

    public Task<decimal> GetTotalSpentByUserIdAsync(Guid userId, bool tracking = true, bool? status = true)
    {
        var query = Entity
            .Where(o => o.UserId == userId).SelectMany(o => o.OrderItems.Select(oi => new
            {
                o.Status,
                oi.TotalPrice,
            }));
        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (!tracking)
            query = query.AsNoTracking();

        return query.SumAsync(x => x.TotalPrice);
    }

    public IQueryable<Order> GetAllOrderWithUsers(bool traking = true, bool? status = true)
    {
        // No .Include(s => s.User): User lives in the Identity module's own DbContext now.
        // OrderManager.GetAllOrderWithUsersAsync backfills user names via UserManager<User>.
        var query = Entity.AsQueryable();
        if (status.HasValue)
            query = query.Where(e => e.Status == status);

        if (!traking)
            query = query.AsNoTracking();

        return query;
    }
}
