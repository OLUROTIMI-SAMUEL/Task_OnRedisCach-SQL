using Microsoft.EntityFrameworkCore;

namespace Task_1.Db_Folder
{
    public class CustomerInfo_DbContext : DbContext
    {
        public CustomerInfo_DbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Models.Customer_Information> CustomersInfo { get; set; }
    }
}
