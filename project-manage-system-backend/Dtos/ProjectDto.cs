﻿namespace project_manage_system_backend.Dtos
{
    public class ProjectDto
    {
        public string ProjectName { set; get; }
        public string UserId { set; get; }
    }

    public class DeleteProjectDto
    {
        public int ProjectId { set; get; }
        public string UserId { set; get; }
    }
}
