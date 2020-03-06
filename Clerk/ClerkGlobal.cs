using Clerk.Model;

namespace Clerk
{
    public class ClerkGlobal
    {
        private readonly ClerkContext _db;
        
        public ClerkGlobal(ClerkContext dbContext)
        {
            _db = dbContext;
        }
        
        
        
    }
}