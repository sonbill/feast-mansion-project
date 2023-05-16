using System;
namespace feast_mansion_project.Repositories
{
	public interface INotificationService
	{
        void Success(string message);

        void Error(string message);
    }
}

