﻿using RudycommerceData.Entities;
using RudycommerceData.Repositories.BaseRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Repositories.IRepo
{
    public interface ILanguageRepository: IBaseRepository<Language>
    {
        Task<Language> MakeNewDefaultLanguage(Language newDefault);

        Task<Language> SwapDefaultLanguages(Language newDefault);
    }
}
