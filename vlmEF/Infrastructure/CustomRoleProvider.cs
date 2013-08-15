using System;
using System.Linq;
using System.Web.Security;

namespace vlmEF.Infrastructure
{
    public class CustomRoleProvider : RoleProvider
    {
        public override bool IsUserInRole(string username, string roleName)
        {
            using (var usersContext = new UsersContext())
            {
                var user = usersContext.Users.SingleOrDefault(u => u.UserName == username && u.Role.RoleName == roleName);
                if (user == null)
                    return false;

                return true;
                // Alex was here
                // Use the Role Property of user, which is a navigation property (kind of like a join)
                //return string.Equals(user.Role.RoleName, roleName, StringComparison.OrdinalIgnoreCase);
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            using (var usersContext = new UsersContext())
            {
                var user = usersContext.Users.SingleOrDefault(u => u.UserName == username);
                if (user == null)
                    return new string[]{};
                // return a string array with just one item, the name of the current role for the user
                return new string[] { user.Role.RoleName };
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            using (var usersContext = new UsersContext())
            {
                return usersContext.Roles.Select(r => r.RoleName).ToArray();
            }
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName { get; set; }
    }
}