using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Task_1.Db_Folder;
using Task_1.Models;
using Task_1.Services;

namespace Task_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersInfoController : ControllerBase
    {
        private readonly ILogger<CustomersInfoController> _logger;

        private readonly CustomerInfo_DbContext dbContext;

        private readonly IRedisCaching _redisCaching;

        // private readonly IDistributedCache _cache;

        public CustomersInfoController(ILogger<CustomersInfoController> logger, IRedisCaching  redisCaching, CustomerInfo_DbContext DbContext/* IDistributedCache cache*/)
        {
            _logger = logger;
            _redisCaching = redisCaching;
            dbContext = DbContext;
          //  _cache = cache;
         }

        [HttpGet]
        [Route("{customers}")]

        public async Task<IActionResult> GetCustomerInfo()
        {
            var cacheData = _redisCaching.GetData<IEnumerable<Customer_Information>>("customers");
            if (cacheData != null && cacheData.Count() > 0)
                return Ok(cacheData);

            cacheData = await dbContext.CustomersInfo.ToListAsync();

            //Setting Our Expiry Time

            var expiryTime = DateTimeOffset.Now.AddSeconds(30);
            _redisCaching.SetData<IEnumerable<Customer_Information>>("customers", cacheData, expiryTime);

            return Ok(cacheData);
        }

        [HttpPost("AddCustomers")]

        public async Task<IActionResult> AddCustomersInfo(Customer_Information customerInfo)
        {
            var customer = await dbContext.CustomersInfo.AddAsync(customerInfo);

            var expiryTime = DateTimeOffset.Now.AddSeconds(45);
            _redisCaching.SetData<Customer_Information>($"customer{customerInfo.Id}", customer.Entity, expiryTime);

            await dbContext.SaveChangesAsync();

            return Ok(customer.Entity);

        }

        [HttpDelete("DeleteCustomer")]

        public async Task<IActionResult> DeleteCustomers(int id)
        {
            var exist = await dbContext.CustomersInfo.FirstOrDefaultAsync(x => x.Id == id);

            if (exist != null)
            {
                dbContext.Remove(exist);
                _redisCaching.RemoveData($"customer{id}");
                await dbContext.SaveChangesAsync();

                return Ok();
            }
            return NotFound();
        }

        /*
        [HttpGet]

        public async Task <IActionResult> GetCustomersInfo()
        {
            var customerInfo = await dbContext.CustomersInfo.ToListAsync();
            return Ok(customerInfo);
        }
        */
        /*
        [HttpGet]
        [Route("{id}")]

        public async Task<IActionResult> GetCustomerInfo([FromRoute] int id)
        {
            var customerInfo = await dbContext.CustomersInfo.FirstOrDefaultAsync(x =>x.Id == id);
            
            if(customerInfo == null)
            {
                return NotFound();
            }
            return Ok(customerInfo);
        }
        */



        /*
        [HttpPost]

        public async Task <IActionResult> AddCustomersInfo (AddCustomer_Information addCustomerInformation)
        {
            
            var customerInfo = new Customer_Information()
            {
              //  Id = addCustomerInformation.Id,
                Name = addCustomerInformation.Name,
                Address = addCustomerInformation.Address,
                Email_Address = addCustomerInformation.Email_Address,
            };

            await dbContext.CustomersInfo.AddAsync(customerInfo);

            await dbContext.SaveChangesAsync();

            return Ok(customerInfo);
        }
        */


        /*
        [HttpPut]
        [Route("{id:int}")]

        public async Task <IActionResult> UpdateCustomersInfo([FromRoute]int id, UpdateCustomersInformation updateCustomerInformation)
        {
            var customersInfo = await dbContext.CustomersInfo.FindAsync(id);

            if(customersInfo != null) 
            {
                customersInfo.Name = updateCustomerInformation.Name;
                customersInfo.Address = updateCustomerInformation.Address;
                customersInfo.Email_Address = updateCustomerInformation.Email_Address;
            }

            await dbContext.SaveChangesAsync();

            return Ok(customersInfo);
        }

        */



    }
}
