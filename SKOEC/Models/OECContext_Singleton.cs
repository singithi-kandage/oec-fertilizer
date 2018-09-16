using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SKOEC.Models
{
    public class OECContext_Singleton
    {
        private static OECContext _context;
        private static object syncLock = new object();

        public static OECContext Context()
        {
            if (_context == null)
            {
                lock (syncLock)
                {
                    if (_context == null)
                    {
                        var optionsBuilder = new DbContextOptionsBuilder<OECContext>();
                        optionsBuilder.UseSqlServer(
                                @"Server=.\sqlexpress;Database=OEC;Trusted_Connection=True;");
                        _context = new OECContext(optionsBuilder.Options);
                    }
                }
            }
            return _context;
        }
    }
}
