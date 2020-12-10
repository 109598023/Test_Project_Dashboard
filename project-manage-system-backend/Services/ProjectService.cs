using Microsoft.EntityFrameworkCore;
using project_manage_system_backend.Dtos;
using project_manage_system_backend.Shares;
using System;
using System.Collections.Generic;
using System.Linq;

namespace project_manage_system_backend.Services
{
    public class ProjectService : BaseService
    {
        public ProjectService(PMSContext dbContext) : base(dbContext) { }

        public void Create(ProjectDto projectDto)
        {
            if (projectDto.ProjectName == "")
            {
                throw new Exception("please enter project name");
            }
            var user = _dbContext.Users.Include(u => u.Projects).ThenInclude(p => p.Project).FirstOrDefault(u => u.Account.Equals(projectDto.UserId));
            
            if (user != null)
            {
                var userProject = (from up in user.Projects
                                   where up.Project.Name == projectDto.ProjectName
                                   select up.Project.Name).ToList();
                if (userProject.Count == 0)
                {
                    var newProject = new Models.UserProject
                    {
                        Project = new Models.Project { Name = projectDto.ProjectName, Owner = user },
                    };

                    user.Projects.Add(newProject);
                }
                else
                {
                    throw new Exception("duplicate project name");
                }
            }
            else
            {
                throw new Exception("user fail, can not find this user");
            }


            if(_dbContext.SaveChanges() == 0)
            {
                throw new Exception("create project fail");
            }
        }

        public void EditProjectName(ProjectDto projectDto)
        {
            if (projectDto.ProjectName == "")
            {
                throw new Exception("please enter project name");
            }
            else if (_dbContext.Projects.Where(p => p.Name == projectDto.ProjectName).ToList().Count != 0)
            {
                throw new Exception("duplicate project name");
            }

            var project = _dbContext.Projects.Find(projectDto.ProjectId);

            project.Name = projectDto.ProjectName;

            _dbContext.Update(project);

            if (_dbContext.SaveChanges() == 0)
            {
                throw new Exception("edit project name fail");
            }
        }

        public List<ProjectResultDto> GetProjectByUserAccount(string account)
        {
            var user = _dbContext.Users.Include(u => u.Projects).ThenInclude(p => p.Project).ThenInclude(p => p.Owner).FirstOrDefault(u => u.Account.Equals(account));
            var query = (from up in user.Projects
                        select new ProjectResultDto { Id = up.Project.ID, Name = up.Project.Name, OwnerId = up.Project.Owner.Account, OwnerName = up.Project.Owner.Name }).ToList();
            return query;
        }

        public ProjectResultDto GetProjectByProjectId(ProjectDto projectDto)
        {
            List<ProjectResultDto> userProject = GetProjectByUserAccount(projectDto.UserId);

            foreach (ProjectResultDto projectResultDto in userProject)
            {
                if (projectResultDto.Id == projectDto.ProjectId)
                {
                    return projectResultDto;
                }
            }
            throw new Exception("error project");
        }

        public void DeleteProject(int projectId)
        {
            var invitations = _dbContext.Invitations.Where(i => i.InvitedProject.ID.Equals(projectId));
            _dbContext.Invitations.RemoveRange(invitations);
            var repos = _dbContext.Repositories.Where(r => r.Project.ID.Equals(projectId));
            _dbContext.Repositories.RemoveRange(repos);
            var project = _dbContext.Projects.Find(projectId);
            _dbContext.Projects.Remove(project);

            if (_dbContext.SaveChanges() == 0)
                throw new Exception("Delete project fail!");
        }
    }
}
