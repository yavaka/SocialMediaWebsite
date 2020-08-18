namespace SocialMedia.Services.Group
{
    using Microsoft.EntityFrameworkCore;
    using SocialMedia.Data;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using SocialMedia.Data.Models;
    using SocialMedia.Services.User;

    public class GroupService : IGroupService
    {
        private readonly SocialMediaDbContext _data;

        public GroupService(
            SocialMediaDbContext data)
        {
            this._data = data;
        }

        /// <summary>
        /// When a new group is saved in the db, get its auto generated id then create new UserInGroup record as the member is creator & admin.
        /// </summary>
        /// <param name="serviceModel"></param>
        /// <returns></returns>
        public async Task AddGroupAsync(GroupServiceModel serviceModel)
        {
            var group = new Group()
            {
                Title = serviceModel.Title,
                Description = serviceModel.Description
            };

            await this._data.Groups.AddAsync(group);
            await this._data.SaveChangesAsync();

            var groupId = GetGroupIdByTitle(group.Title);

            await AddAdminAsync(groupId, serviceModel.AdminId);
        }

        private async Task AddAdminAsync(int groupId, string adminId)
        {
            var admin = new UserInGroup()
            {
                GroupId = groupId,
                UserId = adminId,
                Admin = true
            };

            await this._data.UsersInGroups.AddAsync(admin);
            await this._data.SaveChangesAsync();
        }

        public async Task EditGroupAsync(GroupServiceModel serviceModel)
        {
            var group = await this._data.Groups
                .FirstOrDefaultAsync(i => i.GroupId == serviceModel.GroupId);

            group.Title = serviceModel.Title;
            group.Description = serviceModel.Description;

            this._data.Update(group);
            await this._data.SaveChangesAsync();
        }

        public async Task DeleteGroupAsync(int groupId)
        {
            var group = await this._data.Groups
                .FirstOrDefaultAsync(i => i.GroupId == groupId);

            this._data.Groups.Remove(group);
            await this._data.SaveChangesAsync();
        }

        /// <summary>
        /// Creates new UserInGroup entity and save it to the db
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public async Task JoinGroupAsync(int groupId, string currentUserId)
        {
            if (!await IsCurrentUserMember(currentUserId, groupId))
            {
                var userInGroup = new UserInGroup
                {
                    GroupId = groupId,
                    UserId = currentUserId
                };

                this._data.UsersInGroups.Add(userInGroup);
                await this._data.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Find and get UserInGroup entity and remove it from the db
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public async Task LeaveGroupAsync(int groupId, string currentUserId)
        {
            var userInGroup = await this._data.UsersInGroups
                .FirstOrDefaultAsync(ug => ug.GroupId == groupId &&
                                        ug.UserId == currentUserId);

            this._data.Remove(userInGroup);
            await this._data.SaveChangesAsync();
        }

        /// <summary>
        /// Get all groups as retrieve:
        ///     GroupId - pass groupId to show group details
        ///     Title - show group title in the table
        ///     AdminId - compare current user id and adminId, if equal shows settings buttons
        /// </summary>
        /// <returns>async Task<ICollection<GroupServiceModel>></returns>
        public async Task<ICollection<GroupServiceModel>> GetGroupsAsync()
        => await this._data.Groups
            .Select(g => new GroupServiceModel
            {
                GroupId = g.GroupId,
                Title = g.Title,
                Members = g.Members
                    .Select(u => new UserServiceModel 
                    {
                        Id = u.User.Id,
                        UserName = u.User.UserName,
                        FullName = u.User.FullName,
                        Country = u.User.Country,
                        DateOfBirth = u.User.DOB
                    })
                    .ToList(),
                AdminId = g.Members
                    .FirstOrDefault(a => a.Admin == true)
                    .UserId
            })
            .ToListAsync();

        /// <summary>
        /// Gets all groups where the current user, AKA logged one, is not joined in. 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns>async Task<ICollection<GroupServiceModel>></returns>
        public async Task<ICollection<GroupServiceModel>> GetNonMemberGroupsAsync(UserServiceModel currentUser)
        {
            var groups = await GetGroupsAsync();

            var nonMemberGroups = new List<GroupServiceModel>();

            //Iterate each group and check is the current user is a member
            //If true add the group to nonMemberGroups
            foreach (var group in groups)
            {
                if (!group.Members.Any(i => i.Id == currentUser.Id))
                {
                    nonMemberGroups.Add(group);
                }
            }
            return nonMemberGroups;
        }

        /// <summary>
        /// To list joined groups requires: 
        ///     GroupId - pass groupId to show group details
        ///     Title - show group title in the table
        ///     AdminId - if the current user is admin, it is responsible for settings of the given group
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <returns>Task<ICollection<GroupServiceModel>></returns>
        public async Task<ICollection<GroupServiceModel>> GetJoinedGroupsAsync(UserServiceModel currentUser)
        {
            var groups = await GetGroupsAsync();

            var joinedGroups = new List<GroupServiceModel>();

            //Iterate each group and check does the current user is a member
            //If true add the group to joinedGroups
            foreach (var group in groups.ToList())
            {
                if (group.Members.Any(i => i.Id == currentUser.Id))
                {
                    joinedGroups.Add(group);
                }
            }
            return joinedGroups;
        }

        public async Task<GroupServiceModel> GetGroupAsync(int id)
        => await this._data.Groups
            .Select(g => new GroupServiceModel
            {
                GroupId = g.GroupId,
                Title = g.Title,
                Description = g.Description,
                Members = g.Members
                     .Select(u => new UserServiceModel
                     {
                         Id = u.User.Id,
                         UserName = u.User.UserName,
                         FullName = u.User.FullName,
                         Country = u.User.Country,
                         DateOfBirth = u.User.DOB
                     })
                    .ToList(),
                AdminId = g.Members
                    .FirstOrDefault(a => a.Admin == true)
                    .UserId
            })
            .FirstOrDefaultAsync(i => i.GroupId == id);

        private int GetGroupIdByTitle(string title)
       => this._data.Groups
            .FirstOrDefault(t => t.Title == title)
            .GroupId;

        public string GetGroupTitle(int groupId)
        => this._data.Groups
            .FirstOrDefault(i => i.GroupId == groupId)
            .Title;

        public async Task<bool> IsTitleExistAsync(string title)
        {
            if (await this._data.Groups.AnyAsync(i => i.Title == title))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> IsCurrentUserMember(string currentUserId, int groupId)
        {
            var members = await this._data.Groups
                .Where(i => i.GroupId == groupId)
                .Select(m => m.Members.Select(i => i.UserId))
                .ToListAsync();

            if (members.Any(i => i.Contains(currentUserId)))
            {
                return true;
            }
            return false;
        }
    }
}
