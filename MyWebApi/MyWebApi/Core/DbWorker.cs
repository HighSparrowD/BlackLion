using MyWebApi.Entities.UserInfoEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyWebApi.Core
{
    public static class DbWorker
    {
        public static Random rn = new Random();

        public static int m = 1000;
        public static int x = 9000;

        public static long AddFriendUser(User currentUser, long userId)
        {
            return rn.Next(m, x);
        }

        public static IEnumerable<User> GetUserFriends(long id) // TODO: Add DB functional
        {

            var UserFriends = Enumerable.Range(1, 5).Select(index => User.CreateDummyUser());

            return UserFriends;
        }
    }
}
