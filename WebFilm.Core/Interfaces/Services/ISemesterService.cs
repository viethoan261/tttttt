﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFilm.Core.Enitites.Semesters;
using WebFilm.Core.Enitites.Subject;

namespace WebFilm.Core.Interfaces.Services
{
    public interface ISemesterService : IBaseService<int, Semesters>
    {
        bool create(SemesterDTO dto);
        int update(int id, SemesterDTO dto);
        int delete(int id);
        List<SemesterResponse> findAll();
    }
}
