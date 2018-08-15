using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceWeb.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace RudycommerceWeb.Controllers
{
    public abstract class MultilingualBaseController : RedirectableFromFiltersController
    {
        private ILanguageRepository _langRepo;

        public MultilingualBaseController()
        {
            _langRepo = new LanguageRepository();
        }

        protected int GetLangID()
        {
            return _langRepo.GetLanguageIDByISO(GetISO());
        }

        protected static string GetISO()
        {
            return Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
        }
    }
}