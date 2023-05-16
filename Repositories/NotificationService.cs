using System;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace feast_mansion_project.Repositories
{
	public class NotificationService : INotificationService
    {
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;

        public NotificationService(ITempDataDictionaryFactory tempDataDictionaryFactory)
        {
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
        }

        public void Success(string message)
        {
            var tempData = _tempDataDictionaryFactory.GetTempData(new DefaultHttpContext());

            tempData["SuccessMessage"] = message;
        }

        public void Error(string message)
        {
            var tempData = _tempDataDictionaryFactory.GetTempData(new DefaultHttpContext());
            tempData["ErrorMessage"] = message;
        }
    }
}

