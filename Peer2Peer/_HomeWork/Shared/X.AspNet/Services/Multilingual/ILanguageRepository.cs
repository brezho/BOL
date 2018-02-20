﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.AspNet.Services.Multilingual
{
    public interface ILanguageRepository
    {
        IEnumerable<LanguageDTO> AllLanguages();
    }
}