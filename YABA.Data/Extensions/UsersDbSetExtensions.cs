using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YABA.Models;

namespace YABA.Data.Extensions
{
    public static  class UsersDbSetExtensions
    {
        public static async Task<bool> UserExistsAsync(this DbSet<User> userDbSet, int userId)
        {
            return await userDbSet.AnyAsync(x => x.Id == userId);
        }

        public static bool UserExists(this DbSet<User> userDbSet, int userId)
        {
            return userDbSet.Any(x => x.Id == userId);
        }
    }
}
