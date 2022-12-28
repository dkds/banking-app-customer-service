using CustomerService.Data;
using CustomerService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace CustomerService.Controllers
{
    [Route("/[controller]")]
    [Controller]
    public class AccountsController : ControllerBase
    {
        private readonly CustomerServiceContext _context;
        private readonly IDistributedCache _cache;

        public AccountsController(CustomerServiceContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: /Accounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccount([FromQuery] string? accountNo)
        {
            if (_context.Account == null)
            {
                return NotFound();
            }
            if (accountNo == null)
            {
                var accounts = await _context.Account.ToListAsync();
                accounts.ForEach(a => UpdateAccountsCache(a));
                return accounts;
            }
            else
            {
                var accounts = await _context.Account.Where(a => a.Number == accountNo).ToListAsync();
                accounts.ForEach(a => UpdateAccountsCache(a));
                return accounts;
            }
        }

        // GET: /Accounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            if (_context.Account == null)
            {
                return NotFound();
            }
            var account = await _context.Account.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            UpdateAccountsCache(account);

            return account;
        }

        // PUT: /Accounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(int id, [FromBody] Account account)
        {
            if (id != account.Id)
            {
                return BadRequest();
            }

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                UpdateAccountsCache(account);

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PUT: /Accounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{accountNumber}/Balance")]
        public async Task<IActionResult> UpdateAccountBalance(string accountNumber, [FromBody] TransactionAmountDto transactionAmountDto)
        {
            _context.Account.Where(u => u.Number == accountNumber).ExecuteUpdate(b =>
                b.SetProperty(u => u.Balance,
                u => transactionAmountDto.Type == TransactionType.Credit ? u.Balance + transactionAmountDto.Amount : u.Balance - transactionAmountDto.Amount)
            );

            var account = await _context.Account.Where(a => a.Number == accountNumber).FirstAsync();
            UpdateAccountsCache(account);

            return NoContent();
        }

        // POST: /Accounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount([FromBody] Account account)
        {
            if (_context.Account == null)
            {
                return Problem("Entity set 'CustomerServiceContext.Account'  is null.");
            }
            _context.Account.Add(account);
            await _context.SaveChangesAsync();

            account.Number = account.Id.ToString("D8");
            await _context.SaveChangesAsync();

            UpdateAccountsCache(account);

            return CreatedAtAction("GetAccount", new { id = account.Id }, account);
        }

        // DELETE: /Accounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            if (_context.Account == null)
            {
                return NotFound();
            }
            var account = await _context.Account.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Account.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AccountExists(int id)
        {
            return (_context.Account?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private async Task UpdateAccountsCache(Account account)
        {
            var serialized = JsonSerializer.Serialize(account);
            await _cache.SetStringAsync("accounts:" + account.Number, serialized, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
                SlidingExpiration = TimeSpan.FromMinutes(25)
            });
        }

    }
}
